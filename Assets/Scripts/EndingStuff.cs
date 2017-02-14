using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class EndingStuff : MonoBehaviour {
	public Text lost;
	public Text options;
	public Text newHighScore;
	public Text newTiedScore;
	public Text score;

	public AudioSource audioPlayer;

	public int numFramesBeforeHighScore;
	public int numFramesBeforeTiedScore;
	public int numFramesBeforeFail;

	public bool canAcceptInput;


	// Use this for initialization
	void Start () {
		int maxScore = 0;
		string name = "nobody";
		using (StreamWriter outputfile = new StreamWriter ("Assets/Resources/scores.txt")) {
			foreach (string key in GameController.gc.prev_scores.Keys) {
				outputfile.WriteLine (key + " " + GameController.gc.prev_scores [key]);
				if (key != GameController.gc.player_name && GameController.gc.prev_scores [key] > maxScore) {
					maxScore = GameController.gc.prev_scores [key];
					name = key;
				}
			}
		}
		if (GameController.gc.num_points == maxScore) {
			numFramesBeforeHighScore = 0;
			numFramesBeforeTiedScore = 60;
			canAcceptInput = false;
		} else if (GameController.gc.num_points > maxScore) {
			numFramesBeforeHighScore = 60;
			numFramesBeforeTiedScore = 0;
			canAcceptInput = false;
		} else {
			canAcceptInput = true;
		}
		numFramesBeforeFail = 0;

		newTiedScore.text = "YOU TIED FOR HIGH SCORE\nWITH " + name + "!!!";
		lost.enabled = true;
		options.enabled = true;
		newHighScore.enabled = false;
		newTiedScore.enabled = false;
		score.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (numFramesBeforeHighScore > 0) {
			numFramesBeforeHighScore--;
			if (numFramesBeforeHighScore == 0) {
				lost.enabled = false;
				newHighScore.enabled = true;
				score.text = "0";
				score.GetComponent<ScoreIncrease> ().number = 0;
				score.GetComponent<ScoreIncrease> ().finalScore = GameController.gc.num_points;
				score.enabled = true;
				score.GetComponent<ScoreIncrease> ().IncrementScore ();
				canAcceptInput = true;
				audioPlayer.Play ();
			}
		} else if (numFramesBeforeTiedScore > 0) {
			numFramesBeforeTiedScore--;
			if (numFramesBeforeTiedScore == 0) {
				lost.enabled = false;
				newTiedScore.enabled = true;
				score.text = "0";
				score.GetComponent<ScoreIncrease> ().number = 0;
				score.GetComponent<ScoreIncrease> ().finalScore = GameController.gc.num_points;
				score.enabled = true;
				score.GetComponent<ScoreIncrease> ().IncrementScore ();
				canAcceptInput = true;
				audioPlayer.Play ();
			}
		}

		if (canAcceptInput && Input.GetKeyDown (KeyCode.Return)) {
			SceneManager.LoadScene ("__Scene_0");
		} else if (canAcceptInput && Input.GetKeyDown (KeyCode.Q)) {
			SceneManager.LoadScene ("OpeningScene");
		}
		
	}
}
