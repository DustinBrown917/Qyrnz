using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FractalQirn : MonoBehaviour {


    public delegate void AllFractalQirnzDestroyedEventHandler(object sender, AllFractalQirnzDestroyedEventArgs args);
    public static event AllFractalQirnzDestroyedEventHandler AllFractalQirnzDestroyed;

    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private float fadeTime = 0.5f;

    [SerializeField]
    private float _speed = 1;
    public float Speed { get { return _speed; } set { _speed = value; } }

    [SerializeField]
    private MinMax mm_rotationSpeed;
    [SerializeField]
    private float rotationSpeed = 0;

    private int _worth = 10;
    public int Worth { get { return _worth; } }

    [SerializeField]
    private MinMax mm_bodyCount;
    private int bodyCount;

    [SerializeField]
    private MinMax mm_bodyDistance;
    private Vector3 offset = new Vector3();

    private List<FractalBody> childBodies = new List<FractalBody>();
    private ParticleSystem ps;

    private WaitForSeconds wfs;

    private AudioSource audioSource;









    private static void OnAllFractalQirnzDestroyed(AllFractalQirnzDestroyedEventArgs e)
    {
        AllFractalQirnzDestroyedEventHandler handler = AllFractalQirnzDestroyed;

        if (handler != null)
        {
            handler(typeof(FractalQirn), e);
        }
    }

    private static AllFractalQirnzDestroyedEventArgs nukeArgs = new AllFractalQirnzDestroyedEventArgs();

    public static void DestroyAll(bool addPoints)
    {
        nukeArgs.AddPoints = addPoints;
        OnAllFractalQirnzDestroyed(nukeArgs);
    }








    private void OnEnable()
    {
        rotationSpeed = UnityEngine.Random.Range(mm_rotationSpeed.Min, mm_rotationSpeed.Max);
        SetActiveBodies(true);
    }

    private void OnDisable()
    {
        SetActiveBodies(false);
    }

    // Use this for initialization
    void Start () {
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        CenterQirn.Instance.Revived += Instance_Revived;
        AllFractalQirnzDestroyed += FractalQirn_AllFractalQirnzDestroyed;

        ps = GetComponent<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();

        bodyCount = UnityEngine.Random.Range((int)mm_bodyCount.Min, (int)mm_bodyCount.Max + 1);

        for(int i = 1; i <= bodyCount; i++)
        {
            GameObject body = Instantiate(GameManager.Instance.fractalBodies[UnityEngine.Random.Range(0, GameManager.Instance.fractalBodies.Length)]);
            childBodies.Add(body.GetComponent<FractalBody>());
            body.transform.SetParent(transform);
            body.transform.localPosition = Vector3.zero;
            

            if (i != 1)
            {
                offset.x = UnityEngine.Random.Range(mm_bodyDistance.Min, mm_bodyDistance.Max);
                offset.y = UnityEngine.Random.Range(mm_bodyDistance.Min, mm_bodyDistance.Max);
                body.transform.localPosition = offset;
            }
        }

        wfs = new WaitForSeconds(fadeTime);
	}

    private void FractalQirn_AllFractalQirnzDestroyed(object sender, AllFractalQirnzDestroyedEventArgs e)
    {
        if (e.AddPoints) { CenterQirn.Instance.AddScore(Worth); }
        Pool();
    }

    private void SetActiveBodies(bool active)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    public void PlaySound()
    {
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        audioSource.Play();
    }

    private void Instance_Revived(object sender, System.EventArgs e)
    {
        Pool();
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.POST_PLAY || args.State == GameStates.TUTORIAL)
        {
            Pool();
        }
        
    }

    // Update is called once per frame
    void Update () {
        if (!GameManager.Instance.Paused)
        {
            transform.position = Vector3.MoveTowards(transform.position, CenterQirn.Instance.transform.position, _speed * Time.deltaTime);
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
           
        }
        
	}

    private Coroutine fading = null;

    private IEnumerator FadeAndPool()
    {
        for(int i = 0; i < childBodies.Count; i++)
        {
            childBodies[i].FadeBody(fadeTime);
        }

        yield return wfs;
        ResetBodies();
        FractalLauncher.PoolFractalQirn(this);

        fading = null;
    }

    public void Pool()
    {
        if(fading == null) {
            ps.Emit(bodyCount);
            fading = StartCoroutine(FadeAndPool());
        }      
    }

    public void ResetBodies()
    {
        for(int i = 0; i < childBodies.Count; i++)
        {
            childBodies[i].ResetShape();
        }
    }


    public class AllFractalQirnzDestroyedEventArgs : EventArgs
    {
        public bool AddPoints { get; set; }
    }
}
