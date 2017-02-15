using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreIncrease : MonoBehaviour {
	public int number;
	public int finalScore;
	public Text scoreText;
	public bool incrementing;

	// Use this for initialization
	void Start () {
		number = 0;
		incrementing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (incrementing) {
			print ("gonna call increment score function");
			IncrementScore ();
		}
	}

	public void IncrementScore() {
		print ("called increment score");
		if (finalScore <= 0) {
			print ("negative score");
			scoreText.text = "Your score: " + finalScore;
			EndingStuff.ending.canAcceptInput = true;
			incrementing = false;
		}
		else if (number <= finalScore) {
			print ("doo gon increment dat shit");
			scoreText.text = "Your score: " + number;
			print (scoreText.text);
			number++;
			print ("number is " + number);
			//Invoke// ("IncrementScore", 0.01f);
//			IncrementScore();
		} else {
			scoreText.text = "Your score: " + finalScore;
			print ("done incrementing");
			EndingStuff.ending.canAcceptInput = true;
			incrementing = false;
		}
	}
}
