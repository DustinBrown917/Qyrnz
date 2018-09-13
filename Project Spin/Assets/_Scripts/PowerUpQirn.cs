using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpQirn : MonoBehaviour {

    [SerializeField]
    protected float _speed = 1;
    public float Speed { get { return _speed; } }

    [SerializeField]
    protected int _worth = 50;

    [SerializeField]
    protected MinMax mm_rotationSpeed;
    [SerializeField]
    protected BlastHandler.BlastTypes blastType;

    [SerializeField]
    protected AudioClip[] clips;

    protected float rotationSpeed;
    protected Animator animator;
    protected PolygonCollider2D pol;
    protected ParticleSystem ps;
    protected ParticleSystem.MainModule psMain;
    protected bool started = false;
    protected AudioSource audioSource;

    [SerializeField]
    protected PowerUpManager.PowerUps powerUp;

    private void OnEnable()
    {
        transform.Find("Outer Glow").GetComponent<SpriteRenderer>().color = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color;
        rotationSpeed = UnityEngine.Random.Range(mm_rotationSpeed.Min, mm_rotationSpeed.Max);
        if (started)
        {
            psMain.startColor = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color;
            animator.Play("Reset", -1, 0);
        }
    }

    // Use this for initialization
    protected void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        pol = GetComponent<PolygonCollider2D>();
        ps = transform.Find("Particles").GetComponent<ParticleSystem>();
        psMain = ps.main;
        psMain.startColor = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        CenterQirn.Instance.Revived += Instance_Revived;
        started = true;
    }

    private void Instance_Revived(object sender, System.EventArgs e)
    {
        Pool();
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if (args.State == GameStates.POST_PLAY || args.State == GameStates.TUTORIAL)
        {
            Pool();
        }
    }

    // Update is called once per frame
    protected void Update () {

        if (!GameManager.Instance.Paused)
        {
            transform.position = Vector3.MoveTowards(transform.position, CenterQirn.Instance.transform.position, _speed * Time.deltaTime);
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }

    public void Pool()
    {
        StartCoroutine(DestroyThenPool());
    }

    private IEnumerator DestroyThenPool()
    {
        animator.Play("Destroy", -1, 0f);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        PowerUpLauncher.Instance.Pool(this);
    }

    public virtual void PowerUp()
    {
        
        PowerUpManager.DeployPowerUp(powerUp);
        CenterQirn.Instance.AddScore(_worth);
        StartCoroutine(PowerUpThenPool());
    }

    private IEnumerator PowerUpThenPool()
    {
        
        Vector3 dir = CenterQirn.Instance.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ps.transform.rotation = Quaternion.AngleAxis((angle - transform.rotation.z) - 90, Vector3.forward);

        animator.Play("PowerUp", -1, 0f);
        ps.Play();
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        audioSource.Play();
        if(blastType != BlastHandler.BlastTypes.NONE) { BlastHandler.Instance.DeployBlast(transform.position, blastType, GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color); }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        PowerUpLauncher.Instance.Pool(this);
    }

    
}
