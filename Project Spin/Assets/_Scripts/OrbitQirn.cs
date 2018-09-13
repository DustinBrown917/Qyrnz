using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQirn : MonoBehaviour {

    private static OrbitQirn _instance;
    public static OrbitQirn Instance { get { return _instance; } }

    private TrailRenderer trailRenderer;
    private SpriteRenderer outerGlow;
    private SpeedTrail speedTrail;

    [SerializeField]
    private AudioClip[] clips;
    private AudioSource audioSource;
    private AudioSource audioSource2;
    private AudioSource audioSource3;
    public Gradient[] speedTrailGrads;

    [SerializeField]
    private MinMax _speedMinMax = new MinMax(-360, 360);
    //private float speedActualMax;
    public float MaxSpeed { get { return _speedMinMax.Max; } set { _speedMinMax.Max = value; } }
    public float MinSpeed { get { return _speedMinMax.Min; } set { _speedMinMax.Min = value; } }

    [SerializeField]
    private float _currentSpeed = 0;
    public float CurrentSpeed { get { return _currentSpeed; } }
    [SerializeField]
    private float _targetSpeed = 0;

    private bool adjusting = false;

    private Vector3 _orbitTarget;
    public Vector3 OrbitTarget { get { return _orbitTarget; } set { _orbitTarget = value; } }

    [SerializeField]
    private MinMax _orbitDistanceMinMax = new MinMax(1, 3);
    public float OrbitDistanceMin { get { return _orbitDistanceMinMax.Min; } set { _orbitDistanceMinMax.Min = value; } }
    public float OrbitDistanceMax { get { return _orbitDistanceMinMax.Max; } set { _orbitDistanceMinMax.Max = value; } }


    private float _currentOribitDistance = 1;
    public float CurrentOrbitDistance { get { return _currentOribitDistance; } }

    private bool deployed = false;

    private Coroutine adjustSpeed;

    private void Awake()
    {
        _instance = this;
    }

    void Start () {

        outerGlow = transform.Find("Outer Glow").GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource2 = transform.Find("Glow").GetComponent<AudioSource>();
        audioSource3 = transform.Find("Outer Glow").GetComponent<AudioSource>();
        speedTrail = transform.Find("SpeedTrail").GetComponent<SpeedTrail>();
        _orbitTarget = CenterQirn.Instance.transform.position;
        //speedActualMax = _speedMinMax.Max;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        GameManager.Instance.GameModeChanged += Instance_GameModeChanged;
	}

    private void Instance_GameModeChanged(object sender, System.EventArgs e)
    {
        trailRenderer.colorGradient = GameManager.Instance.GetGameModeGradient(GameManager.GameMode);
        outerGlow.color = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[0].color;
        
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.PRE_PLAY) {
            StartCoroutine(Deploy());
        }
        else if(args.State == GameStates.POST_PLAY) {
            StartCoroutine(Recede());
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.Paused && (GameManager.CurrentState == GameStates.PLAYING || GameManager.CurrentState == GameStates.TUTORIAL))
        {
            transform.RotateAround(_orbitTarget, Vector3.forward, _currentSpeed * Time.deltaTime);
        }
        
    }

    private void ResetQirn()
    {
        StopAllCoroutines();
        transform.position = OrbitTarget;
        transform.rotation = Quaternion.identity;
        _currentSpeed = 0;
        _targetSpeed = 0;
        adjusting = false;
    }

    public void ActivateSpeedTrail()
    {
        speedTrail.ActivateSpeedTrail();
    }

    public void DeactivateSpeedTrail()
    {
        speedTrail.DeactivateSpeedTrail();
    }

    /// <summary>
    /// Change speed based on a factor. 1 = max speed CW, -1 = max speed CCW, 0 = stationary.
    /// </summary>
    /// <param name="factor"></param>
    public void ChangeSpeedFactor(float factor)
    {
        float speed = -1 * factor * MaxSpeed;

        ChangeSpeed(speed);
    }

    /// <summary>
    /// Changes orbital speed of orbital qirn.
    /// </summary>
    /// <param name="speed">The speed to adjust the qirn to.</param>
    /// <param name="clamp">Whether or not to clamp the speed to the qirns MinMax speed limits.</param>
    public void ChangeSpeed(float speed, bool clamp = true)
    {
        
        if (!deployed) { return; }
        if (clamp) { speed = Mathf.Clamp(speed, _speedMinMax.Min, _speedMinMax.Max); }
        _targetSpeed = speed;
        if (!adjusting)
        {
            adjustSpeed = StartCoroutine(AdjustSpeed());
        }
    }

    public void ChangeOrbitDistanceFactor(float factor)
    {
        float distance = factor * (_orbitDistanceMinMax.Max - _orbitDistanceMinMax.Min);

        ChangeOrbitDistance(distance + _orbitDistanceMinMax.Min);
    }

    public void ChangeOrbitDistance(float distance, bool clamp = true)
    {
        if (clamp) { distance = Mathf.Clamp(distance, _orbitDistanceMinMax.Min, _orbitDistanceMinMax.Max); }
        
    }


    //private float speedFactor = 0;
    private float speedLastFrame = 0;
    private float lastQuackTime = 0;
    [SerializeField]
    private MinMax mm_QuackVol = new MinMax(0, 0.4f);
    [SerializeField]
    private float timeForMaxQuackVol = 1;
    /// <summary>
    /// Adjusts speed and height of Qirn smoothly.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AdjustSpeed()
    {
        float velF = 0;
        Vector3 velV3 = Vector3.zero;

        adjusting = true;
        while(_targetSpeed != _currentSpeed)
        {
            speedLastFrame = _currentSpeed;
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref velF, 0.25f); //Adjust current speed towards target
            //speedFactor = Mathf.Abs(CurrentSpeed) / speedActualMax;

            if((speedLastFrame >= 0 && _currentSpeed < 0) || (speedLastFrame <= 0 && _currentSpeed > 0))
            {
                float vol = (Mathf.Clamp01((Time.time - lastQuackTime) / timeForMaxQuackVol) * mm_QuackVol.Difference) + mm_QuackVol.Min;

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    audioSource.volume = vol;
                    audioSource.Play();
                } else if(!audioSource2.isPlaying)
                {
                    audioSource2.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    audioSource2.volume = vol;
                    audioSource2.Play();
                }
                else if(!audioSource3.isPlaying)
                {
                    audioSource3.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                    audioSource3.volume = vol;
                    audioSource3.Play();
                }
                        
                lastQuackTime = Time.time;
            }

            float height = 1- ((Mathf.Abs(_currentSpeed) / _speedMinMax.Max) * (_orbitDistanceMinMax.Difference)) + _orbitDistanceMinMax.Min;
            height = Mathf.Clamp(height, OrbitDistanceMin, OrbitDistanceMax); //Set target height based on speed

            Vector3 targetPos = transform.localPosition.normalized * height; //Adjust target position based on target height
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velV3, 0.5f); //Adjust height based on speed (but sexily)
            yield return null;
        }
        adjusting = false;
    }


    private IEnumerator Deploy()
    {
        Vector3 vel = Vector3.zero;

        while(transform.position.y < 0.999f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Vector3.up, ref vel, 0.5f);
            yield return null;
        }

        deployed = true;

        GameManager.Instance.ChangeGameState(GameStates.TUTORIAL);

        
    }

    private IEnumerator Recede()
    {
        
        if (adjusting) { StopCoroutine(adjustSpeed); }
        
        deployed = false;
        Vector3 vel = Vector3.zero;

        while (transform.position.magnitude > 0.001f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Vector3.zero, ref vel, 0.5f);
            yield return null;
        }

        ResetQirn();
        GameManager.Instance.ChangeGameState(GameStates.START_SCREEN);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        string tag = collision.gameObject.tag;

        switch (tag)
        {
            case "PowerUp":
                PowerUpQirn puq = collision.gameObject.GetComponent<PowerUpQirn>();
                puq.PowerUp();
                break;

            case "FractalQirn":

                FractalQirn fractal = collision.transform.parent.gameObject.GetComponent<FractalQirn>();

                if (fractal)
                {

                    if (CenterQirn.Instance)
                    {
                        CenterQirn.Instance.AddScore(fractal.Worth);
                    }
                    fractal.PlaySound();
                    fractal.Pool();
                }

                break;
        }
    }
}
