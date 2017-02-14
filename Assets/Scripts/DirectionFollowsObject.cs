using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionFollowsObject : MonoBehaviour {
	public GameObject go_follow;
	public bool is_following;
	public int framesAlive;
	public Text thisText;

	// Use this for initialization
	void Start () {
		is_following = false;
		framesAlive = 120;
	}
	
	// Update is called once per frame
	void Update () {
		if (go_follow == null) {
			is_following = false;
			thisText.enabled = false;
		}
		if (is_following) {
			framesAlive--;
			Follow ();
			if (framesAlive == 0) {
				thisText.enabled = false;
			}
		}
	}

	void Follow() {
		//print ("following");
		thisText.GetComponent<RectTransform>().position = new Vector3 (go_follow.transform.position.x, 
			go_follow.transform.position.y + 2f, 0f);
	}
}
