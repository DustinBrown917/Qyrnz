using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerData : MonoBehaviour {

    private static PlayerData _instance;
    public static PlayerData Instance { get { return _instance; } }

    private static bool saved = true;

    private int[] _highScores = new int[Enum.GetNames(typeof(GameModes)).Length];
    public int[] HighScores { get { return _highScores; } }

    private bool _tutorialEnabled = true;
    public bool TutorialEnabled { get { return _tutorialEnabled; } set { _tutorialEnabled = value; } }

    private ScoreChangedEventArgs scoreChangedEventArgs = new ScoreChangedEventArgs();
    public event EventHandler HighScoreChanged;
    public event EventHandler PlayerDataReset;
    private EventArgs evArgs = EventArgs.Empty;

    private void Awake()
    {
        
        _instance = this;
    }


    void Start () {
     
        CenterQirn.Instance.ScoreChanged += Instance_ScoreChanged;

        Load();
    }

    private void Instance_ScoreChanged(object sender, System.EventArgs e)
    {
        scoreChangedEventArgs = e as ScoreChangedEventArgs;
        UpdateHighScore(scoreChangedEventArgs.CurrentScore, scoreChangedEventArgs.CurrentGameMode);
    }

    private void UpdateHighScore(int score, GameModes gameMode)
    {

        if(score <= _highScores[(int)gameMode]) { return; }
        _highScores[(int)gameMode] = score;
        
        OnHighScoreChanged(evArgs);
        saved = false;
    }

    private void OnHighScoreChanged(EventArgs e)
    {
        EventHandler handler = HighScoreChanged;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    public void Save()
    {
        if (!saved)
        {
            SaveLoadManager.SavePlayerData(this);
            saved = true;
        }
        
    }

    public void Load()
    {
        PlayerDataContainer pdc = SaveLoadManager.LoadPlayerData();
        if(pdc != null)
        {

            _highScores = pdc.HighScores;
            if(_highScores == null)
            {
                _highScores = new int[Enum.GetNames(typeof(GameModes)).Length];
            }

            _tutorialEnabled = pdc.TutorialEnabled;
        }
    }

    public void ResetData()
    {
        SaveLoadManager.ClearPlayerData();

        for(int i = 0; i < _highScores.Length; i++)
        {
            _highScores[i] = 0;
        }
        OnPlayerDataReset(evArgs);
        OnHighScoreChanged(evArgs);
    }
    
    private void OnPlayerDataReset(EventArgs e)
    {
        EventHandler handler = PlayerDataReset;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) { Save(); }
    }

    private void OnApplicationQuit()
    {
        Save();
    }


}
