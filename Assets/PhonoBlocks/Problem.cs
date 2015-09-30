using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class Problem : MonoBehaviour
{


	    
		public const int PLACE_LETTERS_INITIAL = 0;
		public const int TO_MAKE_THE_WORD = 1;
		public const int TARGET_WORD = 2;


		protected AudioClip[] instructions;
		protected string initialWord;
		protected AudioClip soundToEmphasize;
		protected AudioClip sounded_out_word;
		protected int currInstruction = 0;
		protected string cachedMissingLettersAsString;
		protected static string emptyWord = "";

		public static string EmptyWord ()
		{
				if (emptyWord.Length == 0)
						CacheEmptyWord (UserInputRouter.numArduinoControlledLetters);
				return emptyWord;

		}

		public string CachedMissingLettersAsString {
				get {
						return cachedMissingLettersAsString;
				}


		}

		public string InitialWord {
				get {
						return initialWord;
				}
				set {
						initialWord = value;
			
				}


		}

		protected string targetWord;
		protected readonly string cachedTargetWord;

		public string TargetWord (bool skipBlanks)
		{
				if (skipBlanks)
						return cachedTrimmedTargetWord;
				return targetWord;


				

		}

		protected readonly string cachedTrimmedTargetWord;
		protected int numNonBlankInitialLetters;

		public int NumInitialLetters {
				get {
						return numNonBlankInitialLetters;
				}

		}

	public Problem (string initialWord, string targetWord)
	{
		initialWord=Clean (initialWord); 
		targetWord=Clean(targetWord);


		CacheFixedInstructions (initialWord, targetWord);
		CacheNumberOfNonBlankInitialLetters (initialWord);
		cachedTrimmedTargetWord = targetWord;
		this.targetWord = AppendBlanksToEnd (targetWord, UserInputRouter.numArduinoControlledLetters);
		this.initialWord = AppendBlanksToFrontOrEnd (initialWord, this.targetWord);
		CacheMissingLettersAsString ();
		this.soundToEmphasize = soundToEmphasize;
		
		cachedTargetWord = this.targetWord;


		Debug.Log ("initial word Length: " + initialWord.Length);
		
		
	}


	string Clean(string word){
		return word.Trim ().ToLower ();
	}





		static void CacheEmptyWord (int numArduinoControlledLetters)
		{      
				StringBuilder s = new StringBuilder ();
				for (int i=0; i<numArduinoControlledLetters; i++) {
						s.Append (' ');

				}

				emptyWord = s.ToString ();
		}

		public void SetTargetWordToEmpty ()
		{
				if (emptyWord.Length == 0)
						CacheEmptyWord (UserInputRouter.numArduinoControlledLetters);
				
				targetWord = emptyWord;

		}

		public void Reset ()
		{
				currInstruction = 0;
				targetWord = cachedTargetWord;
			
		}

		protected void CacheMissingLettersAsString ()
		{
				StringBuilder cachedMissingLettersAsString = new StringBuilder ();

		           for (int i=0; i<initialWord.Length; i++) {
						if (initialWord [i] != targetWord [i])
								cachedMissingLettersAsString.Append (targetWord [i]);
						else
								cachedMissingLettersAsString.Append (' ');
				}

				this.cachedMissingLettersAsString = cachedMissingLettersAsString.ToString ();
		        


		}

		protected virtual void CacheFixedInstructions (string initialWord, string targetWord)
		{       
				InstructionsAudio source = InstructionsAudio.instance;
				instructions = new AudioClip[3];
				instructions [PLACE_LETTERS_INITIAL] = source.placeInitialLettersInstruction;
				instructions [TO_MAKE_THE_WORD] = source.makeTheWordInsructions;
				instructions [TARGET_WORD] = AudioSourceController.GetWordFromResources (targetWord);//(AudioClip)Resources.Load ("audio/words/" + targetWord, typeof(AudioClip));//WordImageAndAudioMapAccessor.GetInstance ().GetWordData (targetWord).ParseAudio ();
				sounded_out_word = AudioSourceController.GetSoundedOutWordFromResources (targetWord);
	
		}

		protected void CacheNumberOfNonBlankInitialLetters (string initialWord)
		{
				for (int i=0; i<initialWord.Length; i++)
						if (initialWord [i] != ' ')
								numNonBlankInitialLetters++;


		}

		protected string AppendBlanksToEnd (string targetWord, int numArduinoControlledLetters)
		{
				StringBuilder s = new StringBuilder (targetWord);
				for (int i=s.Length; i<numArduinoControlledLetters; i++)
						s.Append (' ');

				return s.ToString ();


		}

		public void PlayInstructionsToPlaceInitialLetters ()
		{

				AudioSourceController.PushClip (instructions [PLACE_LETTERS_INITIAL]);

		}

		public void PlayCurrentInstruction ()
		{
				if (currInstruction == PLACE_LETTERS_INITIAL)
						AudioSourceController.PushClip (instructions [currInstruction]);
				else {
						for (int i=TO_MAKE_THE_WORD; i<TARGET_WORD+1; i++) {
							
								if (instructions [i] != null)
										AudioSourceController.PushClip (instructions [i]);
						}

				}
		}

		public void PlayTargetWord ()
		{

				AudioSourceController.PushClip (instructions [TARGET_WORD]);
		}
		

		public void PlayFirstHint (string currentWord)
		{
				
				PlayFirstHint ();
			
		}

		public void PlayFirstHint ()
		{

				AudioSourceController.PushClip (sounded_out_word);
				PlayTargetWord ();
		}

		public void PlaySecondHint ()
		{     
				PlayFirstHint ();

		}

		public void PlayAnswer ()
		{
				PlayTargetWord ();
			


		}

		public void AdvanceInstruction (string currentWord)
		{   
				if (currInstruction < instructions.Length - 1) {
						currInstruction++;

				}
		}
	  


		protected string AppendBlanksToFrontOrEnd (string initialWord, string targetWord)
		{
			
				StringBuilder s = new StringBuilder (initialWord);
				int numBlanksToAppendToFront = FindDifferenceInIndexesOfFirstMatchingLetter (initialWord, targetWord);
				for (int i=0; i<numBlanksToAppendToFront; i++) {
						s.Insert (0, ' ');
		
				}
				int numBlanksToAppendToEnd = targetWord.Length - s.Length;
				for (int i=0; i<numBlanksToAppendToEnd; i++)
						s.Append (' ');

				return s.ToString ();


		}

		protected int FindDifferenceInIndexesOfFirstMatchingLetter (string initialWord, string targetWord)
		{
				//D=idx 1 in itial; D idx 2 in target. if we were to ADD ONE MORE CHATR the indexes would match. so add that many blanks
				for (int i=0; i<initialWord.Length; i++) {
						char a = initialWord [i];
						for (int j=0; j<targetWord.Length; j++) {
								char b = targetWord [j];
								if (a == b) 
										return j - i;
					 		
						}
				}
				return targetWord.Length - initialWord.Length;
		}

	   
}

