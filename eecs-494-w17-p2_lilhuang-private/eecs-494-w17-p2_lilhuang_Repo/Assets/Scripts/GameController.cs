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
	public float[] prob_pickup; //0 is pt+5, 1 is pt+10, 2 is pt+50, 3 is pb+5, 4 is pb+10, 5 is pbx2, 6 is blast
	public int pickup_drop_prob;
	public float showInitDirectionsFrames;
	public int remainingDamageFrames = 0; //Damagge frames left
	public int remainingDamageFlashes = 0;
	public int showDamageForFrames = 10;
	public int remainingIncreaseFrames = 0; //Damagge frames left
	public int remainingIncreaseFlashes = 0;
	public int showIncreaseForFrames = 10;
	public int remainingFlashFrames = 0;
	public int remainingFlashFlashes = 0;
	public int showFlashForFrames = 10;
	public int bossEnemyDelay = 0;
	public bool hasDroppedBlockedEnemy;
	public bool hasDroppedEncircledEnemy;
	public bool hasShownBlockIncrease;
	public bool hasShownEncircleIncrease;
	public bool hasDroppedP5;
	public bool hasDroppedP10;
	public bool hasDroppedP50;
	public bool hasDroppedC5;
	public bool hasDroppedC10;
	public bool hasDroppedC2;
	public bool hasDroppedBlast;

	public bool startSpawning;

	public Text drawLineDirection;
	public Text keepInScreen;
	public Text howGameEnds;
	public Text blockDirection;
	public Text encircleDirection;
	public Text pointIncreaseDirection_block;
	public Text pointIncreaseDirection_encircle;
	public Text encirclePickupDirection_pt_five;
	public Text encirclePickupDirection_pt_ten;
	public Text encirclePickupDirection_pt_fifty;
	public Text encirclePickupDirection_c_five;
	public Text encirclePickupDirection_c_ten;
	public Text encirclePickupDirection_cxtwo;
	public Text encirclePickupDirection_blast;
	public Text blastStorageDirection;
	public Text bossDirection;

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
	public GameObject enemy4_prefab;
	public GameObject pickup_pt_five_prefab;
	public GameObject pickup_pt_ten_prefab;
	public GameObject pickup_pt_fifty_prefab;
	public GameObject pickup_pb_five_prefab;
	public GameObject pickup_pb_ten_prefab;
	public GameObject pickup_pbxtwo_prefab;
	public GameObject pickup_blast;
	public GameObject arrow;

	public AudioSource audioPlayer;
	public AudioClip pickup;
	public AudioClip collide;
	public AudioClip morePoints;
	public AudioClip died;
	public AudioClip hitEnemy;
	public AudioClip breakLine;

	public Text num_plebs_text;
	public Text points_text;
	public Text num_blasts_text;

	public TextAsset scores;
	public string player_name;

	//public bool getNameFrom

	void Start() {
		gc = this;
		//if (Directions.d != null) {
		player_name = Directions.d.player_name;
		print ("Player name " + player_name);
//		} else {
//			player_name = "none";
//		}
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
		pickup_drop_prob = 200;
		showInitDirectionsFrames = 300;
		prev_scores = new Dictionary<string, int> ();
		startSpawning = false;
		prob_enemy = new float[4];
		prob_enemy [0] = 1f;
		prob_enemy [1] = 0f;
		prob_enemy [2] = 0f;
		prob_enemy [3] = 0f;

		prob_pickup = new float[7];
		prob_pickup [0] = 0.4f;
		prob_pickup [1] = 0.3f;
		prob_pickup [2] = 0.0f;
		prob_pickup [3] = 0.3f;
		prob_pickup [4] = 0f;
		prob_pickup [5] = 0f;
		prob_pickup [6] = 0f;

		for (int i = 0; i < num_init_plebs; i++) {
			GameObject go = Instantiate (pleb_prefab) as GameObject;
			Vector3 init_pos = pleb_init_spot + UnityEngine.Random.insideUnitSphere * pleb_init_spot_radius;
			init_pos.z = 0;
			go.transform.position = init_pos;
			plebs.Add (go);
		}
		if (scores.text != "") {
			string[] lines = scores.text.Split ('\n');
			foreach (string line in lines) {
				if (line != "") {
					string[] this_line = line.Split (' ');
					string name = this_line [0];
//					print ("this line " + line + "!!!");
//					print ("name " + this_line [0] + ".");
//					print ("number should be " + this_line [1] + ".");
					int score = Int32.Parse (this_line [1]);
					if (prev_scores.ContainsKey (name)) {
						prev_scores [name] = score;
					} else {
						prev_scores.Add (name, score);
					}
				}
			}
		}

		foreach (string key in prev_scores.Keys) {
			print (key + " " + prev_scores [key]);
		}

		drawLineDirection.enabled = true;
		keepInScreen.enabled = false;
		howGameEnds.enabled = false;
		blockDirection.enabled = false;
		encircleDirection.enabled = false;
		pointIncreaseDirection_block.enabled = false;
		pointIncreaseDirection_encircle.enabled = false;
		encirclePickupDirection_pt_five.enabled = false;
		encirclePickupDirection_pt_ten.enabled = false;
		encirclePickupDirection_pt_fifty.enabled = false;
		encirclePickupDirection_c_five.enabled = false;
		encirclePickupDirection_c_ten.enabled = false;
		encirclePickupDirection_cxtwo.enabled = false;
		encirclePickupDirection_blast.enabled = false;
		blastStorageDirection.enabled = false;
		bossDirection.enabled = false;
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
		ProcessBlast ();
		if (startSpawning) {
			SpawnPickups ();
		}

		//print ("max segments is " + max_segments);
		//print ("num current segments is " + num_segments);

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
		num_blasts_text.text = "Blasts available: " + num_blasts;

		if (showInitDirectionsFrames > 0) {
			showInitDirectionsFrames--;
			if (showInitDirectionsFrames == 0) {
				if (drawLineDirection.enabled) {
					drawLineDirection.enabled = false;
					keepInScreen.enabled = true;
					showInitDirectionsFrames = 300;
				} else if (keepInScreen.enabled) {
					keepInScreen.enabled = false;
					bossDirection.enabled = true;
					showInitDirectionsFrames = 300;
				} else if (bossDirection.enabled) {
					bossDirection.enabled = false;
					howGameEnds.enabled = true;
					showInitDirectionsFrames = 300;
				} else if (howGameEnds.enabled) {
					howGameEnds.enabled = false;
					Invoke ("SpawnEnemies", enemySpawnRate);
				} else if (blastStorageDirection.enabled) {
					blastStorageDirection.enabled = false;
				} else if (pointIncreaseDirection_block.enabled) {
					pointIncreaseDirection_block.enabled = false;
					pointIncreaseDirection_encircle.enabled = true;
					showInitDirectionsFrames = 150;
				} else if (pointIncreaseDirection_encircle.enabled) {
					pointIncreaseDirection_encircle.enabled = false;
				}
			}
		}

		if (remainingDamageFlashes > 0) {
			//print ("frame number: " + frame);
			//print (remainingDamageFlashes + " damage flashes left");
			if (remainingDamageFrames > 0) {
				//print (remainingDamageFrames + " damage frames left");
				remainingDamageFrames--;
				if (remainingDamageFrames == showDamageForFrames / 2) {
					//print ("no damage frames left!");
					UnshowDamage ();
				}
			} else {
				//print ("decreasing damage flashes left");
				remainingDamageFlashes--;
				ShowDamage (remainingDamageFlashes);
			}
		} else {
			//print ("no damage flashes left!");
			UnshowDamage ();
		}

		if (remainingIncreaseFlashes > 0) {
			//print ("frame number: " + frame);
			//print (remainingDamageFlashes + " damage flashes left");
			if (remainingIncreaseFrames > 0) {
				//print (remainingDamageFrames + " damage frames left");
				remainingIncreaseFrames--;
				if (remainingIncreaseFrames == showIncreaseForFrames / 2) {
					//print ("no damage frames left!");
					UnshowIncrease ();
				}
			} else {
				//print ("decreasing damage flashes left");
				remainingIncreaseFlashes--;
				ShowIncrease (remainingIncreaseFlashes);
			}
		} else {
			//print ("no damage flashes left!");
			UnshowIncrease ();
		}

		if (remainingIncreaseFlashes == 0 && remainingIncreaseFrames == 0 
			&& remainingDamageFrames == 0 && remainingDamageFlashes == 0) {
			points_text.color = Color.white;
		}

		if (remainingFlashFlashes > 0) {
			//print ("frame number: " + frame);
			//print (remainingDamageFlashes + " damage flashes left");
			if (remainingFlashFrames > 0) {
				//print (remainingDamageFrames + " damage frames left");
				remainingFlashFrames--;
				if (remainingFlashFrames == showFlashForFrames / 2) {
					//print ("no damage frames left!");
					UnshowFlash ();
				}
			} else {
				//print ("decreasing damage flashes left");
				remainingFlashFlashes--;
				ShowFlash (remainingFlashFlashes);
			}
		} else {
			//print ("no damage flashes left!");
			UnshowFlash ();
		}

		if (bossEnemyDelay > 0) {
			bossEnemyDelay--;
		}
	}

	public void DeleteFirstSeg() {
		if (lines.Count == 0) {
			print ("out of lines");
		} else if (lines [0].GetComponent<DrawLine> ().segmentsList.Count == 0) {
			print ("out of segments");
		}
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
		int temp = UnityEngine.Random.Range (1, pickup_drop_prob);
		if (temp == 1) {
			GameObject go;
			float temp_index = UnityEngine.Random.Range (0f, 1f);
			print (temp_index);
			if (temp_index <= prob_pickup [0]) {
				go = Instantiate (pickup_pt_five_prefab) as GameObject;
				if (!hasDroppedP5) {
					encirclePickupDirection_pt_five.enabled = true;
					encirclePickupDirection_pt_five.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_pt_five.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedP5 = true;
				} 
			} else if (temp_index <= (prob_pickup [0] + prob_pickup [1])) {
				go = Instantiate (pickup_pt_ten_prefab) as GameObject;
				if (!hasDroppedP10) {
					encirclePickupDirection_pt_ten.enabled = true;
					encirclePickupDirection_pt_ten.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_pt_ten.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedP10 = true;
				} 
			} else if (temp_index <= (prob_pickup [0] + prob_pickup [1] + prob_pickup [2])) {
				go = Instantiate (pickup_pt_fifty_prefab) as GameObject;
				if (!hasDroppedP50) {
					encirclePickupDirection_pt_fifty.enabled = true;
					encirclePickupDirection_pt_fifty.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_pt_fifty.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedP50 = true;
				} 
			} else if (temp_index <= (prob_pickup [0] + prob_pickup [1] + prob_pickup [2] + prob_pickup [3])) {
				go = Instantiate (pickup_pb_five_prefab) as GameObject;
				if (!hasDroppedC5) {
					encirclePickupDirection_c_five.enabled = true;
					encirclePickupDirection_c_five.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_c_five.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedC5 = true;
				}
			} else if (temp_index <= (prob_pickup [0] + prob_pickup [1] + prob_pickup [2] + prob_pickup [3] + prob_pickup [4])) {
				go = Instantiate (pickup_pb_ten_prefab) as GameObject;
				if (!hasDroppedC10) {
					encirclePickupDirection_c_ten.enabled = true;
					encirclePickupDirection_c_ten.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_c_ten.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedC10 = true;
				} 
			} else if (temp_index <= (prob_pickup [0] + prob_pickup [1] + prob_pickup [2] + prob_pickup [3] + prob_pickup [4] + prob_pickup [5])) {
				go = Instantiate (pickup_pbxtwo_prefab) as GameObject;
				if (!hasDroppedC2) {
					encirclePickupDirection_cxtwo.enabled = true;
					encirclePickupDirection_cxtwo.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_cxtwo.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedC2 = true;
				}
			} else {
				go = Instantiate (pickup_blast) as GameObject;
				if (!hasDroppedBlast) {
					encirclePickupDirection_blast.enabled = true;
					encirclePickupDirection_blast.GetComponent<DirectionFollowsObject> ().go_follow = go;
					encirclePickupDirection_blast.GetComponent<DirectionFollowsObject> ().is_following = true;
					hasDroppedBlast = true;
				}
			}
			Vector3 pos = Vector3.zero;
			float x = UnityEngine.Random.Range (Utils.camBounds.min.x + 5, Utils.camBounds.max.x - 5);
			float y = UnityEngine.Random.Range (Utils.camBounds.min.y + 5, Utils.camBounds.max.y - 5);
			pos.x = x;
			pos.y = y;
			go.transform.position = pos;
		}
	}

	void SpawnEnemies() {
		GameObject go;
		int idk = UnityEngine.Random.Range (1, 10);
		Vector3 pos = Vector3.zero;
		if (plebs.Count > 10 && idk == 1 && bossEnemyDelay == 0) {
			go = Instantiate (enemy4_prefab) as GameObject;
			pos.x = Utils.camBounds.min.x - 2f;
			pos.y = UnityEngine.Random.Range (Utils.camBounds.min.y + 5f, Utils.camBounds.max.y - 5f);
		} else {
			float temp0 = UnityEngine.Random.Range (0f, 1f);
			if (temp0 <= prob_enemy [0]) {
				go = Instantiate (enemy_prefab) as GameObject;
			} else if (temp0 <= prob_enemy [0] + prob_enemy [1]) {
				go = Instantiate (enemy1_prefab) as GameObject;
			} else if (temp0 <= prob_enemy [0] + prob_enemy [1] + prob_enemy [2]) {
				go = Instantiate (enemy2_prefab) as GameObject;
			} else {
				go = Instantiate (enemy3_prefab) as GameObject;
			}
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
		}
		go.transform.position = pos;
		enemies.Add (go);
		if (!hasDroppedBlockedEnemy) {
			blockDirection.enabled = true;
			blockDirection.GetComponent<DirectionFollowsObject> ().go_follow = go;
			blockDirection.GetComponent<DirectionFollowsObject> ().is_following = true;
			hasDroppedBlockedEnemy = true;
		} else if (!hasDroppedEncircledEnemy) {
			encircleDirection.enabled = true;
			encircleDirection.GetComponent<DirectionFollowsObject> ().go_follow = go;
			encircleDirection.GetComponent<DirectionFollowsObject> ().is_following = true;
			hasDroppedEncircledEnemy = true;
			startSpawning = true;
		}
		Invoke("SpawnEnemies", enemySpawnRate);
	}

	void ManageLevel() {
		if (num_points >= 20 && num_points < 50) {
			max_segments = 75;
		} else if (num_points >= 50 && num_points < 100) {
			max_segments = 50;
			enemySpawnRate = 3f;
			prob_enemy [0] = 0.65f;
			prob_enemy [1] = 0.2f;
			prob_enemy [2] = 0f;
			prob_enemy [3] = 0.15f;

			prob_pickup [0] = 0.2f;
			prob_pickup [1] = 0.2f;
			prob_pickup [2] = 0.15f;
			prob_pickup [3] = 0.2f;
			prob_pickup [4] = 0.2f;
			prob_pickup [5] = 0.05f;
			prob_pickup [6] = 0f;
		} else if (num_points >= 100 && num_points < 200) {
			max_segments = 35;
			enemySpawnRate = 2f;
			delete_delay_value = 6f;
			prob_pickup [0] = 0.2f;
			prob_pickup [1] = 0.2f;
			prob_pickup [2] = 0.13f;
			prob_pickup [3] = 0.2f;
			prob_pickup [4] = 0.2f;
			prob_pickup [5] = 0.05f;
			prob_pickup [6] = 0.02f;
		} else if (num_points >= 200 && num_points < 300) {
			max_segments = 25;
			prob_enemy [0] = 0.4f;
			prob_enemy [1] = 0.2f;
			prob_enemy [2] = 0.2f;
			prob_enemy [3] = 0.2f;
		} else if (num_points >= 300 && num_points < 500) {
			max_segments = 10;
			enemySpawnRate = 1f;
			prob_pickup [0] = 0.35f;
			prob_pickup [1] = 0.35f;
			prob_pickup [2] = 0.1f;
			prob_pickup [3] = 0.05f;
			prob_pickup [4] = 0.05f;
			prob_pickup [5] = 0.05f;
			prob_pickup [6] = 0.05f;
		} else if (num_points >= 500) {
			max_segments = 5;
			enemySpawnRate = 0.5f;
			prob_pickup [0] = 0.2f;
			prob_pickup [1] = 0.2f;
			prob_pickup [2] = 0.15f;
			prob_pickup [3] = 0.15f;
			prob_pickup [4] = 0.15f;
			prob_pickup [5] = 0.05f;
			prob_pickup [6] = 0.1f;
		}
	}

	public void ShowDamage(int flashes_left) {
		print ("entered ShowDamage");
		//receive_damage = false;
		remainingDamageFlashes = flashes_left;
		num_plebs_text.color = Color.red;
		points_text.color = Color.red;
		remainingDamageFrames = showDamageForFrames;
	}

	public void UnshowDamage() {
		num_plebs_text.color = Color.white;
		if (remainingIncreaseFrames <= showIncreaseForFrames / 2) {
			points_text.color = Color.white;
		}

	}

	public void ShowIncrease(int flashes_left) {
		print ("entered ShowIncrease");
		//receive_damage = false;
		remainingIncreaseFlashes = flashes_left;
		points_text.color = Color.cyan;
		remainingIncreaseFrames = showIncreaseForFrames;
	}

	public void UnshowIncrease() {
		if (remainingDamageFrames <= showDamageForFrames / 2) {
			points_text.color = Color.white;
		}
	}

	public void ShowFlash(int flashes_left) {
		print ("entered ShowFlash");
		//receive_damage = false;
		remainingFlashFlashes = flashes_left;
		num_blasts_text.color = Color.cyan;
		remainingFlashFrames = showFlashForFrames;
	}

	public void UnshowFlash() {
		num_blasts_text.color = Color.white;
	}

	public void DelayedRestart(float delay) {
		print ("got to delayed restart");
		Invoke("Restart", delay);
	}

	void Restart() {
		SceneManager.LoadScene ("EndScene");
	}

}
