using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnableButton : MonoBehaviour {

    [SerializeField]
    private Sprite[] toggleImages;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        CheckImage();
    }

    public void ToggleTutorial()
    {
        PlayerData.Instance.TutorialEnabled = !PlayerData.Instance.TutorialEnabled;
        CheckImage();
    }

    private void CheckImage()
    {
        if (PlayerData.Instance.TutorialEnabled)
        {
            button.image.sprite = toggleImages[1];
        }
        else
        {
            button.image.sprite = toggleImages[0];
        }
    }
}
