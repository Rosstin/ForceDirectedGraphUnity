using UnityEngine;
using System.Collections;

public class GenerateRandomGraph : MonoBehaviour {

	public float CHARGE_CONSTANT = 1000.0f;
	public float SPRING_CONSTANT = 2.0f;

	AdjacencyList<int> adjacencyList = new AdjacencyList<int>(0);

	GameObject[] myNodes;
	NodeForce[] myNodeForces;

	Node[] masterNodeList;

	// Use this for initialization
	void Start () {

		int numNodes = 4;

		myNodes = new GameObject[numNodes];
		myNodeForces = new NodeForce[numNodes];

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

		myNodes [0] = myNodeInstance0;
		myNodes [1] = myNodeInstance1;
		myNodes [2] = myNodeInstance2;
		myNodes [3] = myNodeInstance3;

		myNodeForces [0] = myNodeInstance0.GetComponent<NodeForce>();
		myNodeForces [1] = myNodeInstance1.GetComponent<NodeForce>();
		myNodeForces [2] = myNodeInstance2.GetComponent<NodeForce>();
		myNodeForces [3] = myNodeInstance3.GetComponent<NodeForce>();

		Node myNode0 = new Node (myNodes[0], myNodeForces[0],0);
		Node myNode1 = new Node (myNodes[1], myNodeForces[1],1);
		Node myNode2 = new Node (myNodes[2], myNodeForces[2],2);
		Node myNode3 = new Node (myNodes[3], myNodeForces[3],3);

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
		for (int i = 0; i < myNodes.Length; i++) {
			for (int j = 0; j < myNodes.Length; j++) {
				if (i != j) {
					// apply force
					// there should only be one interaction for each
					// force = constant * absolute(myNodes[i].charge * myNodes[j].charge)/square(distance(myNodes[i], myNodes[j]))

					// CALC REPULSIVE FORCE
					float distance = Vector3.Distance (myNodes [i].transform.position, myNodes [j].transform.position); 

					float chargeForce = (CHARGE_CONSTANT) * ((myNodeForces [i].charge * myNodeForces [j].charge) / (distance * distance));
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
						if (myNodeForces [i].myLineRenderer != null) {
							myNodeForces [i].myLineRenderer.SetVertexCount (2);
							myNodeForces [i].myLineRenderer.SetPosition (0, myNodes [i].transform.position);
							myNodeForces [i].myLineRenderer.SetPosition (1, myNodes [j].transform.position);
						}
					} else {
						//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
					}

					float totalForce = chargeForce - springForce; //only if they're in the same direction

					float accel = totalForce / myNodeForces [i].mass;
					float distanceChange = /* v0*t */ 0.5f * (accel) * (Time.deltaTime) * (Time.deltaTime);

					Vector3 direction = myNodes[i].transform.position - myNodes[j].transform.position;

					// apply it
					Vector3 newPositionForI = myNodes[i].transform.position + direction.normalized * distanceChange;
					myNodes [i].transform.position = newPositionForI;

					// put in something to dampen it and stop calculations after it settles down
					// TODO


				}
			}
		}
	}
}
