using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy4 : MonoBehaviour {
	public Bounds bounds;
	public Vector3 boundsCenterOffset;
	public Vector3 target;
	public float speed;
	public AudioSource audioPlayer;

	public float search_radius = 20f;
	public Vector3[] directions;

	void Awake() {
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}

	// Use this for initialization
	void Start () {
		speed = 3f;
		Vector3 pos = Vector3.zero;
		float x = Utils.camBounds.max.x + 20f;
		pos.x = x;
		float yMin = Utils.camBounds.min.y;
		float yMax = Utils.camBounds.max.y;
		pos.y = Random.Range (yMin, yMax);
		target = pos;
	}

	// Update is called once per frame
	void Update () {
		//print ("goin' after mah target");
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, target, step);
	}

//	void OnCollisionEnter(Collision coll) {
//		if (coll.gameObject.tag == "Pleb") {
//			print ("hit a pleb");
//			GameController.gc.audioPlayer.clip = GameController.gc.hitEnemy;
//			GameController.gc.audioPlayer.Play ();
//			GetComponent<Rigidbody> ().velocity *= -5;
//		}
//	}

	void CheckOffScreen() {
		if (Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//...then destroy this GameObject
			GameController.gc.enemies.Remove(this.gameObject);
			Destroy(this.gameObject);
		}
	}
}
