using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : MonoBehaviour {
    protected static GameMode instance;

    public static GameMode Instance
    { get { return instance; } }

	[SerializeField]
	protected Text lapTimer;
	public Text LapTimer
	{ get { return lapTimer; } }

	protected WebSocket w;
	protected string gameMode = "none";

	virtual protected IEnumerator Start ()
	{
    	Debug.Log("Opening socket");
		w = new WebSocket (new System.Uri ("ws://localhost:8000"));
		yield return StartCoroutine (w.Connect ());

		Debug.Log("Sending gamemode");
		w.SendString (gameMode);
	}

    virtual protected void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("More than one FreePlayMode in the scene!!! deleting one...");
            Destroy(this);
        }
    }

	public void LoadScene(string name) {
		Time.timeScale = 1f;
		SceneManager.LoadScene("Assets/Scenes/" + name + ".unity", LoadSceneMode.Single);
	}


	public void Quit()
	{
		Application.Quit();
	}

	virtual protected void OnDestroy() {
		Debug.Log ("Closing socket");
        w.SendString("qut gamemode destroyed");
		w.Close ();
	}
}
