using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]

public class MainPage : MonoBehaviour
{

    public int nextPageNumber = -1;

    [Header("시작 이벤트")]
    public UnityEvent onStartPage;
    [Header("리셋 이벤트")]
    public UnityEvent onReset;
    [Header("종료 이벤트")]
    public UnityEvent onEndPage;
    public SubPage SubPage { get { return _subPage; } set { _subPage = value; } }
    public GameObject Container;

    public int PageNumber { get { return pageNumber; } set { pageNumber = value; } }
    public SequenceScript[] sequenceScripts;
    [SerializeField] private int pageNumber;

    Coroutine _mainCoroutine;

    Coroutine _currentPageResetCoroutine = null;

    CanvasGroup _canvasGroup;

    SubPage _subPage;


    WaitForSeconds _fadeDelay = null;

    WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();


    /// <summary>
    /// 현재 순서의 스퀀스 실행 인스펙터에서 시퀀스 상속받는 스크립트 add하여 사용
    /// </summary>
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
            else
            {
                if (nextPageNumber == -1)
                {
                    Debug.Log($"페이지 이동: {pageNumber} - > " + (pageNumber + 1));
                    PageController.Instance.ChangePage(pageNumber + 1);

                }

                else
                {
                    Debug.Log($"페이지 이동: {pageNumber} - > " + (nextPageNumber));
                    PageController.Instance.ChangePage(nextPageNumber);

                }
            }

        }
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }



    void Start()
    {
    }


    void OnEnable()
    {
        _subPage?.gameObject.SetActive(true);
    }
    public void Initialize()
    {
        sequenceScripts = GetComponentsInChildren<SequenceScript>();

        sequenceScripts = sequenceScripts.OrderBy(script => script.currentIndex).ToArray();

        for (int i = 0; i < sequenceScripts.Length; i++)
        {
            //같이 트리거할 페어 연결

            SubPage.sequenceScripts[i].AddTrigger(sequenceScripts[i]);
            //트리거 시 다음 시퀀스로 이동 콜백 연결
            sequenceScripts[i].AddNextSequenceCallback(NextSequence);
        }

        if (PageController.Instance.FadeDuration > 0)
            _fadeDelay = new WaitForSeconds(PageController.Instance.FadeDuration);
    }

    public void OpenPage()
    {

        ResetSequence();
        onStartPage?.Invoke();
        _subPage?.OpenPage();
        CanvasFadeIn();

    }
    public void ClosePage()
    {
        onEndPage?.Invoke();
        _subPage?.ClosePage();
        CanvasFadeOut();
    }

    public void PageDown()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        _canvasGroup.alpha = 0f;
        // if (isSubPageLinked)
        //     SubPage.gameObject.SetActive(false);

    }

    public CanvasGroup GetCanvasGroup()
    {
        return _canvasGroup;
    }



    public void StopPage()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void ResetSequence()
    {
        _subPage?.ResetSequence();
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

    public void CurrentPageReset()
    {
        if (_fadeDelay == null)
            return;
        if (_currentPageResetCoroutine != null)
        {
            StopCoroutine(_currentPageResetCoroutine);
            _currentPageResetCoroutine = null;
        }
        _currentPageResetCoroutine = StartCoroutine(CurrentResetCoroutine());
    }

    public IEnumerator CurrentResetCoroutine()
    {
        FadeManager.Instance.TargetFade(_canvasGroup, 0f, PageController.Instance.FadeDuration);
        yield return _fadeDelay;
        Container?.SetActive(false);

        yield return _waitForFixedUpdate;
        Container?.SetActive(true);



        ResetSequence();
        FadeManager.Instance.TargetFade(_canvasGroup, 1f, PageController.Instance.FadeDuration);
        yield return _fadeDelay;


    }

    public void NextSequence()
    {
        CurrentIndex++;
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
        SubPage.TriggerON(index);
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