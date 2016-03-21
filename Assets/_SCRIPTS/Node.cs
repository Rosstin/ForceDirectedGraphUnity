using UnityEngine;
using System.Collections;

public class Node {
	public GameObject gameObject;
	public NodeForce nodeForce;
	public int index;

	public Node(GameObject myGameObject, int myIndex)
	{
		gameObject = myGameObject;
		nodeForce = myGameObject.GetComponent<NodeForce> ();
		index = myIndex;
	}

}
