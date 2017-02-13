using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DrawLine : MonoBehaviour
{
	private bool isMousePressed;
	public List<Vector3> pointsList;
	public List<GameObject> segmentsList;
	private Vector3 mousePos;
	public bool doneDrawing;

	// Structure for line points
	struct myLine
	{
		public Vector3 StartPoint;
		public Vector3 EndPoint;
	};
	//    -----------------------------------    
	void Awake ()
	{
		isMousePressed = false;
		doneDrawing = false;
		pointsList = new List<Vector3> ();
		if (Input.GetMouseButtonDown (0) && !doneDrawing) {
			isMousePressed = true;
		}
	}
	//    -----------------------------------    
	void Update ()
	{
		if (Input.GetMouseButtonUp (0)) {
			isMousePressed = false;
			doneDrawing = true;
		}
		// Drawing line when mouse is moving(presses)
		if (isMousePressed && !doneDrawing) {
			mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			mousePos.z = 0;
			if (!pointsList.Contains (mousePos)) {
				pointsList.Add (mousePos);
				//print ("Mouse pos " + mousePos);
				Vector3 inBetween;
				Vector3 midpoint;
				if (pointsList.Count > 1) {
					inBetween = pointsList [pointsList.Count - 1] - pointsList [pointsList.Count - 2];
					midpoint = new Vector3 ((pointsList [pointsList.Count - 1].x + pointsList [pointsList.Count - 2].x) / 2f,
						(pointsList [pointsList.Count - 1].y + pointsList [pointsList.Count - 2].y) / 2f, 0);
				} else {
					inBetween = Vector3.up;
					midpoint = mousePos;
				}
				//print ("in between " + inBetween);
				//print ("midpoint " + midpoint);
				GameObject seg = Instantiate(GameController.gc.segment_prefab);
				seg.GetComponent<LineSegment>().between = inBetween;
				seg.GetComponent<LineSegment> ().midpoint = midpoint;
				seg.GetComponent<LineSegment> ().line_parent = this.gameObject;
				seg.GetComponent<LineSegment> ().SetPhysicsOfSegment ();
				segmentsList.Add (seg);
				GameController.gc.num_segments++;
				if (GameController.gc.num_segments >= GameController.gc.max_segments) {
					GameController.gc.DeleteFirstSeg ();
				}
			}
		}
	}
}
