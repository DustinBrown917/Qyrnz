using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLabel : FadeLabel {

    private Text text;

	protected override void Start () {
        base.Start();
        CenterQirn.Instance.ScoreChanged += Instance_ScoreChanged;
        PlayerData.Instance.HighScoreChanged += Instance_HighScoreChanged;
        GameManager.Instance.GameModeChanged += Instance_GameModeChanged;
        text = GetComponent<Text>();

        UpdateLabel();
	}

    private void Instance_GameModeChanged(object sender, System.EventArgs e)
    {
        UpdateLabel();
    }

    private void Instance_HighScoreChanged(object sender, System.EventArgs e)
    {
        UpdateLabel();
        
    }

    private void Instance_ScoreChanged(object sender, System.EventArgs e)
    {
        if (GameManager.CurrentState == GameStates.PLAYING)
        {
            TriggerFade();
        }       
    }


    private void UpdateLabel()
    {
        text.text = PlayerData.Instance.HighScores[(int)GameManager.GameMode].ToString();
    }
}
