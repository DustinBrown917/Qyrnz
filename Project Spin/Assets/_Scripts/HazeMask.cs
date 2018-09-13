using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazeMask : MonoBehaviour {

	// Use this for initialization
	void Start () {

        float heightFactor =  (float)Screen.height / (float)Screen.width;
        float width = (Camera.main.orthographicSize * 2.0f * Camera.main.aspect) * 2f;
        transform.localScale = new Vector3(width, width * heightFactor, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
