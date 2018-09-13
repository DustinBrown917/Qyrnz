using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonWrapper : MonoBehaviour {

    private static StartButtonWrapper _instance;
    public static StartButtonWrapper Instance { get { return _instance; } }


    [SerializeField]
    private float timerWaitTime = 2f;
    [SerializeField]
    private float targetScale = 5f;
    [SerializeField]
    private float shrinkTime = 0.3f;

    private Coroutine expander = null;
    private Coroutine shrinker = null;

    private Coroutine currentTimer = null;
    [SerializeField]
    private WaitForSecondsRealtime waitForSeconds;
    private Animator animator;
    private Image touchGraphic;

    private AudioSource audioSource;

    

    private GameObject touchExpander;
    private Image touchExpanderImage;
    private Sprite initialGraphic;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        touchGraphic = transform.Find("Graphic").GetComponent<Image>();
        touchExpander = transform.Find("TouchExpander").gameObject;
        touchExpanderImage = touchExpander.GetComponent<Image>();
        initialGraphic = touchExpanderImage.sprite;

        audioSource = GetComponent<AudioSource>();
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.PRE_PLAY)
        {
            touchGraphic.enabled = false;
        }
        else if(args.State == GameStates.START_SCREEN)
        {
            touchGraphic.enabled = true;
            animator.Play("Empty", -1, 0);

        }
    }

    /// <summary>
    /// Called at the end of the RingDestroy animation.
    /// </summary>
    public void ResetTouchExpander()
    {
        touchExpanderImage.sprite = initialGraphic;
        touchExpander.transform.localScale = Vector3.zero;
    }


    public void StartHoldTimer()
    {
        if(currentTimer != null || (GameManager.CurrentState != GameStates.PLAYING && GameManager.CurrentState != GameStates.START_SCREEN && GameManager.CurrentState != GameStates.TUTORIAL)) { return; }
        audioSource.pitch = 1.2f;
        audioSource.Play();
        currentTimer = StartCoroutine(Timer());
        if(shrinker != null) {
            StopCoroutine(shrinker);
            shrinker = null;
        }
        expander = StartCoroutine(Expand());
        
    }

    public void StopHoldTimer()
    {
        if( currentTimer != null)
        {
            StopCoroutine(currentTimer);
            if(expander != null)
            {
                StopCoroutine(expander);
                expander = null;
            }
            shrinker = StartCoroutine(Shrink());
            currentTimer = null;
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(timerWaitTime);

        ChangePrePostState();
        shrinker = null;
        expander = null;      
        currentTimer = null;
    }

    

    private IEnumerator Expand()
    {
        float t = 0;
        float s = 1;
        while (t < timerWaitTime)
        {

            touchExpander.transform.localScale = Vector2.one * Mathf.Lerp(s, targetScale, t / timerWaitTime);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        expander = null;
    }

    private IEnumerator Shrink()
    {
        float t = 0;
        float currentScale = touchExpander.transform.localScale.x;

        while (t < shrinkTime)
        {

            touchExpander.transform.localScale = Vector2.one * Mathf.Lerp(currentScale, 0, t / shrinkTime);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        touchExpander.transform.localScale = Vector3.zero;
        shrinker = null;
    }

    private void ChangePrePostState()
    {
        Handheld.Vibrate();
        //audioSource.pitch = 1.2f;
        //audioSource.Play();
        animator.Play("RingDestroy", -1, 0f);
        if (GameManager.CurrentState == GameStates.START_SCREEN)
        {
            GameManager.Instance.ChangeGameState(GameStates.PRE_PLAY);
        }
        else if (GameManager.CurrentState == GameStates.PLAYING || GameManager.CurrentState == GameStates.TUTORIAL)
        {           
            GameManager.Instance.ChangeGameState(GameStates.POST_PLAY);
            
        }

        
    }
}
