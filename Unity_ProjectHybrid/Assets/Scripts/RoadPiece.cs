using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPiece : MonoBehaviour {
	public RoadType roadType;
	public List<Transform> pointsList = new List<Transform>();
	
	private void Start() {
		foreach(Transform _point in pointsList) {
			_point.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
