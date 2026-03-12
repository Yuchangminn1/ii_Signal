using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public enum MorseType
{
    Dot,
    Dash,
    Shadow
}

public class MorseImage : MonoBehaviour
{
    protected Texture _graphic_Dash;

    protected Texture _graphic_Dot;

    protected RawImage _rawImage;

    protected RectTransform _rectTransform;


    public bool isNativeSize = false;
    protected Arduino_MorseKey arduino_MorseKey;
    protected MorseType _currentMorseType;

    readonly protected Color32 _colorFull = new Color32(90, 90, 90, 255);



    public MorseType CurrentMorseType
    {
        get { return _currentMorseType; }
    }


    void OnEnable()
    {
        Reset();
    }


    public void SetTextures(Texture dotTexture, Texture dashTexture)
    {
        _graphic_Dot = dotTexture;
        _graphic_Dash = dashTexture;
    }

    virtual protected void Start()
    {
        _rawImage = GetComponent<RawImage>();
        _rectTransform = GetComponent<RectTransform>();


        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();
        SetTextures(arduino_MorseKey.DotTexture, arduino_MorseKey.DashTexture);

    }


    void OnDisable()
    {
        if (GameManager.Instance.IsStarted)
        {
            Reset();
        }

    }

    virtual public void SetMorseType(MorseType morseType)
    {

        if (morseType == MorseType.Dot)
        {
            _rawImage.texture = _graphic_Dot;

            _currentMorseType = MorseType.Dot;
        }
        else if (morseType == MorseType.Dash)
        {
            _rawImage.texture = _graphic_Dash;

            _currentMorseType = MorseType.Dash;
        }

        if (isNativeSize)
        {
            _rawImage.SetNativeSize();
        }
        _rawImage.color = _colorFull;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        _rectTransform.localPosition = pos;
    }


    virtual public void Reset()
    {
        ;

    }


}
