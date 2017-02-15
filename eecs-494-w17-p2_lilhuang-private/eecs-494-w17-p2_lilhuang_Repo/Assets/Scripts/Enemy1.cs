using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {
	public Vector3 target;
	public float speed;
	public float resetTargetDelay;
	public int total_cooldown_frames;
	public int num_cooldown_frames;
	public bool canIncreasePoints;

	public float search_radius = 20f;
	public Vector3[] directions;

	void Awake() {
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}

	// Use this for initialization
	void Start () {
		speed = 50f;
		resetTargetDelay = 24f;
		target = new Vector3 (-1f, -1f, -1f);
		directions = all_directions ();
		num_cooldown_frames = 0;
		total_cooldown_frames = 24;
		canIncreasePoints = true;
	}

	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (target.x - transform.position.x) <= 0.1 &&
			Mathf.Abs (target.y - transform.position.y) <= 0.1 &&
			Mathf.Abs (target.z - transform.position.z) <= 0.1) {
			target = new Vector3 (-1f, -1f, -1f);
		}

		if (target.x == -1f && target.y == -1f && target.z == -1f) {
			resetTargetDelay--;
			if (resetTargetDelay <= 0) {
				print ("setting new target");
				SetTarget ();
				resetTargetDelay = 24f;
			}
		} else {
			print ("goin' after mah target");
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target, step);
		}

		if (num_cooldown_frames > 0) {
			num_cooldown_frames--;
		}

		bool surrounded = true;
		foreach (Vector3 direction in directions) {
			//print ("checking direction " + direction);
			Vector3 ray = transform.TransformDirection (direction);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, ray, out hit, search_radius)) {
				if (hit.transform.tag != "Line") {
					surrounded = false;
					break;
				}
			} else {
				surrounded = false;
				break;
			}
		}
		if (surrounded) {
			print ("omg i ded");
			//increase score of gamecontroller by some amount
			GameController.gc.enemies.Remove(this.gameObject);
			Destroy(this.gameObject);
			GameController.gc.num_points += 5;
			GameController.gc.ShowIncrease (5);
		}

	}
		
	void SetTarget() {
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x - GameController.gc.enemySpawnPadding;
		float xMax = Utils.camBounds.max.x + GameController.gc.enemySpawnPadding;
		float yMin = Utils.camBounds.min.y - GameController.gc.enemySpawnPadding;
		float yMax = Utils.camBounds.max.y + GameController.gc.enemySpawnPadding;
		int temp = Random.Range (0, 3);
		if (temp == 0) {
			pos.x = Random.Range (xMin, xMax);
			pos.y = Random.Range (Utils.camBounds.max.y, yMax);
		} else if (temp == 1) {
			pos.x = Random.Range (xMin, xMax);
			pos.y = Random.Range (yMin, Utils.camBounds.min.y);
		} else if (temp == 2) {
			pos.x = Random.Range (Utils.camBounds.max.x, xMax);
			pos.y = Random.Range (yMin, yMax);
		} else if (temp == 3) {
			pos.x = Random.Range (xMin, Utils.camBounds.min.x);
			pos.y = Random.Range (yMin, yMax);
		}
		target = pos;
	}

	void OnCollisionEnter(Collision coll) {
		if (coll.gameObject.tag == "Line") {
			target = new Vector3 (-1f, -1f, -1f);
			GameController.gc.num_points += 1;
			num_cooldown_frames = total_cooldown_frames;
			GetComponent<Rigidbody> ().velocity *= -1;
			GameController.gc.audioPlayer.clip = GameController.gc.morePoints;
			GameController.gc.audioPlayer.Play ();
			canIncreasePoints = false;
			if (!GameController.gc.hasShownBlockIncrease) {
				GameController.gc.pointIncreaseDirection_block.enabled = true;
				GameController.gc.hasShownBlockIncrease = true;
				GameController.gc.showInitDirectionsFrames = 120;
			}
			GameController.gc.ShowIncrease (5);
		} else if (coll.gameObject.tag == "Pleb" && num_cooldown_frames == 0) {
			print ("hit a pleb");
			coll.gameObject.GetComponent<Pleb> ().ShowDamage (5);
			coll.gameObject.GetComponent<Pleb> ().health--;
			GameController.gc.audioPlayer.clip = GameController.gc.hitEnemy;
			GameController.gc.audioPlayer.Play ();
			GetComponent<Rigidbody> ().velocity *= -1;
			target = new Vector3 (-1f, -1f, -1f);
		}
	}

	void OnCollisionLeave(Collision coll) {
		if (coll.gameObject.tag == "Line") {
			canIncreasePoints = true;
		}
	}

	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Enemy4") {
			Destroy (this.gameObject);
			GameController.gc.enemies.Remove(this.gameObject);
			GameController.gc.num_points += 5;
			GameController.gc.audioPlayer.clip = GameController.gc.morePoints;
			GameController.gc.audioPlayer.Play ();
			GameController.gc.ShowIncrease (5);
		}
	}

	void CheckOffScreen() {
		if (Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//...then destroy this GameObject
			GameController.gc.enemies.Remove(this.gameObject);
			Destroy(this.gameObject);
		}
	}

	public Vector3[] all_directions() {
		Vector3[] directions = new Vector3[360];

		for (int theta = 0; theta < 360; theta++) {
			float x = Mathf.Cos (Mathf.Deg2Rad * theta);
			float y = Mathf.Sin (Mathf.Deg2Rad * theta);
			directions [theta] = new Vector3 (x, y, 0f);
		}
		return directions;
	}
}
