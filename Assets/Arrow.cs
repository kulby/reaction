using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

	public Text arrowsPassedText;
	public Text debugX, debugY;
	public bool roundStarted, roundEnded;

	private GameObject arrow;
	private Animation arrowAnim;
	private Dictionary<int, string> directions;

	private string rightDirection;
	private float gyroX, gyroY, accX, accY;
	private float startTime, elapsedTime;
	private int elapsedMS;
	private int curRand, lastRand;
	private int arrowsPassed;

	void Start() {
		roundStarted = false;
		roundEnded = false;
		arrowsPassed = 0;
		lastRand = 5;
		directions = new Dictionary<int, string>(4);
		directions.Add (0, "up");
		directions.Add (1, "left");
		directions.Add (2, "down");
		directions.Add (3, "right");
		arrowAnim = GetComponent<Animation> ();
		GenerateArrow ();
	}

	void Update() {

		if (roundEnded == true) {
			GenerateArrow ();
			roundEnded = false;
		}

		/* ACCELEROMETR */ 

		Vector3 acceleration = Input.acceleration;
		accX = acceleration.x;
		accY = acceleration.y;
		
		/* GYROSCOPE */

		Quaternion gyroQaternion = Input.gyro.attitude;
		Vector3 eulerGyro = gyroQaternion.eulerAngles;

		gyroX = eulerGyro.x;
		gyroY = eulerGyro.y;

		if (gyroX > 180f) {
			gyroX -= 360f;
		}

		if (gyroY > 180f) {
			gyroY -= 360f;
		}

		/* DEBUG */

		debugX.text = gyroX.ToString();
		debugY.text = gyroY.ToString();

		//debugX.text = accX.ToString();
		//debugY.text = accY.ToString();

		/* REACTION */

		if (roundStarted) {
			if (rightDirection == "right") {
				if ((gyroY > 25f && accX >= 0.5f) || Input.GetKeyDown (KeyCode.RightArrow)) {
					GoodRound ();
				} else if (gyroX > 30f || gyroX < -10f || gyroY < -25f) {
					GameOver ();
				}
			} else if (rightDirection == "left") {
				if ((gyroY < -25f && accX <= -0.5f) || Input.GetKeyDown (KeyCode.LeftArrow)) {
					GoodRound ();
				} else if (gyroX > 30f || gyroX < -10f || gyroY > 25f) {
					GameOver ();
				}
			} else if (rightDirection == "up") {
				if ((gyroX < -10f && accY >= 0.5f) || Input.GetKeyDown (KeyCode.UpArrow)) {
					GoodRound ();
				} else if (gyroY > 20f || gyroY < -20f || gyroX > 25f) {
					GameOver ();
				}
			} else if (rightDirection == "down") {
				if ((gyroX > 25f && accY <= -0.5f) || Input.GetKeyDown (KeyCode.DownArrow)) {
					GoodRound ();
				} else if (gyroY > 25f || gyroY < -25f || gyroX < -5f) {
					GameOver ();
				}
			}
		}
	}

	void GoodRound() {
		roundStarted = false;
		elapsedTime = Time.realtimeSinceStartup - startTime;
		elapsedMS = Mathf.RoundToInt (elapsedTime * 1000);
		Debug.Log (elapsedMS);
		arrowsPassed++;
		arrowsPassedText.text = arrowsPassed.ToString ();
		arrowAnim.Play ("FadeOut");
	}

	void GenerateArrow() {
		curRand = Random.Range (0, 4);
		while (curRand == lastRand) {
			curRand = Random.Range (0, 4);
		}
		lastRand = curRand;
		rightDirection = directions [curRand];
		transform.eulerAngles = new Vector3 (0f, 0f, curRand * 90f);
		arrowAnim.Play ("FadeIn");
		startTime = Time.realtimeSinceStartup;
	}

	void GameOver() {
		roundStarted = false;
		transform.localScale = new Vector3 (0f, 0f, 0f);
		arrowsPassedText.text = "GameOver!";
	}
}
