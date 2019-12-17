using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController:MonoBehaviour {
	[Header("Debugging")]
	public Transform pointdebug;
	public bool drawGizmo = true;
	private bool gameStart = false;

	[Header("Tweakable stats")]
	[SerializeField][Range(0.5f, 10f)]
	private float boosterSpeed = 10f;
	[SerializeField]
	private float movespeedMud = 5f, movespeedRoad = 10f;
	[SerializeField]
	private Vector3 offsetDown, offsetForward;
	[SerializeField]
	private Transform rayDownPos, rayForwardPos;
	[SerializeField]
	private float rayDownDist = 0.5f, rayForwardDist = 0.2f, boxRayDist = 1.2f;
	[SerializeField][Min(0.01f)]
	private float boxRayScale = 1f;

	private Rigidbody rb;
	private RoadPiece roadPoint;
	private RaycastHit hitDown, hitForward, boxRayHit;
	private Vector3 movePoint, carForward;
	private int roadStartPoint, currentPoint;
	private bool roadInfoSet = false;
	private bool boxRayDetect;
	private Vector3 boxRaySize;
	private float curSpeed;

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
		}
	}

	bool OnRoadCheck() {
		Debug.DrawRay(rayDownPos.position, -transform.up * rayDownDist, Color.blue);
		Physics.Raycast(rayDownPos.position, -transform.up, out hitDown, rayDownDist);
		//print("pos: " + transform.position + " |pos + ofset: " + (transform.position + offsetDown) + " |down: " + -transform.up);
		//print("roadcheck: " + hitDown.transform.root.name);
		if(hitDown.transform.root.GetComponent<RoadPiece>() != null) {
			roadPoint = hitDown.transform.root.GetComponent<RoadPiece>();

			if(hitDown.transform.root.GetComponent<RoadPiece>().roadType == RoadType.booster) {
				curSpeed = boosterSpeed;
				//print("booster speed");
			} else {
				curSpeed = movespeedRoad;
				//print("normal speed");
			}

			return true;
		} else {
			curSpeed = movespeedMud;
			//print("mud speed");
			return false;
		}	
	}

	bool CheckWall() {
		Debug.DrawRay(rayForwardPos.position, transform.forward * rayForwardDist, Color.green);
		Physics.Raycast(rayForwardPos.position, transform.forward, out hitForward, rayForwardDist);
		if(hitForward.transform) {
			//print("true: " + hitForward.transform.name + " >from: " + hitForward.transform.root.name);

			//print("wall check: " + hitForward.transform.name);
			if(hitForward.transform.name.Contains("road points")) {				
				return false;
			} else {
				return true;
			}
		} else {
			//print("wall check: sees nothing");
			return false;
		}
	}

	void OffRoadMovement() {
		if(roadInfoSet)	roadInfoSet = false;
		carForward = transform.forward;
		transform.position = Vector3.MoveTowards(transform.position, transform.position + carForward, curSpeed * Time.deltaTime);
		//print("~~~ not on road < < <");
	}

	void OnEnterRoad() {
		if(!roadInfoSet) {
			currentPoint = roadStartPoint = 0;

			boxRaySize = boxRayScale * transform.localScale;
			boxRayDetect = Physics.BoxCast(rayForwardPos.position, boxRaySize, transform.forward, out boxRayHit, transform.rotation, boxRayDist);

			if(boxRayDetect && boxRayHit.transform.name.Contains("road point")) {
				currentPoint = roadStartPoint = roadPoint.pointsList.IndexOf(boxRayHit.transform);				
			}

			//print("currentPoint: " + currentPoint + " | length: " + roadPoint.pointsList.Count);
			pointdebug = roadPoint.pointsList[currentPoint];
			Vector3 _nextPoint = roadPoint.pointsList[currentPoint].position;
			_nextPoint.y = transform.position.y;
			movePoint = _nextPoint;
			transform.LookAt(movePoint);
			roadInfoSet = true;
		}		
	}

	void OnRoadMovement() {
		//print("^^^ on road stay");
		transform.position = Vector3.MoveTowards(transform.position, movePoint, curSpeed * Time.deltaTime);

		if(Vector3.Distance(transform.position, movePoint) <= 0.1f) {
			Vector3 _nextPoint;

			if (CheckNextPoint()) {
				_nextPoint = roadPoint.pointsList[currentPoint].position;
				_nextPoint.y = transform.position.y;
				transform.LookAt(_nextPoint);
				//print("nextpos point: " + _nextPoint);
				//movePoint = _nextPoint;
			} else {
				_nextPoint = transform.forward;
				//print("nextpos forward: " + _nextPoint);
			}

			movePoint = _nextPoint;
		}
	}

	bool CheckNextPoint() {
		if(roadStartPoint == 0) {
			if(currentPoint < roadPoint.pointsList.Count - 1) {
				currentPoint++;
				return true;
			} else {
				return false;
			}				
		} else if(roadStartPoint == roadPoint.pointsList.Count - 1) {
			if(currentPoint > 0) {
				currentPoint--;
				return true;
			} else {
				return false;
			}	
		} else {
			return false;
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
				Gizmos.DrawRay(rayForwardPos.position, transform.forward * boxRayHit.distance);
				//Draw a cube that extends to where the hit exists
				Gizmos.DrawWireCube(rayForwardPos.position + transform.forward * boxRayHit.distance, boxRaySize);
			}
			//If there hasn't been a hit yet, draw the ray at the maximum distance
			else {
				//Draw a Ray forward from GameObject toward the maximum distance
				Gizmos.DrawRay(rayForwardPos.position, transform.forward * boxRayDist);
				//Draw a cube at the maximum distance
				Gizmos.DrawWireCube(rayForwardPos.position + transform.forward * boxRayDist, boxRaySize);
			}
		}
	}
}
