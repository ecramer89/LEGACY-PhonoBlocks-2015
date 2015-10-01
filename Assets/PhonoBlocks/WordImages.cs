using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;


public class WordImages : MonoBehaviour {

	public Texture2D bet;


	public Texture2D default_image; //in case no image is found; return a default image instead of null.



	public static WordImages instance;
	Dictionary<string, Texture2D> cachedWords;

	void Awake(){
		instance = gameObject.GetComponent<WordImages> ();



		cachedWords = new Dictionary<string,Texture2D> ();
			//initialize dictionary.
			foreach (FieldInfo prop in typeof(WordImages).GetFields ()) {
			cachedWords.Add (prop.Name, (UnityEngine.Texture2D)prop.GetValue (instance));
			}

		}


	
	public Texture2D GetWordImage (string word)
	{
	
		word = word.Trim ().ToLower ();
		
		UnityEngine.Texture2D wordImage;
		bool inCache = cachedWords.TryGetValue (word, out wordImage);
		
		if (inCache)
			return wordImage;
		return default_image;
	}










}
