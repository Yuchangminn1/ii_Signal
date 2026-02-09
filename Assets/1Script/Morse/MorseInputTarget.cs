using UnityEngine;
using UnityEngine.UI;
public class MorseInputTarget : MonoBehaviour
{
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


        if (_rectTransform != null)
        {
            _currentFillAmount = 0;
            UpdateBar(_currentFillAmount);
        }
        FadeManager.Instance.SetAlphaZero(_rawImage);
        IsCheck = false;

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
