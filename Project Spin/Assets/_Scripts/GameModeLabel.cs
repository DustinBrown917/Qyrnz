using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeLabel : MonoBehaviour {

    [SerializeField]
    private GameModes gameMode;

    private Text highScoreText;
    //private Outline scoreOutline;
    private Outline graphicOutline;

    private void Awake()
    {
        highScoreText = transform.Find("CurrentHighScore").Find("CurrentHighScoreText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        UpdateHighScoreLabel();
        PlayerData.Instance.PlayerDataReset += Instance_PlayerDataReset;
    }

    private void Instance_PlayerDataReset(object sender, System.EventArgs e)
    {
        UpdateHighScoreLabel();
    }

    void Start () {
        UpdateHighScoreLabel();
        graphicOutline = transform.Find("GameModeImage").GetComponent<Outline>();

        graphicOutline.effectColor = GameManager.Instance.GetGameModeGradient(gameMode).colorKeys[1].color;

    }

    public void UpdateHighScoreLabel()
    {
        highScoreText.text = PlayerData.Instance.HighScores[(int)gameMode].ToString();
    }
}
