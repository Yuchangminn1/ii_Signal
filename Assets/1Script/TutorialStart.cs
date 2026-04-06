using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStart : MonoBehaviour
{
    const float tutorialSyncTimeout = 20f;
    const int maxTutorialSyncRetries = 3;

    SequenceScript sequenceScript;

    Coroutine startcheckCoroutine = null;

    void Start()
    {
        sequenceScript = GetComponent<SequenceScript>();
    }

    void OnEnable()
    {
        if (GameManager.Instance.IsStarted == false)
            return;
        startcheckCoroutine = null;

    }

    public void StartCheck()
    {
        if (startcheckCoroutine != null)
        {
            StopCoroutine(startcheckCoroutine);
            startcheckCoroutine = null;
        }

        startcheckCoroutine = StartCoroutine(StartTutorial());
    }

    public void StopCheck()
    {


        if (startcheckCoroutine != null)
        {
            StopCoroutine(startcheckCoroutine);
            startcheckCoroutine = null;
        }
    }



    IEnumerator StartTutorial()
    {
        NetworkManager.Instance.IsTutorialRead = false;

        NetworkManager.Instance.SendData("Go");

        int count = 0;
        int retryCount = 0;
        float tutorialSyncStartTime = Time.time;

        while (NetworkManager.Instance.IsTutorialRead == false && gameObject.activeInHierarchy)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);

            if (Time.time - tutorialSyncStartTime >= tutorialSyncTimeout)
            {
                retryCount++;
                Debug.LogWarning($"Tutorial sync timed out after {tutorialSyncTimeout} seconds. retry={retryCount}");

                if (retryCount > maxTutorialSyncRetries)
                {
                    Debug.LogWarning("Tutorial sync retries exceeded. Forcing local continue.");
                    break;
                }

                if (NetworkManager.Instance.IsConnected)
                {
                    NetworkManager.Instance.RequestStateSync();
                    NetworkManager.Instance.SendData("Go");
                }

                tutorialSyncStartTime = Time.time;
                continue;
            }

            if (NetworkManager.Instance.IsServer == false)
                NetworkManager.Instance.SendData("Go");

            count++;

            if (count > 10)
            {
                if (NetworkManager.Instance.IsServer == false)
                    Debug.Log("C Go");
                else
                {
                    Debug.Log("S Go");
                }
                count = 0;


            }
        }
        if (NetworkManager.Instance.IsServer)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(1f);
            NetworkManager.Instance.SendData("Go");
        }

        SoundManager.Instance.PlayEffectSound(EffectSoundNum.ConfirmSound);

        if (NetworkManager.Instance.IsServer)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
                NetworkManager.Instance.SendData("Go");
            }
        }
        sequenceScript?.TriggerForceOn();

        startcheckCoroutine = null;
    }

}
