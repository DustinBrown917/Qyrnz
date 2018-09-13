using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour {

    private static FadeManager _instance;
    public static FadeManager Instance { get { return _instance; } }

    private List<List<FadePackage>> _fadeLayers = new List<List<FadePackage>>();

    private List<Coroutine> fadeActions = new List<Coroutine>();

    private void Awake()
    {
        _instance = this;
    }

    public void AddFadeItem(int fadeLayer, ref Color col, float minAlpha)
    {
        if(fadeLayer < 0) { fadeLayer = 0; }

        FadePackage fp = new FadePackage(ref col, minAlpha);

        if(_fadeLayers[fadeLayer] == null)
        {
            _fadeLayers[fadeLayer] = new List<FadePackage>();
        }

        _fadeLayers[fadeLayer].Add(fp);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FadeLayerToMin(int layer)
    {
        if(fadeActions[layer] != null)
        {
            StopCoroutine(fadeActions[layer]);

        }

        fadeActions[layer] = StartCoroutine(IFadeLayerToMin(layer, 1f));
    }

    private IEnumerator IFadeLayerToMin(int layer, float time)
    {
        
        float t = 0;

        while(t < time)
        {
            for(int i = 0; i < _fadeLayers[layer].Count; i++)
            {
                _fadeLayers[layer][i].FadeToMin(t);
            }

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeActions[layer] = null;
    }
}

public class FadePackage
{
    private Color _col;
    public Color Col{ get { return _col; } }

    private Color _colMax;
    public Color ColMax { get { return _colMax; } }
    private Color _colMin;
    public Color ColMin { get { return _colMin; } }


    public FadePackage(ref Color col)
    {
        _col = col;
        _colMax = new Color(col.r, col.g, col.b, col.a);
        _colMin = new Color(col.r, col.g, col.b, 0);
    }

    public FadePackage(ref Color col, float minAlpha)
    {
        _col = col;
        _colMax = new Color(col.r, col.g, col.b, col.a);
        _colMin = new Color(col.r, col.g, col.b, minAlpha);
    }

    public void SetColor(ref Color col)
    {
        _col = col;
    }

    public void SetColorMax(Color col)
    {
        _colMax = col;
    }

    public void SetColorMin(Color col)
    {
        _colMin = col;
    }

    public void FadeToMin(float t)
    {
        _col = Color.Lerp(_colMax, _colMin, t);
    }

    public void FadeToMax(float t)
    {
        _col = Color.Lerp(_colMin, _colMax, t);
    }
}
