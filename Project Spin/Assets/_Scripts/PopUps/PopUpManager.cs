using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopUpManager : MonoBehaviour {

    private static PopUpManager _instance;
    public static PopUpManager Instance { get { return _instance; } }

    private GameObject currentPopUp = null;

    public event EventHandler AllPopUpsClosed;
    public event EventHandler PopUpOpened;

    private EventArgs evArgs = EventArgs.Empty;

    private void Awake()
    {
        _instance = this;
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PopUp(string popupName)
    {
        bool popUpFound = false;
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.name == popupName)
            {
                if(currentPopUp != null) { currentPopUp.SetActive(false); }
                transform.GetChild(i).gameObject.SetActive(true);
                currentPopUp = transform.GetChild(i).gameObject;
                popUpFound = true;
                OnPopUpOpened(evArgs);
                break;
            }
        }

        if (!popUpFound)
        {
            Debug.LogWarning("Pop up " + popupName + " not found.");
        }
    }

    public void ClosePopUp()
    {
        if(currentPopUp == null) { return; }

        currentPopUp.SetActive(false);

        currentPopUp = null;

        OnAllPopUpsClosed(evArgs);
    }

    private void OnAllPopUpsClosed(EventArgs e)
    {
        EventHandler handler = AllPopUpsClosed;

        if (GameManager.Instance.Paused && GameManager.CurrentState == GameStates.PLAYING) { GameManager.Instance.TriggerPause(); }

        if (handler != null)
        {
            handler(this, e);
        }
    }

    private void OnPopUpOpened(EventArgs e)
    {
        EventHandler handler = PopUpOpened;

        if (!GameManager.Instance.Paused && GameManager.CurrentState == GameStates.PLAYING) { GameManager.Instance.TriggerPause(); }

        if(handler != null)
        {
            handler(this, e);
        }
    }
}
