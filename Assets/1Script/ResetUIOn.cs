using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ResetUIOn : MonoBehaviour
{

    const float ResetBarDelay = 0.4f;

    GraphicMove _graphicMoves;



    MorseColoringImage _resetMorseImage;


    void Start()
    {
        _graphicMoves = GetComponent<GraphicMove>();
        _resetMorseImage = transform.parent.GetComponentInChildren<MorseColoringImage>();
    }

    public bool StartResetUIOn()
    {

        if (_graphicMoves.MoveGraphicBool())
        {
            StartCoroutine(DelayToStartCoroutine());
            return true;
        }
        return false;
    }




    IEnumerator DelayToStartCoroutine()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(ResetBarDelay);
        _resetMorseImage.SetColor(false);

    }

    public void ResetBarUpdate(float filling)
    {
        _resetMorseImage.UpdateBar(filling);
    }

    public void Reset()
    {
        _resetMorseImage.Reset();
        _resetMorseImage.SetColor(true);

        _graphicMoves.Reset();
    }
}
