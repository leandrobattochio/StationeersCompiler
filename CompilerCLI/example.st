# Codes can have comments like this

# Get a device from housing pin
StationeersDevice airConditioner = referenceDevice(d0);
StationeersDevice airConditioner1 = referenceDevice(d1);
StationeersDevice airConditioner2 = referenceDevice(d2);
StationeersDevice airSensor = referenceDevice(d3);


# Access device property
Float temp = airSensor.Temperature;

# We can set property like this
airConditioner.On = Math.convertToCelsius(temp) == 25.0;
airConditioner1.On = Math.convertToCelsius(temp) == 25.0;

# Alternatively, if cases is also supported for more complex logic
#if (temp > 25.0) {
#    airConditioner2.On = true;
#} else {
#    airConditioner2.On = false;
#}

# We have type checking so 25 is not the same as 25.0
# if (temp > 25) -> This will not compile. temp is Float while 25 is Int