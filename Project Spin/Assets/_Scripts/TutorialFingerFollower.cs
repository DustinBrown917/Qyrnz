using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFingerFollower : MonoBehaviour {

    [SerializeField]
    private Vector3 offset = new Vector3();

    private Vector3 pos = new Vector3();

    [SerializeField]
    private GameObject followTarget;

    private TrailRenderer tr;

    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        pos = Camera.main.ScreenToWorldPoint(followTarget.transform.position) + offset;

        transform.position = pos;

        tr.enabled = true;
    }

    void Update () {
        pos = Camera.main.ScreenToWorldPoint( followTarget.transform.position) + offset;

        transform.position = pos;
	}

    private void OnDisable()
    {
        tr.enabled = false;
    }
}
