using UnityEngine;
using System.Collections;

public class CheckedWordImageController : MonoBehaviour
{
		public GameObject checkedWordImage;
		UITexture img;
		BoxCollider clickTrigger;
		long showTime = -1;
		bool disableTextureOnPress;
		public long defaultDisplayTime = 2000;

		void Start ()
		{
				img = checkedWordImage.GetComponent<UITexture> ();
				img.enabled = false;
				clickTrigger = checkedWordImage.GetComponent<BoxCollider> ();
				clickTrigger.enabled = false;
		}

		public void ShowImage (Texture2D newimg, long showTime)
		{
				this.showTime = showTime;
				SetAndEnableTexture (newimg);


		}

		public void ShowImage (Texture2D newimg, bool disableTextureOnPress)
		{
				if (newimg != null) {
						if (disableTextureOnPress) {
								this.disableTextureOnPress = disableTextureOnPress;
								SetAndEnableTexture (newimg);
								clickTrigger.enabled = true;
						} else
								ShowImage (newimg, defaultDisplayTime);
				}
		}

		void SetAndEnableTexture (Texture2D newImg)
		{
				img.mainTexture = newImg;
				img.enabled = true;
		}

		void OnPress (bool isPressed)
		{
				if (isPressed && disableTextureOnPress) {
						EndInputControlledDisplay ();
				}
		}

		public void EndDisplay ()
		{
				if (disableTextureOnPress) {
						EndInputControlledDisplay ();
				}
				if (showTime > 0) {
						EndTimedDisplay ();
				}

		}

		void Update ()
		{
				if (showTime > 0)
						showTime--;
				if (showTime == 0) {
						EndTimedDisplay ();
				}
				
		}

	public bool WordImageIsOnDisplay(){
		return img.enabled;


		}

		void EndTimedDisplay ()
		{
				img.enabled = false;
				showTime = -1;

		}

		void EndInputControlledDisplay ()
		{
				img.enabled = false;
				disableTextureOnPress = false;
				clickTrigger.enabled = false;

		}


}
