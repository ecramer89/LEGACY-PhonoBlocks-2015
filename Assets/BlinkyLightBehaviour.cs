using UnityEngine;
using System.Collections;
using Uniduino;

public class BlinkyLightBehaviour : MonoBehaviour
{
		public Arduino arduino; //reference to the arduino board.
		// Use this for initialization
		void Start ()
		{
				arduino = Arduino.global;
				//interesting... so it seems that you can pass functions around as variable.s
				//you just need to treat them as "System.Action" parameters.
				arduino.Setup (ConfigurePins);
				StartCoroutine ("BlinkLoop");
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		IEnumerator BlinkLoop ()
		{
				while (true) {
			            
						arduino.digitalWrite (13, Arduino.HIGH);
			Debug.Log ("wrote high");
						yield return new WaitForSeconds (10); //...hey, we could use this to have timed out display evenrts
						arduino.digitalWrite (13, Arduino.LOW);
			Debug.Log ("wrote low");
						yield return new WaitForSeconds (10);

				}


		}

		void ConfigurePins ()
	{       Debug.Log ("called configure");
				arduino.pinMode (13, PinMode.OUTPUT);

		}
}
