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

    [SerializeField]
    private RectTransform speedo;

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
        
        float angle = Mathf.Lerp(speedo.rotation.ToEuler().z, -(_cars[_curCar].GetComponent<WheelVehicle>().speed) / 220 * 180, 0.53f);

        

        speedo.rotation = Quaternion.AngleAxis(4 * angle, Vector3.forward);
    }
}
