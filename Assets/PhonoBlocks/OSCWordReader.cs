// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class OSCWordReader : MonoBehaviour {
	private string UDPHost = "127.0.0.1";
	private int listenerPort = 12000;
	private int broadcastPort = 9001;
	[HideInInspector]
	public Osc oscHandler;
	//[HideInInspector]
	//public Destroypop2 pop;

	private int counter = 0;
	public static OSCWordReader instance;


	public void Start(){

		instance = this;
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		print("osc!");
		udp.init(UDPHost, broadcastPort, listenerPort);
		//udp.init(UDPHost, broadcastPort);
		oscHandler = GetComponent<Osc>(); 
		oscHandler.init(udp);
		print("Running");

	}


	public void  Update (){
		/*Destroypop2 destroypop2 = GetComponent<Destroypop2>();
		bool vender = destroypop2.vend;
		if(vender == true) oscHandler.Send(Osc.StringToOscMessage("/vend 1"));
		destroypop2.vend = false;
		*/
	}	



	public void Read(string word){
		oscHandler.Send(Osc.StringToOscMessage("/TTS "+word));



	}

}