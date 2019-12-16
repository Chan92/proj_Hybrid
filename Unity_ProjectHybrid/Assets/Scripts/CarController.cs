using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController:MonoBehaviour {
	[Header("Debugging")]
	public Transform pointdebug;
	public bool drawGizmo = true;
	private bool gameStart = false;

	[Header("Tweakable stats")]
	[SerializeField]
	private float boosterSpeed;
	[SerializeField]
	private float movespeedMud = 5f, movespeedRoad = 10f;
	[SerializeField]
	private Vector3 offsetDown, offsetForward;
	[SerializeField]
	private float rayDownDist = 0.5f, rayForwardDist = 0.2f, boxRayDist = 1.2f;
	[SerializeField][Min(0.01f)]
	private float boxRayScale = 1;

	private Rigidbody rb;
	private RoadPiece roadPoint;
	private RaycastHit hitDown, hitForward, boxRayHit;
	private Vector3 movePoint, carForward;
	private int roadStartPoint, currentPoint;
	private bool roadInfoSet = false;
	private bool boxRayDetect;
	private Vector3 boxRaySize;

	void Start(){
		if(movespeedMud >= movespeedRoad) {
			throw new System.NotSupportedException("movespeedMud needs to be lower than movespeedRoad.");
		}

		rb = transform.GetComponent<Rigidbody>();
		carForward = transform.forward;
		carForward.y = transform.position.y;
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Space)) {
			gameStart = !gameStart;
		}

		if(gameStart) {
			if(CheckWall()) {
				Vector3 _newRot = new Vector3(0, transform.eulerAngles.y + 180f, 0);
				transform.Rotate(_newRot);
			}

			if(!OnRoadCheck()) {
				OffRoadMovement();
			} else {
				OnEnterRoad();
				OnRoadMovement();
			}
		} else {
		//	if(Input.GetKeyDown(KeyCode.Space)) {
		//		gameStart = true;
		//	}
		}
	}

	bool OnRoadCheck() {
		Debug.DrawRay(transform.position + offsetDown, -transform.up * rayDownDist, Color.blue);
		Physics.Raycast(transform.position + offsetDown, -transform.up, out hitDown, rayDownDist);
		if(hitDown.transform.root.GetComponent<RoadPiece>() != null) {
			roadPoint = hitDown.transform.root.GetComponent<RoadPiece>();
			print("TRUE: " + hitDown.transform.name);
			return true;
		} else {
			print("FALSE: " + hitDown.transform.name);
			return false;
		}	
	}

	bool CheckWall() {
		Debug.DrawRay(transform.position + offsetForward, transform.forward * rayForwardDist, Color.green);
		Physics.Raycast(transform.position + offsetForward, transform.forward, out hitForward, rayForwardDist);
		if(hitForward.transform) {
			print("true: " + hitForward.transform.name + " >from: " + hitForward.transform.root.name);

			if(hitForward.transform.name.Contains("road points")) {
				return false;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}

	void OffRoadMovement() {
		if(roadInfoSet)	roadInfoSet = false;
		carForward = transform.forward;
		//carForward.y = transform.position.y;
		transform.position = Vector3.MoveTowards(transform.position, transform.position + carForward, movespeedMud * Time.deltaTime);
		print("~~~ not on road < < <");
	}

	void OnEnterRoad() {
		if(!roadInfoSet) {
			print("> > road enter");
			boxRaySize = boxRayScale * transform.localScale;
			boxRayDetect = Physics.BoxCast(transform.position, boxRaySize, transform.forward, out boxRayHit, transform.rotation, boxRayDist);

			if(boxRayDetect && boxRayHit.transform.name.Contains("road point")) {
				currentPoint = roadStartPoint = roadPoint.pointsList.IndexOf(boxRayHit.transform);
			}

			//pointdebug = roadPoint.pointsList[currentPoint];
			Vector3 _nextPoint = roadPoint.pointsList[currentPoint].position;
			_nextPoint.y = transform.position.y;
			movePoint = _nextPoint;
			transform.LookAt(movePoint);
			roadInfoSet = true;
		}		
	}

	void OnRoadMovement() {
		print("^^ on road stay");
		transform.position = Vector3.MoveTowards(transform.position, movePoint, movespeedRoad * Time.deltaTime);

		if(Vector3.Distance(transform.position, movePoint) <= 0.1f) {
			if(roadStartPoint == 0) {
				if(currentPoint < roadPoint.pointsList.Count - 1) currentPoint++;
			} else if(roadStartPoint == roadPoint.pointsList.Count - 1) {
				if (currentPoint > 0) currentPoint--;
			}

			Vector3 _nextPoint = roadPoint.pointsList[currentPoint].position;
			_nextPoint.y = transform.position.y;
			movePoint = _nextPoint;
			transform.LookAt(movePoint);
		}
	}

	//draws the boxray
	void OnDrawGizmos() {
		if(drawGizmo) {
			Gizmos.color = Color.red;
			boxRaySize = boxRayScale * transform.localScale;

			//Check if there has been a hit yet
			if(boxRayDetect) {
				//Draw a Ray forward from GameObject toward the hit
				Gizmos.DrawRay(transform.position, transform.forward * boxRayHit.distance);
				//Draw a cube that extends to where the hit exists
				Gizmos.DrawWireCube(transform.position + transform.forward * boxRayHit.distance, boxRaySize);
			}
			//If there hasn't been a hit yet, draw the ray at the maximum distance
			else {
				//Draw a Ray forward from GameObject toward the maximum distance
				Gizmos.DrawRay(transform.position, transform.forward * boxRayDist);
				//Draw a cube at the maximum distance
				Gizmos.DrawWireCube(transform.position + transform.forward * boxRayDist, boxRaySize);
			}
		}
	}
}
