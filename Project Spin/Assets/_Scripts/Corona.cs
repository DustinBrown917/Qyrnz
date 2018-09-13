using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corona : MonoBehaviour {

    [SerializeField]
    private float maskRotationDegPerSec;
    [SerializeField]
    private float glowRotationDegPerSec;

    private Transform mask1;
    private Transform mask2;
    private Transform glow;


    private void Start()
    {
        mask1 = transform.GetChild(0);
        mask2 = transform.GetChild(1);
        glow = transform.GetChild(2);
    }

    private void Update()
    {

        mask1.Rotate(Vector3.forward, maskRotationDegPerSec * Time.deltaTime);
        mask2.Rotate(Vector3.forward, -maskRotationDegPerSec * Time.deltaTime);
        glow.Rotate(Vector3.forward, glowRotationDegPerSec * Time.deltaTime);

    }
}
