using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private static GameStates _state = GameStates.START_SCREEN;
    public static GameStates CurrentState { get { return _state; } }

    private static GameModes _gameMode = GameModes.INFINITE;
    public static GameModes GameMode { get { return _gameMode; } }

    public GameObject[] fractalBodies;

    public int[] gameModeUnlockThreshold;

    [SerializeField]
    private Gradient[] grads;

    public event EventHandler GameStarted;
    public event EventHandler GamePaused;
    public event EventHandler GameUnpaused;
    public event EventHandler GameOver;
    public event EventHandler GameModeChanged;

    private EventArgs emptyArgs = EventArgs.Empty;

    public delegate void GameStateChangedEventHandler(object sender, GameStateChangedArgs args);
    public event GameStateChangedEventHandler GameStateChanged;

    private bool _paused = false;
    public bool Paused { get { return _paused; } }

    private bool _started = false;
    public bool Started { get { return _started; } }

    private bool _over = false;
    public bool Over { get { return _over; } }

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip qirnEmerge;
    [SerializeField]
    private AudioClip qirnDescend;

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(InitGameMode());
        
	}

    private IEnumerator InitGameMode()
    {
        yield return new WaitForEndOfFrame();
        OnGameModeChanged(emptyArgs);
    }

    public Gradient GetGameModeGradient(GameModes gameMode)
    {
        return grads[(int)gameMode];
    }

    public void ToggleThroughGameModes()
    {
        if(_gameMode == GameModes.INFINITE) { _gameMode = GameModes.LIMITED; }
        else { _gameMode = GameModes.INFINITE; }
        OnGameModeChanged(emptyArgs);
        Debug.Log("Game mode changed to: " + _gameMode.ToString());
    }

    public void SetGameMode(GameModes mode)
    {
        if(_gameMode == mode) { return; }
        _gameMode = mode;
        OnGameModeChanged(emptyArgs);
        Debug.Log("Game mode changed to: " + mode.ToString());
    }

    /// <summary>
    /// Triggers the OnGameStarted event
    /// </summary>
    public void TriggerGameStart()
    {
        OnGameStarted(new EventArgs());
    }

    /// <summary>
    /// Called when the game starts.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnGameStarted(EventArgs e)
    {
        _started = true;
        EventHandler handler = GameStarted;
        
        if(handler != null)
        {
            handler(this, e);
        }
    }

    /// <summary>
    /// Toggles pause
    /// </summary>
    public void TriggerPause()
    {
        if(_state != GameStates.PLAYING && _state != GameStates.TUTORIAL) { return; }
        if (_paused)
        {
            OnGameUnpaused(new EventArgs());
        }
        else
        {
            OnGamePaused(new EventArgs());
        }
    }

    /// <summary>
    /// Called when the game changes from unpaused to paused.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnGamePaused(EventArgs e)
    {
        _paused = true;

        Time.timeScale = 0;

        EventHandler handler = GamePaused;

        if (handler != null)
        {
            handler(this, e);
        }
    }


    /// <summary>
    /// Called when the game is changed from paused to unpaused.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnGameUnpaused(EventArgs e)
    {
        _paused = false;

        Time.timeScale = 1f;

        EventHandler handler = GameUnpaused;

        if (handler != null)
        {
            handler(this, e);
        }
    }


    /// <summary>
    /// Called when game ends.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnGameOver(EventArgs e)
    {
        _over = true;

        EventHandler handler = GameOver;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    //****************Cached variables for ChangeGameState****************
    private GameStateChangedArgs gscArgs = new GameStateChangedArgs(GameStates.START_SCREEN);
    //****************Cached variables for ChangeGameState****************

    /// <summary>
    /// Change the game's state.
    /// </summary>
    /// <param name="state"></param>
    public void ChangeGameState(GameStates state)
    {
        if(state != _state)
        {
            if(state == GameStates.PRE_PLAY)
            {
                audioSource.clip = qirnEmerge;
                audioSource.Play();
            }
            else if(state == GameStates.POST_PLAY)
            {
                audioSource.clip = qirnDescend;
                audioSource.Play();
            }
            gscArgs.State = state;
            gscArgs.PreviousState = _state;
            OnGameStateChanged(gscArgs);
        }
        
        if(state == GameStates.START_SCREEN) {
            GC.Collect();
        }
        
    }


    /// <summary>
    /// Called when the game state changes.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnGameStateChanged(GameStateChangedArgs e)
    {
        if((e.State == GameStates.PRE_PLAY || e.State == GameStates.POST_PLAY) && Paused) { TriggerPause(); }
        _state = e.State;

        Debug.Log("Current State: " + _state.ToString());

        GameStateChangedEventHandler handler = GameStateChanged;

        if(handler != null)
        {
            handler(this, e);
        }

        if(_state == GameStates.POST_PLAY) {
            SaveLoadManager.SavePlayerData(PlayerData.Instance);
            Debug.Log("Saved");
        } else if (_state == GameStates.START_SCREEN) {
            if (!StartButtonWrapper.Instance.gameObject.activeSelf)
            {
                StartButtonWrapper.Instance.gameObject.SetActive(true);
            }
        }
    }

    protected virtual void OnGameModeChanged(EventArgs e)
    {
        EventHandler handler = GameModeChanged;

        if(handler != null)
        {
            handler(this, e);
        }
    }

}

public enum GameStates
{
    START_SCREEN,
    GAMEOVER_SCREEN,
    TUTORIAL,
    PLAYING,
    PRE_PLAY,
    POST_PLAY
}

[Serializable]
public enum GameModes
{
    //TUTORIAL = 0,
    INFINITE = 0,
    LIMITED,
    WARP
}

public class GameStateChangedArgs : EventArgs
{
    private GameStates _state;
    public GameStates State { get { return _state; } set { _state = value; } }

    private GameStates _previousState;
    public GameStates PreviousState { get { return _previousState; } set { _previousState = value; } }

    public GameStateChangedArgs(GameStates state)
    {
        _state = state;
    }

    public GameStateChangedArgs(GameStates state, GameStates previousState)
    {
        _state = state;
        _previousState = previousState;
    }
}
