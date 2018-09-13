using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpLauncher : MonoBehaviour {

    private static PowerUpLauncher _instance;
    public static PowerUpLauncher Instance { get { return _instance; } }

    [SerializeField]
    private List<PowerUpQirn> powerUps = new List<PowerUpQirn>();

    [SerializeField]
    private float launchWaitTime = 30f;

    private WaitForSeconds wfs;
    private Coroutine launchSequence;

    private Transform deployPoint;


    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        deployPoint = transform.GetChild(0);
        wfs = new WaitForSeconds(launchWaitTime);
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
	}

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.PLAYING)
        {
            StartLaunchSequence();
        }
        else if(args.PreviousState == GameStates.PLAYING)
        {
            StopLaunchSequence();
        }
    }

    private void RandomRotate()
    {
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.value * 359f);
    }

    private void Launch()
    {
        if(powerUps.Count <= 0) { return; }

        int launchIndex = UnityEngine.Random.Range(0, powerUps.Count);

        powerUps[launchIndex].GetComponent<PowerUpQirn>().enabled = true;
        powerUps[launchIndex].gameObject.transform.position = deployPoint.transform.position;

        powerUps.RemoveAt(launchIndex);
    }

    private IEnumerator LaunchSequence()
    {
        yield return wfs;
        RandomRotate();
        Launch();
        launchSequence = StartCoroutine(LaunchSequence());
    }

    private void StartLaunchSequence()
    {
        if(launchSequence != null)
        {
            StopCoroutine(launchSequence);
        }

        launchSequence = StartCoroutine(LaunchSequence());
    }

    private void StopLaunchSequence()
    {
        if (launchSequence != null)
        {
            StopCoroutine(launchSequence);
        }

        launchSequence = null;
    }

    public void Pool(PowerUpQirn p)
    {
        p.transform.position = new Vector3(1 * powerUps.Count, 25f, p.transform.position.z);

        p.gameObject.GetComponent<PowerUpQirn>().enabled = false;

        powerUps.Add(p);
    }

}
