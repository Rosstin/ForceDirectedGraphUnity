using UnityEngine;
using System.Collections;

public class GenerateRandomGraph : MonoBehaviour {

	float CHARGE_CONSTANT = 100.0f;
	float SPRING_CONSTANT = 2.0f;

	float CHANCE_OF_CONNECTION = 0.09f;
	int NUMBER_NODES = 20;

	AdjacencyList adjacencyList = new AdjacencyList(0);

	//GameObject[] myNodes;
	//NodeForce[] myNodeForces;

	Node[] masterNodeList;

	// Use this for initialization
	void Start () {

		masterNodeList = new Node[NUMBER_NODES];

		// add nodes
		for (int i = 0; i < NUMBER_NODES; i++) {
			if (i != 0) { adjacencyList.AddVertex (i);}
			GameObject myNodeInstance = 
				Instantiate (Resources.Load ("Node"),
					new Vector3 (Random.Range (-10.0f, 10.0f) + 20.0f, Random.Range (-10.0f, 10.0f)+5.0f, Random.Range (-10.0f, 10.0f)),
					Quaternion.identity) as GameObject;
			masterNodeList [i] = new Node (myNodeInstance, i); 
		}


		// populate adjacency
		for (int i = 0; i < NUMBER_NODES; i++) {
			for (int j = 0; j < NUMBER_NODES; j++) {
				if (Random.Range (0.00f, 1.00f) < CHANCE_OF_CONNECTION) {
					if (adjacencyList.isAdjacent (i, j) == false) {
						adjacencyList.AddEdge (i, j);
					}
				}
			}
		}

	}

	// Update is called once per frame
	void Update () {

		// update only one per frame? don't update every node every frame

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

						int smaller = j;
						int bigger = i;
						if (i < j) {
							smaller = i;
							bigger = j;
						}

						LineRenderer myLineRenderer = adjacencyList._edgesToRender ["" + smaller + "." + bigger];
						myLineRenderer.SetVertexCount (2);
						myLineRenderer.SetPosition (0, masterNodeList [i].gameObject.transform.position);
						myLineRenderer.SetPosition (1, masterNodeList [j].gameObject.transform.position);
						myLineRenderer.enabled = true;

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
