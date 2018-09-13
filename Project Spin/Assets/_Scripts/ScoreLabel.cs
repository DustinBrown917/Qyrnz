using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLabel : FadeLabel {

    private static ScoreLabel _instance;
    public static ScoreLabel Instance { get { return _instance; } }

    private Text text;

    [SerializeField]
    private Color increaseColour;
    [SerializeField]
    private Color decreaseColour;

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        CenterQirn.Instance.ScoreChanged += Instance_ScoreChanged;
        GameManager.Instance.GameModeChanged += Instance_GameModeChanged;
	}

    private void Instance_GameModeChanged(object sender, System.EventArgs e)
    {
        //increaseColour = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color;
    }

    private void Instance_ScoreChanged(object sender, System.EventArgs e)
    {
        ScoreChangedEventArgs s = e as ScoreChangedEventArgs;

        UpdateScore(s.CurrentScore, s.ScoreChangedBy);

    }

    public void UpdateScore(int score, int change)
    {
        if (change <= 0) { outline.effectColor = decreaseColour; }
        else { outline.effectColor = increaseColour; }
        text.text = score.ToString();
        if(GameManager.CurrentState == GameStates.PLAYING) { TriggerFade(); }

    }

}
