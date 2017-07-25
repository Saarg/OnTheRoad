using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using Controls;

public class RoadGeneration : GameMode
{
    new public static RoadGeneration Instance
    { get { return (RoadGeneration)instance; } }

	public int seed;

	public int curTrack = 0;
	public TrackType[] track;

	private GameObject car;

	[SerializeField]
	private RectTransform speedo;
	[SerializeField]
	private Text seedInput;

	[SerializeField]
	private Text pauseText;
	[SerializeField]
	private EventSystem eventSystem;
	private float pauseTimer;

    protected int turnCount = 0;

	protected CheckPoint start;
	protected CheckPoint finish;

	void Start() {
		car = Instantiate (track[curTrack].Car, Vector3.up * 2, Quaternion.identity);
		Camera.main.GetComponent<SmoothFollow> ().Target = car.transform;

		car.transform.position = Vector3.up * 2;
		car.transform.rotation = Quaternion.identity;
		car.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		car.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

		Generate ();
	}

	void Update() {
		if (Controls.MultiOSControls.Instance.getValue("Cancel") != 0 && Time.realtimeSinceStartup - pauseTimer > 1f)
		{
			pauseTimer = Time.realtimeSinceStartup;

			eventSystem.sendNavigationEvents = !pauseText.gameObject.activeSelf;
			car.GetComponent<WheelVehicle>().handbreak = !pauseText.gameObject.activeSelf;

			Time.timeScale = pauseText.gameObject.activeSelf ? 1 : 0;

			pauseText.gameObject.SetActive (!pauseText.gameObject.activeSelf);
		}

		if (Controls.MultiOSControls.Instance.getValue("Submit") != 0)
		{
			start.ResetTime ();
			finish.ResetTime ();

			car.transform.position = Vector3.up * 2;
			car.transform.rotation = Quaternion.identity;
			car.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			car.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;

			Generate ();
		}

		float angle = Mathf.Lerp(speedo.rotation.ToEuler().z, -(car.GetComponent<WheelVehicle>().speed) / 220 * 180, 0.53f);

		speedo.rotation = Quaternion.AngleAxis(4 * angle, Vector3.forward);
	}

	public void UpdateSeed(string s) {
		try {
			seed = System.Int32.Parse(s);
		} catch {
			seed = 0;
		}
	}

    public void Generate () {
		int turnCount = 0;

		Vector3 lastExitPoint = new Vector3(0,0,0);
		Quaternion lastExitPointRot = new Quaternion();

		GameObject startTile = gameObject;
		GameObject finishTile = gameObject;

		car.transform.position = Vector3.up * 2;
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}

		seed = seed != 0 ? seed : Random.Range (0, 99999);
		seedInput.text = seed.ToString();
		Random.InitState (seed);

		for(int i = 0; i < track[curTrack].length; i++)
        {
			GameObject toCreate = track[curTrack].Tiles[Random.Range(0, 255) % track[curTrack].Tiles.Length];
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
					toCreate = track[curTrack].Tiles[Random.Range(0, 255) % track[curTrack].Tiles.Length];
                }
                turnCount = 0;
            }
            else
            {
                turnCount = futureTurnCount;
            }

            /** Creating the Track **/
			GameObject lastCreated = Instantiate(toCreate, lastExitPoint, new Quaternion(), transform);

			if (i == 0)
				startTile = lastCreated;
			if (i == track[curTrack].length-1)
				finishTile = lastCreated;

            /** Aligning **/
            Vector3 translation = lastCreated.transform.Find("EntryPoint").transform.position - lastExitPoint;
            Vector3 rotation = lastExitPointRot.eulerAngles - lastCreated.transform.Find("EntryPoint").eulerAngles;

            lastCreated.transform.Rotate(rotation);
            lastCreated.transform.Translate(- translation);

            /** Preparing next creation **/
            lastExitPoint = lastCreated.transform.Find("ExitPoint").transform.position;
            lastExitPointRot = lastCreated.transform.Find("ExitPoint").transform.rotation;
        }

		if (startTile != gameObject && finishTile != gameObject) {
			startTile.SetActive (false);
			startTile.tag = "EntryPoint";
			startTile.name = track[curTrack].name + "-Start";
			BoxCollider b1 = startTile.AddComponent<BoxCollider> ();
			b1.size = new Vector3 (6, 4, 6);
			b1.center = Vector3.up * 2;
			b1.isTrigger = true;
			start = startTile.AddComponent<CheckPoint> ();

			finishTile.SetActive (false);
			finishTile.tag = "EntryPoint";
			finishTile.name = seed.ToString () + "-End";
			BoxCollider b2 = finishTile.AddComponent<BoxCollider> ();
			b2.size = new Vector3 (6, 4, 6);
			b2.center = Vector3.up * 2;
			b2.isTrigger = true;
			finish = finishTile.AddComponent<CheckPoint> ();

			start.End = finish.gameObject;
			finish.End = start.gameObject;

			startTile.SetActive (true);
			finishTile.SetActive (true);
		}
    }

    protected int isTurn(GameObject t)
    {
        float diff = 180 + t.transform.Find("EntryPoint").transform.eulerAngles.y - t.transform.Find("ExitPoint").transform.eulerAngles.y;

        if (diff == 0)
        {
            return 0;
        }
        if (diff > 0) return 1;
        else return -1;
    }

    protected int isDown(GameObject t)
    {
        float diff = t.transform.Find("EntryPoint").transform.position.y - t.transform.Find("ExitPoint").transform.position.y;
        if (Mathf.Abs(diff) < 0.2) //small range to avoir rounding errors
        {
            return 0;
        }
        if (diff > 0) return 1;
        else return -1;
    }

	public void SelectTrack(System.Int32 t)
	{    
		curTrack = t;

		Destroy (car);

		car = Instantiate (track[curTrack].Car, Vector3.up * 2, Quaternion.identity);
		Camera.main.GetComponent<SmoothFollow> ().Target = car.transform;

		Generate ();
	}

}
