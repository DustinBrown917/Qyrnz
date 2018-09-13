using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class PU_Revive : MonoBehaviour {

    private Animator animator;
    private ShowOptions showOptions = new ShowOptions();

    private void OnEnable()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        animator.Play("FadeIn", -1, 0f);
    }

    private void Start()
    {
        showOptions.resultCallback = HandleShowResult;
    }

    public void ShowAdvertisement()
    {
        Advertisement.Show("rewardedVideo", showOptions);
    }

    void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                CenterQirn.Instance.Revive();
                break;
            case ShowResult.Skipped:
                GameManager.Instance.ChangeGameState(GameStates.POST_PLAY);
                break;
            case ShowResult.Failed:
                CenterQirn.Instance.Revive();
                break;
        }

        PopUpManager.Instance.ClosePopUp();
    }

    public void Close()
    {
        GameManager.Instance.ChangeGameState(GameStates.POST_PLAY);
        PopUpManager.Instance.ClosePopUp();
    }
}
