using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (GameManager.Instance.IsStarted)
        {
            StartCoroutine(CheckCoroutine());
        }
    }
    void OnDisable()
    {
        StopCoroutine(CheckCoroutine());
    }
    // 코루틴 0.5~ 1초 사이로 체크해서 시퀀스 스크립트 넘기는거 작성
    IEnumerator CheckCoroutine()
    {
        //상대방 답변 갯수 체크
        int answerCount = 0;
        while (UserDataManager.Instance.GetPlayer().PartnerPassCode == "")
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(1f);
            Debug.Log($"상대방 답변 갯수 체크 : {answerCount} 목표 {QuestionManager.Instance.QuestionInfos.Count + 1}");
            answerCount = UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count;
        }
        sequenceScript?.TriggerFroceOn();
    }
}
