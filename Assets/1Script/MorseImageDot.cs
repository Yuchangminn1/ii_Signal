using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseImageDot : MorseImage
{

    void Awake()
    {
        _currentMorseType = MorseType.Dot;
    }
    override public void StartColoring()
    {
        FadeManager.Instance.SetAlphaOne(_graphic_Full);

        FadeManager.Instance.SetAlphaZero(_graphic_Half);
        IsCheck = true;

    }
}
