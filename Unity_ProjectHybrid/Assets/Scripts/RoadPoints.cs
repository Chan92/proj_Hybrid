using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPoints : MonoBehaviour {
	public RoadType roadType;
	public Transform[] points;
	
	private void Start() {
		foreach(Transform child in transform) {
			child.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
