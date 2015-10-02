using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LetterGridController : PhonoBlocksController
{       
		public GameObject letterGrid;
		public GameObject letterHighlightsGrid;
		public int letterImageWidth;
		public int letterImageHeight;
		float selectH;
		float selectW;

		public int LetterImageHeight {
				get {
						return letterImageHeight;
				}
				set {
						letterImageHeight = value;
				}

		
		
		}

		public Texture2D blankLetter;
		LetterImageTable letterImageTable;

		void Start ()
		{      
				InitializeFieldsIfNecessary ();
		}

		void InitializeFieldsIfNecessary ()
		{
				if (letterGrid == null)
						letterGrid = gameObject;
				if (letterImageTable == null)
						letterImageTable = GameObject.Find ("DataTables").GetComponent<LetterImageTable> ();
				if (blankLetter == null)
						blankLetter = letterImageTable.getBlankLetterImage ();
				if (letterImageWidth == 0 || letterImageHeight == 0) //you can specify dimensions for the image that are different from those of the grid.
						MatchLetterImageToGridCellDimensions (); //but if nothing is specified it defaults to make it the same size as the grid cells.

				if (letterHighlightsGrid) {
						UIGrid high = letterHighlightsGrid.GetComponent<UIGrid> ();
						UIGrid letters = letterGrid.GetComponent<UIGrid> ();
						high.cellWidth = letters.cellWidth;
						high.cellHeight = letters.cellHeight;
						letterHighlightsGrid.transform.position = letterGrid.transform.position;


						selectH = letterImageHeight / 1.3f;
						selectW = letterImageWidth / 1.3f;

				}
	
	
		}

		public void RemoveImageOfLetter (int position)
		{

				GameObject letterCell = GetLetterCell (position);
				letterCell.GetComponent<InteractiveLetter> ().SwitchImageTo (letterImageTable.without_line_blank);


		}

		public void ToggleHighlightAt (int position)
		{ 

				GameObject highlightCell = GetCell (position, letterHighlightsGrid);
				bool val = highlightCell.GetComponent<UITexture> ().enabled;
				highlightCell.GetComponent<UITexture> ().enabled = !val;

		}

		public void SetHighlightAt (int position, bool val)
		{ 
		
				GameObject highlightCell = GetCell (position, letterHighlightsGrid);
				
				if (highlightCell)
						highlightCell.GetComponent<UITexture> ().enabled = val;
		
		}

		void MatchLetterImageToGridCellDimensions ()
		{
			
				UIGrid grid = letterGrid.GetComponent<UIGrid> ();
				letterImageWidth = (int)grid.cellWidth;
				letterImageHeight = (int)grid.cellHeight;

		}
	
		public void InitializeBlankLetterSpaces (int numCells, GameObject localUpdaterOfLetterCell)
		{
        InitializeFieldsIfNecessary ();
    
				for (int i= 0; i<numCells; i++) {
				
						CreateLetterBarCell (" ", blankLetter, i + "", Color.white, localUpdaterOfLetterCell);
				}
				RepositionGrids ();
		}

		public void RepositionGrids ()
		{
				letterGrid.GetComponent<UIGrid> ().Reposition ();
				letterHighlightsGrid.GetComponent<UIGrid> ().Reposition ();

		}

		public void SetAllLettersToBlank (GameObject localUpdaterOfLetters)
		{
				SetLettersToBlank (0, transform.childCount, localUpdaterOfLetters);
		}

		public void SetLettersToBlank (int startingFrom, int number, GameObject localUpdaterOfLetterCell)
		{
				for (int i=startingFrom; i<startingFrom+number; i++) {
						UpdateLetter (localUpdaterOfLetterCell, i, " ", Color.white);
			
				}


		}

		public InteractiveLetter UpdateLetter (GameObject localUpdaterOfLetterCell, int position, String letter, Color newNonLockColour)
		{
				
				InteractiveLetter l = LetterAtPosition (position);
				bool attemptErasure = letter.Equals (" ");
	
				Texture2D letterImage = CopyAndScaleTexture (letterImageWidth, letterImageHeight, letterImageTable.GetLetterImageFromLetter (letter));
				l.UpdateLetter (letter, letterImage, newNonLockColour);
			
				l.Handler = localUpdaterOfLetterCell;
			
				return l;
		
		}

		public InteractiveLetter UpdateLetter (GameObject localUpdaterOfLetterCell, int position, String letter)
		{     
				return UpdateLetter (localUpdaterOfLetterCell, position, letter, Color.white);

		}

		public InteractiveLetter UpdateLetter (int position, Color c)
		{
				
				InteractiveLetter l = LetterAtPosition (position);
				l.UpdateDefaultColour (c);
				return l;

		}

    public InteractiveLetter UpdateLetterImage(int position, Texture2D img)
    {

        InteractiveLetter l = LetterAtPosition(position);
        l.UpdateLetterImage(img);
        return l;

    }

    public InteractiveLetter LetterAtPosition (int position)
		{

        GameObject cell = GetLetterCell(position);
       //if(ReferenceEquals(cell,null)) Debug.Log("null component at " + position + " position of letter ");
        return cell.GetComponent<InteractiveLetter>(); 
    }

		public GameObject GetLetterCell (int position)
		{     
			
				return GetCell (position, letterGrid);
		}

		public GameObject GetCell (int position, GameObject fromGrid)
		{     
				if (position < fromGrid.transform.childCount && position > -1) {
						return fromGrid.transform.Find (position + "").gameObject; //cell to update
				} else {

						return null;
			
			}
		}

		public GameObject CreateLetterBarCell (String letter, Texture2D tex2D, string position, Color c, GameObject localUpdaterOfLetterCell)
		{ 
				Texture2D tex2dCopy = CopyAndScaleTexture (letterImageWidth, letterImageHeight, tex2D);
				UITexture ut = NGUITools.AddChild<UITexture> (letterGrid);
				ut.material = new Material (Shader.Find ("Unlit/Transparent Colored"));
				ut.shader = Shader.Find ("Unlit/Transparent Colored");
				ut.mainTexture = tex2dCopy;
				ut.color = c;
				BoxCollider b = ut.gameObject.AddComponent<BoxCollider> ();
				b.isTrigger = true;
				b.size = new Vector2 (.6f, .6f);

				Selectable s = ut.gameObject.AddComponent<Selectable> ();


			
				s.ActivateByTouch ();
				s.SelectBySwipe ();
				s.Select (false);
				//s.ForceActivate = true;
				InteractiveLetter l = ut.gameObject.AddComponent<InteractiveLetter> ();
				l.Trigger = b;
				l.UpdateLetter (letter, tex2dCopy, c);
				l.Handler = localUpdaterOfLetterCell;
				ut.gameObject.name = position;
				ut.MakePixelPerfect ();
				

		       

				if (letterHighlightsGrid) {
						UITexture selectHighlight = NGUITools.AddChild<UITexture> (letterHighlightsGrid);
						selectHighlight.transform.localScale = new Vector2 (selectW, selectH);
						selectHighlight = SetShaders (selectHighlight);
						selectHighlight.mainTexture = CopyAndScaleTexture (selectW, selectH, LetterImageTable.SelectLetterImage);
						selectHighlight.enabled = false;
						selectHighlight.gameObject.name = position;
				}
	
		        
				return ut.gameObject;

		}

		UITexture SetShaders (UITexture ut)
		{
				ut.material = new Material (Shader.Find ("Unlit/Transparent Colored"));
				ut.shader = Shader.Find ("Unlit/Transparent Colored");

				return ut;
		}
	
		Texture2D CopyAndScaleTexture (float w, float h, Texture tex2D)
		{
				Texture2D tex2dCopy = Instantiate (tex2D) as Texture2D;
				TextureScale.Bilinear (tex2dCopy, (int)w, (int)h);
				return tex2dCopy;
		}

		public List<InteractiveLetter> GetLetters (bool skipBlanks)
		{
				List<InteractiveLetter> list = new List<InteractiveLetter> ();
				foreach (Transform child in letterGrid.transform) {
						InteractiveLetter letter = child.GetComponent<InteractiveLetter> ();
						if (!skipBlanks || !letter.IsBlank ()) {
						
								list.Add (letter);
						}


				}
				return list;
		
		}




	




	  
	
}
	