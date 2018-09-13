using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct MinMax
{
    [SerializeField]
    private float _min;
    public float Min { get { return _min; } set { _min = value; } }

    [SerializeField]
    private float _max;
    public float Max { get { return _max; } set { _max = value; } }

    public float Difference { get { return _max - _min; } }


    public MinMax(float min, float max)
    {
        _min = min;
        _max = max;
    }
}
