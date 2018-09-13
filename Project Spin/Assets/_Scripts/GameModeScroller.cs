using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameModeScroller : MonoBehaviour, IEndDragHandler, IBeginDragHandler {

    private ScrollRect scrollRect;
    private int childCount = 1;
    private int targetChild = 0;
    private float TargetChildPos { get { return (float)targetChild / ((float)childCount - 1f); } }
    private Transform content;

    private AudioSource audioSource;

    private Coroutine autoSlide = null;

    private bool started = false;

    private GameObject rightSideSliderArrow;
    private Image raImage;
    private GameObject leftSideSliderArrow;
    private Image laImage;



    private Color arrowsMaxColour = new Color(1, 1, 1, 0.30588f);

    private void OnEnable()
    {
        if (started)
        {
            UpdateChildCount();
            scrollRect.horizontalNormalizedPosition = (float)GameManager.GameMode / ((float)childCount - 1f);
            OnScrollListener(scrollRect.normalizedPosition);
        }
    }

    void Start () {
        content = transform.Find("Viewport").Find("Content");
        rightSideSliderArrow = transform.Find("Viewport").Find("Right Slider Arrow").gameObject;
        raImage = rightSideSliderArrow.GetComponent<Image>();
        leftSideSliderArrow = transform.Find("Viewport").Find("Left Slider Arrow").gameObject;
        laImage = leftSideSliderArrow.GetComponent<Image>();

        audioSource = GetComponent<AudioSource>();
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScrollListener);
        FixContent();
        UpdateChildCount();
        PlayerData.Instance.PlayerDataReset += Instance_PlayerDataReset;
        GameManager.Instance.SetGameMode((GameModes)childCount-1);
        scrollRect.horizontalNormalizedPosition = (float)GameManager.GameMode / ((float)childCount - 1f); //Squashes the bug that caused the game to start with game mode and slider misaligned. (ie. Warp game mode, but infinite symbol)
        OnScrollListener(scrollRect.normalizedPosition);
        started = true;
	}


    private void Instance_PlayerDataReset(object sender, System.EventArgs e)
    {
        UpdateChildCount();
        if(targetChild >= childCount) { targetChild = childCount - 1; }
        GameManager.Instance.SetGameMode((GameModes)targetChild);
        
        OnScrollListener(scrollRect.normalizedPosition);
        
    }

    private void FixContent()
    {
        if(content.transform.position.y > 320)
        {
            content.localPosition = new Vector3(0, 320, 0);
        }
        
    }

    private void UpdateChildCount()
    {
        int c = 0;

        for(int i = 0; i < content.childCount; i++)
        {
            if(i > 0)
            {
                if(PlayerData.Instance.HighScores[i - 1] < GameManager.Instance.gameModeUnlockThreshold[i])
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    content.GetChild(i).gameObject.SetActive(true);
                    c++;
                }
            }
            else { c++; }
        }

        childCount = c;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (autoSlide != null)
        {
            StopCoroutine(autoSlide);
            autoSlide = null;
        }
    }

    private void OnScrollListener(Vector2 value)
    {
        float scaledValue = value.x * (childCount - 1);
        int checkChild = Mathf.RoundToInt(scaledValue);
        
        if(checkChild != targetChild)
        {
            audioSource.Play();
            targetChild = checkChild;
            GameManager.Instance.SetGameMode((GameModes)targetChild);
        }

        EvaluateArrows(scaledValue - targetChild);
    }

    private Vector3 rightArrowTransitionPosition;
    private Vector3 leftArrowTransitionPosition;

    [SerializeField]
    private float arrowPullDistance = 0.75f;

    private void EvaluateArrows(float value)
    {
        rightArrowTransitionPosition = rightSideSliderArrow.transform.position;
        leftArrowTransitionPosition = leftSideSliderArrow.transform.position;

        rightArrowTransitionPosition.x = (((Screen.width * arrowPullDistance) * -value) + (Screen.width));
        leftArrowTransitionPosition.x = ((Screen.width * arrowPullDistance) * -value);

        EvaluateArrowColours(value);

        rightArrowTransitionPosition.x = Mathf.Clamp(rightArrowTransitionPosition.x, 0, Screen.width);
        leftArrowTransitionPosition.x = Mathf.Clamp(leftArrowTransitionPosition.x, 0, Screen.width);

        rightSideSliderArrow.transform.position = rightArrowTransitionPosition;
        leftSideSliderArrow.transform.position = leftArrowTransitionPosition;
    }

    [SerializeField]
    private float longFade = 0.5f;
    [SerializeField]
    private float shortFade = 0.1f;

    private void EvaluateArrowColours(float value)
    {
        if(value >= 0)
        {
            raImage.color = Color.Lerp(arrowsMaxColour, Color.clear, value/ longFade);
            laImage.color = Color.Lerp(arrowsMaxColour, Color.clear, value / shortFade);
        }
        else
        {
            laImage.color = Color.Lerp(arrowsMaxColour, Color.clear, value / -longFade);
            raImage.color = Color.Lerp(arrowsMaxColour, Color.clear, value / -shortFade);
        }

        if(targetChild == 0)
        {
            laImage.color = Color.clear;
        }

        if(targetChild == childCount - 1)
        {
            raImage.color = Color.clear;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        autoSlide = StartCoroutine(AutoSlide());

    }

    private WaitForSeconds wfs = new WaitForSeconds(0.15f);
    private IEnumerator AutoSlide()
    {
        yield return wfs;
        float vel = 0;

        while(scrollRect.horizontalNormalizedPosition > TargetChildPos + 0.001 || scrollRect.horizontalNormalizedPosition < TargetChildPos - 0.001)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.SmoothDamp(scrollRect.horizontalNormalizedPosition, TargetChildPos, ref vel, 0.1f);
            yield return null;
        }
        scrollRect.horizontalNormalizedPosition = TargetChildPos;
        autoSlide = null;
    }

}
