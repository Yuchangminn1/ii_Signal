using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public enum MorseType
{
    Dot,
    Dash
}

public class MorseImage : MonoBehaviour
{
    protected Graphic _graphic_Half;

    protected Graphic _graphic_Full;

    protected float fadetime = 0.5f;

    protected MorseType _currentMorseType;

    bool isCheck = false;

    public bool IsCheck
    {
        get { return isCheck; }
        set { isCheck = value; }
    }

    public MorseType CurrentMorseType
    {
        get { return _currentMorseType; }
    }


    virtual protected void Start()
    {
        _graphic_Half = GetComponent<Graphic>();
        _graphic_Full = transform.GetChild(0).GetComponent<Graphic>();
    }

    virtual protected void OnEnable()
    {

    }

    virtual protected void OnDisable()
    {

        FadeManager.Instance.SetAlphaOne(_graphic_Half);

        FadeManager.Instance.SetAlphaZero(_graphic_Full);
    }

    virtual public void StartColoring()
    {
        ;

    }

}
