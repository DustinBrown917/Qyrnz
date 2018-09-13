using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Graphic))]
[RequireComponent(typeof(Outline))]
public class FadeLabel : MonoBehaviour {

    private Animator animator;
    protected Graphic graphic;
    protected Outline outline;

	// Use this for initialization
	protected virtual void Start () {
        animator = GetComponent<Animator>();
        graphic = GetComponent<Text>();
        outline = GetComponent<Outline>();

	}

    // Update is called once per frame
    void Update () {
		
	}

    public void TriggerFade()
    {
        animator.Play("Fade", -1, 0);
    }
}
