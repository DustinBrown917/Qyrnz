using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] tutorialPieces;

    private Animator animator;


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
	}




    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(GameManager.CurrentState == GameStates.TUTORIAL)
        {
            if (PlayerData.Instance.TutorialEnabled)
            {
                SetActiveAllTutorials(true);
                animator.Play("Tutorial", -1, 0);
            }
            else
            {
                TutorialStartPlay();
            }
            
        }
        else if(args.PreviousState == GameStates.TUTORIAL)
        {
            SetActiveAllTutorials(false);
            animator.Play("Empty", -1, 0);
        }
    }




    public void TutorialStartPlay()
    {
        GameManager.Instance.ChangeGameState(GameStates.PLAYING);
    }


    private void SetActiveAllTutorials(bool active)
    {
        for(int i = 0; i < tutorialPieces.Length; i++)
        {
            tutorialPieces[i].SetActive(active);
        }
    }

}
