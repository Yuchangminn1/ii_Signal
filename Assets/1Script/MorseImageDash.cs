using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class MorseImageDash : MorseImage
{
    RectTransform _rectTransform;

    RawImage _rawImage;
    float maxWidth = 1f;

    float _currentFillAmount = 0;

    float _fillSpeed = 4f;

    bool isFilling = false;
    void Awake()
    {
        _currentMorseType = MorseType.Dash;
    }


    override public void StartColoring()
    {
        FillingBar();
    }

    override protected void OnEnable()
    {
        base.OnEnable();

        if (_rectTransform != null)
        {
            _currentFillAmount = 0;
            UpdateBar(_currentFillAmount);
        }
    }

    override protected void Start()
    {
        base.Start();

        _rawImage = _graphic_Full.GetComponent<RawImage>();

        _rectTransform = _graphic_Full.rectTransform;

        maxWidth = _rectTransform.rect.width;
    }

    void FixedUpdate()
    {
        if (isFilling)
        {
            _currentFillAmount += Time.fixedDeltaTime * _fillSpeed;
            if (_currentFillAmount >= 1f)
            {
                _currentFillAmount = 1f;

            }
            UpdateBar(_currentFillAmount);
        }
    }
    public void FillingBar()
    {
        isFilling = true;
    }

    public void UpdateBar(float fillAmount)
    {

        _rectTransform.SetSizeWithCurrentAnchors(
        RectTransform.Axis.Horizontal,
        maxWidth * fillAmount
        );
        _rawImage.uvRect = new Rect(0, 0, fillAmount, 1);


        if (fillAmount >= 1f)
        {
            IsCheck = true;
            isFilling = false;
        }
    }
}
