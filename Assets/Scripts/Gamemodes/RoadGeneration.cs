using UnityEngine;

public class RoadGeneration : GameMode
{
    new public static RoadGeneration Instance
    { get { return (RoadGeneration)instance; } }

    public GameObject[] prefabs;

    protected Vector3 lastExitPoint = new Vector3(0,0,0);
    protected Quaternion lastExitPointRot = new Quaternion();

    // Use this for initialization
    void Start () {

        for(int i = 0; i < 60; i++)
        {
            GameObject lastCreated = Instantiate(prefabs[Random.Range(0,255) % prefabs.Length], lastExitPoint, new Quaternion());

            /** Aligning **/
            Vector3 translation = lastCreated.transform.Find("EntryPoint").transform.position - lastExitPoint;
            Vector3 rotation = lastExitPointRot.eulerAngles - lastCreated.transform.Find("EntryPoint").eulerAngles;

            if(false)
            {
                print("EntryPointPos " + lastCreated.transform.Find("EntryPoint").transform.position);
                print("ExitPointPos " + lastExitPoint);
                print("translate " + -translation);
            }

            if(false)
            {
                print("EntryPointPosRot " + lastCreated.transform.Find("EntryPoint").eulerAngles + ". local : " + lastCreated.transform.Find("EntryPoint").localEulerAngles);
                print("ExitPointPosRot " + lastExitPointRot.eulerAngles);
                print("Rotate " + rotation);
            }

            lastCreated.transform.Rotate(rotation);
            lastCreated.transform.Translate(- translation);

            print("Item " + i + " Placed.");
            /**************/


            lastExitPoint = lastCreated.transform.Find("ExitPoint").transform.position;
            lastExitPointRot = lastCreated.transform.Find("ExitPoint").transform.rotation;
        }
		
	}
}
