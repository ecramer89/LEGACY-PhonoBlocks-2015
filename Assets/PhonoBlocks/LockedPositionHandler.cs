using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockedPositionHandler : PhonoBlocksController
{
		HashSet<int> lockedPositions;
		int numLockedPositionsWithIncorrectLetter;
		StudentActivityController studentActivityController;

		public void Initialize ()
		{
				
				studentActivityController = gameObject.GetComponent<StudentActivityController> ();
				
		}

		public void ResetForNewProblem ()
		{
				lockedPositions = new HashSet<int> ();
				
			

		}

		public int UserMustRemoveAllTheLettersOf {
				get {
						return numLockedPositionsWithIncorrectLetter;
				}
		
				set {
			
						numLockedPositionsWithIncorrectLetter = value;
					
				}
		
		
		}

		public int NumTangibleLettersThatUserMustMatchToLockedUILetters {
				get {
						return numLockedPositionsWithIncorrectLetter;
				}

				set {

						numLockedPositionsWithIncorrectLetter = value;
				}


		}


		//REQUIRES the 2 strings have equal length.
		//(i.e., the "shorter" has had blanks appended to it.
		public void RememberPositionsThatShouldNotBeChanged (string initialLetters, string targetLetters)
		{
				
				//a position is locked iff the char at that position is the same between initial and target letters
				for (int i=0; i<initialLetters.Length; i++) 
						if (initialLetters [i] == targetLetters [i])
								lockedPositions.Add (i); //!! possibly should be i+1 because the user does not add items to the position 0

		}

		public bool IsLocked (int position)
		{

				return lockedPositions.Contains (position);
		}

		public void HandleChangeToLockedPosition (int position, char change, string targetState, char[] usersMostRecentChanges, ArduinoLetterController arduinoLetterController)
		{
				//Debug.Log ("users most recent changes: " + usersMostRecentChanges);
				//if the user changed a position that is supposed to have a letter (i.e., is not blank)
				//I think that for remove all letters the target state is blanks
				//if (!CharacterInTargetStateIsBlank (targetState, position)) {

				//then: if the state of that letter before the change was correct
		if (StateOfLettersBeforeUserChangeWasCorrect (position, targetState, usersMostRecentChanges)) {
						//then say there is an error.
						HandleError (position, change, targetState, arduinoLetterController);
				} else {
						//if the user's change is what should be there
						if (UsersChangeMatchesCharacterInTargetWord (change, targetState [position])) {
								//Debug.Log ("recognized that users change matches character in target word ");
								DecreaseNumLockedPositionsWithIncorrectLetter (position, arduinoLetterController);

						} else if (numLockedPositionsWithIncorrectLetter > 0)
								arduinoLetterController.LockASingleLetter (position);
						else
								arduinoLetterController.UnLockASingleLetter (position); 
			
				}
				//}
		
		}


		void DecreaseNumLockedPositionsWithIncorrectLetter (int positionOfNewLetter, ArduinoLetterController arduinoLetterController)
		{
				numLockedPositionsWithIncorrectLetter--;
				if (numLockedPositionsWithIncorrectLetter == 0)
						arduinoLetterController.UnLockAllLetters ();
				else
						arduinoLetterController.UnLockASingleLetter (positionOfNewLetter);

		}

		bool PreviousStateAndNewLetterAreNotIdentical (int positionOfChange, string desiredStateString, char change)
		{
				return change != desiredStateString [positionOfChange];


		}

		public bool AllLockedPositionsAreInCorrectState ()
		{
				return numLockedPositionsWithIncorrectLetter == 0;

		}

		/*void HandleRestorationOfDesireableState (int position, char change, string targetLetters, ArduinoLetterController arduinoLetterController)
		{
				numLockedPositionsWithIncorrectLetter--;
				if (numLockedPositionsWithIncorrectLetter == 0)
						arduinoLetterController.UnLockAllLetters ();
				//else
				arduinoLetterController.UnLockASingleLetter (position);

		}*/

		void HandleError (int position, char change, string targetLetters, ArduinoLetterController arduinoLetterController)
		{
				if (numLockedPositionsWithIncorrectLetter == 0)
						arduinoLetterController.LockAllLetters ();
				
				numLockedPositionsWithIncorrectLetter++;
			
					
				//arduinoLetterController.LockASingleLetter (position);
				
				

	
				StudentsDataHandler.instance.LogEvent ("unproductive_error", change + "", position + "");

			
		}

		bool AddedALetter (char change)
		{
				return change != ' ';

		}

		bool UsersChangeMatchesCharacterInTargetWord (char change, char desiredLetter)
		{
				return LettersAreSameIgnoreCase (change, desiredLetter);
			

		}

		bool StateOfLettersBeforeUserChangeWasCorrect (int position, string targetLetters, char[] usersMostRecentChanges)
		{
				return LettersAreSameIgnoreCase (targetLetters [position], usersMostRecentChanges [position]);

		}
		
		bool LettersAreSameIgnoreCase (char a, char b)
		{
			   
				int ai = (int)a;
				int bi = (int)b;
				return ai == bi || ai + 32 == bi || ai - 32 == bi;


		}




}
