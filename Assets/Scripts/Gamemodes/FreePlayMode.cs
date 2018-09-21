using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using Controls;

public class FreePlayMode : GameMode {
    private Camera _camera;

    [SerializeField]
    private GameObject[] _cars;
    [SerializeField]
    private int _curCar = 0;

    [SerializeField]
    private Canvas inGameUI;
    [SerializeField]
    private Dropdown carDropDown;

    [SerializeField]
    private Canvas pauseUI;
    [SerializeField]
    private RectTransform speedo;

	private float posTimer = 0;

    new public static FreePlayMode Instance
    { get { return (FreePlayMode) instance; } }

    override protected void Awake()
    {
        base.Awake();

        carDropDown.options.Clear();
        foreach (GameObject car in _cars)
        {
            carDropDown.options.Add(new Dropdown.OptionData(car.name));
        }
    }

    protected override IEnumerator Start ()
    {
        _camera = Camera.main;

        foreach (GameObject c in _cars)
            c.GetComponent<WheelVehicle>().handbreak = true;

        _camera.GetComponent<SmoothFollow>().Target = _cars[_curCar].transform;
        _cars[_curCar].GetComponent<WheelVehicle>().handbreak = false;

		Dictionary<int, GameObject> ghosts = new Dictionary<int, GameObject>();
        
        gameMode = "freemode";
        yield return StartCoroutine (base.Start());

        // // main server loop
        // while (true) {
        //     string reply = w.RecvString ();
        //     if (reply != null) {
        //         string opcode = reply.Substring (0, 3);
		// 		string package = reply.Substring (4);

		// 		if (opcode == "tra") {
		// 			package = package.Substring (1);
		// 			string[] args = package.Split('(');
		// 			args[0] = args[0].Substring (0, args[0].Length-1);
		// 			args[1] = args[1].Substring (0, args[1].Length-1);
		// 			args[2] = args[2].Substring (0, args[2].Length-1);
		// 			/*Debug.Log(args[0]);
		// 			Debug.Log(args[1]);
		// 			Debug.Log(args[2]);*/

		// 			GameObject ghost = ghosts [System.Int32.Parse (args[0])];

		// 			string[] pos = args[1].Split(' ');
		// 			pos[0] = pos[0].Substring (0, pos[0].Length-1);
		// 			pos[1] = pos[1].Substring (0, pos[1].Length-1);
		// 			ghost.transform.position = new Vector3 (float.Parse (pos [0]), float.Parse (pos [1]), float.Parse (pos [2]));

		// 			string[] rot = args[2].Split(' ');
		// 			rot[0] = rot[0].Substring (0, rot[0].Length-1);
		// 			rot[1] = rot[1].Substring (0, rot[1].Length-1);
		// 			rot[2] = rot[2].Substring (0, rot[2].Length-1);
		// 			ghost.transform.rotation = new Quaternion (float.Parse (rot [0]), float.Parse (rot [1]), float.Parse (rot [2]), float.Parse (rot [3]));
					
		// 		} else if (opcode == "new") {

		// 			GameObject newGhost = Instantiate (_cars[_curCar]);

		// 			newGhost.tag = "Ghost";
		// 			newGhost.GetComponent<Rigidbody>().isKinematic = true;

		// 			foreach (Collider c in newGhost.GetComponentsInChildren<Collider>())
		// 			{
		// 				c.isTrigger = true;
		// 				c.gameObject.layer = LayerMask.NameToLayer("Ghost");
		// 			}

		// 			ghosts.Add(int.Parse(package), newGhost);
		// 			Debug.Log ("Added new ghost id:" + int.Parse (package));
		// 		}
        //     }
        //     if (w.error != null) {
        //         Debug.LogError ("Error: " + w.error);
        //         break;
        //     }

        //     yield return 0;

		// 	if (Time.realtimeSinceStartup - posTimer > 0.1f) {
		// 		posTimer = Time.realtimeSinceStartup;

		// 		w.SendString ("tra " + _cars[_curCar].transform.position.ToString () + _cars[_curCar].transform.rotation.ToString ());
		// 		yield return 0;
		// 	}
        // }
        // Debug.Log ("Closing socket");
        // w.Close ();
    }

    void Update () {
		if (Controls.MultiOSControls.Instance.getValue("Submit") != 0)
        {
            SelectCar(_curCar);
        }

		if (Controls.MultiOSControls.Instance.getValue("Cancel") != 0)
        {
            Pause();
        }
        
        float angle = Mathf.Lerp(speedo.rotation.ToEuler().z, -(_cars[_curCar].GetComponent<WheelVehicle>().speed) / 220 * 180, 0.53f);

        

        speedo.rotation = Quaternion.AngleAxis(4 * angle, Vector3.forward);
    }

    public void SelectCar(Int32 c)
    {        
        _cars[_curCar].SetActive(false);

        _curCar = c;

        _camera.GetComponent<SmoothFollow>().Target = _cars[_curCar].transform;
        _cars[_curCar].transform.position = Vector3.up;
        _cars[_curCar].transform.rotation = Quaternion.identity;
        _cars[_curCar].SetActive(true);
    }

    public void Pause()
    {
        _cars[_curCar].GetComponent<WheelVehicle>().handbreak = true;

        Time.timeScale = 0;
        
        pauseUI.gameObject.SetActive(true);
        inGameUI.gameObject.SetActive(false);
    }

    public void Resume()
    {
        _cars[_curCar].GetComponent<WheelVehicle>().handbreak = false;

        Time.timeScale = 1;

        pauseUI.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
    }
}
