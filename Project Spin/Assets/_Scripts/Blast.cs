using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour {

    [SerializeField]
    private float blastDuration = 1f;

    [SerializeField]
    private Vector3 startScale = new Vector3(1,1,1);
    [SerializeField]
    private Vector3 endScale = new Vector3(2,2,1);

    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
	}
	
    public void DeployBlast(Vector3 location)
    {
        transform.position = location;
        StartCoroutine(ExecuteBlast());
    }

    public void DeployBlast(Vector3 location, Color col)
    {
        transform.position = location;
        spriteRenderer.color = col;

        StartCoroutine(ExecuteBlast());
    }

    private Color targetColor = Color.white;
    private Color startColor;
    private float t = 0;
    private float elapsedTime = 0;

    private IEnumerator ExecuteBlast()
    {
        audioSource.Play();
        startColor = spriteRenderer.color;
        targetColor = spriteRenderer.color;
        targetColor.a = 0;

        t = 0;
        elapsedTime = 0;

        transform.localScale = startScale;
        spriteRenderer.enabled = true;

        while(elapsedTime < blastDuration)
        {
            t = elapsedTime / blastDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
            elapsedTime += Time.deltaTime;           
        }

        spriteRenderer.enabled = false;
    }
}
