using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : Singleton<PageController>
{
    [SerializeField] private int openingPage = 0;

    PlayerPageController[] playerControllers;

    Coroutine pageResetCoroutine = null;

    WaitForSeconds _requestResetCoroutine = CoroutineReturnManager.GetWaitForSeconds(0.5f);

    WaitForSeconds _setupDelay = CoroutineReturnManager.GetWaitForSeconds(3f);
    override protected void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        //OpenPage - > CurrentPage프로퍼티 호출로 변경


        if (Input.inputString.Length > 0)
        {
            char inputChar = Input.inputString[0];

            if (char.IsDigit(inputChar))
            {
                foreach (var playerController in playerControllers)
                {
                    playerController.CurrentPage = inputChar - '0';
                }

            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach (var playerController in playerControllers)
            {
                playerController.DebugTrigger();
            }
        }

    }

    public void RequestResetOpenPage(int pageNum)
    {
        if (pageResetCoroutine == null)
        {
            pageResetCoroutine = StartCoroutine(RequestResetOpenPageCoroutine(pageNum));
        }

    }

    IEnumerator RequestResetOpenPageCoroutine(int pageNum)
    {

        foreach (var playerController in playerControllers)
        {
            playerController.OpenShow(pageNum);
        }
        yield return _requestResetCoroutine;
        pageResetCoroutine = null;

    }
    void Start()
    {
        playerControllers = GetComponentsInChildren<PlayerPageController>();
        GameManager.Instance?.AddProgramStart(StartPrograms());
    }


    public bool IsIdle()
    {
        bool isIdle = true;
        foreach (var playerController in playerControllers)
        {
            if (playerController.CurrentPage != 0)
            {
                isIdle = false;
                break;
            }
        }
        return isIdle;
    }

    public int CurrentPage
    {
        get
        {
            return playerControllers[0].CurrentPage;
        }
    }

    public IEnumerator StartPrograms()
    {
        yield return null;

        foreach (var playerController in playerControllers)
        {
            foreach (GameObject settingPage in playerController.SettingPages)
            {
                settingPage.SetActive(true);
            }
        }
        foreach (var playerController in playerControllers)
        {
            playerController.PageSetUp(openingPage);
        }


    }





}
