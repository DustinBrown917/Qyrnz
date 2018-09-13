using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrail : MonoBehaviour {

    private AudioSource audioSource;

    private TrailRenderer speedTrail;
    private ParticleSystem speedParticles;
    private ParticleSystem.MainModule spMain;

    [SerializeField]
    private float fadeTime = 0.5f;
    [SerializeField]
    private float maxVolume = 0.2f;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        speedTrail = GetComponent<TrailRenderer>();
        speedParticles = GetComponent<ParticleSystem>();
        spMain = speedParticles.main;
        GameManager.Instance.GameStateChanged += Instance_GameStateChanged;
        GameManager.Instance.GamePaused += Instance_GamePaused;
        GameManager.Instance.GameUnpaused += Instance_GameUnpaused;
    }

    private void Instance_GameUnpaused(object sender, System.EventArgs e)
    {
        audioSource.mute = false;
    }

    private void Instance_GamePaused(object sender, System.EventArgs e)
    {
        audioSource.mute = true;
    }

    private void Instance_GameStateChanged(object sender, GameStateChangedArgs args)
    {
        spMain.startColor = new ParticleSystem.MinMaxGradient(OrbitQirn.Instance.speedTrailGrads[(int)GameManager.GameMode]);
        speedTrail.colorGradient = OrbitQirn.Instance.speedTrailGrads[(int)GameManager.GameMode];
    }

    private Coroutine fadingIn = null;

    public void ActivateSpeedTrail()
    {
        speedTrail.enabled = true;
        audioSource.Play();
        speedParticles.Play();

        StopFades();

        fadingIn = StartCoroutine(FadeInVolume());
    }

    private Coroutine fadingOut = null;

    public void DeactivateSpeedTrail()
    {
        speedTrail.enabled = false;
        
        speedParticles.Stop();

        StopFades();

        fadingOut = StartCoroutine(FadeOutVolume());
    }

    private void StopFades()
    {
        if (fadingIn != null)
        {
            StopCoroutine(fadingIn);
            fadingIn = null;
        }

        if(fadingOut != null)
        {
            StopCoroutine(fadingOut);
            fadingOut = null;
        }
    }

    private IEnumerator FadeOutVolume()
    {
        float t = 0;
        float volStart = audioSource.volume;
        float factor = 0;

        while(audioSource.volume > 0)
        {
            factor = t / fadeTime;
            audioSource.volume = volStart - (volStart * factor);
            t += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        fadingOut = null;
    }

    private IEnumerator FadeInVolume()
    {
        float t = 0;
        float factor = 0;

        while (audioSource.volume < maxVolume)
        {
            factor = t / fadeTime;
            audioSource.volume = maxVolume * factor;
            t += Time.deltaTime;
            yield return null;
        }

        fadingIn = null;
    }

}
