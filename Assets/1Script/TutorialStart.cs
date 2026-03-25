using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStart : MonoBehaviour
{
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

        while (NetworkManager.Instance.IsTutorialRead == false && gameObject.activeInHierarchy)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
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
