using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectWobble : MonoBehaviour {

    [SerializeField]
    private float wobbleSpeedMin = 1f;
    [SerializeField]
    private float wobbleSpeedMax = 4f;
    [SerializeField]
    private float xWobbleMax = 0f;
    [SerializeField]
    private float xWobbleMin = 0f;
    [SerializeField]
    private float yWobbleMax = 0f;
    [SerializeField]
    private float yWobbleMin = 0f;
    [SerializeField]
    private float zRotateMax = 0f;
    [SerializeField]
    private float zRotateMin = 0f;

    private Coroutine wobbling = null;

	// Use this for initialization
	void Start () {
        StartWobble();
        
	}

    private void StartWobble()
    {
        if(wobbling != null)
        {
            StopCoroutine(wobbling);
            wobbling = null;
        }
        wobbling = StartCoroutine(Wobble(UnityEngine.Random.Range(wobbleSpeedMin, wobbleSpeedMax)));
    }

    private float t = 0;

    private Vector3 targetScale = Vector3.forward;
    private Vector3 vel = Vector3.zero;

    private Vector3 targetRotation = Vector3.zero;
    private Vector3 vel2 = Vector3.zero;

    private IEnumerator Wobble(float time)
    {
        
        if(transform.eulerAngles.z > 360) { transform.Rotate(Vector3.forward, -360); }
        else if (transform.eulerAngles.z < -360) { transform.Rotate(Vector3.forward, 360); }

        t = 0;

        targetScale.x = UnityEngine.Random.Range(xWobbleMin, xWobbleMax);
        targetScale.y = UnityEngine.Random.Range(yWobbleMin, yWobbleMax);
        targetRotation.z += UnityEngine.Random.Range(zRotateMin, zRotateMax);

        while (t <= time)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref vel, time);
            transform.eulerAngles = Vector3.SmoothDamp(transform.rotation.eulerAngles, targetRotation, ref vel2, time);
            yield return null;
        }
        wobbling = null;
        StartWobble();
    }
}
