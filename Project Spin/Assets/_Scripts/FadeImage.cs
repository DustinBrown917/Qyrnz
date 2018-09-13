using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeImage : FadeLabel {

	// Use this for initialization
	protected override void Start () {
        base.Start();
        CenterQirn.Instance.ScoreChanged += Instance_ScoreChanged;
    }

    private void Instance_ScoreChanged(object sender, ScoreChangedEventArgs args)
    {
        if (GameManager.CurrentState == GameStates.PLAYING)
        {
            TriggerFade();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
