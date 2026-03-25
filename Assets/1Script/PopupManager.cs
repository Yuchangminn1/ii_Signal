using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public enum PopupType
{
    None,
    PleaseInput,
    ResetNotice,
}

public class PopupManager : Singleton<PopupManager>
{
    Text _pleaseInputText;
    Text _resetText;


    readonly float ResetPopupDelay = 10f;
    readonly float ResetPopupTime = 3f;

    public bool IsPopupOn = false;



    PopupType _currentPopupType = PopupType.None;


    public PopupType CurrentPopupType
    {
        get { return _currentPopupType; }
        set { _currentPopupType = value; }
    }

    public CanvasGroup _popupCanvasGroup;

    Coroutine _popupCoroutine = null;

    override protected void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        if (_popupCanvasGroup != null)
        {
            var q = _popupCanvasGroup.GetComponentsInChildren<Text>();
            _pleaseInputText = q[0];
            _resetText = q[1];
        }
    }


    public void SetPleaseInputText(PopupType popupType)
    {
        if (popupType == PopupType.PleaseInput)
        {
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.PopupSound);

            FadeManager.Instance.SetAlphaOne(_pleaseInputText);

            FadeManager.Instance.SetAlphaZero(_resetText);
            return;
        }
        else if (popupType == PopupType.ResetNotice)
        {
            SoundManager.Instance.PlayEffectSound(EffectSoundNum.PopupSound);

            FadeManager.Instance.SetAlphaOne(_resetText);

            FadeManager.Instance.SetAlphaZero(_pleaseInputText);
        }
        else if (popupType == PopupType.None)
        {
            FadeManager.Instance.SetAlphaZero(_pleaseInputText);
            FadeManager.Instance.SetAlphaZero(_resetText);
        }
    }

    public void PopUpOpen()
    {
        if (_popupCoroutine != null)
            StopCoroutine(_popupCoroutine);
        _popupCoroutine = StartCoroutine(PopUpCoroutine());
    }
    IEnumerator PopUpCoroutine()
    {
        IsPopupOn = true;

        Debug.Log("Popup Open");

        CurrentPopupType = PopupType.PleaseInput;
        SetPleaseInputText(CurrentPopupType);
        float startTime = Time.time;

        if (CurrentPopupType != PopupType.None)
        {
            FadeManager.Instance.SetAlphaOne(_popupCanvasGroup);
            while (Time.time - startTime < ResetPopupTime)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
            FadeManager.Instance.SetAlphaZero(_popupCanvasGroup);
        }
        yield return CoroutineReturnManager.WaitForFixedUpdate;

        startTime = Time.time;
        while (Time.time - startTime < ResetPopupDelay)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        CurrentPopupType = PopupType.ResetNotice;
        SetPleaseInputText(CurrentPopupType);
        Debug.Log("Popup Open2");

        if (CurrentPopupType != PopupType.None)
        {
            FadeManager.Instance.SetAlphaOne(_popupCanvasGroup);
            startTime = Time.time;
            while (Time.time - startTime < ResetPopupTime)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
            FadeManager.Instance.SetAlphaZero(_popupCanvasGroup);
        }
        yield return CoroutineReturnManager.WaitForFixedUpdate;


        startTime = Time.time;
        while (Time.time - startTime < 1f) //사라지고 여유시간 ? 임시로 
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        NetworkManager.Instance.SendData($"Reset");


        NetworkManager.Instance.ResetRequested = true;

        _popupCoroutine = null;
    }

    public void ClosePopup()
    {
        if (IsPopupOn)
        {
            if (_popupCoroutine != null)
                StopCoroutine(_popupCoroutine);
            CurrentPopupType = PopupType.None;
            FadeManager.Instance.SetAlphaZero(_popupCanvasGroup);
            IsPopupOn = false;

        }
    }

}
