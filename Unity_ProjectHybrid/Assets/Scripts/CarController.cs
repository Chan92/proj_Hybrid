using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour{
	[SerializeField]
	private float movespeedMud = 5f, movespeedRoad = 10f;

	private Rigidbody rb;
	private bool onroad;// = true;
	private RoadPoints roadPoint;
	private Vector3 movePoint;
	private int roadStartPoint;
	private int currentPoint;
	public bool gameStart = false;
	public Transform pointdebug;

    void Start(){
		if(movespeedMud >= movespeedRoad) {
			throw new System.NotSupportedException("movespeedMud needs to be lower than movespeedRoad.");
		}

		rb = transform.GetComponent<Rigidbody>();
	}

	private void Update() {
		if(!onroad) {
			transform.position = Vector3.MoveTowards(transform.position, Vector3.forward, MoveSpeed() * Time.deltaTime);
			print("x");
		}
	}

	float MoveSpeed() {
		if(onroad) {
			return movespeedRoad;
		} else {
			return movespeedMud;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.transform.name == "Road") {
			onroad = true;
			roadPoint = collision.transform.GetComponent<RoadPoints>();
			float _distanceA = Vector3.Distance(roadPoint.points[0].position, transform.position);
			float _distanceB = Vector3.Distance(roadPoint.points[roadPoint.points.Length - 1].position, transform.position);

			if(_distanceA < _distanceB) {
				currentPoint = roadStartPoint = 0;
			} else {
				currentPoint = roadStartPoint = (roadPoint.points.Length - 1);
			}

			pointdebug = roadPoint.points[currentPoint];
			Vector3 _nextPoint = roadPoint.points[currentPoint].position;
			_nextPoint.y = transform.position.y;
			movePoint = _nextPoint;
			transform.LookAt(movePoint);
		}
	}

	private void OnCollisionStay(Collision collision) {
		if(collision.transform.name == "Road") {
			onroad = true;
			transform.position = Vector3.MoveTowards(transform.position, movePoint, MoveSpeed() * Time.deltaTime);

			if(Vector3.Distance(transform.position, movePoint) <= 0.1f) {
				if(roadStartPoint == 0) {
					currentPoint++;
				} else {
					currentPoint--;
				}

				Vector3 _nextPoint = roadPoint.points[currentPoint].position;
				_nextPoint.y = transform.position.y;
				movePoint = _nextPoint;
				transform.LookAt(movePoint);
			}
		}
	}

	private void OnCollisionExit(Collision collision) {
		if(collision.transform.name == "Road") {
			onroad = false;
		}
	}
}
