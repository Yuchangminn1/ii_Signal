using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetUIOn : MonoBehaviour
{


    GraphicMove graphicMoves;

    MorseInputTarget _resetMorseImageDash;

    Graphic _resetMorseImageDashBarGraphic;

    void Start()
    {
        graphicMoves = GetComponent<GraphicMove>();
        _resetMorseImageDash = transform.parent.GetComponentInChildren<MorseInputTarget>();
        _resetMorseImageDashBarGraphic = _resetMorseImageDash.transform.parent.GetComponent<Graphic>();
    }
    public void StartResetUIOn()
    {
        graphicMoves.MoveGraphic();

    }

    public void ResetBarUpdate(float filling)
    {
        if (filling > 1f)
        {
            filling = 1f;
        }
        if (filling < 0.1)
        {
            FadeManager.Instance.SetAlphaOne(_resetMorseImageDashBarGraphic);
        }


        _resetMorseImageDash.UpdateBar(filling);
    }

    public void Reset()
    {
        _resetMorseImageDash.Reset();
        graphicMoves.Reset();
    }
}
