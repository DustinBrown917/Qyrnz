using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FractalLauncher : MonoBehaviour {

    public GameObject[] fractalQirns;

    private Transform deployPoint;
    [SerializeField]
    private Transform _pooledFractalHolder;
    [SerializeField]
    private Transform _activeFractalHolder;
    
    private static Transform PooledFractalHolder;
    private static Transform ActiveFractalHolder;

    private static List<GameObject> pooledFractals = new List<GameObject>();
    public static int PoolSize { get { return pooledFractals.Count; } }

    private Animator animator;

    [SerializeField]
    private float easyLaunchHandicap = 0.5f;
    [SerializeField]
    private float easyLaunchDecrementFactor = 0.001f;

    [SerializeField]
    private MinMax animTimeFactorMinMax = new MinMax(0.9f, 0.97f);

    Array launchPatternValues = Enum.GetValues(typeof(LaunchPatterns));

    private void Awake()
    {
        PooledFractalHolder = _pooledFractalHolder;
        ActiveFractalHolder = _activeFractalHolder;
    }

    // Use this for initialization
    void Start () {
        deployPoint = transform.GetChild(0);
        animator = GetComponent<Animator>();
        GameManager.Instance.GamePaused += Instance_GamePaused;
        GameManager.Instance.GameUnpaused += Instance_GameUnpaused;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        transform.Rotate(Vector3.forward, 180);
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.PLAYING)
        {
            animator.Play("LaunchCycle", -1, 0.97f);
        }
        else if(args.State == GameStates.POST_PLAY || args.State == GameStates.TUTORIAL)
        {
            animator.Play("New State", -1, 0);
        }
    }

    private void Instance_GameUnpaused(object sender, EventArgs e)
    {
        animator.speed = 1;
    }

    private void Instance_GamePaused(object sender, EventArgs e)
    {
        animator.speed = 0;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void PoolFractalQirn(FractalQirn f)
    {
        f.transform.position = new Vector3(1 * PoolSize, -25f, f.transform.position.z);

        f.transform.parent = PooledFractalHolder;

        f.enabled = false;

        pooledFractals.Add(f.gameObject);

        
    }

    private static GameObject PullFractalQirn()
    {
        if(pooledFractals.Count >= 1)
        {
            FractalQirn f = pooledFractals[PoolSize - 1].GetComponent<FractalQirn>();

            pooledFractals.RemoveAt(PoolSize - 1);

            if (f.enabled)
            {
                return PullFractalQirn();
            }
            else
            {
                f.GetComponent<FractalQirn>().enabled = true;
            }

            return f.gameObject;
        }
        else
        {
            return null;
        }
    }

    //****************Cached variables for LaunchFractalQirn****************
    float newAngle = 0f;
    float handicap = 0f;
    int launchCount = 5;
    int launchTotalCount = 5;
    LaunchPatterns pattern = LaunchPatterns.RANDOM;

    private int scoreCutoff = 100000;
    private float rotationIncrement = 15f;
    private float rotationFactor = 1;
    private int flipSwitch = 1;
    //****************Cached variables for LaunchFractalQirn****************


    public void LaunchFractalQirn()
    {
        //Determine launcher rotation
        switch (pattern)
        {
            case LaunchPatterns.RANDOM:
                newAngle = (UnityEngine.Random.value * 359f) - 180;
                break;
            case LaunchPatterns.FLIP:
                flipSwitch *= -1;
                newAngle = (((rotationIncrement * 0.7f) * (launchCount - launchTotalCount)) * flipSwitch);
                break;
            case LaunchPatterns.CURVE:
                newAngle = (rotationIncrement * rotationFactor);
                break;
        }

        //Apply rotation to the launcher
        transform.Rotate(0, 0, newAngle);
        if(transform.rotation.z >= 360) { transform.Rotate(0, 0, -360); }
        else if(transform.rotation.z < 0) { transform.Rotate(0, 0, 360); }

        //Let loose the qirns of war!
        ReleaseFractalQirn();

        //decrement launch count
        launchCount -= 1;

        //Determine and restrict handicap.
        //if(CenterQirn.Instance.Score * easyLaunchDecrementFactor >= easyLaunchHandicap) { handicap = 0f; }
        //else { handicap = Mathf.Clamp(easyLaunchHandicap - (CenterQirn.Instance.Score * easyLaunchDecrementFactor), 0, easyLaunchHandicap); }

        if(CenterQirn.Instance.Score <= scoreCutoff)
        {
            handicap = easyLaunchHandicap * Mathf.Pow((easyLaunchHandicap * 0.25f), CenterQirn.Instance.Score * easyLaunchDecrementFactor);
            if(handicap <= 0.0001) { scoreCutoff = CenterQirn.Instance.Score; }
        }
        else
        {
            handicap = 0;
        }

        //Set time factor.
        float tf = GetAnimTimeFactor(Mathf.Abs(newAngle)) - handicap;

        //If all qirns launched, set up new pattern.
        if (launchCount <= 0)
        {
            SetUpNewPattern();
        }
        //Debug.Log(pattern.ToString() + " :: {New Angle " + newAngle + "} :: {Angle Change " + angleChange + "} :: {TF " + tf + "}");
        animator.Play("LaunchCycle", -1, tf);
    }

    /// <summary>
    /// Gets the angle scaled time factor for the animation.
    /// </summary>
    /// <param name="changeInAngle">Difference in angle from the last launch.</param>
    /// <returns>Angle scaled time factor to pass to the animation.</returns>
    private float GetAnimTimeFactor(float changeInAngle)
    {
        return ((1f-(changeInAngle / 180f)) * animTimeFactorMinMax.Difference) + animTimeFactorMinMax.Min;
    }


    /// <summary>
    /// Place fractal qirn at the launch point.
    /// </summary>
    private void ReleaseFractalQirn()
    {
        //Get the fractal qirn
        GameObject fq = PullFractalQirn();
        if (fq == null) { fq = Instantiate(fractalQirns[0]); }
        
        //set fractal qirn position
        fq.transform.position = deployPoint.position;
        fq.transform.parent = ActiveFractalHolder;
    }

    /// <summary>
    /// Prepares a new pattern for launching.
    /// </summary>
    private void SetUpNewPattern()
    {
        pattern = (LaunchPatterns)launchPatternValues.GetValue(UnityEngine.Random.Range(0, launchPatternValues.Length));

        switch (pattern)
        {
            case LaunchPatterns.RANDOM:
                break;
            case LaunchPatterns.CURVE:
                //newAngle = UnityEngine.Random.value * 359f;
                rotationFactor = (UnityEngine.Random.value * 2) - 1;
                break;
            case LaunchPatterns.FLIP:
                //newAngle = UnityEngine.Random.value * 359f;
                break;
            default:
                break;
        }

        launchTotalCount = UnityEngine.Random.Range(0, CenterQirn.Instance.Score/100) + 1;
        launchCount = launchTotalCount;
    }


    public enum LaunchPatterns
    {
        RANDOM,
        CURVE,
        FLIP
    }

}
