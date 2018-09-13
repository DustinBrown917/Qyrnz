using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    private Camera cam;
    private Coroutine transitioningColour = null;

    [SerializeField]
    private float colFactor = 0.3f;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        GameManager.Instance.GameModeChanged += Instance_GameModeChanged;
        CenterQirn.Instance.FractalQirnCollided += Instance_FractalQirnCollided;
	}

    private void Instance_FractalQirnCollided(object sender, System.EventArgs e)
    {

        if (hitColourChanging != null)
        {
            StopCoroutine(hitColourChanging);
            hitColourChanging = null;
        }

        hitColourChanging = StartCoroutine(HitColourChange(1f));

    }

    private void Instance_GameModeChanged(object sender, System.EventArgs e)
    {
        if (transitioningColour != null)
        {
            StopCoroutine(transitioningColour);
            transitioningColour = null;
        }

        StartCoroutine(TransitionColour(0.75f));
    }

    private IEnumerator TransitionColour(float time)
    {
        float t = 0f;
        Color initialColour = cam.backgroundColor;
        Color transitColour = new Color();

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transitColour = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color;
            transitColour *= colFactor;

            cam.backgroundColor = Color.Lerp(initialColour, transitColour, t);
            yield return null;
        }

        transitioningColour = null;
    }


    private Coroutine hitColourChanging = null;

    private IEnumerator HitColourChange(float time)
    {
        float t = 0f;
        Color initialColour = cam.backgroundColor;
        Color targetColour = GameManager.Instance.GetGameModeGradient(GameManager.GameMode).colorKeys[1].color * colFactor;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            cam.backgroundColor = Color.Lerp(Color.red, targetColour, t);
            yield return null;
        }

        hitColourChanging = null;
    }
}
