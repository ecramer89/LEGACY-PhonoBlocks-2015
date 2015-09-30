using UnityEngine;
using System.Collections;

public class UserImage : MonoBehaviour {    
    void Start () {
        if (DataManager.currentUserID != "")
        {
            Texture2D currentUserImage = DataManager.currentUserImage;
            Rect imageRect = new Rect(0, 0, currentUserImage.width, currentUserImage.height);
            this.GetComponent<SpriteRenderer>().sprite = Sprite.Create(currentUserImage, imageRect, new Vector2(0.5f, 0.5f), 40);
            this.gameObject.AddComponent<BoxCollider2D>();
        }
	}	
}
