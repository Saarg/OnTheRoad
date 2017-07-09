using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class RecorededTime
{
    public string startID;
    public string endID;

    public float lapTime;
    public List<float> checkPoints;
}

public class CheckPoint : MonoBehaviour {
    static private string startID;
    static private string endID;

    static private float startTime = 0;
    static private List<float> checkPoints = new List<float>();

    private void Update()
    {
        if (startTime != 0)
            FreePlayMode.Instance.LapTimer.text = "Lap Time: " + (Time.realtimeSinceStartup - startTime).ToString("F2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.tag == "Player")
        {
            if (tag == "EntryPoint" && startTime == 0)
            {
                startTime = Time.realtimeSinceStartup;

                startID = name;
            }
            else if (tag == "EntryPoint" && startTime != 0)
            {
                endID = name;
                RegisterTime();
            }
            else if (startTime != 0)
            {
                checkPoints.Add(Time.realtimeSinceStartup - startTime);
            }
        }
    }

    private void RegisterTime()
    {
        if (startID == endID)
        {
            Reset();
            return;
        }
        
        float currentTime = Time.realtimeSinceStartup - startTime;

        RecorededTime BestTime = new RecorededTime();
        if (File.Exists(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "-" + startID + "-" + endID))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "-" + startID + "-" + endID, FileMode.Open, FileAccess.Read, FileShare.Read);
            BestTime = (RecorededTime)formatter.Deserialize(stream);
            stream.Close();
        }

        if ((BestTime.lapTime == 0 || BestTime.lapTime > currentTime))
        {
            Debug.Log("New record!!! " + currentTime);
            FreePlayMode.Instance.LapTimer.text = "New record!!! " + currentTime.ToString("F2");

            RecorededTime recordedTime = new RecorededTime();
            recordedTime.startID = startID;
            recordedTime.endID = endID;
            recordedTime.lapTime = currentTime;
            recordedTime.checkPoints = checkPoints;

            // Serialize token and decoded payload
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "-" + startID + "-" + endID, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, recordedTime);
            stream.Close();
        }
        else
        {
            Debug.Log("Lap time: " + currentTime);
            FreePlayMode.Instance.LapTimer.text = "Lap Time: " + currentTime.ToString("F2");
        }

        Reset();
    }

    public void InvokeReset()
    {
        Reset();
    }

    static public void Reset()
    {
        startTime = 0;
        checkPoints.Clear();
    }
}
