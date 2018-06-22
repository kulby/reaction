using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

	private GameObject arrow;
	private float reaction;
	public float speed = 10.0f;

	void Start() {
		GenerateDirection ();
	}

	void Update() {
		Vector3 dir = Vector3.zero;
		dir.x = -Input.acceleration.y;
		dir.z = Input.acceleration.x;
		if (dir.sqrMagnitude > 1)
			dir.Normalize();

		dir *= Time.deltaTime;

		reaction = dir.z * speed;
		//transform.Translate(dir * speed);
		//inputDirection.text = (dir * speed).ToString();
		if (reaction >= 0.2f) {
			GenerateDirection ();
		}
	}

	void GenerateDirection() {
		transform.Rotate (0, 0, 90f);
	}
}
