using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeText1 : MonoBehaviour {
	public string full_text;
	public int index;
	public char[] textchars;

	public Text direction;


	void Awake() {
		full_text = direction.text;
		direction.text = "";
		index = 0;
		textchars = full_text.ToCharArray ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartTyping() {
		Invoke ("AddLetter", 0.05f);
	}

	void AddLetter() {
		if (index < textchars.Length) {
			direction.text += textchars [index];
			index++;
			Invoke ("AddLetter", 0.05f);
		}
	}
}
