using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class FreePlayMode : MonoBehaviour {

    private Camera _camera;

    [SerializeField]
    private GameObject[] _cars;
    [SerializeField]
    private int _curCar = 0;
    
    void Start () {
        _camera = Camera.main;

        foreach (GameObject c in _cars)
            c.GetComponent<WheelVehicle>().handbreak = true;

        _camera.GetComponent<SmoothFollow>().Target = _cars[_curCar].transform;
        _cars[_curCar].GetComponent<WheelVehicle>().handbreak = false;

    }

    void Update () {

        if (Input.GetButtonDown("Submit"))
        {
            _cars[_curCar].GetComponent<WheelVehicle>().handbreak = true;

            _curCar = (_curCar + 1) % (_cars.Length);
            Debug.Log(_curCar);

            _camera.GetComponent<SmoothFollow>().Target = _cars[_curCar].transform;
            _cars[_curCar].GetComponent<WheelVehicle>().handbreak = false;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            _cars[_curCar].transform.position = new Vector3(0, 4, 0);
        }
    }
}
