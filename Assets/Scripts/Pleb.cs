using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pleb : MonoBehaviour {
	public Vector2 driftSpeedMinMax = new Vector2 (4f, 10f);
	public float driftSpeed;
	public float theta;
	public float changeDirectionDelay;
	public Rigidbody rb;
	public int health;
	public bool canGetHurt;
	public int cooldownFrames;
	public AudioSource audio;

	public Color[] originalColors;
	public Material[] materials; //All the materials of this and its children
	public int remainingDamageFrames = 0; //Damagge frames left
	public int remainingDamageFlashes = 0;
	public int showDamageForFrames = 10;

	public List<GameObject> enemiesTargetingMe;

	void Awake() {
		rb = this.GetComponent<Rigidbody> ();
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
		canGetHurt = true;
		cooldownFrames = 0;
	}

	// Use this for initialization
	void Start () {
		enemiesTargetingMe = new List<GameObject> ();
		driftSpeed = Random.Range (driftSpeedMinMax.x, driftSpeedMinMax.y);
		changeDirectionDelay = Random.Range (1, 10);
		theta = Random.Range (0, 359);
		//print ("first theta " + theta);
		rb.velocity = new Vector3 (driftSpeed * Mathf.Cos (Mathf.Deg2Rad * theta),
			driftSpeed * Mathf.Sin (Mathf.Deg2Rad * theta), 0f);
		//print ("First velocity " + rb.velocity);
		health = 3;
	}
	
	// Update is called once per frame
	void Update () {
		//print ("current real velocity " + rb.velocity);
		//Vector3 tempDir = rb.velocity.normalized;
		//print ("current vel " + tempDir);
		//theta = Mathf.Rad2Deg * Mathf.Acos (tempDir.x);
		//print ("current theta " + theta);

		if (changeDirectionDelay > 0) {
			changeDirectionDelay--;
			if (changeDirectionDelay == 0) {
				changeDirectionDelay = Random.Range (3, 10);
				int temp = Random.Range (0, 2);
				float tempThetaChange = Random.Range (5, 30);
				if (temp == 0) {
					theta += tempThetaChange;
				} else if (temp == 1) {
					theta -= tempThetaChange;
				}
				rb.velocity = new Vector3 (driftSpeed * Mathf.Cos (Mathf.Deg2Rad * theta),
					driftSpeed * Mathf.Sin (Mathf.Deg2Rad * theta), 0f);
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

		if (cooldownFrames > 0) {
			cooldownFrames--;
		}
	}

	void OnCollisionEnter(Collision coll) {
		rb.velocity *= -1;
		theta = (theta + 180) % 360;
		audio.clip = GameController.gc.collide;
		audio.Play ();
		if (coll.gameObject.tag == "Enemy" && cooldownFrames == 0) {
			ShowDamage (5);
			health--;
			//canGetHurt = false;
			cooldownFrames = 24;
			if (health <= 0) {
				foreach (GameObject go in enemiesTargetingMe) {
					if (go != null) {
						go.GetComponent<Enemy> ().target = new Vector3 (-1f, -1f, -1f);
					}
				}
				enemiesTargetingMe.Clear ();
				GameController.gc.plebs.Remove (this.gameObject);
				Destroy (this.gameObject);
				GameController.gc.num_points -= 5;
				GameController.gc.audioPlayer.clip = GameController.gc.died;
				GameController.gc.audioPlayer.Play ();
				GameController.gc.ShowDamage (5);
			}
		}
	}

	void OnCollisionLeave(Collision coll) {
		if (coll.gameObject.tag == "Enemy") {
			//canGetHurt = true;
		}
	}

	void CheckOffScreen() {
		//print ("checking offscreen for plebs");
		//print ("offscreen test " + Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen));
		if (Utils.ScreenBoundsCheck(GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
			//...then destroy this GameObject
			foreach (GameObject go in enemiesTargetingMe) {
				if (go != null) {
					go.GetComponent<Enemy> ().target = new Vector3 (-1f, -1f, -1f);
				}
			}
			enemiesTargetingMe.Clear ();
			GameController.gc.plebs.Remove(this.gameObject);
			Destroy(this.gameObject);
			GameController.gc.num_points -= 5;
			GameController.gc.audioPlayer.clip = GameController.gc.died;
			GameController.gc.audioPlayer.Play ();
			GameController.gc.ShowDamage (5);
		}
	}

	void ShowDamage(int flashes_left) {
		//print ("entered ShowDamage");
		//receive_damage = false;
		remainingDamageFlashes = flashes_left;
		foreach (Material m in materials) {
			m.color = Color.black;
		}
		remainingDamageFrames = showDamageForFrames;
	}

	void UnshowDamage() {
		for (int i = 0; i < materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
	}
}
