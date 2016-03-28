using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseRaycast : MonoBehaviour {

	// the screen aspect ratio matters for the vert and horiz, you'll need to recognize that

	// use "force pull" / "force push" to bring objects closer or farther

	public Camera playerCamera;

	public GameObject pointer;

	public Canvas canvas;

	public LineRenderer myLineRenderer;

	public GameObject rightHandObject;
	public CapsuleHand rightHandScript;
	private Transform pointerFingerTip;

	int state;
	int STATE_NORMAL = 0;
	int STATE_DRAGGING = 1;

	GameObject draggedObject = null;
	float distanceOfDraggedObject = 0.0f;

	//private Transform objectToMove;     // The object we will move.
	//private Vector3 offSet;       // The object's position relative to the mouse position.
	//private float dist;

	void Start () {
		GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
		GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
		rightHandScript = rightHandObject.GetComponent<CapsuleHand> ();
		//GameObject prefabNode = Resources.Load ("Node") as GameObject;
	}

	// Attach this script to an orthographic camera.

	void FixedUpdate () {
		// MOUSE POINTER STUFF
		var rt = pointer.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), new Vector2(Input.mousePosition.x, Input.mousePosition.y), playerCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
		}

		if (Input.GetMouseButton(0)){
			//Debug.Log ("mouse being held");
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("pointerSpriteRed");
		} else {
			pointer.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("pointerSprite");
		}


		if (rightHandScript.pointerTip != null) {
			pointerFingerTip = rightHandScript.pointerTip;
		}

		if (pointerFingerTip != null) {
			Vector3 p = pointerFingerTip.position;
			//Vector3 p = globalMousePos;

			// RAY STUFF
			RaycastHit hit = new RaycastHit ();

			Vector3 heading = p - playerCamera.transform.position;

			if (Physics.Raycast (playerCamera.transform.position, p, out hit)) { // if you hit something
				if (hit.transform.gameObject.tag == "Draggable") { // if it was a draggable object
					hit.transform.gameObject.GetComponent<Draggable> ().OnHit ();
					//if (Input.GetMouseButtonDown (0)) {
						state = STATE_DRAGGING;
						draggedObject = hit.transform.gameObject;
						distanceOfDraggedObject =  Vector3.Distance(playerCamera.transform.position, hit.transform.gameObject.transform.position);
					//}
				}
			}

			//if (!Input.GetMouseButton (0)) {
			//	state = STATE_NORMAL;
			//}

			if (state == STATE_DRAGGING) {
				Vector3 direction = heading / distanceOfDraggedObject;
				Vector3 objectPosition = playerCamera.transform.position + (direction.normalized * distanceOfDraggedObject);
				draggedObject.transform.position = objectPosition;

				//print ("distanceOfDraggedObject: "+distanceOfDraggedObject);
				//print ("objectPosition: "+objectPosition);

			}

			Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 500.0f);
			//print ("endRayPosition: "+endRayPosition);

			myLineRenderer.SetVertexCount (2);
			myLineRenderer.SetPosition (0, p);
			myLineRenderer.SetPosition (1, endRayPosition);
			myLineRenderer.enabled = true;
		}
	}
}
