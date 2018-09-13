using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenChild : MonoBehaviour {

    public string Name { get { return gameObject.name; } }

    [SerializeField]
    private GameStates _exclusiveState = GameStates.START_SCREEN;
    public GameStates ExclusiveState { get { return _exclusiveState; } }

    public bool startActive = false;
    // Use this for initialization
    void Start () {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        if (!startActive) { gameObject.SetActive(false); }
	}

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State != _exclusiveState) { gameObject.SetActive(false); }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void TempStartTheDamnGameAlready()
    {
        GameManager.Instance.ChangeGameState(GameStates.PRE_PLAY);
    }
}
