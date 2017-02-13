using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController gc;
	public float delete_delay;
	public float delete_delay_value;
	public int max_segments;
	public int num_segments;
	public int num_points;
	public float enemySpawnPadding;
	public float enemySpawnRate;
	public Vector3 pleb_init_spot;
	public float pleb_init_spot_radius;
	public int num_init_plebs;
	public float restart_delay;
	public int num_blasts;
	public float[] prob_enemy; //0 is Enemy, 1 is Enemy1, 2 is Enemy2, etc.
	public float[] prob_pickup; //0 is pt+5, 1 is pt+10, 2 is pt+50, 3 is pb+5, 4 is pb+10, 5 is pbx2

	public List<GameObject> lines;
	public List<GameObject> plebs;
	public List<GameObject> enemies;
	public Dictionary<string, int> prev_scores;

	public GameObject line_prefab;
	public GameObject segment_prefab;
	public GameObject pleb_prefab;
	public GameObject enemy_prefab;
	public GameObject enemy1_prefab;
	public GameObject enemy2_prefab;
	public GameObject enemy3_prefab;
	public GameObject pickup_pt_five_prefab;
	public GameObject pickup_pt_ten_prefab;
	public GameObject pickup_pt_fifty_prefab;
	public GameObject pickup_pb_five_prefab;
	public GameObject pickup_pb_ten_prefab;
	public GameObject pickup_pbxtwo_prefab;

	public Text num_plebs_text;
	public Text points_text;

	public TextAsset scores;
	public string player_name;

	void Start() {
		gc = this;
		restart_delay = 6f;
		delete_delay_value = 12f;
		delete_delay = delete_delay_value;
		num_segments = 0;
		num_points = 0;
		max_segments = 100;
		num_init_plebs = 5;
		enemySpawnPadding = 0.5f;
		enemySpawnRate = 5f;
		pleb_init_spot = Vector3.zero;
		pleb_init_spot_radius = 20f;
		num_blasts = 0;
		prob_enemy = new float[4];
		prob_enemy [0] = 1f;
		prob_enemy [1] = 0f;
		prob_enemy [2] = 0f;
		prob_enemy [3] = 0f;

		prob_pickup = new float[6];
		prob_pickup [0] = 0.4f;
		prob_pickup [1] = 0.3f;
		prob_pickup [2] = 0.0f;
		prob_pickup [3] = 0.3f;
		prob_pickup [4] = 0f;
		prob_pickup [5] = 0f;

		for (int i = 0; i < num_init_plebs; i++) {
			GameObject go = Instantiate (pleb_prefab) as GameObject;
			Vector3 init_pos = pleb_init_spot + UnityEngine.Random.insideUnitSphere * pleb_init_spot_radius;
			init_pos.z = 0;
			go.transform.position = init_pos;
			plebs.Add (go);
		}
		if (scores.text != "") {
			string[] lines = scores.text.Split('\n');
			foreach (string line in lines) {
				string[] this_line = scores.text.Split(' ');
				string name = this_line [0];
				int score = Int32.Parse(this_line [1]);
				prev_scores.Add (name, score);
			}
		}

		Invoke ("SpawnEnemies", enemySpawnRate);
	}

	public void CreatePlebs(int num_new_plebs) {
		for (int i = 0; i < num_new_plebs; i++) {
			GameObject go = Instantiate (pleb_prefab) as GameObject;
			Vector3 init_pos = pleb_init_spot + UnityEngine.Random.insideUnitSphere * pleb_init_spot_radius;
			init_pos.z = 0;
			go.transform.position = init_pos;
			plebs.Add (go);
		}
	}

	void Update() {
		if (lines.Count > 0) {
			delete_delay--;
			if (delete_delay <= 0) {
				DeleteFirstSeg ();
			}
		}
		ProcessLine ();
		ManageLevel ();
		SpawnPickups ();
		ProcessBlast ();

		print ("max segments is " + max_segments);
		print ("num current segments is " + num_segments);

		if (plebs.Count == 0) {
			print ("out of plebs");
			//show losing text
			//losing sound
			//save score
			if (prev_scores.ContainsKey (player_name) && prev_scores [player_name] < num_points) {
				prev_scores [player_name] = num_points;
			} else if (!prev_scores.ContainsKey (player_name)) {
				prev_scores.Add (player_name, num_points);
			}
			//DelayedRestart(restart_delay);
			Restart();
		}
		points_text.text = "Points: " + num_points;
		num_plebs_text.text = "Citizens left: " + plebs.Count;
	}

	public void DeleteFirstSeg() {
		GameObject first_seg = lines [0].GetComponent<DrawLine> ().segmentsList [0];
		lines [0].GetComponent<DrawLine> ().segmentsList.Remove (first_seg);
		Destroy (first_seg);
		num_segments--;
		if (delete_delay <= 0) {
			delete_delay = delete_delay_value;
		}
		if (lines [0].GetComponent<DrawLine> ().segmentsList.Count == 0) {
			GameObject first_line = lines [0];
			lines.Remove (first_line);
			Destroy (first_line);
		}
	}

	void ProcessBlast() {
		if (Input.GetKeyDown (KeyCode.B) && num_blasts > 0) {
			foreach (GameObject go in GameController.gc.enemies) {
				Destroy (go);
			}
			GameController.gc.enemies.Clear ();
			num_blasts--;
		}
	}

	void ProcessLine() {
		if (Input.GetMouseButtonDown (0)) {
			CreateLine ();
		}
	}

	void CreateLine() {
		GameObject new_line = Instantiate (line_prefab) as GameObject;
		lines.Add (new_line);
	}

	void SpawnPickups() {
		int temp = UnityEngine.Random.Range (1, 500);
		if (temp == 1) {
			GameObject go;
			float temp_index = UnityEngine.Random.Range (0, 1);
			if (temp_index <= prob_pickup [0]) {
				go = Instantiate (pickup_pt_five_prefab) as GameObject;
			} else if (temp_index <= (prob_pickup [0] + prob_pickup[1])) {
				go = Instantiate (pickup_pt_ten_prefab) as GameObject;
			} else if (temp_index <= (prob_pickup [0] + prob_pickup[1] + prob_pickup[2])) {
				go = Instantiate (pickup_pt_fifty_prefab) as GameObject;
			} else if (temp_index <= (prob_pickup [0] + prob_pickup[1] + prob_pickup[2] + prob_pickup[3])) {
				go = Instantiate (pickup_pb_five_prefab) as GameObject;
			} else if (temp_index <= (prob_pickup [0] + prob_pickup[1] + prob_pickup[2] + prob_pickup[3] + prob_pickup[4])) {
				go = Instantiate (pickup_pb_ten_prefab) as GameObject;
			} else {
				go = Instantiate (pickup_pbxtwo_prefab) as GameObject;
			}
			Vector3 pos = Vector3.zero;
			float x = UnityEngine.Random.Range (Utils.camBounds.min.x, Utils.camBounds.max.x);
			float y = UnityEngine.Random.Range (Utils.camBounds.min.y, Utils.camBounds.max.y);
			pos.x = x;
			pos.y = y;
			go.transform.position = pos;
		}
	}

	void SpawnEnemies() {
		float temp0 = UnityEngine.Random.Range (0f, 1f);
		GameObject go;
		if (temp0 <= prob_enemy [0]) {
			go = Instantiate (enemy_prefab) as GameObject;
		} else if (temp0 <= prob_enemy [0] + prob_enemy [1]) {
			go = Instantiate (enemy1_prefab) as GameObject;
		} else {
			go = Instantiate (enemy2_prefab) as GameObject;
		}
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x - enemySpawnPadding;
		float xMax = Utils.camBounds.max.x + enemySpawnPadding;
		float yMin = Utils.camBounds.min.y - enemySpawnPadding;
		float yMax = Utils.camBounds.max.y + enemySpawnPadding;
		int temp = UnityEngine.Random.Range (0, 3);
		if (temp == 0) {
			pos.x = UnityEngine.Random.Range (xMin, xMax);
			pos.y = UnityEngine.Random.Range (Utils.camBounds.max.y, yMax);
		} else if (temp == 1) {
			pos.x = UnityEngine.Random.Range (xMin, xMax);
			pos.y = UnityEngine.Random.Range (yMin, Utils.camBounds.min.y);
		} else if (temp == 2) {
			pos.x = UnityEngine.Random.Range (Utils.camBounds.max.x, xMax);
			pos.y = UnityEngine.Random.Range (yMin, yMax);
		} else if (temp == 3) {
			pos.x = UnityEngine.Random.Range (xMin, Utils.camBounds.min.x);
			pos.y = UnityEngine.Random.Range (yMin, yMax);
		}
		go.transform.position = pos;
		enemies.Add (go);
		Invoke("SpawnEnemies", enemySpawnRate);
	}

	void ManageLevel() {
		if (num_points >= 50 && num_points < 100) {
			max_segments = 75;
		} else if (num_points >= 100 && num_points < 200) {
			max_segments = 50;
			enemySpawnRate = 3f;
			prob_enemy [0] = 0.65f;
			prob_enemy [1] = 0.2f;
			prob_enemy [2] = 0f;
			prob_enemy [3] = 0.15f;
		} else if (num_points >= 200 && num_points < 500) {
			max_segments = 35;
			delete_delay_value = 6f;
		} else if (num_points >= 500 && num_points < 1000) {
			max_segments = 25;
			prob_enemy [0] = 0.5f;
			prob_enemy [1] = 0.2f;
			prob_enemy [2] = 0.2f;
			prob_enemy [3] = 0.1f;
		} else if (num_points >= 1000 && num_points < 5000) {
			max_segments = 10;
			enemySpawnRate = 1f;
		} else if (num_points >= 5000) {
			max_segments = 5;
			enemySpawnRate = 0.5f;
		}
	}

	public void DelayedRestart(float delay) {
		print ("got to delayed restart");
		Invoke("Restart", delay);
	}

	void Restart() {
		SceneManager.LoadScene ("EndScene");
	}

}
