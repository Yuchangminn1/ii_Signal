using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(CanvasGroup))]
public class SubPage : MonoBehaviour
{
    CanvasGroup _canvasGroup;

    [Header("시작 이벤트")]
    public UnityEvent onStartPage;
    [Header("리셋 이벤트")]
    public UnityEvent onReset;
    [Header("종료 이벤트")]
    public UnityEvent onEndPage;

    Coroutine _mainCoroutine;

    public SequenceScript[] sequenceScripts;

    public int currentindex;
    public int CurrentIndex
    {
        get { return currentindex; }
        set
        {
            if (sequenceScripts.Length == 0) return;

            if (sequenceScripts.Length > value)
            {
                currentindex = value;

                RunSequence();
            }


        }
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

    }

    public void Initialize()
    {
        sequenceScripts = GetComponentsInChildren<SequenceScript>();
        sequenceScripts = sequenceScripts.OrderBy(script => script.currentIndex).ToArray();

        foreach (var seq in sequenceScripts)
        {
            seq.AddNextSequenceCallback(NextSequence);
        }
    }

    public void NextSequence()
    {
        CurrentIndex++;
    }
    public void OpenPage()
    {
        ResetSequence();
        onStartPage?.Invoke();
        CanvasFadeIn();

    }
    public void ClosePage()
    {
        onEndPage?.Invoke();
        CanvasFadeOut();

    }

    void OnDisable()
    {
        _canvasGroup.alpha = 0f;
        if (_mainCoroutine != null)
        {
            StopCoroutine(_mainCoroutine);
            _mainCoroutine = null;
        }
    }

    public void ResetSequence()
    {
        CurrentIndex = 0;
    }
    public void RunSequence()
    {
        if (_mainCoroutine != null)
        {
            StopCoroutine(_mainCoroutine);
            _mainCoroutine = null;
        }

        if (sequenceScripts.Length == 0) return;

        _mainCoroutine = StartCoroutine(sequenceScripts[currentindex].StartSequence());

    }


    /// <summary>
    /// 디버그용 함수 
    /// </summary>
    public void CurrentIndexTriggerON()
    {
        if (sequenceScripts == null || sequenceScripts.Length < 1) return;
        sequenceScripts[currentindex].TriggerOn();
    }


    public void TriggerON(int index)
    {
        if (index == currentindex)
        {
            sequenceScripts[currentindex].TriggerOn();
        }
        else
        {
            Debug.LogWarning("현재 각 플레이어 사이의 페이지 인덱스가 다릅니다.");
        }

    }
    public void PageDown()
    {
        gameObject.SetActive(false);
    }
    public CanvasGroup GetCanvasGroup()
    {
        return _canvasGroup;
    }

    public void CanvasFadeIn()
    {
        FadeManager.Instance.TargetFade(GetCanvasGroup(), 1f, PageController.Instance.FadeDuration);
    }

    public void CanvasFadeOut()
    {
        FadeManager.Instance.TargetFade(GetCanvasGroup(), 0f, PageController.Instance.FadeDuration);
    }
}
