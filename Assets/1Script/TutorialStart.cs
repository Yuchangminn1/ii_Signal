using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStart : MonoBehaviour
{
    SequenceScript sequenceScript;

    void Start()
    {
        sequenceScript = GetComponent<SequenceScript>();
    }

    void OnEnable()
    {
        if (GameManager.Instance.IsStarted == false)
            return;

    }

    public void StartCheck()
    {
        StartCoroutine(StartTutorial());

    }


    IEnumerator StartTutorial()
    {
        NetworkManager.Instance.IsTutorialRead = false;
        if (NetworkManager.Instance.IsServer == false)
            NetworkManager.Instance.SendData("Go");

        while (NetworkManager.Instance.IsTutorialRead == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.25f);
            if (NetworkManager.Instance.IsServer == false)
                NetworkManager.Instance.SendData("Go");
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
        sequenceScript?.TriggerFroceOn();


    }

}
