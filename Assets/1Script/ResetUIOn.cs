using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ResetUIOn : MonoBehaviour
{


    GraphicMove graphicMoves;

    public MorseImage _resetMorseImage;


    void Start()
    {
        graphicMoves = GetComponent<GraphicMove>();
        _resetMorseImage = transform.parent.GetComponentInChildren<MorseImage>();
    }
    public void StartResetUIOn()
    {
        graphicMoves.MoveGraphic();

    }

    public void ResetBarUpdate(float filling)
    {
        _resetMorseImage.UpdateBar(filling);
    }

    public void Reset()
    {
        _resetMorseImage.Reset();
        graphicMoves.Reset();
    }
}
