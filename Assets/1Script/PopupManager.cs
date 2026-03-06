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

    public void ResetPopUpOpen()
    {
        if (_popupCoroutine == null)
            _popupCoroutine = StartCoroutine(PopUpCoroutine());

    }
    public IEnumerator PopUpCoroutine()
    {
        CurrentPopupType = PopupType.PleaseInput;
        SetPleaseInputText(CurrentPopupType);
        float startTime = Time.time;

        if (CurrentPopupType != PopupType.None)
        {
            FadeManager.Instance.TargetFade(_popupCanvasGroup, 1f);
            while (Time.time - startTime < ResetPopupTime)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
            FadeManager.Instance.TargetFade(_popupCanvasGroup, 0f);
        }
        yield return CoroutineReturnManager.WaitForFixedUpdate;

        startTime = Time.time;
        while (Time.time - startTime < ResetPopupDelay)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        CurrentPopupType = PopupType.ResetNotice;
        SetPleaseInputText(CurrentPopupType);
        if (CurrentPopupType != PopupType.None)
        {
            FadeManager.Instance.TargetFade(_popupCanvasGroup, 1f);
            startTime = Time.time;
            while (Time.time - startTime < ResetPopupTime)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
            FadeManager.Instance.TargetFade(_popupCanvasGroup, 0f);
        }
        yield return CoroutineReturnManager.WaitForFixedUpdate;


        startTime = Time.time;
        while (Time.time - startTime < 1f) //사라지고 여유시간 ? 임시로 
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        NetworkManager.Instance.SendData($"Reset");

        if (NetworkManager.Instance.IsServer)
            NetworkManager.Instance.ResetRequested = true;



        _popupCoroutine = null;
    }

    public void ClosePopup()
    {
        if (_popupCoroutine != null)
            StopCoroutine(_popupCoroutine);
        _popupCoroutine = null;
        CurrentPopupType = PopupType.None;
        FadeManager.Instance.TargetFade(_popupCanvasGroup, 0f);

    }

}
