using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonWrapper : MonoBehaviour {

    [SerializeField]
    private Sprite pauseSprite;
    [SerializeField]
    private Sprite playSprite;

    private Image image;
    private Button button;

	void Start () {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        GameManager.Instance.GamePaused += Instance_GamePaused;
        GameManager.Instance.GameUnpaused += Instance_GameUnpaused;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        PopUpManager.Instance.PopUpOpened += Instance_PopUpOpened;
        PopUpManager.Instance.AllPopUpsClosed += Instance_AllPopUpsClosed;

	}

    private void Instance_AllPopUpsClosed(object sender, System.EventArgs e)
    {
        if (!button.interactable) { button.interactable = true; }
    }

    private void Instance_PopUpOpened(object sender, System.EventArgs e)
    {
        if (button.interactable) { button.interactable = false; }
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.PLAYING || args.State == GameStates.TUTORIAL) { image.enabled = true; }
        else
        {
            if (image.enabled) { image.enabled = false; }
        }
    }

    private void Instance_GameUnpaused(object sender, System.EventArgs e)
    {
        image.sprite = pauseSprite;
    }

    private void Instance_GamePaused(object sender, System.EventArgs e)
    {
        image.sprite = playSprite;
    }

    public void TogglePause()
    {
        GameManager.Instance.TriggerPause();
    }
}
