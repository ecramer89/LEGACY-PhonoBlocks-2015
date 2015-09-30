using UnityEngine;
using System.Collections;

//I think that Perry saved a "string" for each user,
//but he formatted the string in such a way that he could distinguish different kinds of data within it.
public class TestPlayerPrefs : MonoBehaviour {

	public GUIText message;

	// Use this for initialization
	void Start () {
		Debug.Log (PlayerPrefs.GetString ("message"));
	}
	
	void OnGUI() {
		if (GUI.Button (new Rect (10, 10, 150, 100), "SAVE SOMETHING"))
						PlayerPrefs.SetString ("message", "HELLO WORLD!");
		
	}
}
