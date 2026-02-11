using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorseColoringImage : MorseImage
{

    float maxWidth = 1f;

    public bool isColorWhite = false;

    public RawImage _remainingImage;

    readonly Color32 _colorHalf = new Color32(90, 90, 90, 127);

    readonly Color32 _colorWhiteHalf = new Color32(255, 255, 255, 127);

    readonly Color32 _colorWhiteFull = new Color32(255, 255, 255, 255);
    RectTransform __remainingRectTransform;

    float _currentFillAmount = 0;
    float fillSpeed = 10f;

    bool isFilling = false;

    bool isCheck = false;
    public bool IsCheck
    {
        get { return isCheck; }
        set { isCheck = value; }
    }


    override protected void Start()
    {
        base.Start();


        foreach (RawImage child in GetComponentsInChildren<RawImage>())
        {
            if (child != _rawImage)
            {
                _remainingImage = child;
                break;
            }
        }
        __remainingRectTransform = _remainingImage.GetComponent<RectTransform>();

        maxWidth = __remainingRectTransform.rect.width;
    }

    override public void SetMorseType(MorseType morseType)
    {
        base.SetMorseType(morseType);

        _remainingImage.uvRect = new Rect(0, 0, 1, 1);

        if (morseType == MorseType.Dot)
        {
            _remainingImage.texture = _graphic_Dot;
            fillSpeed = 10f;
        }
        else if (morseType == MorseType.Dash)
        {
            _remainingImage.texture = _graphic_Dash;
            fillSpeed = 3f;

        }

        if (isNativeSize)
        {
            _remainingImage.SetNativeSize();
            maxWidth = __remainingRectTransform.rect.width;
        }
        if (isColorWhite)
        {
            _remainingImage.color = _colorWhiteFull;
            _rawImage.color = _colorWhiteHalf;
        }
        else
        {
            _remainingImage.color = _colorFull;
            _rawImage.color = _colorHalf;
        }
        UpdateBar(0f);
    }

    public void SetColor(bool isColorClear)
    {
        if (isColorClear)
        {
            _remainingImage.color = Color.clear;
            _rawImage.color = Color.clear;
            return;
        }
        if (isColorWhite)
        {
            _remainingImage.color = _colorWhiteFull;
            _rawImage.color = _colorWhiteHalf;
        }
        else
        {
            _remainingImage.color = _colorFull;
            _rawImage.color = _colorHalf;
        }
    }

    void OnDisable()
    {
        if (__remainingRectTransform != null)
        {
            _currentFillAmount = 0;
            UpdateBar(_currentFillAmount);
        }
        IsCheck = false;

    }
    void FixedUpdate()
    {
        if (isFilling)
        {
            _currentFillAmount += Time.fixedDeltaTime * fillSpeed;
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
    public void StartColoring()
    {

        arduino_MorseKey.IsAccuracyRateCheck = true;

        IsCheck = false;

        FillingBar();

    }
    public void UpdateBar(float fillAmount)
    {

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
        if (__remainingRectTransform == null)
        {
            return;
        }

        _currentFillAmount = 0;

        UpdateBar(_currentFillAmount);
        IsCheck = false;
    }

}
