using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    private static PowerUpManager _instance;
    public static PowerUpManager Instance { get { return _instance; } }

    private static MinMax mm_originalSpeed = new MinMax();
    private static MinMax mm_speedUp = new MinMax(-300f, 300f);
    private static WaitForSeconds speedUpTime = new WaitForSeconds(10);
    private static Coroutine cr_speedUp = null;


    private float vel = 0;

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        mm_originalSpeed.Max = OrbitQirn.Instance.MaxSpeed;
        mm_originalSpeed.Min = OrbitQirn.Instance.MinSpeed;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(args.State == GameStates.POST_PLAY)
        {
            ForceSpeedUpStop();
        }
    }

    void Update()
    {
        if (GameManager.GameMode == GameModes.WARP)
        {
            Spin();
        }
    }

    private void Spin()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, OrbitQirn.Instance.transform.rotation.eulerAngles.z, ref vel, 0.8f));
    }


    public static void DeployPowerUp(PowerUps powerUp)
    {
        switch (powerUp)
        {
            case PowerUps.SPEED_UP:
                ForceSpeedUpStop();
                cr_speedUp = Instance.StartCoroutine(SpeedUp());
                break;

            case PowerUps.NUKE:
                FractalQirn.DestroyAll(true);
                break;

            case PowerUps.SHIELD:
                CenterQirn.Instance.SetShieldActive(true);
                break;

        } 
    }

    
    
    /// <summary>
    /// Sets OrbitQirn speed for a set period of time. Can be used to set slower speed.
    /// </summary>
    /// <returns></returns>
    private static IEnumerator SpeedUp()
    {
        SetOrbitQirnSpeed(mm_speedUp);

        OrbitQirn.Instance.ActivateSpeedTrail();

        yield return speedUpTime;

        SetOrbitQirnSpeed(mm_originalSpeed);

        OrbitQirn.Instance.DeactivateSpeedTrail();

        OrbitQirn.Instance.ChangeSpeed(OrbitQirn.Instance.CurrentSpeed);

        cr_speedUp = null;
    }

    /// <summary>
    /// Sets OrbitQirn speed MinMax
    /// </summary>
    /// <param name="mm">MinMax to set OrbitQirn speed bounds to.</param>
    private static void SetOrbitQirnSpeed(MinMax mm)
    {
        OrbitQirn.Instance.MinSpeed = mm.Min;
        OrbitQirn.Instance.MaxSpeed = mm.Max;
    }

    /// <summary>
    /// Forces the SpeedUp Coroutine to stop cleanly. Safely exits if CoRoutine is not running.
    /// </summary>
    private static void ForceSpeedUpStop()
    {
        if(cr_speedUp == null) { return; }

        Instance.StopCoroutine(cr_speedUp);
        cr_speedUp = null;
        OrbitQirn.Instance.DeactivateSpeedTrail();
        SetOrbitQirnSpeed(mm_originalSpeed);
    }













    public enum PowerUps
    {
        SPEED_UP,
        NUKE,
        SHIELD
    }
}
