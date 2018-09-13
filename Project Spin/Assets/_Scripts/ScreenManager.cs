using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    private static ScreenManager _instance;
    public static ScreenManager Instance { get { return _instance; } }

    private Dictionary<string, ScreenChild> screenChilds = new Dictionary<string, ScreenChild>();

    private void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(FindChildScreens());
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
	}

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.START_SCREEN) { screenChilds["StartScreen"].gameObject.SetActive(true); }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private IEnumerator FindChildScreens()
    {
        yield return new WaitForEndOfFrame();

        ScreenChild[] childScreens = FindObjectsOfType<ScreenChild>();

        for(int i = 0; i < childScreens.Length; i++)
        {
            screenChilds.Add(childScreens[i].Name, childScreens[i]);
        }

    }
}
