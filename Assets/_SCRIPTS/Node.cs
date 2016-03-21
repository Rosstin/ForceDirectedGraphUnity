using UnityEngine;
using System.Collections;

public class Node {
	public GameObject gameObject;
	public NodeForce nodeForce;
	public int index;

	public Node(GameObject myGameObject, NodeForce myNodeForce, int myIndex)
	{
		gameObject = myGameObject;
		nodeForce = myNodeForce;
		index = myIndex;
	}

}
