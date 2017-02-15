using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment : MonoBehaviour {
	public Vector3 pos;
	public Vector3 between;
	public Vector3 midpoint;
	public GameObject line_parent;

	public void SetPhysicsOfSegment() {
		pos = midpoint;
		gameObject.transform.position = midpoint;
		gameObject.tag = "Line";
		//nope need to find the correct length...
		gameObject.transform.localScale = new Vector3 (between.magnitude, 1f, 1f);
		gameObject.transform.rotation = Quaternion.Euler (0f, 0f, Mathf.Rad2Deg * Mathf.Atan (between.y / between.x));
		//gameObject.GetComponent<BoxCollider> ().size = new Vector3(between.magnitude, 1f, 1f);
		gameObject.GetComponent<BoxCollider> ().transform.rotation = Quaternion.Euler (0f, 0f, Mathf.Rad2Deg * Mathf.Atan (between.y / between.x));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
