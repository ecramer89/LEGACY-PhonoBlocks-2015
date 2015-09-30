using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

public class WordImageAndAudioMapAccessor : MonoBehaviour
{
		public static WordImageAndAudioMapAccessor instance;
		WordImages wordDataCache;
		Dictionary<string, UnityEngine.Object[]> cachedWords;
		UnityEngine.Object[] psuedoword;

		public static WordImageAndAudioMapAccessor GetInstance ()
		{
				if (instance == null) {
						instance = GameObject.Find ("DataTables").GetComponent<WordImageAndAudioMapAccessor> ();

				}
				return instance;
		}

		void Initialize ()
		{
				wordDataCache = GameObject.Find ("DataTables").GetComponent<WordImages> ();
				cachedWords = new Dictionary<string,UnityEngine.Object[]> ();
				//initialize dictionary.
				foreach (FieldInfo prop in typeof(WordImages).GetFields ()) {
						//cachedWords.Add (prop.Name, (UnityEngine.Object[])prop.GetValue (wordDataCache));
			
			
				}
				psuedoword = wordDataCache.psuedoword;




		}

		void Start ()
		{
				
				wordDataCache = GameObject.Find ("DataTables").GetComponent<WordImages> ();
				cachedWords = new Dictionary<string,UnityEngine.Object[]> ();
				//initialize dictionary.
				foreach (FieldInfo prop in typeof(WordImages).GetFields ()) {
						//cachedWords.Add (prop.Name, (UnityEngine.Object[])prop.GetValue (wordDataCache));
		
			
				}
				psuedoword = wordDataCache.psuedoword;
				


		}

		public WordDataParser GetPsuedowordData ()
		{

				return new WordDataParser (psuedoword);
		}
	
		public WordDataParser GetWordData (string word)
		{
				if (cachedWords == null)
						Initialize ();
				word = word.Trim ().ToLower ();
		        
				UnityEngine.Object[] wordData;
				bool inCache = cachedWords.TryGetValue (word, out wordData);
			
				if (inCache)
						return new WordDataParser (wordData);
				else
						return GetPsuedowordData (); //else return the data that is the "dictionary" of the psuedoword image, depending, plus an audio recording.
		}
	
	
		public class WordDataParser
		{
				UnityEngine.Object[] wordData;
				int imgIdx;
				int recordingIdx;
		
				public WordDataParser (UnityEngine.Object[] wordData)
				{
						this.wordData = wordData;
						SetIndexes ();
			
				}
		
				void SetIndexes ()
				{
						if (wordData [0] != null) {
								if (wordData [0] is Texture2D) 
										imgIdx = 0;
								else
										recordingIdx = 0;
						}

						if (wordData [1] != null) {
								if (wordData [1] is Texture2D) 
										imgIdx = 1;
								else
										recordingIdx = 1;
						}

				}
		
				public Texture2D ParseImage ()
				{
						UnityEngine.Object datum = wordData [imgIdx];
						if (datum is Texture2D)
								return (Texture2D)datum;
						return null;
				}

				public AudioClip ParseAudio ()
				{
						UnityEngine.Object datum = wordData [recordingIdx];
						if (datum is AudioClip) {
								return (AudioClip)datum;
						}
						//else see if we have a recording of this particular syllable.
			          
						return null;
				

				}
		
				//another one for the audio recording...
		
		
		}
}
