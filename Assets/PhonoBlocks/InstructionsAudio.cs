using UnityEngine;
using System.Collections;

/*
 * singleton pattern. use the global instance to access the public audio clips.
 * 
 * */
public class InstructionsAudio : MonoBehaviour
{

		public static InstructionsAudio instance;

		//first index --> "add one letter", second, "add two letters, " etc.

		public AudioClip placeInitialLettersInstruction;
		public AudioClip removeAllLettersInstruction;
		public AudioClip makeTheWordInsructions;
	    public AudioClip readTheWordInsructions;


		void Awake ()
		{
				instance = this;
		}















}
