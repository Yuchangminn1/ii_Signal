using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum InputSymbolGapDuration
{
    Dot_Dot = 125,
    Dot_Dash = 223,
    Dash_Dash = 257
}
public class MorseImageContainer_Control_RectTransform : MorseImageContainer
{
    RectTransform[] _inputImageRectTransforms;
    const int GAP_COUNT = 3;
    InputSymbolGapDuration[] _dotDashArray = new InputSymbolGapDuration[GAP_COUNT];

    string currentMorseInput = "    ";




    override public void Reset()
    {
        base.Reset();

        for (int i = 0; i < _dotDashArray.Length; i++)
        {
            _dotDashArray[i] = InputSymbolGapDuration.Dot_Dot;
        }
        currentMorseInput = "";
    }





    protected override void Start()
    {
        base.Start();
        _inputImageRectTransforms = new RectTransform[morseInputImages.Length];
        for (int i = 0; i < morseInputImages.Length; i++)
        {
            _inputImageRectTransforms[i] = morseInputImages[i].GetComponent<RectTransform>();
        }


    }
    override protected IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
    {
        isAnswer = false;

        while (morseInputImages[_currentIndex].IsFilled == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }
        currentMorseInput += morseType == MorseType.Dot ? '0' : '1';

        _currentIndex++;

        if (_currentIndex == 4)
        {
            arduino_MorseKey.IsColoringDone = true;
            Debug.Log("Coloring Done");
        }
        yield return CoroutineReturnManager.WaitForFixedUpdate;

        if (_currentIndex < morseInputImages.Length && _morseInput.Count > 0)
        {
            StartCoroutine(InputDequeue());
        }

        _morseIndexCheckCoroutine = null;

    }

    public override void CheckStart()
    {
        for (int i = 0; i < _dotDashArray.Length; i++)
        {
            _dotDashArray[i] = InputSymbolGapDuration.Dot_Dot;
        }
        currentMorseInput = "";
        base.CheckStart();

    }

    override public void ColoringMorseImage(MorseType morseType)
    {
        if (_currentIndex >= morseInputImages.Length)
        {
            return;
        }


        //TODO 급하게 막았는데 구조 좀 생각해서 수정 
        //TODO Guide모드에서 틀린 입력 들어왔을 때 인덱스 떄문에 구조 고민해야함

        if (_morseIndexCheckCoroutine == null)
        {
            if (_currentIndex != 0)
            {
                Debug.Log($"Index{_currentIndex} / currentMorseInput : " + currentMorseInput);
                if (currentMorseInput[_currentIndex - 1] == '0' && morseType == MorseType.Dash)
                {
                    _dotDashArray[_currentIndex - 1] = InputSymbolGapDuration.Dot_Dash;
                    if (_dotDashArray.Length > _currentIndex)
                        _dotDashArray[_currentIndex] = InputSymbolGapDuration.Dot_Dash;
                }
                else if (currentMorseInput[_currentIndex - 1] == '1' && morseType == MorseType.Dash)
                {
                    _dotDashArray[_currentIndex - 1] = InputSymbolGapDuration.Dash_Dash;
                    if (_dotDashArray.Length > _currentIndex)
                        _dotDashArray[_currentIndex] = InputSymbolGapDuration.Dot_Dash;

                }
                else if (currentMorseInput[_currentIndex - 1] == '0' && morseType == MorseType.Dot)
                {
                    _dotDashArray[_currentIndex - 1] = InputSymbolGapDuration.Dot_Dot;
                }



                float totalX = 0;

                for (int i = 0; i < _dotDashArray.Length; i++)
                {
                    Debug.Log($"_dotDashArray[{i}] : " + _dotDashArray[i]);
                    totalX += (float)_dotDashArray[i];
                }
                Vector3 pos = totalX / -2f * Vector3.right;

                _inputImageRectTransforms[0].localPosition = pos;




                for (int i = 0; i < _currentIndex; i++)
                {
                    pos += (float)_dotDashArray[i] * Vector3.right;
                    _inputImageRectTransforms[i + 1].localPosition = pos;
                }
            }
            else
            {
                if (morseType == MorseType.Dash)
                {
                    _dotDashArray[_currentIndex] = InputSymbolGapDuration.Dot_Dash;
                    _inputImageRectTransforms[0].localPosition = ((float)InputSymbolGapDuration.Dot_Dot * 2f + (float)InputSymbolGapDuration.Dot_Dash) / -2f * Vector3.right;
                }
                else if (morseType == MorseType.Dot)
                {
                    _dotDashArray[_currentIndex] = InputSymbolGapDuration.Dot_Dot;
                    _inputImageRectTransforms[0].localPosition = (float)InputSymbolGapDuration.Dot_Dot * 3f / -2f * Vector3.right;
                }
            }



            morseInputImages[_currentIndex].StartColoring(morseType);
            _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));

        }
        else
        {
            //Debug.Log($"코루틴 돌리는중 추가입력 {morseType} 큐에 추가");
            _morseInput.Enqueue(morseType);
        }

    }

}
