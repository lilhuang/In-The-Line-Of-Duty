using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Directions : MonoBehaviour {
	public Image titleImage;
	public Text pressEnter1;
	public Text pressEnter2;
	public Text pressEnter3;
	public Text Directions1;
	public Text Directions2;
	public Text Directions3;
	public Text Directions4;
	public Text Directions5;
	public Text Directions6;

	// Use this for initialization
	void Start () {
		titleImage.enabled = true;
		pressEnter1.enabled = true;

		pressEnter2.enabled = false;
		pressEnter3.enabled = false;
		Directions1.enabled = false;
		Directions2.enabled = false;
		Directions3.enabled = false;
		Directions4.enabled = false;
		Directions5.enabled = false;
		Directions6.enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return)) {
			if (titleImage.enabled && pressEnter1.enabled) {
				titleImage.enabled = false;
				pressEnter1.enabled = false;

				pressEnter2.enabled = true;
				Directions1.enabled = true;
				Directions1.GetComponent<TypeText1> ().StartTyping ();
			} else if (pressEnter2.enabled && Directions1.enabled) {
				Directions1.enabled = false;
				Directions2.enabled = true;
				Directions2.GetComponent<TypeText1> ().StartTyping ();
			} else if (pressEnter2.enabled && Directions2.enabled) {
				Directions2.enabled = false;
				Directions3.enabled = true;
				Directions3.GetComponent<TypeText1> ().StartTyping ();
			} else if (pressEnter2.enabled && Directions3.enabled) {
				Directions3.enabled = false;
				Directions4.enabled = true;
				Directions4.GetComponent<TypeText1> ().StartTyping ();
			} else if (pressEnter2.enabled && Directions4.enabled) {
				Directions4.enabled = false;
				Directions5.enabled = true;
				Directions5.GetComponent<TypeText1> ().StartTyping ();
			} else if (pressEnter2.enabled && Directions5.enabled) {
				Directions5.enabled = false;
				pressEnter2.enabled = false;

				Directions6.enabled = true;
				Directions6.GetComponent<TypeText1> ().StartTyping ();
				pressEnter3.enabled = true;
			} else if (pressEnter3.enabled && Directions6.enabled) {
				DelayedRestart (2f);
			}
		}
	}

	public void DelayedRestart(float delay) {
		Invoke("Restart", delay);
	}

	void Restart() {
		SceneManager.LoadScene ("__Scene_0");
	}
}
