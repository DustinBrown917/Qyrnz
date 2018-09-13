using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButtonWrapper : MonoBehaviour {

    [SerializeField]
    private float slideTime = 0.3f;
    [SerializeField]
    private float waitTime = 1f;

    private WaitForSeconds wfs;

    private GameObject confirmButton;

    private Vector3 confirmButtonRestPos;
    private Vector3 confirmButtonTargetPos; 

    private bool confirmButtonShowing = false;

	void Start () {
        confirmButton = transform.GetChild(0).gameObject;
        confirmButtonRestPos = confirmButton.transform.localPosition;
        confirmButtonTargetPos = new Vector3(0, confirmButton.transform.localPosition.y, confirmButton.transform.localPosition.z);
        wfs = new WaitForSeconds(waitTime);
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
	}

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.START_SCREEN)
        {
            confirmButton.transform.position = confirmButtonRestPos;
            confirmButtonShowing = false;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void DeletePlayerData()
    {
        PlayerData.Instance.ResetData();
    }

    public void ShowConfirmButton()
    {
        if (!confirmButtonShowing)
        {
            StartCoroutine(IShowConfirmButton());
        }
    }

    private IEnumerator IShowConfirmButton()
    {
        confirmButtonShowing = true;
        float t = 0;

        while (t < slideTime)
        {
            confirmButton.transform.localPosition = Vector3.Lerp(confirmButtonRestPos, confirmButtonTargetPos, t/slideTime);
            t += Time.deltaTime;
            yield return null;
        }
        confirmButton.transform.localPosition = confirmButtonTargetPos;
        StartCoroutine(WaitForConfirm());
    }

    private IEnumerator WaitForConfirm()
    {
        yield return wfs;
        StartCoroutine(IHideConfirmButton());
    }

    private IEnumerator IHideConfirmButton()
    {
        float t = 0;
        while (t < slideTime)
        {
            confirmButton.transform.localPosition = Vector3.Lerp(confirmButtonTargetPos, confirmButtonRestPos, t / slideTime);
            t += Time.deltaTime;
            yield return null;
        }
        confirmButton.transform.localPosition = confirmButtonRestPos;
        confirmButtonShowing = false;
    }
}
