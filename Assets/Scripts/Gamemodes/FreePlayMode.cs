﻿using System;
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
    [SerializeField]
    private Text lapTimer;

    new public static FreePlayMode Instance
    { get { return (FreePlayMode) instance; } }

    public Text LapTimer
    { get { return lapTimer; } }

    override protected void Awake()
    {
        base.Awake();

        carDropDown.options.Clear();
        foreach (GameObject car in _cars)
        {
            carDropDown.options.Add(new Dropdown.OptionData(car.name));
        }
    }

    void Start () {
        _camera = Camera.main;

        foreach (GameObject c in _cars)
            c.GetComponent<WheelVehicle>().handbreak = true;

        _camera.GetComponent<SmoothFollow>().Target = _cars[_curCar].transform;
        _cars[_curCar].GetComponent<WheelVehicle>().handbreak = false;

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

    public void Quit()
    {
        Application.Quit();
    }
}
