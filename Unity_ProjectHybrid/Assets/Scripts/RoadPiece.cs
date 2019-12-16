using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPiece : MonoBehaviour {
	[SerializeField]
	private bool hidePoints = true;

	[Space(10)]
	public RoadType roadType;
	public List<Transform> pointsList = new List<Transform>();
	
	private void Start() {
		foreach(Transform _point in pointsList) {
			if (hidePoints) _point.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
