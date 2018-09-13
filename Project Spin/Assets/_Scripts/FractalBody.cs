using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalBody : MonoBehaviour {

    [SerializeField]
    private MinMax rotationSpeedMinMax;

    [SerializeField]
    private float rotationSpeed = 0;

    private SpriteRenderer body;
    private SpriteRenderer glow;
    private SpriteRenderer outerGlow;

    private Color bodyStartCol;
    private Color glowStartCol;
    private Color outerGlowStartCol;

    private EdgeCollider2D edgeCollider;
    private Animator animator;

    private bool started = false;

    // Use this for initialization
    private void OnEnable()
    {
        rotationSpeed = UnityEngine.Random.Range(rotationSpeedMinMax.Min, rotationSpeedMinMax.Max);
        if (started)
        {
            edgeCollider.enabled = true;
            ResetShape();
        }
        
    }

    public void ResetShape()
    {
        body.color = bodyStartCol;
        glow.color = glowStartCol;
        outerGlow.color = outerGlowStartCol;
        outerGlow.transform.localScale = Vector3.one;
        glow.transform.localScale = Vector3.one;
    }

    void Start () {
        body = GetComponent<SpriteRenderer>();
        glow = transform.Find("Glow").GetComponent<SpriteRenderer>();
        outerGlow = transform.Find("Outer Glow").GetComponent<SpriteRenderer>();

        bodyStartCol = body.color;
        glowStartCol = glow.color;
        outerGlowStartCol = outerGlow.color;

        edgeCollider = GetComponent<EdgeCollider2D>();

        animator = GetComponent<Animator>();

        started = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.Instance.Paused)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        
	}

    public void FadeBody(float fadeTime)
    {
        if(!gameObject.activeSelf) { return; }
        StartCoroutine(FadeOut(fadeTime));
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        edgeCollider.enabled = false;

        animator.Play("Swell", -1, 0f);

        float t = 0;
        float factor;
        while(t < fadeTime)
        {
            factor = t / fadeTime;
            body.color = Color.Lerp(bodyStartCol, Color.clear, factor);
            glow.color = Color.Lerp(glowStartCol, Color.clear, factor);
            outerGlow.color = Color.Lerp(outerGlowStartCol, Color.clear, factor);
            yield return null;
            t += Time.deltaTime;
        }

        body.color = bodyStartCol;
    }
}
