using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MorseResetBar : MorseColoringImage
{
    bool isResetInput = false;


    override public void UpdateBar(float fillAmount)
    {
        if (fillAmount > 0.1)
        {
            SoundManager.Instance.PlayingLoopSound();
        }
        __remainingRectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                maxWidth * fillAmount
                );
        _remainingImage.uvRect = new Rect(0, 0, fillAmount, 1);


        if (fillAmount >= 1f)
        {
            IsCheck = true;
            isFilling = false;

        }
    }

    override public void Reset()
    {
        base.Reset();

        SoundManager.Instance.StopLoopSound();
    }
}