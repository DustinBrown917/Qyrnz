using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitQirnShield : MonoBehaviour {

    private Transform ring;
    private Transform maskPivot;
    private Transform mask;
    private SpriteRenderer core;
    private Animator animator;
    private Vector3 maskRestPos;
    private bool started = false;

    [SerializeField]
    private float standardRotationSpeed = 1f;
    private float rotationSpeed = 0;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip shieldDeploySound;
    [SerializeField]
    private AudioClip shieldDestructionSound;

    private Coroutine disabling = null;
    public bool IsInDisablingProcess { get { return (disabling != null); } }
    private WaitForSeconds destructionWfs;

    private void OnEnable()
    {
        if (started)
        {
            if(disabling != null)
            {
                StopCoroutine(disabling);
                disabling = null;
            }
            animator.Play("Activate", -1, 0);
            audioSource.clip = shieldDeploySound;
            audioSource.Play();
            rotationSpeed = standardRotationSpeed;
        }
    }

    // Use this for initialization
    void Start () {
        CenterQirn.Instance.Shield = this;
        ring = transform.Find("Ring");
        maskPivot = transform.Find("MaskPivot");
        mask = maskPivot.Find("Mask");
        maskRestPos = mask.transform.position;
        core = transform.Find("Core").GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        destructionWfs = new WaitForSeconds(7);
        started = true;
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.Instance.Paused)
        {
            ring.Rotate(Vector3.forward, rotationSpeed);
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        switch (tag)
        {

            case "FractalQirn":

                FractalQirn fractal = collision.transform.parent.gameObject.GetComponent<FractalQirn>();
                if (fractal)
                {
                    fractal.Pool();
                    PivotMaskTowardTarget(collision.transform);
                    BeginDisable();
                }

                break;
        }
    }

    private void PivotMaskTowardTarget(Transform target)
    {
        float angle = Mathf.Atan2(target.position.y, target.position.x) * Mathf.Rad2Deg;
        maskPivot.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }

    private void BeginDisable()
    {
        audioSource.clip = shieldDestructionSound;
        audioSource.Play();
        rotationSpeed = 0;
        disabling = StartCoroutine(DisableAfterSound());
        animator.Play("Deactivate", -1, 0);
    }

    private void DisableThis()
    {
        
    }

    private IEnumerator DisableAfterSound()
    {
        yield return destructionWfs;

        gameObject.SetActive(false);

        disabling = null;
    }

    public void SetCoreVisibleInsideMask()
    {
        core.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        
    }

    public void SetCoreVisibleOutsideMask()
    {
        mask.position = maskRestPos;
        core.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    }

}
