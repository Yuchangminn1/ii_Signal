using System.Collections;
using UnityEngine;

public class PartnerDataChecker : MonoBehaviour
{
    SequenceScript sequenceScript;

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
        StopCoroutine(CheckCoroutine());
    }

    public void ArrivePartnerDataCheck()
    {
        StartCoroutine(CheckCoroutine());
    }
    // 코루틴 0.5~ 1초 사이로 체크해서 시퀀스 스크립트 넘기는거 작성
    IEnumerator CheckCoroutine()
    {
        yield return CoroutineReturnManager.GetWaitForSeconds(2f);
        //상대방 답변 갯수 체크
        int answerCount = 0;
        while (UserDataManager.Instance.IsUser() && UserDataManager.Instance.GetPlayer().PartnerPassCode == "")
        {
            if (UserDataManager.Instance.IsUser() == false)
                yield break;
            yield return CoroutineReturnManager.GetWaitForSeconds(1f);
            Debug.Log($"상대방 답변 갯수 체크 : {answerCount} 목표 {QuestionManager.Instance.QuestionInfos.Count}");
            answerCount = UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count;
        }
        sequenceScript?.TriggerForceOn();
    }
}
