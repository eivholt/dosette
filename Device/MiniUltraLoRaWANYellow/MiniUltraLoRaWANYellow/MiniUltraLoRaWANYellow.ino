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

#define SLEEP_PERIOD 5000
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
  // 4  Pin Change Interrupt Request 0 (pins D8 to D13) (PCINT0_vect)
  // 6  Pin Change Interrupt Request 2 (pins D0 to D7)  (PCINT2_vect)
  // 5  Pin Change Interrupt Request 1 (pins A0 to A5)  (PCINT1_vect)
   PCMSK1 |= bit (PCINT8);  // want pin A0
   PCMSK1 |= bit (PCINT9);  // want pin A1
   PCMSK1 |= bit (PCINT10);  // want pin A2
   PCMSK1 |= bit (PCINT11);  // want pin A3
   PCIFR  |= bit (PCIF1);   // clear any outstanding interrupts
   PCICR  |= bit (PCIE1);   // enable pin change interrupts for A0 to A5

  //PCMSK2 |= bit (PCINT21);  // want pin D5
  //PCMSK2 |= bit (PCINT18);  // want pin D2
  //PCIFR  |= bit (PCIF2);   // clear any outstanding interrupts
  //PCICR  |= bit (PCIE2);   // enable pin change interrupts for D8 to D13
}

void loop()
{
    //debugSerial.print(F("."));
    
    debugSerial.println(F(" MCU Sleep"));
    debugSerial.flush();
    LowPower.powerDown(SLEEP_FOREVER, ADC_OFF, BOD_OFF);
    debugSerial.println(F(" MCU Wake"));
    //debugSerial.print(lidStatus, DEC);
    // debugSerial.print(F(" "));
    debugSerial.println(lidStatus, HEX);
    debugSerial.flush();

    // Send & sleep
    if(lidStatus != 0)
    {
      PCICR &= ~(1 << PCIE1); // Disable pin change interrupts for A0 to A5
      pendingTransmission = false;
      byte payload[1] = {lidStatus};
      lidStatus = 0;
      debugSerial.print(F("Payload: "));
      debugSerial.println(payload[0]);

      digitalWrite(LED_BUILTIN, HIGH);
      //loraSerial.begin(BAUD_RATE_LORA);
      debugSerial.println(F(" Transmitting"));
      debugSerial.flush();
      
      ttn.sendBytes(payload, sizeof(payload));
      debugSerial.println(F(" Transmitted"));
      debugSerial.flush();
      ttn.saveState();
      //ttn.sleep(SLEEP_PERIOD);
      digitalWrite(LED_BUILTIN, LOW);
      PCICR |= bit (PCIE1); // Enable pin change interrupts for A0 to A5
    }

    // if(!pendingTransmission && lidStatus != 0)
    // {
    //   PCICR &= ~(1 << PCIE1); // Disable pin change interrupts for A0 to A5
    //   loraSerial.begin(BAUD_RATE_LORA);
    //   debugSerial.println(F(" Start radio timer"));
    //   debugSerial.flush();
      
    //   // Use RN2483/RN2903 as MCU wake-up source after RN2483/RN2903 sleep period 
    //   // expires 

    //   ttn.sleep(SLEEP_PERIOD);
    //   //detachInterrupt(digitalPinToInterrupt(radioWakeUpPin));
    //   attachInterrupt(digitalPinToInterrupt(radioWakeUpPin), radioWakeUp, LOW);
    //   // Put IO pins (D8 & D9) used for software serial into low power 
    //   loraSerial.end();
    //   //Put MCU into sleep mode and to be woken up by pin change interrupts or RN2483/RN2903
    //   //debugSerial.println(F(" MCU Sleep"));
    //   //debugSerial.flush();
    //   //LowPower.powerDown(SLEEP_FOREVER, ADC_OFF, BOD_OFF);
    //   //debugSerial.println(F(" MCU Wake"));
    //   //loraSerial.begin(BAUD_RATE_LORA);
    //   PCICR |= bit (PCIE1); // Enable pin change interrupts for A0 to A5
    // }
}

void radioWakeUp()
{
  //noInterrupts();
  detachInterrupt(digitalPinToInterrupt(radioWakeUpPin));
  pendingTransmission = true;
  //debugSerial.println(F(" radioWakeUp"));
  //debugSerial.flush();
  //interrupts();
}

ISR (PCINT1_vect)
 {
    //noInterrupts();
    if (!digitalRead(lid1Pin)) lidStatus |= (1 << 0);
    if (!digitalRead(lid2Pin)) lidStatus |= (1 << 1);
    if (!digitalRead(lid3Pin)) lidStatus |= (1 << 2);
    if (!digitalRead(lid4Pin)) lidStatus |= (1 << 3);
    debugSerial.println(F(" PCINT1_vect"));
    debugSerial.println(lidStatus, HEX);
    debugSerial.flush();
    //interrupts();
 }

//  ISR (PCINT2_vect)
//  {
//     noInterrupts();
//     if (!digitalRead(lid1Pin)) lidStatus |= (1 << 0);
//     if (!digitalRead(lid2Pin)) lidStatus |= (1 << 1);
//     if (!digitalRead(lid3Pin)) lidStatus |= (1 << 2);
//     if (!digitalRead(lid4Pin)) lidStatus |= (1 << 3);
//     interrupts();
//  }