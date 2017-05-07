using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class StudentActivityController : PhonoBlocksController
{


		enum State
		{
				PLACE_INITIAL_LETTERS,
				MAIN_ACTIVITY,
				REMOVE_ALL_LETTERS,
				HINT_PLACE_EACH_LETTER
			
		}

		State state = State.PLACE_INITIAL_LETTERS;
		public void EnterGuidedLetterPlacementMode(){
			state = State.HINT_PLACE_EACH_LETTER;
		}
		LockedPositionHandler lockedPositionHandler;
		HintController hintController;
		ArduinoLetterController arduinoLetterController;
		Problem currProblem;

		public string TargetLetters{
		get {
			string targetWord = currProblem.TargetWord(true);

			return targetWord;
		}
	}

		UserWord targetWordAsLetterSoundComponents;

		public bool StringMatchesTarget (string s)
		{
				return s.Equals (currProblem.TargetWord (true));

		}



		char[] usersMostRecentChanges;
		AudioClip excellent;
		AudioClip incorrectSoundEffect;
		AudioClip notQuiteIt;
		AudioClip offerHint;
		AudioClip youDidIt;
		AudioClip correctSoundEffect;
		AudioClip removeAllLetters;
		AudioClip triumphantSoundForSessionDone;

		public string UserChangesAsString {
				get {
						StringBuilder result = new StringBuilder ();

						for (int i=0; i<usersMostRecentChanges.Length; i++)
								result.Append (usersMostRecentChanges [i]);

						return result.ToString ();
		
				}

		}

		public void Initialize (GameObject hintButton, ArduinoLetterController arduinoLetterController)
		{
				this.arduinoLetterController = arduinoLetterController;
				arduinoLetterController.studentActivityController = this;
				usersMostRecentChanges = new char[UserInputRouter.numOnscreenLetterSpaces];
			
				lockedPositionHandler = gameObject.GetComponent<LockedPositionHandler> ();
			
				hintController = gameObject.GetComponent<HintController> ();
				hintController.Initialize (hintButton);
			
				SetUpNextProblem ();
	


				excellent = InstructionsAudio.instance.excellent;
				incorrectSoundEffect = InstructionsAudio.instance.incorrectSoundEffect;
				notQuiteIt = InstructionsAudio.instance.notQuiteIt;
				offerHint = InstructionsAudio.instance.offerHint;
				youDidIt = InstructionsAudio.instance.youDidIt;
				correctSoundEffect = InstructionsAudio.instance.correctSoundEffect;
				removeAllLetters = InstructionsAudio.instance.removeAllLetters;

				triumphantSoundForSessionDone = InstructionsAudio.instance.allDoneSession;
		}

		
	public void SetUpNextProblem ()
		{  

				//get the next specific problem from the ProblemType class
				ClearSavedUserChanges ();
				hintController.Reset ();
			
				currProblem = ProblemsRepository.instance.GetNextProblem ();
	     
				StudentsDataHandler.instance.RecordActivityTargetWord (currProblem.TargetWord (false));
		        targetWordAsLetterSoundComponents = LetterSoundComponentFactoryManager.Decode (currProblem.TargetWord (true), 
		                                                                               SessionsDirector.instance.IsSyllableDivisionMode);
				lockedPositionHandler.ResetForNewProblem ();
				lockedPositionHandler.RememberPositionsThatShouldNotBeChanged (currProblem.InitialWord, currProblem.TargetWord (false).Trim ()); 
	
				arduinoLetterController.ReplaceEachLetterWithBlank ();
				arduinoLetterController.PlaceWordInLetterGrid (currProblem.InitialWord);



				arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (false);
				arduinoLetterController.LockAllLetters ();
		   
	
				userInputRouter.RequestTurnOffImage ();

				hintController.DeActivateHintButton ();
			
				PlayInstructions (); //dont bother telling to place initial letters during assessment mode

	
				state = State.PLACE_INITIAL_LETTERS;


				//In case the initial state is already correct (...which happens when the user needs to build the word "from scratch". This makes it so
				//we don;t need to trigger the check by adding a "blank"!
				ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ();
		        
		         
				//




		//april 20; added this so that we only show as many lines as there are letters in the current word.
		arduinoLetterController.activateLinesBeneathLettersOfWord(currProblem.TargetWord(true));

		}
	
        
		public void PlayInstructions ()
		{
			
				currProblem.PlayCurrentInstruction ();
			
				
		}

		void ClearSavedUserChanges ()
		{
				for (int i=0; i<usersMostRecentChanges.Length; i++) {
						usersMostRecentChanges [i] = ' ';
					
				}


		}

		public LetterSoundComponent GetTargetLetterSoundComponentFor(int index){
		   return targetWordAsLetterSoundComponents.GetLetterSoundComponentForIndexRelativeWholeWord (index);
		}

		bool CurrentStateOfLettersMatches (string targetLetters)
		{       
				for (int i=0; i<usersMostRecentChanges.Length; i++) {
						if (i >= targetLetters.Length) {
								if (usersMostRecentChanges [i] != ' ')
										return false;
					} else if (targetLetters [i] != ' ' && usersMostRecentChanges [i] != targetLetters [i])
								return false;
				}
				return true;

		}

		public void ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ()
		{
			
				if (state == State.PLACE_INITIAL_LETTERS) {
						if (CurrentStateOfLettersMatches (currProblem.InitialWord))
								BeginMainProblemState ();
				
				}
				if (state == State.REMOVE_ALL_LETTERS) {
					
						if (CurrentStateOfLettersMatches (currProblem.TargetWord (false))) {
							
								HandleEndOfActivity ();
						}
					
				}
	
		}

		void HandleEndOfActivity ()
		{
				if (ProblemsRepository.instance.AllProblemsDone ()) {
						StudentsDataHandler.instance.UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs ();
						AudioSourceController.PushClip (triumphantSoundForSessionDone);
						
				
				} else {
						SetUpNextProblem ();
				}

		}

		void BeginMainProblemState ()
		{
				arduinoLetterController.UnLockAllLetters (); //we do this to unlock the letters that are outside the range of the initial word.
				//during the initial stage only the letters in the initial word will absolutely be locked.
				state = State.MAIN_ACTIVITY;

		}

		public void UserRequestsHint ()
		{
				if (hintController.UsedLastHint ()) {
						currProblem.PlayAnswer ();
						arduinoLetterController.PlaceWordInLetterGrid (currProblem.TargetWord (false));
						CurrentProblemCompleted (false);
				} else 
						hintController.ProvideHint (currProblem);

		}

	   public void SkipToNextLetterToHint(){
		if (IsErroneous (hintController.TargetLetterIndex)) {
					hintController.DisplayAndPlaySoundOfCurrentTargetLetter ();
				} else {
						int alreadyCorrect = hintController.TargetLetterIndex; 
						while (!IsErroneous(alreadyCorrect) && alreadyCorrect < hintController.NumTargetLetters) {
								hintController.AdvanceTargetLetter ();
								hintController.DisplayAndPlaySoundOfCurrentTargetLetter ();
								alreadyCorrect++;
						}

						if (alreadyCorrect == hintController.NumTargetLetters)
								state = State.MAIN_ACTIVITY;
				}

			arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (true);
		}
	
		public void HandleNewArduinoLetter (char letter, int atPosition)
		{       
				if(state == State.HINT_PLACE_EACH_LETTER){
					RecordUsersChange (atPosition, letter);
			        //child needs to place the correct letters they haven't already placed in order starting from the target index.
					if(atPosition == hintController.TargetLetterIndex && !IsErroneous(atPosition)){
							SkipToNextLetterToHint();
					} else {
						hintController.DisplayAndPlaySoundOfCurrentTargetLetter();
					}
				
					return;
				} 

				if (LetterIsActuallyNew (letter, atPosition)) {
						bool positionWasLocked = lockedPositionHandler.IsLocked (atPosition); 
						//we treat all positions as "locked" when the state is the end of the activity.
						if (positionWasLocked || state == State.REMOVE_ALL_LETTERS) {
								lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges, arduinoLetterController);
						}

						if (state == State.PLACE_INITIAL_LETTERS)
								arduinoLetterController.UnLockASingleLetter (atPosition);

						RecordUsersChange (atPosition, letter); 

						ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ();
			   
						arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (state != State.PLACE_INITIAL_LETTERS);

				}
		}

		/// <summary>
		/// Letters the is actually new.
		/// </summary>
		/// <returns><c>true</c>, if the user actually changed the letter, <c>false</c> otherwise, i.e., if we're reading a "new value" from an arduino circuit or input error.</returns>
		/// <param name="letter">Letter.</param>
		/// <param name="atPosition">At position.</param>
		bool LetterIsActuallyNew (char letter, int atPosition)
		{
				return usersMostRecentChanges [atPosition] != letter;


		}

	   public bool IsErroneous(int atPosition){
		if (!ReferenceEquals(currProblem, null) && !ReferenceEquals(currProblem.TargetWord (true), null)) {
			string target = currProblem.TargetWord(true);
			if(atPosition > target.Length-1 || atPosition > usersMostRecentChanges.Length) return false;
			char targetChar = target[atPosition];
			char actualChar = usersMostRecentChanges[atPosition];
			return (int)actualChar != 32 && targetChar != actualChar;
			}
		return false;
	    }

		bool PositionIsOutsideBoundsOfTargetWord (int wordRelativeIndex)
		{
				return wordRelativeIndex >= currProblem.TargetWord (true).Length; 
		}

		public virtual void HandleSubmittedAnswer ()
		{      if (state == State.MAIN_ACTIVITY) {
						StudentsDataHandler.instance.LogEvent ("submitted_answer", UserChangesAsString, currProblem.TargetWord (false));
				
						currProblem.IncrementTimesAttempted ();
	
						if (IsSubmissionCorrect ()) {
								//TO DO!!! then if this was the first time that student submitted an answer (get the data from the current student object)
								//then play the good hint else play the less good hint
								AudioSourceController.PushClip (correctSoundEffect);
								if (currProblem.TimesAttempted > 1)
										AudioSourceController.PushClip (youDidIt);
								else
										AudioSourceController.PushClip (excellent);
								currProblem.PlayAnswer ();
								CurrentProblemCompleted (true);
				
						} else {
								HandleIncorrectAnswer ();				
				
						}
				}

		}

		protected void HandleIncorrectAnswer ()
		{
				
				AudioSourceController.PushClip (incorrectSoundEffect);
				
				if (!hintController.HintButtonActive ()) {
						hintController.ActivateHintButton ();
						AudioSourceController.PushClip (notQuiteIt);
						AudioSourceController.PushClip (offerHint);
				}

				hintController.AdvanceHint ();

		}

		public void CurrentProblemCompleted (bool userSubmittedCorrectAnswer)
		{
			
		     
				state = State.REMOVE_ALL_LETTERS;

				currProblem.SetTargetWordToEmpty ();
				userInputRouter.AddCurrentWordToHistory (false);
				//arduinoLetterController.LockAllLetters ();
		   
				userInputRouter.RequestDisplayImage (currProblem.TargetWord (true), false, true);

				bool solvedOnFirstTry = currProblem.TimesAttempted == 1;
				if (solvedOnFirstTry) {
		
						userInputRouter.DisplayNewStarOnScreen (ProblemsRepository.instance.ProblemsCompleted-1);

				}


				StudentsDataHandler.instance.RecordActivitySolved (userSubmittedCorrectAnswer, UserChangesAsString, solvedOnFirstTry);
			
				StudentsDataHandler.instance.SaveActivityDataAndClearForNext (currProblem.TargetWord (false), currProblem.InitialWord);


      
		}

		public void RecordUsersChange (int position, char change)
		{
		
				usersMostRecentChanges [position] = change;

		
		}

		public bool IsSubmissionCorrect ()
		{      
				string target = currProblem.TargetWord (true);

				bool result = CurrentStateOfLettersMatches (target);

				
				return result;

		}



}
