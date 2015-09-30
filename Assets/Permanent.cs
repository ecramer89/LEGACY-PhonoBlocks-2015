using UnityEngine;
using System.Collections;

public class Permanent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad (gameObject);
	}
	

}
