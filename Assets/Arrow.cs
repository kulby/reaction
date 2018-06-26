using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

	private GameObject arrow;
	private float reactionZ;
	private float reactionX;
	public float speed = 10.0f;
	private Animation arrowAnim;
	private Dictionary<int, string> directions;
	private string rightDirection;

	private float startTime, elapsedTime;
	private int elapsedMS;

	void Start() {
		directions = new Dictionary<int, string>(4);
		directions.Add (0, "up");
		directions.Add (1, "left");
		directions.Add (2, "down");
		directions.Add (3, "right");
		arrowAnim = GetComponent<Animation> ();
		NextArrow ();
	}

	void Update() {
		Vector3 dir = Vector3.zero;
		dir.x = -Input.acceleration.y;
		dir.z = Input.acceleration.x;
		if (dir.sqrMagnitude > 1)
			dir.Normalize();

		dir *= Time.deltaTime;

		reactionZ = dir.z * speed;
		reactionX = dir.x * speed;
		//transform.Translate(dir * speed);
		//inputDirection.text = (dir * speed).ToString();
		if ((reactionZ >= 0.2f || Input.GetKeyDown (KeyCode.RightArrow)) && rightDirection == "right") {
			NextArrow ();
		} else if ((reactionZ <= -0.2f || Input.GetKeyDown (KeyCode.LeftArrow)) && rightDirection == "left") {
			NextArrow ();
		} else if ((reactionX >= 0.2f || Input.GetKeyDown (KeyCode.UpArrow)) && rightDirection == "up") {
			NextArrow ();
		} else if ((reactionX <= -0.2f || Input.GetKeyDown (KeyCode.DownArrow)) && rightDirection == "down") {
			NextArrow ();
		}
	}

	void NextArrow() {
		elapsedTime = Time.realtimeSinceStartup - startTime;
		elapsedMS = Mathf.RoundToInt (elapsedTime * 1000);
		arrowAnim.Play ("FadeOut");
		int r = Random.Range (0, 4);
		rightDirection = directions [r];
		Debug.Log (elapsedMS);
		transform.eulerAngles = new Vector3 (0f, 0f, r * 90f);
		arrowAnim.Play ("FadeIn");
		startTime = Time.realtimeSinceStartup;
	}
}
