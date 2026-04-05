using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseAnswerContainer : MonoBehaviour
{
    MorseImage[] morseImages;

    RectTransform _rectTransform;

    Coroutine moveCoroutine = null;

    const int GAP_COUNT = 3;

    const float DEFAULT_X_GAP = 85f;
    const float DEFAULT_DASH_GAP = 121f;

    //Dot -> Dash  190
    // else 85
    bool[] _dotDashArray = new bool[3];

    Coroutine playSoundCoroutine = null;


    void Start()
    {
        morseImages = GetComponentsInChildren<MorseImage>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        //TODO 임시 포지션
        if (_rectTransform != null)
            _rectTransform.localPosition = Vector3.one * 1600f;
    }


    void OnDisable()
    {
        if (moveCoroutine != null)
        {
            Debug.LogWarning($"[{name}] OnDisable - 이동 중 비활성화됨");

            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        if (playSoundCoroutine != null)
        {
            StopCoroutine(playSoundCoroutine);
            playSoundCoroutine = null;
        }
    }


    public void SetMorse(string morseAnswer)
    {
        for (int i = 0; i < morseAnswer.Length; i++)
        {
            if (morseAnswer[i] == '0')
            {
                morseImages[i].SetMorseType(MorseType.Dot);
            }
            else if (morseAnswer[i] == '1')
            {
                morseImages[i].SetMorseType(MorseType.Dash);
            }
        }
        _dotDashArray[0] = false;
        _dotDashArray[1] = false;
        _dotDashArray[2] = false;
        Debug.Log($"{name}MorseAnswerContainer SetMorse : " + morseAnswer);
        if (morseImages.Length != morseAnswer.Length)
        {
            Debug.LogError("morseImages / morseAnswer 길이 불일치");
            return;
        }

        int count = 0;
        for (int i = 0; i < morseAnswer.Length; i++)
        {
            if (morseAnswer[i] == '1')
            {
                if (i - 1 >= 0)
                    _dotDashArray[i - 1] = true;
                if (i < _dotDashArray.Length)
                    _dotDashArray[i] = true;

            }
        }

        foreach (bool dotDash in _dotDashArray)
        {
            if (dotDash)
            {
                count++;
            }
        }

        float totalX = (GAP_COUNT - count) * DEFAULT_X_GAP + count * DEFAULT_DASH_GAP;

        Vector3 startPos = Vector3.right * totalX / -2f;
        morseImages[0].SetLocalPosition(startPos);
        morseImages[morseImages.Length - 1].SetLocalPosition(startPos * -1f);

        for (int i = 1; i < morseImages.Length - 1; i++)
        {
            float gapX = DEFAULT_X_GAP;
            if (_dotDashArray[i - 1])
            {
                gapX = DEFAULT_DASH_GAP;
            }
            startPos += Vector3.right * gapX;
            morseImages[i].SetLocalPosition(startPos);
            //Debug.Log($"morseImages[{i}] pos : " + startPos);
        }


    }
    public bool IsMove()
    {
        return moveCoroutine != null;
    }
    public void MoveStart(Vector2 startPos, Vector2 endPos, float moveSpeed)
    {
        transform.localPosition = startPos;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(MoveStartCoroutine(endPos, moveSpeed));
        if (playSoundCoroutine == null)
            playSoundCoroutine = StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        foreach (MorseImage morseImage in morseImages)
        {
            if (morseImage.CurrentMorseType == MorseType.Dot)
            {
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDotSound_1);
                yield return CoroutineReturnManager.GetWaitForSeconds(MorseTranslator.DefaultDotTime);
                SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDotSound_1);

                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);

            }
            else if (morseImage.CurrentMorseType == MorseType.Dash)
            {
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDashSound_1);
                yield return CoroutineReturnManager.GetWaitForSeconds(MorseTranslator.DefaultDashTime / 2);
                SoundManager.Instance.StopEffectSound(EffectSoundNum.MorseDashSound_1);
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);

            }
        }
        playSoundCoroutine = null;


    }
    IEnumerator MoveStartCoroutine(Vector2 endPos, float moveSpeed)
    {
        Debug.Log($"[{name}] 이동시작 x:{_rectTransform.localPosition.x:F0}");
        while (_rectTransform.localPosition.x > endPos.x)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;
            _rectTransform.localPosition += Vector3.left * Time.deltaTime * moveSpeed;
        }
        _rectTransform.localPosition = endPos;
        Debug.Log($"[{name}] 이동완료");
        moveCoroutine = null;
    }








}
