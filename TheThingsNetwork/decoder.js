function Decoder(bytes, port) {
    // Decode an uplink message from a buffer
    // (array) of bytes to an object of fields.
    var decoded = {};
  
    if (port === 1) 
    {
      decoded.lid1 = (Boolean)(bytes[0] & 1);
      decoded.lid2 = (Boolean)((bytes[0] >> 1) & 1);
      decoded.lid3 = (Boolean)((bytes[0] >> 2) & 1);
      decoded.lid4 = (Boolean)((bytes[0] >> 3) & 1);
    }
  
    return decoded;
  }