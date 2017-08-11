using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftMode : GameMode {

	[SerializeField] private GameObject _car;

	[SerializeField] private Canvas _inGameUi;

	[SerializeField] private Canvas _pauseUi;
	[SerializeField] private RectTransform _speedo;

	[SerializeField] private Text _score;
	[SerializeField] private Text _bestScoreT;
	[SerializeField] private Text _scoreIncrement;
	[SerializeField] private Text _timeT;

	private int _bestScore;

	private float _timer;

	public new static FreePlayMode Instance
	{ get { return (FreePlayMode) instance; } }

	private float posTimer = 0;

	protected override IEnumerator Start ()
	{
		posTimer = _timer = Time.realtimeSinceStartup;
		Dictionary<int, GameObject> ghosts = new Dictionary<int, GameObject>();

		gameMode = "driftmode";
		yield return StartCoroutine (base.Start());
		
		StartCoroutine(DriftCoroutine());
		
		// main server loop
		while (true) {
			string reply = w.RecvString ();
			if (reply != null) {
				string opcode = reply.Substring (0, 3);
				string package = reply.Substring (4);

				if (opcode == "tra") {
					package = package.Substring (1);
					string[] args = package.Split('(');
					args[0] = args[0].Substring (0, args[0].Length-1);
					args[1] = args[1].Substring (0, args[1].Length-1);
					args[2] = args[2].Substring (0, args[2].Length-1);
					/*Debug.Log(args[0]);
					Debug.Log(args[1]);
					Debug.Log(args[2]);*/

					GameObject ghost = ghosts [System.Int32.Parse (args[0])];

					string[] pos = args[1].Split(' ');
					pos[0] = pos[0].Substring (0, pos[0].Length-1);
					pos[1] = pos[1].Substring (0, pos[1].Length-1);
					ghost.transform.position = new Vector3 (float.Parse (pos [0]), float.Parse (pos [1]), float.Parse (pos [2]));

					string[] rot = args[2].Split(' ');
					rot[0] = rot[0].Substring (0, rot[0].Length-1);
					rot[1] = rot[1].Substring (0, rot[1].Length-1);
					rot[2] = rot[2].Substring (0, rot[2].Length-1);
					ghost.transform.rotation = new Quaternion (float.Parse (rot [0]), float.Parse (rot [1]), float.Parse (rot [2]), float.Parse (rot [3]));
					
				} else if (opcode == "new") {

					GameObject newGhost = Instantiate (_car);

					newGhost.tag = "Ghost";
					newGhost.GetComponent<Rigidbody>().isKinematic = true;

					foreach (Collider c in newGhost.GetComponentsInChildren<Collider>())
					{
						c.isTrigger = true;
						c.gameObject.layer = LayerMask.NameToLayer("Ghost");
					}

					ghosts.Add(int.Parse(package), newGhost);
					Debug.Log ("Added new ghost id:" + int.Parse (package));
				} else if (opcode == "acc") {
					Debug.Log (package);
				}

			}
			if (w.error != null) {
				Debug.LogError ("Error: " + w.error);
				break;
			}

			yield return 0;

			if (Time.realtimeSinceStartup - posTimer > 0.1f) {
				posTimer = Time.realtimeSinceStartup;

				w.SendString ("tra " + _car.transform.position.ToString () + _car.transform.rotation.ToString ());
				yield return 0;
			}
		}

		Debug.Log ("Closing socket");
		w.Close ();
	}
	
	void Update () {
		{
			float timeLeft = (120f - (Time.realtimeSinceStartup - _timer));
			_timeT.text = "" + (int)(timeLeft / 60) + ":" + (int)(timeLeft % 60);
		}

		if (Controls.MultiOSControls.Instance.getValue("Submit") != 0 || Time.realtimeSinceStartup - _timer > 120f)
		{
			StopAllCoroutines();
			StartCoroutine(DriftCoroutine());
			
			_car.transform.position = Vector3.up;
			_car.transform.rotation = Quaternion.identity;

			_timer = Time.realtimeSinceStartup;
		}
		if (Controls.MultiOSControls.Instance.getValue("Cancel") != 0)
		{
			Pause();
		}

		float angle = Mathf.Lerp(_speedo.rotation.ToEuler().z, -(_car.GetComponent<WheelVehicle>().speed) / 220 * 180, 0.53f);
		_speedo.rotation = Quaternion.AngleAxis(4 * angle, Vector3.forward);
	}

	public void Pause()
	{
		_car.GetComponent<WheelVehicle>().handbreak = true;

		Time.timeScale = 0;

		_pauseUi.gameObject.SetActive(true);
		_inGameUi.gameObject.SetActive(false);
	}

	public void Resume()
	{
		_car.GetComponent<WheelVehicle>().handbreak = false;

		Time.timeScale = 1;

		_pauseUi.gameObject.SetActive(false);
		_inGameUi.gameObject.SetActive(true);
	}

	private IEnumerator DriftCoroutine()
	{
		int s = 0;
		_score.text = "score: " + s;
		_scoreIncrement.text = "";
		_bestScoreT.text = "best: " + _bestScore;
		
		Debug.Log("Start DriftCoroutine");
		
		while (true)
		{
			float driftAngle = Vector3.Angle(_car.transform.forward, _car.GetComponent<Rigidbody>().velocity);

			if (driftAngle > 20f && _car.GetComponent<Rigidbody>().velocity.magnitude > 5)
			{
				yield return new WaitForSeconds(0.1f);
				float i = 0;
				do
				{
					_scoreIncrement.text = "+" + (++i * 100);
					driftAngle = Vector3.Angle(_car.transform.forward, _car.GetComponent<Rigidbody>().velocity);
					yield return new WaitForSeconds(0.5f);
				} while (driftAngle > 15f && _car.GetComponent<Rigidbody>().velocity.magnitude > 3);
				
				s += (int) (i * 100);
				_score.text = "score: " + s;
				_scoreIncrement.text = "";
				
				if (s > _bestScore)
				{
					_bestScore = s;
					_bestScoreT.text = "best: " + _bestScore;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		
		Debug.Log("Stop DriftCoroutine");
	}
}
