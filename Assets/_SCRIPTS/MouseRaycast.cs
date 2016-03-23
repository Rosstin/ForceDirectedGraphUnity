using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseRaycast : MonoBehaviour {

	// the screen aspect ratio matters for the vert and horiz, you'll need to recognize that

	// use "force pull" / "force push" to bring objects closer or farther

	public Camera playerCamera;

	public GameObject pointer;

	public Canvas canvas;

	int state;
	int STATE_NORMAL = 0;
	int STATE_DRAGGING = 1;

	GameObject draggedObject = null;
	float distanceOfDraggedObject = 0.0f;

	//private Transform objectToMove;     // The object we will move.
	//private Vector3 offSet;       // The object's position relative to the mouse position.
	//private float dist;

	void Start () {
	}

	// Attach this script to an orthographic camera.

	void FixedUpdate () {

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

		RaycastHit hit = new RaycastHit ();

		if (Physics.Raycast (playerCamera.transform.position, globalMousePos, out hit)) {
			print ("a hit!");
			if (hit.transform.gameObject.tag == "Draggable") {
				hit.transform.gameObject.GetComponent<Draggable> ().OnHit ();
				if (Input.GetMouseButtonDown(0) ) {
					state = STATE_DRAGGING;
					draggedObject = hit.transform.gameObject;
					distanceOfDraggedObject = hit.distance;
				}
			}
		}

		if (!Input.GetMouseButton (0)) {
			state = STATE_NORMAL;
		}
		if (state == STATE_DRAGGING) {

			Vector3 heading = globalMousePos - playerCamera.transform.position;

			Vector3 direction = heading / distanceOfDraggedObject;

			Vector3 objectPosition = playerCamera.transform.position + (direction.normalized * distanceOfDraggedObject);

			draggedObject.transform.position = objectPosition;
		}





	}
}
