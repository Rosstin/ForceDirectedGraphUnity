using UnityEngine;
using System.Collections;

// TODO: read in the data
// TODO: improve speed/performance, only update a subset of the nodes each call
// TODO: 

public class GenerateRandomGraph : MonoBehaviour {

	float CHARGE_CONSTANT = 10000.0f;
	float SPRING_CONSTANT = 2.0f;

	float CHANCE_OF_CONNECTION = 0.09f;
	int NUMBER_NODES = 20;

	int NODES_PROCESSED_PER_FRAME = 10; // could also do as a percentage, could have some logic for that, or the max number that can be done

	float DISTANCE_FROM_FACE = 10.0f;
	float NODE_SPREAD = 10.0f;

	int currentIndex = 0;

	int highestNode = 0;

	AdjacencyList adjacencyList = new AdjacencyList(0);

	Node[] masterNodeList;

	// Use this for initialization
	void Start () {
		generateGraphFromCSV ();
	}

	void generateGraphFromCSV(){
		print ("readCSVData");
		TextAsset text = Resources.Load ("L") as TextAsset;
		string[,] outputGrid = CSVReader.SplitCsvGrid (text.text);

		int numberOfEdges = outputGrid.GetUpperBound(1);


		// find the highest node
		for (int i = 1; i < numberOfEdges; i++) {
			float source = float.Parse(outputGrid [0,i]); // source
			float target = float.Parse(outputGrid [1,i]); // target
			// change it so that it ignores the last newline in a file

			int s = (int) source;
			int t = (int) target;

			if (s > highestNode) {
				highestNode = s;
			}

			if (t > highestNode) {
				highestNode = t;
			}

			//Debug.Log ("outputGrid[0,i]: " + outputGrid[0,i] + "... " + "outputGrid[1,i]: " + outputGrid[1,i]);
		}

		masterNodeList = new Node[highestNode];

		// add nodes
		randomlyPlaceNodes ();

		// add edges
		for (int i = 1; i < numberOfEdges; i++) {

			//Debug.Log ("outputGrid[0,i]: " + outputGrid[0,i] + "... " + "outputGrid[1,i]: " + outputGrid[1,i]);

			float source = float.Parse(outputGrid [0,i]); // source
			float target = float.Parse(outputGrid [1,i]); // target

			int s = (int) source;
			int t = (int) target;

			addEdgeToAdjacencyListAfterValidation (s, t);
		}

	}

	void generateGraphRandomly(){
		masterNodeList = new Node[NUMBER_NODES];

		// add nodes
		randomlyPlaceNodes();

		// populate adjacency
		for (int i = 0; i < NUMBER_NODES; i++) {
			for (int j = 0; j < NUMBER_NODES; j++) {
				if (Random.Range (0.00f, 1.00f) < CHANCE_OF_CONNECTION) {
					addEdgeToAdjacencyListAfterValidation (i, j);
				}
			}
		}
	}

	void randomlyPlaceNodes(){ //also adds vertexes to adjacencylist
		int numNodes = masterNodeList.Length;
		// add nodes
		for (int i = 0; i < numNodes; i++) {

			// add vertexes
			if (i != 0) { adjacencyList.AddVertex (i);}

			GameObject myNodeInstance = 
				Instantiate (Resources.Load ("Node"),
					new Vector3 (Random.Range (-NODE_SPREAD, NODE_SPREAD), Random.Range (-NODE_SPREAD, NODE_SPREAD)+5.0f, Random.Range (-NODE_SPREAD, NODE_SPREAD)-DISTANCE_FROM_FACE),
					Quaternion.identity) as GameObject;
			masterNodeList [i] = new Node (myNodeInstance, i); 
		}
	}

	void addEdgeToAdjacencyListAfterValidation(int source, int target){
		int smaller = source;
		int bigger = target;
		if (target < source) {
			smaller = target;
			bigger = source;
		}

		if (adjacencyList.isAdjacent (smaller, bigger) == false) {
			adjacencyList.AddEdge (smaller, bigger);
		}
	}

	void applyForcesBetweenTwoNodes(int i, int j){
		// apply force
		// there should only be one interaction for each
		// force = constant * absolute(myNodes[i].charge * myNodes[j].charge)/square(distance(myNodes[i], myNodes[j]))

		// CALC REPULSIVE FORCE
		float distance = Vector3.Distance (masterNodeList [i].gameObject.transform.position, masterNodeList [j].gameObject.transform.position); 

		float chargeForce = (CHARGE_CONSTANT) * ((masterNodeList [i].nodeForce.charge * masterNodeList [j].nodeForce.charge) / (distance * distance));

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

			string key = "" + smaller + "." + bigger;

			//print ("key: " + key);

			LineRenderer myLineRenderer = adjacencyList._edgesToRender [key];
			myLineRenderer.SetVertexCount (2);
			myLineRenderer.SetPosition (0, masterNodeList [smaller].gameObject.transform.position);
			myLineRenderer.SetPosition (1, masterNodeList [bigger].gameObject.transform.position);
			myLineRenderer.enabled = true;
		} else {
			//print ("Number " + i + " and number " + j + " NOT ADJACENT.");
		}

		float totalForce = chargeForce - springForce; //only if they're in the same direction

		float accel = totalForce / masterNodeList [i].nodeForce.mass;
		float distanceChange = /* v0*t */ 0.5f * (accel) * (Time.deltaTime) * (Time.deltaTime);

		Vector3 direction = masterNodeList [i].gameObject.transform.position - masterNodeList [j].gameObject.transform.position;

		// apply it
		Vector3 newPositionForI = masterNodeList [i].gameObject.transform.position + direction.normalized * distanceChange;
		masterNodeList [i].gameObject.transform.position = newPositionForI;

		// put in something to dampen it and stop calculations after it settles down
		// TODO
	}

	// Update is called once per frame
	void Update () {

		// update only one per frame? don't update every node every frame
		// render lines

		int nodesProcessedThisFrame = 0;
		while( nodesProcessedThisFrame < NODES_PROCESSED_PER_FRAME){
			nodesProcessedThisFrame += 1;
			currentIndex += 1;
			if (currentIndex >= masterNodeList.Length) {
				currentIndex = 0;
			}
			int i = currentIndex;
			for (int j = 0; j < masterNodeList.Length; j++) {
				if (i != j) {
					applyForcesBetweenTwoNodes (i, j);
				}
			}
		}
	}

}
