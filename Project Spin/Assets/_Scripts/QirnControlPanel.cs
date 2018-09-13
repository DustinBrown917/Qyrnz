using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QirnControlPanel : MonoBehaviour {

    private float currentSpeedFactor = 0;

	void Start () {
        
    }

    public void ChangeOrbitSpeedByInput()
    {
        if ((GameManager.CurrentState != GameStates.PLAYING && GameManager.CurrentState != GameStates.TUTORIAL) || GameManager.Instance.Paused) { return; }

        ChangeOrbitSpeed(true);
    }

    private void ChangeOrbitSpeed(bool useInputPos, float speedFactor = 0)
    {
        if(useInputPos)
        {
            speedFactor = ((Input.mousePosition.x / Screen.width) * 2) - 1;
        }

        currentSpeedFactor = speedFactor;

        OrbitQirn.Instance.ChangeSpeedFactor(currentSpeedFactor);
    }
}
