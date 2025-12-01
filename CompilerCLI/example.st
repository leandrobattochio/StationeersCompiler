StationeersDevice airConditioner = referenceDevice(d0);
StationeersDevice airSensor = referenceDevice(d1);

Float temp = airConditioner.Temperature;
Float pressure = airSensor.Pressure;

temp = Math.convertToCelsius(temp);
if (temp >= 25 && pressure < 101325.0) {
   airConditioner.On = true;
} else {
   airConditioner.On = false;
   airSensor.On = true;
}