using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnterCheck : MonoBehaviour
{

    public SequenceScript NextPageTrigger;

    Coroutine _checkCoroutine = null;

    WaitForSeconds _checkWait = new WaitForSeconds(1f);

    PlayerPageController _pageController;

    bool isAllChecked = false;


    void OnEnable()
    {
        isAllChecked = false;
        _checkCoroutine = StartCoroutine(CheckCoroutine());
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
        while (isAllChecked == false)
        {
            yield return _checkWait;
            //Debug.Log($"플레이어 데이터 수 {PlayerDatas.Instance.GetCurrentPlayersNum()}");
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

        }
        NextPageTrigger?.TriggerFroceOn();

        _checkCoroutine = null;
    }
}
