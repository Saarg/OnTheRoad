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

    virtual protected void Awake()
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
}
