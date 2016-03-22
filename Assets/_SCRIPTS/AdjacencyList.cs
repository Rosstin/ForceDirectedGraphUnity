using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AdjacencyList : MonoBehaviour
{
	private List<List<int>> _vertexList = new List<List<int>>();
	private Dictionary<int, List<int>> _vertexDict = new Dictionary<int, List<int>>();

	// make a composite key for edges
	public Dictionary<string, LineRenderer> _edgesToRender = new Dictionary<string, LineRenderer>();

	public AdjacencyList(int rootVertexKey)
	{
		AddVertex(rootVertexKey);
	}

	public List<int> AddVertex(int key)
	{
		List<int> vertex = new List<int>();
		_vertexList.Add(vertex);
		_vertexDict.Add(key, vertex);

		return vertex;
	}

	public void AddEdge(int startKey, int endKey)
	{      
		List<int> startVertex = _vertexDict.ContainsKey(startKey) ? _vertexDict[startKey] : null;
		List<int> endVertex = _vertexDict.ContainsKey(endKey) ? _vertexDict[endKey] : null;

		if (startVertex == null)
			throw new ArgumentException("Cannot create edge from a non-existent start vertex.");

		if (endVertex == null)
			endVertex = AddVertex(endKey);

		startVertex.Add(endKey);
		endVertex.Add(startKey);

		int smaller = endKey;
		int bigger = startKey;
		if (startKey < endKey) {
			smaller = startKey;
			bigger = endKey;
		}

		GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		LineRenderer myLineRenderer = lineToRender.GetComponent<LineRenderer> ();

		if ( UnityEngine.Random.Range (0.0f, 1.0f) < 0.50f) {
			myLineRenderer.SetColors (Color.blue, Color.cyan);
		} else {
			myLineRenderer.SetColors (Color.red, Color.magenta);
		}

		myLineRenderer.enabled = false;

		string edgeKey = "" + smaller + "." + bigger;
		_edgesToRender.Add (edgeKey, myLineRenderer);
	}

	public void RemoveVertex(int key)
	{
		List<int> vertex = _vertexDict[key];

		//First remove the edges / adjacency entries
		int vertexNumAdjacent = vertex.Count;
		for (int i = 0; i < vertexNumAdjacent; i++)
		{  
			int neighbourVertexKey = vertex[i];
			RemoveEdge(key, neighbourVertexKey);
		}

		//Lastly remove the vertex / adj. list
		_vertexList.Remove(vertex);
		_vertexDict.Remove(key);
	}

	public void RemoveEdge(int startKey, int endKey)
	{
		((List<int>)_vertexDict[startKey]).Remove(endKey);
		((List<int>)_vertexDict[endKey]).Remove(startKey);
	}

	public bool Contains(int key)
	{
		return _vertexDict.ContainsKey(key);
	}

	public List<int> GetEdgesForVertex(int key)
	{
		return _vertexDict[key];
	}

	public bool isAdjacent(int key1, int key2)
	{
		if (_vertexDict.ContainsKey (key1)) {
			List<int> edges = _vertexDict [key1];
			if (edges.Contains (key2)) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public int VertexDegree(int key)
	{
		return _vertexDict[key].Count;
	}
}