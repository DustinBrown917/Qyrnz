using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFractalHolder : MonoBehaviour {

    private float vel = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.GameMode == GameModes.WARP)
        {
            Spin();
        }
    }

    private void Spin()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, -OrbitQirn.Instance.transform.rotation.eulerAngles.z, ref vel, 0.8f));
    }
}
