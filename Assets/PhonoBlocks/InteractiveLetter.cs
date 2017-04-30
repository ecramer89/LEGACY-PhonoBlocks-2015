using UnityEngine;
using System.Collections;
using System;

//container for the letter of this cell.
//stores data the image, the letter and the color
//but isn't responsible for using this data to do anything.
public class InteractiveLetter : PhonoBlocksController
{
    //changed file!
		String letter;
		Color defaultColour;

		public Color DefaultColour {
				get {

						return defaultColour;
				}
		}

		bool isLocked = false;

		public bool IsLocked {
				get {
						return isLocked;
				}
		}

		bool isSelected = false;

		public bool Selected {
				get {
						return isSelected;
				}
		}

		public delegate void SelectAction (bool wasSelected,GameObject o);

		public static event SelectAction LetterSelectedDeSelected;
		public delegate void PressAction (GameObject o);

		public static event PressAction LetterPressed;

		Color lockedColor = Color.clear;
		Color off = Color.black;
		UITexture selectHighlight;
		Color selectColor = Color.clear;

		public void SetSelectColour (Color newColor)
		{
				selectColor = newColor;


		}

		public Color  SelectColour {
				get {
						return selectColor;
				}
		
		
		}

		BoxCollider trigger;
		LetterSoundComponent lc;
		int flashCounter = 0;
		const int defaultTimesToFlash = 1;
		float secondsDelayBetweenFlashes = .5f;
		const int NOT_AN_ARDUINO_CONTROLLED_LETTER = -1;
		int idxAsArduinoControlledLetter = NOT_AN_ARDUINO_CONTROLLED_LETTER; //i.e., if it's a word history controlled letter. you have to "opt in" to be an arduino controlled letter.

		public int IdxAsArduinoControlledLetter {
				set {
						idxAsArduinoControlledLetter = value;
					

				}
				get {
						return idxAsArduinoControlledLetter;
				}
		}

		public LetterSoundComponent LetterSoundComponentIsPartOf {
				get {
						return lc;
				}

				set {
						lc = value;
				}


		}

		public BoxCollider Trigger {
				get {
						return trigger;
				}
				set {

						trigger = value;
				}


		}

		public UITexture SelectHighlight {
				get {
						return selectHighlight;
				}
				set {
						selectHighlight = value;
						selectHighlight.enabled = false;

				}


		}

		public void Lock ()
		{
				if (lockedColor == Color.clear)
						lockedColor = SessionsDirector.colourCodingScheme.GetColorsForOff ();
				UpdateDisplayColour (lockedColor);
				isLocked = true;
		}

		public void UnLock ()
		{
				UpdateDisplayColour (defaultColour);
				isLocked = false;
		}

		public String Letter ()
		{
				return letter;
		}
	
		public Texture2D Image ()
		{
				return (Texture2D)gameObject.GetComponent<UITexture> ().mainTexture;
		}

		public UnityEngine.Color CurrentColor ()
		{
				return gameObject.GetComponent<UITexture> ().color;
		}

		public bool IsBlank ()
		{
				return letter [0] == ' ';
		}

	public IEnumerator Flash(Color flashColor, int timesToFlash = defaultTimesToFlash){
		timesToFlash *= 2;
		int default_color_mod = ((timesToFlash-1) % 2);
		
		while (flashCounter<timesToFlash) {
			
			if (flashCounter % 2 == default_color_mod) {
				UpdateDisplayColour (defaultColour);
			} else {
				UpdateDisplayColour (flashColor);
			}
			flashCounter++;
			
			yield return new WaitForSeconds (secondsDelayBetweenFlashes);
		}
		
		flashCounter = 0;

		}

	public void StartFlash (Color flashOff, int timesToFlash = defaultTimesToFlash)
	{      
		IEnumerator coroutine = Flash (flashOff, timesToFlash);
		StartCoroutine (coroutine);
		
	}
		public void StartFlash (int timesToFlash = defaultTimesToFlash)
		{      

				StartFlash (off, timesToFlash);

		}

		public void UpdateDisplayColour (UnityEngine.Color c_)
		{
				if (c_ == Color.clear)
						c_ = Color.white;

				GetComponent<UITexture> ().color = c_;
				//change colour of counterpart tangible letter
				ChangeColourOfTangibleCounterpartIfThereIsOne (c_);
			
		}

		public void RevertToDefaultColour ()
		{
				UpdateDisplayColour (defaultColour);

		}

