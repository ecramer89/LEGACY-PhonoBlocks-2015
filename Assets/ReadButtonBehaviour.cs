using UnityEngine;
using System.Collections;
using Uniduino;

public class ReadButtonBehaviour : MonoBehaviour {
	
	public Arduino arduino;
	
	public int pin = 53;
	public int pinValue;
	public int testLed = 13;
	bool begin=false;
	
	void Start( )
	{
		arduino = Arduino.global;
		arduino.Setup(ConfigurePins);
	}
	
	void ConfigurePins( )
	{   Debug.Log ("called configure");
		arduino.pinMode(pin, PinMode.INPUT);
		arduino.reportDigital((byte)(pin/8), (byte)1);
		// set the pin mode for the test LED on your board, pin 13 on an Arduino Uno
		arduino.pinMode(testLed, PinMode.OUTPUT);
		begin = true;
		Debug.Log ("begin: " + begin);
	}
	
	void Update () 
	{       if (begin) {
						// read the value from the digital input
						pinValue = arduino.digitalRead (pin);
						// apply that value to the test LED
						arduino.digitalWrite (testLed, pinValue);
				}
	}
		


	

}
