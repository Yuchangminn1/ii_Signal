using UnityEngine;
using UnityEngine.UI;
public enum SoundOptions
{
    Sound_1 = 0,
    Sound_2 = 1,
}
public class MorseInputTarget : MonoBehaviour
{
    public SoundOptions CurrentSoundOption = SoundOptions.Sound_1;

    RectTransform _rectTransform;

    RawImage _rawImage;
    float maxWidth = 1f;

    float _currentFillAmount = 0;

    bool isFilling = false;
    bool isCheck = false;

    public float fillSpeed = 2f;

    public bool IsCheck
    {
        get { return isCheck; }
        set { isCheck = value; }
    }

    public MorseType CurrentMorseType;


    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }
    public void StartColoring()
    {
        FillingBar();
    }

    public void Reset()
    {
        if (_rectTransform != null)
        {
            _currentFillAmount = 0;
            UpdateBar(_currentFillAmount);
        }
        FadeManager.Instance.SetAlphaZero(_rawImage);
        IsCheck = false;
    }

    void OnEnable()
    {
        if (GameManager.Instance.IsStarted)
        {

            if (_rectTransform != null)
            {
                _currentFillAmount = 0;
                UpdateBar(_currentFillAmount);
            }
            FadeManager.Instance.SetAlphaZero(_rawImage);
            IsCheck = false;

            if (CurrentMorseType == MorseType.Dot)
            {
                fillSpeed = 1 / MorseTranslator.DefaultDotTime;
            }

            else if (CurrentMorseType == MorseType.Dash)
            {
                fillSpeed = 1 / MorseTranslator.DefaultDashTime;
            }
        }
    }

    void Start()
    {


        _rectTransform = _rawImage.rectTransform;

        maxWidth = _rectTransform.rect.width;
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
        if (CurrentSoundOption == SoundOptions.Sound_1)
        {
            if (CurrentMorseType == MorseType.Dot)
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDotSound_1);
            else if (CurrentMorseType == MorseType.Dash)
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDashSound_1);
        }
        else if (CurrentSoundOption == SoundOptions.Sound_2)
        {
            if (CurrentMorseType == MorseType.Dot)
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDotSound_2);
            else if (CurrentMorseType == MorseType.Dash)
                SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseDashSound_2);
        }
    }

    public void UpdateBar(float fillAmount)
    {

        if (fillAmount > 0 && _rawImage.color.a < 0.8f)
        {
            FadeManager.Instance.SetAlphaOne(_rawImage);
        }

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
