using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {
	float rot = 0f;
	public Vector2 driftMinMax = new Vector2 (0.25f, 2);
	public float lifeTime = 10f;
	public float fadeTime = 4f;
	public float birthTime;

	public float search_radius = 20f;
	public Vector3[] directions;

	void Awake() {
		Vector3 vel = Random.onUnitSphere; 
		vel.z = 0;
		vel.Normalize();
		vel *= Random.Range(driftMinMax.x, driftMinMax.y);
		GetComponent<Rigidbody>().velocity = vel;
		birthTime = Time.time;
		InvokeRepeating("CheckOffscreen", 2f, 2f);
	}

	// Use this for initialization
	void Start () {
		directions = all_directions ();
	}
	
	// Update is called once per frame
	void Update () {
		rot = (rot + 5f) % 360f;
		transform.rotation = Quaternion.Euler (0f, 0f, rot);

		float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
		if (u >= 1) {
			Destroy (this.gameObject);
			return;
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
			print ("omg I got picked up!");
			//increase score of gamecontroller by some amount
			//also do whatever it is else that you need to do like add more plebs or whatever
			if (this.gameObject.tag == "pt+5") {
				GameController.gc.num_points += 5;
			} else if (this.gameObject.tag == "pt+10") {
				GameController.gc.num_points += 10;
			} else if (this.gameObject.tag == "pt+50") {
				GameController.gc.num_points += 50;
			} else if (this.gameObject.tag == "pb+5") {
				GameController.gc.CreatePlebs (5);
			} else if (this.gameObject.tag == "pb+10") {
				GameController.gc.CreatePlebs (10);
			} else if (this.gameObject.tag == "pbx2") {
				GameController.gc.CreatePlebs (GameController.gc.plebs.Count);
			} else if (this.gameObject.tag == "blast") {
				GameController.gc.num_blasts++;
				GameController.gc.blastStorageDirection.enabled = true;
				GameController.gc.showInitDirectionsFrames = 120;
			}
			GameController.gc.audioPlayer.clip = GameController.gc.pickup;
			GameController.gc.audioPlayer.Play ();
			Destroy(this.gameObject);
		}
	}

	void CheckOffscreen() {
		//if the PowerUp has drifted entirely off screen...
		if (Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//...then destroy this GameObject
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
