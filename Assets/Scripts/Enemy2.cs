using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : MonoBehaviour {
	public Vector3 target;
	public float speed;
	public float resetTargetDelay;
	public int total_cooldown_frames;
	public int num_cooldown_frames;

	public float search_radius = 20f;
	public Vector3[] directions;

	void Awake() {
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}

	// Use this for initialization
	void Start () {
		speed = 40f;
		resetTargetDelay = 24f;
		target = new Vector3 (-1f, -1f, -1f);
		directions = all_directions ();
		num_cooldown_frames = 0;
		total_cooldown_frames = 24;
	}

	// Update is called once per frame
	void Update () {
		if (Mathf.Abs (target.x - transform.position.x) <= 0.5 &&
			Mathf.Abs (target.y - transform.position.y) <= 0.5 &&
			Mathf.Abs (target.z - transform.position.z) <= 0.5) {
			target = new Vector3 (-1f, -1f, -1f);
		}

		if (target.x == -1f && target.y == -1f && target.z == -1f) {
			resetTargetDelay--;
			if (resetTargetDelay <= 0) {
				print ("setting new target");
				SetTarget ();
				resetTargetDelay = 24f;
				Vector3 vel_direction = target - transform.position;
				vel_direction.Normalize ();
				GetComponent<Rigidbody> ().velocity = vel_direction * speed;
			}
		} else {
//			print ("goin' after mah target");
//			Vector3 vel_direction = target - transform.position;
//			vel_direction.Normalize ();
//			float step = speed * Time.deltaTime;
//			transform.position = Vector3.MoveTowards (transform.position, target, step);
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
		}

	}

	void SetTarget() {
		Vector3 pos = Random.insideUnitSphere * 15f;
		pos.z = 0f;
		target = pos;
	}

	void OnCollisionEnter(Collision coll) {
		if (coll.gameObject.tag == "Line") {
			target = new Vector3 (-1f, -1f, -1f);
			GameController.gc.num_points += 1;
			num_cooldown_frames = total_cooldown_frames;
			GetComponent<Rigidbody> ().velocity *= -1;
		} else if (coll.gameObject.tag == "Pleb" && num_cooldown_frames == 0) {
			print ("hit a pleb");
			GetComponent<Rigidbody> ().velocity *= -1;
			target = new Vector3 (-1f, -1f, -1f);
		}
	}

	void CheckOffScreen() {
		if (Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//...then destroy this GameObject
			GameController.gc.enemies.Remove(this.gameObject);
			Destroy(this.gameObject);
//			SetTarget();
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

