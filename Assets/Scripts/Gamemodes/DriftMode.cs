using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftMode : GameMode {

	[SerializeField]
	private GameObject _car;

	[SerializeField]
	private Canvas inGameUI;

	[SerializeField]
	private Canvas pauseUI;
	[SerializeField]
	private RectTransform speedo;

	new public static FreePlayMode Instance
	{ get { return (FreePlayMode) instance; } }

	void Update () {
		if (Controls.MultiOSControls.Instance.getValue("Submit") != 0)
		{
			_car.transform.position = Vector3.up;
			_car.transform.rotation = Quaternion.identity;
		}
		if (Controls.MultiOSControls.Instance.getValue("Cancel") != 0)
		{
			Pause();
		}

		float angle = Mathf.Lerp(speedo.rotation.ToEuler().z, -(_car.GetComponent<WheelVehicle>().speed) / 220 * 180, 0.53f);

		speedo.rotation = Quaternion.AngleAxis(4 * angle, Vector3.forward);
	}

	public void Pause()
	{
		_car.GetComponent<WheelVehicle>().handbreak = true;

		Time.timeScale = 0;

		pauseUI.gameObject.SetActive(true);
		inGameUI.gameObject.SetActive(false);
	}

	public void Resume()
	{
		_car.GetComponent<WheelVehicle>().handbreak = false;

		Time.timeScale = 1;

		pauseUI.gameObject.SetActive(false);
		inGameUI.gameObject.SetActive(true);
	}
}
