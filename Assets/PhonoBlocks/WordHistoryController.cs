using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WordHistoryController : PhonoBlocksController
{
		int wordLength;
		public GameObject wordHistoryPanelBackground;
		LetterImageTable letterImageTable;

		public int WordLength {
				get {

						return wordLength;

				}

		}

		public GameObject wordHistoryGrid;
		LetterGridController lettersOfWordInHistory;
		//WordImageAndAudioMapAccessor wordDataAccessor; //reference to the data table that has the dictionary linking string reresentations of words to word objects.
		

		public List<Word> words; //words in the word history.
		Word psuedoWord; //a dummy value to return in case there is some kind of error.
		public Word PsuedoWord {
				get {
						if (psuedoWord == null) {
								psuedoWord = new Word ("whoops");
				psuedoWord.Image = WordImages.instance.default_image;
						}
						return psuedoWord;
				}

		}

		public int showImageTime = 60 * 8;

		public void Initialize (int wordLength)
		{
				this.wordLength = wordLength;
				lettersOfWordInHistory = wordHistoryGrid.gameObject.GetComponent<LetterGridController> ();
			
				//wordDataAccessor = gameObject.GetComponent<WordImageAndAudioMapAccessor> ();
				wordHistoryGrid.GetComponent<UIGrid> ().maxPerLine = wordLength;
				letterImageTable = GameObject.Find ("DataTables").GetComponent<LetterImageTable> ();
	
		}

		public void AddCurrentWordToHistory (List<InteractiveLetter> currentWord, bool playSound)
		{
				Word newWord = CreateNewWordAndAddToList (AddLettersOfNewWordToHistory (currentWord));
				//userInputRouter.RequestDisplayImage (newWord.Image, false); //removing this for the experiment.
				if (playSound)
						AudioSourceController.PushClip (newWord.Sound);
			
		}

		string AddLettersOfNewWordToHistory (List<InteractiveLetter> newWord)
		{ 
				StringBuilder currentWordAsString = new StringBuilder ();
				int position = words.Count * wordLength;
				foreach (InteractiveLetter l in newWord) {
						//Debug.Log ("_" + l.Letter () + "_letter");
					
						GameObject letterInWord = lettersOfWordInHistory.CreateLetterBarCell (l.Letter (), l.Image (), (position++) + "", l.CurrentColor (), gameObject);
						//letterInWord.GetComponent<InteractiveLetter> ().SwitchImageTo (letterImageTable.without_line_blank);			


						letterInWord.AddComponent<BoxCollider> ();
						letterInWord.AddComponent<UIDragPanelContents> ();
						Selectable s = letterInWord.GetComponent<Selectable> ();
						s.SelectByTouch ();
						s.Select (false);
						s.ForceActivate = true;
						UIDragPanelContents drag = letterInWord.GetComponent<UIDragPanelContents> ();
						drag.draggablePanel = gameObject.GetComponent<UIDraggablePanel> ();
						currentWordAsString.Append (l.Letter ());
						
				}
				wordHistoryGrid.GetComponent<UIGrid> ().Reposition ();
				return currentWordAsString.ToString ().Trim ().ToLower ();


		}

		public void ClearWordHistory ()
		{
				words.Clear ();
				lettersOfWordInHistory.SetAllLettersToBlank (gameObject);


		}

		Word CreateNewWordAndAddToList (string newWordAsString)
		{
				Word newWord = new Word (newWordAsString);
				//WordImageAndAudioMapAccessor.WordDataParser parser = wordDataAccessor.GetWordData (newWordAsString);
				//newWord.Image = parser.ParseImage ();
				newWord.Sound = AudioSourceController.GetWordFromResources (newWordAsString);
				words.Add (newWord);
				return newWord;

		}
	
		public void SelectedObjectActivated (GameObject selectedAndActivatedLetterCell)
	{       
				Word wordThatLettersBelongTo = RetrieveWordGivenPressedLetterCell (selectedAndActivatedLetterCell);
				Debug.Log (wordThatLettersBelongTo.AsString);
				AudioSourceController.PushClip (wordThatLettersBelongTo.Sound);
				//DeselectAllLettersInWord (selectedAndActivatedLetterCell);
		}

		public void ObjectSelected (GameObject selectedLetterCell)
		{
				if (selectedLetterCell.GetComponent<InteractiveLetter> ().IsBlank ())
						return; //don't select "blanks"
				SelectAllLettersInWord (selectedLetterCell);
		}
	
		public void ObjectDeselected (GameObject pressedLetterCell)
		{
				//DeselectAllLettersInWord (pressedLetterCell);
				return; //for time being-- wont allow for the word history letters to be deselected.
		}

		void DeselectAllLettersInWord (GameObject pressedLetterCell)
		{
				int indexOfFirstLetterInWord = IndexOfWordThatLetterBelongsTo (pressedLetterCell) * wordLength;
				for (int i=indexOfFirstLetterInWord; i<indexOfFirstLetterInWord+wordLength; i++) {
						lettersOfWordInHistory.GetLetterCell (i).GetComponent<Selectable> ().Deselect (false);
				}
		}

		void SelectAllLettersInWord (GameObject pressedLetterCell)
		{
				int indexOfFirstLetterInWord = IndexOfWordThatLetterBelongsTo (pressedLetterCell) * wordLength;
				for (int i=indexOfFirstLetterInWord; i<indexOfFirstLetterInWord+wordLength; i++) {
						lettersOfWordInHistory.GetLetterCell (i).GetComponent<Selectable> ().Select (false);
				}


		}

		int IndexOfWordThatLetterBelongsTo (GameObject pressedLetterCell)
		{
				return (Int32.Parse (pressedLetterCell.name)) / wordLength;

		}
	
		Word RetrieveWordGivenPressedLetterCell (GameObject pressedLetterCell)
		{
				InteractiveLetter l = pressedLetterCell.GetComponent<InteractiveLetter> ();
				int idx = IndexOfWordThatLetterBelongsTo (pressedLetterCell);
				if (idx > -1 && idx < words.Count)
						return words [idx];
				return PsuedoWord;

		}


}
