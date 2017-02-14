using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrease : MonoBehaviour {
	public int number;
	public int finalScore;
	public Text scoreText;

	// Use this for initialization
	void Start () {
		number = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void IncrementScore() {
		while (number < finalScore) {
			scoreText.text = "Your score: " + number.ToString();
			number++;
			Invoke ("IncrementScore", 0.01f);
		}
	}
}
