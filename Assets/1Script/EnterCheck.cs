using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnterCheck : MonoBehaviour
{

    public SequenceScript NextPageTrigger;

    public SequenceScript TagFirst;


    Coroutine _checkCoroutine = null;


    PlayerPageController _pageController;

    bool isAllChecked = false;

    bool _isUsing = false;


    void OnEnable()
    {
        if (GameManager.Instance.IsStarted)
        {
            isAllChecked = false;
            _checkCoroutine = StartCoroutine(CheckCoroutine());
        }

    }
    void OnDisable()
    {
        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }

    }

    void Start()
    {
        _pageController = GetComponentInParent<PlayerPageController>();
    }

    IEnumerator CheckCoroutine()
    {
        UserDataManager.Instance.Reset();
        while (GameManager.Instance.IsStarted == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(1f);
        }

        while (isAllChecked == false)
        {



            yield return StartCoroutine(UserDataManager.Instance.RequestUserTagAll());

            if (UserDataManager.Instance.IsUsingRoom)
            {
                TagFirst.TriggerForceOn();
            }

            if (UserDataManager.Instance.GetPlayer() != null)
            {
                isAllChecked = true;

            }
            else
            {
                if (_pageController.CurrentPage != 0)
                {
                    _pageController.CurrentPage = 0;
                }
            }


            yield return CoroutineReturnManager.GetWaitForSeconds(1f);



        }
        NextPageTrigger?.TriggerForceOn();

        _checkCoroutine = null;
    }

    // IEnumerator CheckCoroutine()
    // {
    //     while (isAllChecked == false)
    //     {
    //         yield return CoroutineReturnManager.GetWaitForSeconds(1f);

    //         while (GameManager.Instance.IsStarted == false)
    //         {
    //             yield return CoroutineReturnManager.GetWaitForSeconds(1f);
    //         }

    //         yield return StartCoroutine(UserDataManager.Instance.RequestUserRoonIn());
    //         if (UserDataManager.Instance.GetCurrentPlayersNum() == 2)
    //         {
    //             isAllChecked = true;

    //         }
    //         else
    //         {
    //             if (_pageController.CurrentPage != 0)
    //             {
    //                 _pageController.CurrentPage = 0;
    //             }
    //         }

    //     }
    //     if (NextPageTriggers != null)
    //     {
    //         for (int i = 0; i < NextPageTriggers.Length; i++)
    //         {
    //             NextPageTriggers[i].TriggerOn();
    //         }
    //     }

    //     _checkCoroutine = null;
    // }
}
