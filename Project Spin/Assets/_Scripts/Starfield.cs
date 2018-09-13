using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Starfield : MonoBehaviour {

    //private float vel = 0;
    private ParticleSystem ps;

    private Coroutine cr_rotating;

    private Coroutine cr_warpingMusic = null;

    //private AudioSource spaceDustMusic;
    [SerializeField]
    private AudioMixer mixer;

    private MinMax musicPitchWarp = new MinMax(0.95f, 1.05f);

    private float vel = 0f;

	// Use this for initialization
	void Start () {
        ps = GetComponentInChildren<ParticleSystem>();
        //spaceDustMusic = transform.Find("SpaceDust").GetComponent<AudioSource>();
        GameManager.Instance.GamePaused += Instance_GamePaused;
        GameManager.Instance.GameUnpaused += Instance_GameUnpaused;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;

	}

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        if(GameManager.GameMode == GameModes.WARP)
        {
            if(args.State == GameStates.PRE_PLAY)
            {
                if(cr_warpingMusic != null)
                {
                    StopCoroutine(cr_warpingMusic);
                }
                cr_warpingMusic = StartCoroutine(WarpSound());
            }
            else if(args.State == GameStates.START_SCREEN)
            {
                StopCoroutine(cr_warpingMusic);
                cr_warpingMusic = null;
                //spaceDustMusic.pitch = 1;
                mixer.SetFloat("sfxPitch", 1);
                
            }
        }
    }

    private void Instance_GameUnpaused(object sender, System.EventArgs e)
    {
        ps.Play();
    }

    private void Instance_GamePaused(object sender, System.EventArgs e)
    {
        ps.Pause();
    }

    private void Spin()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, OrbitQirn.Instance.transform.rotation.eulerAngles.z, ref vel, 0.5f));
    }

	// Update is called once per frame
	void Update () {
        if(GameManager.GameMode == GameModes.WARP)
        {
            Spin();
        }
        
     }

    private IEnumerator WarpSound()
    {
        float targetPitch;
        float startPitch;
        float t;
        while (true)
        {
            targetPitch = UnityEngine.Random.Range(musicPitchWarp.Min, musicPitchWarp.Max);
            //startPitch = spaceDustMusic.pitch;
            mixer.GetFloat("sfxPitch", out startPitch);
            t = 0.5f * UnityEngine.Random.value;
            while(t < 1)
            {
                mixer.SetFloat("sfxPitch", Mathf.Lerp(startPitch, targetPitch, t));
                //spaceDustMusic.pitch = Mathf.Lerp(startPitch, targetPitch, t);
                t += 0.01f;
                yield return null;
            }
        }
    }
}
