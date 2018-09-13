using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour {

    private Image img;
    private Button button;

    void Start () {
        img = GetComponent<Image>();
        button = GetComponent<Button>();
        GameManager.Instance.GamePaused += Instance_GamePaused;
        GameManager.Instance.GameUnpaused += Instance_GameUnpaused;
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

    private void Instance_GameUnpaused(object sender, System.EventArgs e)
    {
        img.enabled = false;
    }

    private void Instance_GamePaused(object sender, System.EventArgs e)
    {
        img.enabled = true;
    }

    public void ReturnHome()
    {
        GameManager.Instance.ChangeGameState(GameStates.POST_PLAY);
    }
}
