using System.Collections;
using UnityEngine;

public class PageController : Singleton<PageController>
{
    [SerializeField] private int openingPage = 0;
    const float resetUserTimeout = 30f;

    PlayerPageController[] playerControllers;

    Coroutine pageResetCoroutine = null;
    Coroutine resetCheckCoroutine = null;


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


    IEnumerator ResetCheckCoroutine()
    {
        while (true)
        {
            if (NetworkManager.Instance.ResetRequested)
            {
                int count = 0;
                float resetStartTime = Time.time;
                UserDataManager.Instance.ResetUserData();

                while (UserDataManager.Instance.IsUser())
                {
                    yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);

                    if (Time.time - resetStartTime >= resetUserTimeout)
                    {
                        Debug.LogWarning($"ResetCheck timeout after {resetUserTimeout} seconds. Forcing room clear.");
                        UserDataManager.Instance.ClearRoom();
                        break;
                    }

                    if (count > 10)
                    {
                        UserDataManager.Instance.ClearRoom();
                        Debug.Log("ClearRoom");
                        count = 0;
                    }
                    else
                        count++;
                }
                if (NetworkManager.Instance.IsServer == false)
                {
                    Debug.Log("StopEndResetRequest");
                    NetworkManager.Instance.StopEndResetRequest();
                }

                yield return CoroutineReturnManager.GetWaitForSeconds(2f);

                RequestResetOpenPage(0);
                Debug.Log("ResetRequested");

                NetworkManager.Instance.ResetRequested = false;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
        }
    }

    public void RequestResetOpenPage(int pageNum)
    {
        if (pageNum == 0)
        {
            if (IsIdle())
            {
                return;
            }
            NetworkManager.Instance.IsTutorialRead = false;

        }




        if (pageResetCoroutine == null)
        {
            pageResetCoroutine = StartCoroutine(RequestResetOpenPageCoroutine(pageNum));
        }

    }

    IEnumerator RequestResetOpenPageCoroutine(int pageNum)
    {
        Debug.Log("RequestResetOpenPageCoroutine Start: " + pageNum);

        foreach (var playerController in playerControllers)
        {
            playerController.OpenShow(pageNum);
        }
        yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
        pageResetCoroutine = null;

    }
    void Start()
    {
        playerControllers = GetComponentsInChildren<PlayerPageController>();

        resetCheckCoroutine = StartCoroutine(ResetCheckCoroutine());
        GameManager.Instance?.AddProgramStart(StartPrograms());
    }

    void OnDisable()
    {
        if (resetCheckCoroutine != null)
        {
            StopCoroutine(resetCheckCoroutine);
            resetCheckCoroutine = null;
        }
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