    bool IsLockedColour(UnityEngine.Color c_)
    {
        
        return c_.r == lockedColor.r && c_.g == lockedColor.g && c_.b == lockedColor.b;
    }

		public void ChangeColourOfTangibleCounterpartIfThereIsOne (UnityEngine.Color c_)
		{
        if (lockedColor == Color.clear)
            lockedColor = SessionsDirector.colourCodingScheme.GetColorsForOff();
        //on the screen, blank letters are just clear.
        //but we issue the black (0,0,0) colour to the arduino.
        if (letter [0] == ' '||IsLockedColour(c_))
						c_ = Color.black;
       

				if (userInputRouter != null)
				if (IdxAsArduinoControlledLetter != NOT_AN_ARDUINO_CONTROLLED_LETTER && userInputRouter.IsArduinoMode ()) 
						userInputRouter.arduinoLetterInterface.ColorNthTangibleLetter (IdxAsArduinoControlledLetter, c_);
				


		}

		public void UpdateLetter (String letter_, Texture2D img_, UnityEngine.Color c_)
		{
				UpdateLetter (letter_, img_);
				UpdateDefaultColour (c_);

			
		}


		//update the letter images; then after they make any change it will just update them all again
		public void UpdateLetterImage (Texture2D img_)
		{
				gameObject.GetComponent<UITexture> ().mainTexture = img_;


		}

		public void UpdateLetter (String letter_, Texture2D img_)
		{
			
				letter = letter_;

				//de-select this cell if it was selected
				if (isSelected)
						DeSelect ();
			
			
				gameObject.GetComponent<UITexture> ().mainTexture = img_;
			

		}

		public bool HasLetterOrSoundChanged (LetterSoundComponent lc)
		{
				//Debug.Log (ReferenceEquals (this.lc, null) + " " + lc.AsString);
				if (ReferenceEquals (this.lc, null))
						return false; //we do not flash the first time.

				//true if old lc's class differed from new lc's class.
				//(e.g., previously it was the vowel a, now it is the vowel digraph ae.
				if (!this.lc.GetType ().Equals (lc.GetType ()))
						return true;

				//else, the classes are the same. (this is why we can check the int and not the strign representation of sound type)
				//refactor- make sound type an enum

				//same class, bt different letters
				if (!this.lc.AsString.Equals (lc.AsString))
						return true;

				//same class, same letters, but the sound type has changed (e.g., long a to short a; silent e to not silent e)
				if (this.lc.SoundType != lc.SoundType)
						return true;


				return false;

		}

		public void UpdateDefaultColour (UnityEngine.Color c_)
		{
				if (defaultColour == Color.clear)
						defaultColour = Color.white;
				defaultColour = c_;
				if (!IsLocked)
						UpdateDisplayColour (defaultColour);

		}

		public void  SwitchImageTo (Texture2D img)
		{
				
		        
				GetComponent<UITexture> ().mainTexture = img;


		}

		void Update ()
		{
	
				if (!IsBlank ()) { //don't select blank letters
						
						if (MouseIsOverSelectable ()) {
					
								if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.RIGHT) {
												
										Select ();
								}
								if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.LEFT) {
										DeSelect ();				
								}
					
						}
			
				}
		}
	
		bool MouseIsOverSelectable ()
		{
				Vector3 mouse = SwipeDetector.GetTransformedMouseCoordinates ();
		
				return (Vector3.Distance (mouse, gameObject.transform.position) < .3);	
		}

		public void Select (bool notifyObservers=true)
		{
				if (!isSelected && !IsBlank ()) {

						isSelected = true;
						if (selectColor == Color.clear)
								selectHighlight.enabled = true;
						else
								UpdateDisplayColour (selectColor);
						if (notifyObservers && LetterSelectedDeSelected != null)
								LetterSelectedDeSelected (true, gameObject);
				}
		
		}

		public void DeSelect (bool notifyObservers=true)
		{
				if (isSelected) {
						isSelected = false;
						if (selectColor == Color.clear) {
								if (selectHighlight)
										selectHighlight.enabled = false;
						} else
								UpdateDisplayColour (defaultColour);
						if (notifyObservers && LetterSelectedDeSelected != null)
								LetterSelectedDeSelected (false, gameObject);
				}
		}

		public void OnPress (bool pressed)
		{
				if (pressed && LetterPressed != null)
						LetterPressed (gameObject);


		}

}
