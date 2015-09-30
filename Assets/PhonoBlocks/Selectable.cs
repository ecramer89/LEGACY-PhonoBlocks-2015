using UnityEngine;
using System.Collections;

//make it so that it "does something" when selected (optionally)
public class Selectable : MonoBehaviour
{

		GameObject handler;
		bool forceActivate;

		public bool ForceActivate {
				get {
						return forceActivate;
				}
				set {
						forceActivate = value;
				}
		}

		public GameObject Handler {
				get {
						return handler;
			
				}
		
				set {
						handler = value;
			
				}
		
		}

		public enum Trigger
		{
				PRESS,
				SWIPE,
				CUSTOM

		}


		bool locked;

		public bool Locked {
				get {
						return locked;
				}
		}

		Trigger selectionTrigger = Trigger.CUSTOM;
		Trigger activationTrigger = Trigger.CUSTOM;
	
		public void SelectByTouch ()
		{
				selectionTrigger = Trigger.PRESS;
		}

		public void ActivateByTouch ()
		{
				activationTrigger = Trigger.PRESS;

		}

		public void SelectBySwipe ()
		{
				selectionTrigger = Trigger.SWIPE;


		}

		public void ActivateBySwipe ()
		{
				activationTrigger = Trigger.SWIPE;

		}

		bool selected = false;

		public bool Selected {
				get {
						return selected;
				}
		
		}

		public void Select (bool notifyHandler, bool force)
		{       
				//if (force || UserInputRouter.global.AcceptingSelectionOrActivation ()) {
				selected = true;
				gameObject.SendMessage ("ChangeStateToSignalSelected", SendMessageOptions.DontRequireReceiver);
				if (notifyHandler)
						handler.SendMessage ("ObjectSelected", gameObject);
				//}
		
		}

		public void Select (bool notifyHandler)
		{       
				Select (notifyHandler, false);
		}

		public void Deselect (bool notifyHandler)
		{       
				selected = false;
				gameObject.SendMessage ("ChangeStateToSignalDeselected", SendMessageOptions.DontRequireReceiver);
				if (notifyHandler)
						handler.SendMessage ("ObjectDeselected", gameObject);
		}

		public void OnPress (bool pressed)
		{
				if (!locked) {
					
						if (handler != null) {
								if (pressed && Input.GetMouseButtonDown (0)) { //left clicks activate; to distinguish from changing letter image
									
										if (selected) {
										
												if (activationTrigger == Trigger.PRESS) {
													
														Activate ();
												} else if (selectionTrigger == Trigger.PRESS) //job of handler to deselect if the activation and selection triggers are identical.
														Deselect (true);
										} else if (selectionTrigger == Trigger.PRESS) {
												Select (true);
										
										}
								}
						}

				}
		}

		void Update ()
		{
                
				if (!locked) {
		
						if (handler != null) {
								if (selectionTrigger == Trigger.SWIPE) {
									
										if (MouseIsOverSelectable ()) {
										
												if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.RIGHT) {
														Select (true);
							
												}
												if (SwipeDetector.swipeDirection == SwipeDetector.Swipe.LEFT) {
														Deselect (true);
												}
					             
										}

								}


						}

				}
		}

		bool MouseIsOverSelectable ()
		{
				Vector3 mouse = SwipeDetector.GetTransformedMouseCoordinates ();

				return (Vector3.Distance (mouse, gameObject.transform.position) < .16);	
		}
	   
		public void Activate ()
		{    
				//if (UserInputRouter.global.AcceptingSelectionOrActivation () || forceActivate)
				handler.SendMessage ("SelectedObjectActivated", gameObject, SendMessageOptions.DontRequireReceiver);


		}

		public void Lock ()
		{
				locked = true;
				gameObject.SendMessage ("ChangeStateToSignalLocked", SendMessageOptions.DontRequireReceiver);
		}

		public void UnLock ()
		{
				locked = false;
				gameObject.SendMessage ("ChangeStateToSignalUnlocked", SendMessageOptions.DontRequireReceiver);
		}


		

}
