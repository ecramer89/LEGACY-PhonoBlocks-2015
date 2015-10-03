using UnityEngine;
using System.Collections;
using System;

//container for the letter of this cell.
//stores data the image, the letter and the color
//but isn't responsible for using this data to do anything.
public class InteractiveLetter : PhonoBlocksController
{

		String letter;
		UnityEngine.Color defaultColour;
		bool isLocked = false;

		public bool IsLocked {
				get {
						return isLocked;
				}
		}

		Color lockedColor = Color.black;
		UITexture selectHighlight;
		BoxCollider trigger;
		LetterSoundComponent lc;
		int flashCounter = 0;
		int timesToFlash = 5;
		float secondsDelayBetweenFlashes = .2f;
		int idxAsArduinoControlledLetter;

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

		public GameObject Handler {
				get {
						return GetComponent<Selectable> ().Handler;

				}

				set {
						GetComponent<Selectable> ().Handler = value;

				}

		}

		public void Lock ()
		{
				ChangeColourOfUITexture (lockedColor);
				isLocked = true;
		}

		public void UnLock ()
		{
				ChangeColourOfUITexture (defaultColour);
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

		public IEnumerator Flash ()
		{

				int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);
	
				while (flashCounter<timesToFlash) {
		
						if (flashCounter % 2 == mod_To_end_on) {
								ChangeColourOfUITexture (defaultColour);
							

						} else {
								ChangeColourOfUITexture (lockedColor);
								

						}
						flashCounter++;
					
						yield return new WaitForSeconds (secondsDelayBetweenFlashes);
				}

				flashCounter = 0;
				



		}

		public void ChangeColourOfUITexture (UnityEngine.Color c_)
		{
				if (c_ == Color.clear)
						c_ = Color.white;

				GetComponent<UITexture> ().color = c_;
				//change colour of counterpart tangible letter
				ChangeColourOfTangibleCounterpartIfThereIsOne (c_);
			
		}

		public void ChangeColourOfTangibleCounterpartIfThereIsOne (UnityEngine.Color c_)
		{
				if (letter [0] == ' ')
						c_ = Color.black;
				if (userInputRouter != null)
				if (userInputRouter.IsArduinoMode ()) 
                
		
						userInputRouter.arduinoLetterInterface.ColorNthTangibleLetter (IdxAsArduinoControlledLetter, c_);
				


		}

		public void UpdateLetter (String letter_, Texture2D img_, UnityEngine.Color c_)
		{
				UpdateLetter (letter_, img_);
				UpdateDefaultColour (c_);

			
		}

		public void UpdateLetterImage (Texture2D img_)
		{
				gameObject.GetComponent<UITexture> ().mainTexture = img_;


		}

		public void UpdateLetter (String letter_, Texture2D img_)
		{
				//if (idxAsArduinoControlledLetter == -2)
				//		return;
				letter = letter_;

				//de-select this cell if it was selected
				Selectable s = gameObject.GetComponent<Selectable> ();
				s.Select (false, true);
			
			
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
				if(!IsLocked) ChangeColourOfUITexture (defaultColour);
				
				/*if (!GetComponent<Selectable> ().Selected) {
					
						Darken ();

				}*/
		}

		public void ChangeStateToSignalSelected ()
		{
				
				Lighten ();
			
		}
	
		public void ChangeStateToSignalDeselected ()
		{
			
				Darken ();
			
		
		}

		void Lighten ()
		{
				ChangeStateToSignalUnlocked ();


		}

		void Darken ()
		{
				ChangeColourOfUITexture (Color.black);

		}

		public void ChangeStateToSignalLocked ()
		{
				
				ChangeColourOfUITexture (Color.black);
			


		}

		public void  SwitchImageTo (Texture2D img)
		{
				
		        
				GetComponent<UITexture> ().mainTexture = img;


		}

		public void ChangeStateToSignalUnlocked ()
		{
				if (defaultColour == Color.clear)
						defaultColour = Color.white;
				ChangeColourOfUITexture (defaultColour);
			
				
		
		}


		/*
	    * 
	    * this is a hack to handle cases where the platforms cant
	    * read the letters properly.
	    * basically it enables the tutor to manually override the physical with a screen letter
	    * using the keyboard. click on the letter and then go head,
	    * type a letter from the keyboard and the system will just treat it as one...
	    * */
		int scaleTimer = 0;
		int i_w, i_h, s_w, s_h;

		public void OnPress (bool pressed)
		{
				
				if (pressed) {

						Handler.SendMessage ("LetterClicked", gameObject, SendMessageOptions.DontRequireReceiver);
				
			
				}


				
		}






	   




}
