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
    Texture _graphic_Dash;

    Texture _graphic_Dot;


    RawImage _graphic_Half;
    RawImage _graphic_Full;

    RectTransform _rectTransform;
    float maxWidth = 1f;

    public bool isColorWhite = false;

    public bool isNativeSize = false;


    Color32 _colorHalf = new Color32(90, 90, 90, 127);

    Color32 _colorFull = new Color32(90, 90, 90, 255);
    Color32 _colorWhite = new Color32(255, 255, 255, 255);


    protected MorseType _currentMorseType;

    float _currentFillAmount = 0;
    float fillSpeed = 10f;

    bool isFilling = false;

    bool isCheck = false;
    Arduino_MorseKey arduino_MorseKey;
    public bool IsCheck
    {
        get { return isCheck; }
        set { isCheck = value; }
    }

    public MorseType CurrentMorseType
    {
        get { return _currentMorseType; }
    }


    public void SetTextures(Texture dotTexture, Texture dashTexture)
    {
        _graphic_Dot = dotTexture;
        _graphic_Dash = dashTexture;
    }

    void Start()
    {
        _graphic_Half = GetComponent<RawImage>();
        foreach (RawImage child in GetComponentsInChildren<RawImage>())
        {
            if (child != _graphic_Half)
            {
                _graphic_Full = child;
                break;
            }
        }
        _rectTransform = _graphic_Full.GetComponent<RectTransform>();
        maxWidth = _rectTransform.rect.width;


        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();
        SetTextures(arduino_MorseKey.DotTexture, arduino_MorseKey.DashTexture);



    }

    public void SetMorseType(MorseType morseType)
    {

        _graphic_Full.uvRect = new Rect(0, 0, 1, 1);
        if (morseType == MorseType.Dot)
        {
            _graphic_Half.texture = _graphic_Dot;
            _graphic_Full.texture = _graphic_Dot;
            if (isNativeSize)
            {
                _graphic_Half.SetNativeSize();
                _graphic_Full.SetNativeSize();
                maxWidth = _rectTransform.rect.width;

            }

            _currentMorseType = MorseType.Dot;
        }
        else if (morseType == MorseType.Dash)
        {
            _graphic_Half.texture = _graphic_Dash;
            _graphic_Full.texture = _graphic_Dash;
            if (isNativeSize)
            {
                _graphic_Half.SetNativeSize();
                _graphic_Full.SetNativeSize();

                maxWidth = _rectTransform.rect.width;

            }
            _currentMorseType = MorseType.Dash;
        }
        _graphic_Half.color = _colorHalf;
        if (isColorWhite)
        {
            _graphic_Full.color = _colorWhite;
            Debug.Log($"White {_graphic_Full.name}");

        }
        else
        {
            _graphic_Full.color = _colorFull;
            Debug.Log($"Full {_graphic_Full.name}");
        }

        UpdateBar(0f);

    }

    void OnEnable()
    {



    }

    void OnDisable()
    {
        if (_rectTransform != null)
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
        if (isColorWhite)
        {
            _graphic_Full.color = _colorWhite;
            Debug.Log($"2White {_graphic_Full.name}");

        }
        else
        {
            _graphic_Full.color = _colorFull;
            Debug.Log($"2Full {_graphic_Full.name}");
        }



        arduino_MorseKey.IsAccuracyRateCheck = true;


        IsCheck = false;

        FillingBar();

    }
    public void UpdateBar(float fillAmount)
    {
        // if (fillAmount < 0.1f)
        // {
        //     if (isColorWhite)
        //     {
        //         _graphic_Full.color = _colorWhite;
        //         Debug.Log($"1White {_graphic_Full.name}");

        //     }
        //     else
        //     {
        //         _graphic_Full.color = _colorFull;
        //         Debug.Log($"1Full {_graphic_Full.name}");
        //     }
        // }

        _rectTransform.SetSizeWithCurrentAnchors(
        RectTransform.Axis.Horizontal,
        maxWidth * fillAmount
        );
        _graphic_Full.uvRect = new Rect(0, 0, fillAmount, 1);


        if (fillAmount >= 1f)
        {
            IsCheck = true;
            isFilling = false;
        }
    }

    public void Reset()
    {
        if (_rectTransform != null)
        {
            _currentFillAmount = 0;
            UpdateBar(_currentFillAmount);
        }
        //FadeManager.Instance.SetAlphaZero(_graphic_Full);
        IsCheck = false;
    }
}
