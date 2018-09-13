using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class CenterQirn : MonoBehaviour {

    private static CenterQirn _instance;
    public static CenterQirn Instance { get { return _instance; } }

    [SerializeField]
    private int _score;
    public int Score { get { return _score; } }

    private OrbitQirnShield _shield;
    public OrbitQirnShield Shield { get { return _shield; } set { _shield = value; } }

    [SerializeField]
    private float pointDecayFactor = 0.25f;

    [SerializeField]
    private int _revivesAllowed = 2;
    private int _revivesRemaining;
    private bool canLoseRevives = true;

    private SpriteRenderer outerGlow;
    private SpriteRenderer horizonGlow;
    private SpriteRenderer coronaGlow;

    private ParticleSystem shimmerPS;
    private ParticleSystem.MainModule shimmerMain;

    [SerializeField]
    private AudioClip[] clips;
    private AudioSource audioSource;

    public delegate void ScoreChangedEventHandler(object sender, ScoreChangedEventArgs args);
    public event ScoreChangedEventHandler ScoreChanged;

    private EventArgs evArgs = EventArgs.Empty;
    public event EventHandler Revived;
    public event EventHandler FractalQirnCollided;

    private ScoreChangedEventArgs scoreChangedArgs = new ScoreChangedEventArgs();

    

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        outerGlow = transform.Find("Outer Glow").GetComponent<SpriteRenderer>();
        horizonGlow = transform.Find("Horizon Glow").GetComponent<SpriteRenderer>();
        coronaGlow = transform.Find("Corona").Find("CoronaGlow").GetComponent<SpriteRenderer>();
        shimmerPS = transform.Find("Shimmer").GetComponent<ParticleSystem>();
        shimmerMain = shimmerPS.main;

        audioSource = GetComponent<AudioSource>();

        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        GameManager.Instance.GameModeChanged += Instance_GameModeChanged;
        FractalQirnCollided += CenterQirn_FractalQirnCollided;
	}

    private void CenterQirn_FractalQirnCollided(object sender, EventArgs e)
    {
        if (hitColourChanging != null)
        {
            StopCoroutine(hitColourChanging);
            hitColourChanging = null;
        }

        hitColourChanging = StartCoroutine(HitColourChange(1f));
    }

    private void Instance_GameModeChanged(object sender, EventArgs e)
    {
        if(transitioningColour != null)
        {
            StopCoroutine(transitioningColour);
            transitioningColour = null;
        }

        transitioningColour = StartCoroutine(TransitionColour(0.75f));
    }




    public void Revive()
    {
        OnRevived(evArgs);
    }




    public void SetShieldActive(bool active)
    {
        if (_shield.IsInDisablingProcess) { _shield.gameObject.SetActive(false); }
        _shield.gameObject.SetActive(active);
    }




    private Coroutine transitioningColour = null;
    private Color targetGlowColor = new Color();
    [SerializeField]
    private float targetGlowAlpha = 0;

    private IEnumerator TransitionColour(float time)
    {
        float t = 0f;
        Color initialColour = outerGlow.color;
        initialColour.a = 1;

        while(t < 1f)
        {
            t += Time.deltaTime / time;
            targetGlowColor = Color.Lerp(initialColour, GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color, t);
            horizonGlow.color = targetGlowColor;
            shimmerMain.startColor = targetGlowColor;
            coronaGlow.color = targetGlowColor;

            targetGlowColor.a = targetGlowAlpha;
            outerGlow.color = targetGlowColor;
            yield return null;
        }

        transitioningColour = null;
    }


    private Coroutine hitColourChanging = null;

    private IEnumerator HitColourChange(float time)
    {
        float t = 0f;
        //Color initialColour = outerGlow.color;


        while (t < 1f)
        {
            t += Time.deltaTime / time;
            targetGlowColor = Color.Lerp(Color.red, GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color, t);
            horizonGlow.color = targetGlowColor;
            shimmerMain.startColor = targetGlowColor;
            coronaGlow.color = targetGlowColor;

            targetGlowColor.a = targetGlowAlpha;
            outerGlow.color = targetGlowColor;
            yield return null;
        }

        hitColourChanging = null;
    }




    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.START_SCREEN) { ResetQirn(); }
        else if(args.State == GameStates.PRE_PLAY) {
            _revivesRemaining = _revivesAllowed;
        }
        else if(args.State == GameStates.POST_PLAY)
        {
            SetShieldActive(false);
        }
    }





    public void ResetQirn()
    {
        AddScore(-_score);
    }

    


    /// <summary>
    /// Trigger enter event.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case "PowerUp":
                PowerUpQirn puq = collision.gameObject.GetComponent<PowerUpQirn>();
                puq.Pool();
                break;

            case "FractalQirn":

                FractalQirn fractal = collision.transform.parent.gameObject.GetComponent<FractalQirn>();
                if (fractal)
                {
                    OnFractalQirnCollided();
                    AddScore(-(int)(_score * pointDecayFactor));

                    audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    audioSource.Play();

                    if (GameManager.GameMode == GameModes.LIMITED || GameManager.GameMode == GameModes.WARP)
                    {
                        if(_revivesRemaining > 0)
                        {
                            if (canLoseRevives)
                            {
                                _revivesRemaining -= 1;
                                canLoseRevives = false;
                                StartCoroutine(ReviveTimer());
                            }
                            
                            PopUpManager.Instance.PopUp("PU_Revive");
                        }
                        else
                        {
                            GameManager.Instance.ChangeGameState(GameStates.POST_PLAY);
                        }
                        
                    }

                    fractal.Pool();
                }

                break;
        }
    }

    private WaitForSeconds wfs_reviveTime = new WaitForSeconds(1);
    private IEnumerator ReviveTimer()
    {
        yield return wfs_reviveTime;
        canLoseRevives = true;
    }




    /// <summary>
    /// Sets the score to the provided int.
    /// </summary>
    /// <param name="score">The number to set the score to. Will default to 0 if less than 0.</param>
    public void SetScore(int score)
    {
        if(score < 0) { score = 0; }
        _score = score;
    }




    /// <summary>
    /// Adds int points to the current score. Can be negative. _score will not drop below 0.
    /// </summary>
    /// <param name="points">The number of points to add to _score.</param>
    public void AddScore(int points)
    {
        _score += points;        

        if(_score < 0) { _score = 0; }

        scoreChangedArgs.CurrentScore = _score;
        scoreChangedArgs.ScoreChangedBy = points;
        scoreChangedArgs.CurrentGameMode = GameManager.GameMode;

        OnScoreChanged(scoreChangedArgs);
        
    }




    /// <summary>
    /// Score changed event handler.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnScoreChanged(ScoreChangedEventArgs e)
    {
        ScoreChangedEventHandler handler = ScoreChanged;

        if (handler != null)
        {
            handler(this, e);
        }
    }




    /// <summary>
    /// Triggered when center qirn is revived after watching an ad.
    /// </summary>
    /// <param name="e"></param>
    private void OnRevived(EventArgs e)
    {
        EventHandler handler = Revived;

        if (handler != null)
        {
            handler(this, e);
        }
    }

    private void OnFractalQirnCollided()
    {
        EventHandler handler = FractalQirnCollided;

        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}






public class ScoreChangedEventArgs : EventArgs
{
    public int CurrentScore { get; set; }
    public int ScoreChangedBy { get; set; }
    public GameModes CurrentGameMode { get; set; }

    public ScoreChangedEventArgs()
    {
        CurrentScore = 0;
        ScoreChangedBy = 0;
        CurrentGameMode = GameModes.INFINITE;
    }

    public ScoreChangedEventArgs(int currentScore, int scoreChangedBy, GameModes currentMode)
    {
        CurrentScore = currentScore;
        ScoreChangedBy = scoreChangedBy;
        CurrentGameMode = currentMode;
    }
}
