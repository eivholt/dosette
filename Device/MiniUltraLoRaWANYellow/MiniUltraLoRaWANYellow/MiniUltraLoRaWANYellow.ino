#define DEBUG 0

#include "secrets.h"
#include <TheThingsNetwork.h>
#include <LowPower.h>
#include <AltSoftSerial.h>

AltSoftSerial loraSerial;

// Set your AppEUI and AppKey in secrets.h, found on TTN console device details
const char *appEui = SECRET_APPEUI;
const char *appKey = SECRET_APPKEY;

const int radioWakeUpPin = 2;
const int loraResetPin = 4;
const int lid1Pin = A0;
const int lid2Pin = A1;
const int lid3Pin = A2;
const int lid4Pin = A3;

volatile byte lidStatus = 0;
volatile bool pendingTransmission = false;

#define SLEEP_PERIOD 10000
#define BAUD_RATE_LORA 19200
#define BAUD_RATE_DEBUG 115200
#define debugSerial Serial
#define freqPlan TTN_FP_EU868

TheThingsNetwork ttn(loraSerial, debugSerial, freqPlan);

void setup()
{
  pinMode(radioWakeUpPin, INPUT);
  pinMode(loraResetPin, OUTPUT);

  pinMode(lid1Pin, INPUT_PULLUP);
  pinMode(lid2Pin, INPUT_PULLUP);
  pinMode(lid3Pin, INPUT_PULLUP);
  pinMode(lid4Pin, INPUT_PULLUP);
  pinMode(LED_BUILTIN, OUTPUT);
  
  // Reset RN2483/RN2903 for a clean power up
  digitalWrite(loraResetPin, LOW);
  delay(500);
  digitalWrite(loraResetPin, HIGH);
  
  loraSerial.begin(BAUD_RATE_LORA);
  debugSerial.begin(BAUD_RATE_DEBUG);
  debugSerial.println(F("-- SETUP"));
  
  // Reset is required to autobaud RN2483 into 19200 bps from the
  // default 57600 bps (autobaud process is called within reset())
  ttn.reset();
  //ttn.showStatus();
  debugSerial.println(F("-- JOIN"));
  ttn.join(appEui, appKey);

  // Pin Change Interrupt
  // https://gammon.com.au/interrupts 
  // 5  Pin Change Interrupt Request 1 (pins A0 to A5)  (PCINT1_vect)
   PCMSK1 |= bit (PCINT8);  // want pin A0
   PCMSK1 |= bit (PCINT9);  // want pin A1
   PCMSK1 |= bit (PCINT10);  // want pin A2
   PCMSK1 |= bit (PCINT11);  // want pin A3
   PCIFR  |= bit (PCIF1);   // clear any outstanding interrupts
   PCICR  |= bit (PCIE1);   // enable pin change interrupts for A0 to A5
}

void loop()
{
  // Put radio to sleep, wake using interrupt
  loraSerial.begin(BAUD_RATE_LORA);
  ttn.wake();
  ttn.sleep(SLEEP_PERIOD);
  detachInterrupt(digitalPinToInterrupt(radioWakeUpPin));
  attachInterrupt(digitalPinToInterrupt(radioWakeUpPin), radioWakeUp, LOW);
  // Put IO pins (D8 & D9) used for software serial into low power 
  loraSerial.end();
  // Put MCU to sleep until interrupted
  LowPower.powerDown(SLEEP_FOREVER, ADC_OFF, BOD_OFF);

  // Transmit any outstanding lid states
  if(pendingTransmission && lidStatus != 0)
  {
    PCICR &= ~(1 << PCIE1); // Disable pin change interrupts for A0 to A5
    pendingTransmission = false;
    // Make a copy of lid states
    byte payload[1] = {lidStatus};
    // Reset lid states
    lidStatus = 0;
    
    loraSerial.begin(BAUD_RATE_LORA);
    ttn.wake();
    ttn.sendBytes(payload, sizeof(payload));
    ttn.saveState();

    PCIFR |= bit (PCIF1);   // clear any outstanding interrupts
    PCICR |= bit (PCIE1); // Enable pin change interrupts for A0 to A5
  }
}

void radioWakeUp()
{
  detachInterrupt(digitalPinToInterrupt(radioWakeUpPin));
  pendingTransmission = true;
}

ISR (PCINT1_vect)
 {
    PCICR &= ~(1 << PCIE1); // Disable pin change interrupts for A0 to A5
    // Don't transmit lid states until transmission interval has passed,
    // accumulate states
    pendingTransmission = false;
    
    if (digitalRead(lid1Pin) == LOW) lidStatus |= (1 << 0);
    if (digitalRead(lid2Pin) == LOW) lidStatus |= (1 << 1);
    if (digitalRead(lid3Pin) == LOW) lidStatus |= (1 << 2);
    if (digitalRead(lid4Pin) == LOW) lidStatus |= (1 << 3);

    PCIFR |= bit (PCIF1);   // clear any outstanding interrupts
    PCICR |= bit (PCIE1); // Enable pin change interrupts for A0 to A5
 }