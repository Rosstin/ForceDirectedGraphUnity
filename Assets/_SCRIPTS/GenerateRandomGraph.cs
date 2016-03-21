using UnityEngine;
using System.Collections;

public class GenerateRandomGraph : MonoBehaviour {

	public float CHARGE_CONSTANT = 1000.0f;
	public float SPRING_CONSTANT = 2.0f;

	AdjacencyList<int> adjacencyList = new AdjacencyList<int>(0);

	//GameObject[] myNodes;
	//NodeForce[] myNodeForces;

	Node[] masterNodeList;

	// Use this for initialization
	void Start () {

		int numNodes = 4;

		masterNodeList = new Node[numNodes];

		GameObject myNodeInstance0 = 
			Instantiate (Resources.Load ("Node"),
			new Vector3(20.0f, 0.0f, 2.0f),
			Quaternion.identity) as GameObject;
		GameObject myNodeInstance1 = 
			Instantiate (Resources.Load ("Node"),
				new Vector3(20.0f, 0.0f, 4.0f),
				Quaternion.identity) as GameObject;
		GameObject myNodeInstance2 = 
			Instantiate (Resources.Load ("Node"),
				new Vector3(20.0f, 5.0f, 2.0f),
				Quaternion.identity) as GameObject;
		GameObject myNodeInstance3 = 
			Instantiate (Resources.Load ("Node"),
				new Vector3(20.0f, 8.0f, 6.0f),
				Quaternion.identity) as GameObject;

		Node myNode0 = new Node (myNodeInstance0, 0); //todo: auto-index?
		Node myNode1 = new Node (myNodeInstance1, 1);
		Node myNode2 = new Node (myNodeInstance2, 2);
		Node myNode3 = new Node (myNodeInstance3, 3);

		masterNodeList [0] = myNode0;
		masterNodeList [1] = myNode1;
		masterNodeList [2] = myNode2;
		masterNodeList [3] = myNode3;

		adjacencyList.AddEdge (0, 1);
		adjacencyList.AddEdge (1, 2);
		adjacencyList.AddEdge (2, 3);
		adjacencyList.AddEdge (3, 0);

	}

	// Update is called once per frame
	void Update () {

		// render lines

		// do forces
		for (int i = 0; i < masterNodeList.Length; i++) {
			for (int j = 0; j < masterNodeList.Length; j++) {
				if (i != j) {
					// apply force
					// there should only be one interaction for each
					// force = constant * absolute(myNodes[i].charge * myNodes[j].charge)/square(distance(myNodes[i], myNodes[j]))

					// CALC REPULSIVE FORCE
					float distance = Vector3.Distance (masterNodeList [i].gameObject.transform.position, masterNodeList [j].gameObject.transform.position); 

					float chargeForce = (CHARGE_CONSTANT) * ((masterNodeList [i].nodeForce.charge * masterNodeList [j].nodeForce.charge) / (distance * distance));
					//print ("force: " + force);
					//float accel = force / myNodeForces[i].mass;

					//float distanceChange = /* v0*t */ 0.5f * (accel) * (Time.deltaTime) * (Time.deltaTime);

					// direction
					//Vector3 direction = myNodes[i].transform.position - myNodes[j].transform.position;

					// apply it
					//Vector3 newPositionForI = myNodes[i].transform.position + direction.normalized * distanceChange;
					//myNodes [i].transform.position = newPositionForI;

					// CALC ATTRACTIVE FORCE, only if they are springed
					// now use Hooke's Law for spring attraction to pull them back together
					// F = kx

					// if these nodes are connected

					// maybe i should do this per node and get all the forces and sum them first, in the previous for-loop

					float springForce = 0;
					if (adjacencyList.isAdjacent (i, j)) {
						// print ("Number " + i + " and number " + j + " are adjacent.");
						springForce = (SPRING_CONSTANT) * (distance);
						// draw a line between the points if it exists
						if (masterNodeList [i].nodeForce.myLineRenderer != null) {
							masterNodeList [i].nodeForce.myLineRenderer.SetVertexCount (2);
							masterNodeList [i].nodeForce.myLineRenderer.SetPosition (0, masterNodeList [i].gameObject.transform.position);
							masterNodeList [i].nodeForce.myLineRenderer.SetPosition (1, masterNodeList [j].gameObject.transform.position);
						}
					} else {
						//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
					}

					float totalForce = chargeForce - springForce; //only if they're in the same direction

					float accel = totalForce / masterNodeList[i].nodeForce.mass;
					float distanceChange = /* v0*t */ 0.5f * (accel) * (Time.deltaTime) * (Time.deltaTime);

					Vector3 direction = masterNodeList[i].gameObject.transform.position - masterNodeList[j].gameObject.transform.position;

					// apply it
					Vector3 newPositionForI = masterNodeList[i].gameObject.transform.position + direction.normalized * distanceChange;
					masterNodeList [i].gameObject.transform.position = newPositionForI;

					// put in something to dampen it and stop calculations after it settles down
					// TODO


				}
			}
		}
	}
}
