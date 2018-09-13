using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastHandler : MonoBehaviour {

    public static BlastHandler Instance { get; private set; }

    private Blast[] blasts;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        blasts = GetComponentsInChildren<Blast>();
    }

    public void DeployBlast(Vector3 location, BlastTypes type, Color col)
    {
        if(type == BlastTypes.NONE) { return; }
        blasts[(int)type].DeployBlast(location, col);
    }

    public enum BlastTypes
    {
        NONE = -1,
        RING
    }
}
