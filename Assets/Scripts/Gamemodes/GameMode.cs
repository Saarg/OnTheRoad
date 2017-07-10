using UnityEngine;

public class GameMode : MonoBehaviour {
    protected static GameMode instance;

    public static GameMode Instance
    { get { return instance; } }

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
}
