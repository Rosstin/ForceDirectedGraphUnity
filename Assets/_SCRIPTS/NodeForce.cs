using UnityEngine;
using System.Collections;

public class NodeForce : MonoBehaviour {

	//public GameObject lineToRender = null;
	//public LineRenderer myLineRenderer = null;

	public float charge = 1.0f;
	public float mass = 1.0f;

	// Use this for initialization
	void Start () {
		//GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		//lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		//myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

	}

}
