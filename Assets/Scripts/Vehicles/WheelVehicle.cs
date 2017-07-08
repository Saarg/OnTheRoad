using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class WheelVehicle : MonoBehaviour {

    [Header("Inputs")]
    public string throtleInput = "Vertical";
    public string turnInput = "Horizontal";
    public bool ignoreThrotleInput { get; set; }
    public bool ignoreThrotleExternalInput { get; set; }
    public bool ignoreTurnInput { get; set; }
    public bool ignoreTurnExternalInput { get; set; }

    [Header("Wheels")]
    public WheelCollider[] driveWheel;
    public WheelCollider[] turnWheel;

    [Header("Behaviour")]
    public AnimationCurve motorTorque;
    public float brakeForce = 1500.0f;
    public float steerAngle = 30.0f;
    [Range(0.001f, 1.0f)]
    public float steerSpeed = 0.2f;

    public Transform centerOfMass;

    [Header("External inputs")]
    public float steering = 0.0f;
    public float throtle { get; set; }

    public bool handbreak = false;

    public float speed = 0.0f;

    [Header("Particles")]
    public ParticleSystem gasParticle;

    private Rigidbody _rb;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();

        if (centerOfMass)
        {
            _rb.centerOfMass = centerOfMass.localPosition;
        }
    }
	
	void FixedUpdate () {
        speed = transform.InverseTransformDirection(_rb.velocity).z * 3.6f;

        if (ignoreThrotleInput && !ignoreThrotleExternalInput)
        {
            throtle = Mathf.Clamp(throtle, -1, 1);
        }
        else if (!ignoreThrotleInput && throtleInput != "" && throtleInput != null)
        {
            // throtle = Input.GetAxis(throtleInput) != 0 ? Input.GetAxis(throtleInput) : Mathf.Clamp(throtle, -1, 1);
            throtle = Input.GetAxis(throtleInput); 
        }

        if (ignoreTurnInput && !ignoreTurnExternalInput)
        {
            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, Mathf.Clamp(steering / 180, -1, 1) * steerAngle, steerSpeed);
            }
        }
        else if (!ignoreTurnInput && turnInput != "" && turnInput != null)
        {
            foreach (WheelCollider wheel in turnWheel)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, Input.GetAxis(turnInput) * steerAngle, steerSpeed);
            }
        }

        foreach (WheelCollider wheel in GetComponentsInChildren<WheelCollider>())
        {
            wheel.brakeTorque = 0;
        }

        if (handbreak)
        {
            foreach (WheelCollider wheel in GetComponentsInChildren<WheelCollider>())
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = brakeForce;
            }
        }
        else if (Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(throtle))
        {
            foreach (WheelCollider wheel in driveWheel)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = throtle * motorTorque.Evaluate(speed) * 4;
            }
        }
        else
        {
            foreach (WheelCollider wheel in GetComponentsInChildren<WheelCollider>())
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = Mathf.Abs(throtle) * brakeForce;
            }
        }

            _rb.AddForce(transform.forward * -speed/10 * _rb.mass);

        if(gasParticle)
        {
            ParticleSystem.EmissionModule em = gasParticle.emission;
            em.rateOverTime = handbreak ? 0 : Mathf.Lerp(em.rateOverTime.constant, Mathf.Clamp(10.0f * throtle, 5.0f, 10.0f), 0.1f);
        }
	}

    public void toogleHandbrake(bool h)
    {
        handbreak = h;
    }
}
