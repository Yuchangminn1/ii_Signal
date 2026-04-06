using System.Collections;
using UnityEngine;

public class PartnerDataChecker : MonoBehaviour
{
    SequenceScript sequenceScript;

    Coroutine checkCoroutine = null;

    void Start()
    {
        sequenceScript = GetComponent<SequenceScript>();
    }

    void OnEnable()
    {
        // if (GameManager.Instance.IsStarted)
        // {
        //     StartCoroutine(CheckCoroutine());
        // }
    }
    void OnDisable()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }
    }

    public void ArrivePartnerDataCheck()
    {
        if (checkCoroutine == null)
        {
            checkCoroutine = StartCoroutine(CheckCoroutine());
        }
    }


    IEnumerator CheckCoroutine()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(2f);
        //상대방 답변 갯수 체크
        int answerCount = 0;
        while (UserDataManager.Instance.IsUser() && UserDataManager.Instance.GetPlayer().PartnerPassCode == "")
        {
            if (UserDataManager.Instance.IsUser() == false)
            {
                checkCoroutine = null;
                yield break;
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(1f);
            Debug.Log($"상대방 답변 갯수 체크 : {answerCount} 목표 {QuestionManager.Instance.QuestionInfos.Count}");
            answerCount = UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count;
        }
        sequenceScript?.TriggerForceOn();
        checkCoroutine = null;
    }
}
