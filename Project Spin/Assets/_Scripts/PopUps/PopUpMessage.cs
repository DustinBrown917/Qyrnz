using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PopUpMessage : MonoBehaviour {

    private Text text;


	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}

    public void SetText(string t)
    {
        text.text = t;
    }


}
