using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class AudioSourceController : MonoBehaviour
{

		static AudioSource source;
		static readonly string RESOURCES_WORD_PATH = "audio/words/";
		static readonly string RESOURCES_SYLLABLE_PATH = "audio/syllables/";
		static readonly string RESOURCES_CON_LE_SYLLABLE_PATH = "audio/syllables/conle/";
		static LinkedList<AudioClip> bufferedClips = new LinkedList<AudioClip> ();
	
		void Start ()
		{
				if (source == null)
						source = gameObject.GetComponent<AudioSource> ();
		}

	public static AudioClip GetSoundedOutWordFromResources(string word){
		StringBuilder path = new StringBuilder (RESOURCES_WORD_PATH);
		path.Append("so_");
		path.Append(word);
		return (AudioClip)Resources.Load (path.ToString (), typeof(AudioClip));

		}

		public static AudioClip GetClipFromResources (string path)
		{

				return (AudioClip)Resources.Load (path, typeof(AudioClip));

		}

		public static AudioClip GetWordFromResources (string word)
		{

				return (AudioClip)Resources.Load (RESOURCES_WORD_PATH + word, typeof(AudioClip));

		}
		
		public static AudioClip GetSyllableFromResources (string syllable)
		{
				if (syllable.Length == 0)
						return null;
				AudioClip result = null;

				bool candidateConLe = (syllable.Length == 3 && syllable [2] == 'e' && syllable [1] == 'l');
				if (candidateConLe) {
						result = (AudioClip)Resources.Load (RESOURCES_CON_LE_SYLLABLE_PATH + syllable, typeof(AudioClip));
				}
				if (result != null)
						return result;
				result = (AudioClip)Resources.Load (MakePathToSyllable (syllable), typeof(AudioClip));
				return result;


		}

		public static string MakePathToSyllable (string syllable)
		{

				return RESOURCES_SYLLABLE_PATH + syllable [0] + "/" + syllable;


		}





		//play the first, when it's done, play the second.
		public static void PushClip (AudioClip next)
		{ 
				if (next != null)
						bufferedClips.AddLast (next);


		}

		void Update ()
		{
				if (bufferedClips.Count > 0) {
						if (!source.isPlaying) {
								Play (bufferedClips.First.Value);
								bufferedClips.RemoveFirst ();
						}
				}

		}

		static void Play (AudioClip clip)
		{
		
		
				if (!source.isPlaying) {
						source.clip = clip;
						source.Play ();
				}
		
		}




	   

}