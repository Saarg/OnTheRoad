using UnityEngine;

public class RoadGeneration : GameMode
{
    new public static RoadGeneration Instance
    { get { return (RoadGeneration)instance; } }

    public GameObject[] prefabs;

    protected int turnCount = 0;

    protected Vector3 lastExitPoint = new Vector3(0,0,0);
    protected Quaternion lastExitPointRot = new Quaternion();

    // Use this for initialization
    void Start () {

        for(int i = 0; i < 60; i++)
        {
            GameObject toCreate = prefabs[Random.Range(0, 255) % prefabs.Length];
            int futureTurnCount = turnCount + isTurn(toCreate);

            /** Detecting if we must enforce a height change **/
            if (isDown(toCreate) != 0)
            {
                turnCount = 0;
            }
            else if (Mathf.Abs(futureTurnCount) >= 3)
            {
                while (isDown(toCreate) == 0)
                {
                    toCreate = prefabs[Random.Range(0, 255) % prefabs.Length];
                }
                turnCount = 0;
                print("levelDown");
            }
            else
            {
                turnCount = futureTurnCount;
            }

            /** Creating the Track **/
            GameObject lastCreated = Instantiate(toCreate, lastExitPoint, new Quaternion());

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

            /** Preparing next creation **/
            lastExitPoint = lastCreated.transform.Find("ExitPoint").transform.position;
            lastExitPointRot = lastCreated.transform.Find("ExitPoint").transform.rotation;
        }

    }

    protected int isTurn(GameObject track)
    {
        float diff = track.transform.Find("EntryPoint").transform.eulerAngles.y - track.transform.Find("ExitPoint").transform.eulerAngles.y;

        if (diff == 0)
        {
            return 0;
        }
        if (diff > 0) return 1;
        else return -1;
    }

    protected int isDown(GameObject track)
    {
        float diff = track.transform.Find("EntryPoint").transform.position.y - track.transform.Find("ExitPoint").transform.position.y;
        if (Mathf.Abs(diff) < 0.2) //small range to avoir rounding errors
        {
            return 0;
        }
        if (diff > 0) return 1;
        else return -1;
    }
}
