using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseAnswerShow : MonoBehaviour
{
    MorseAnswerContainer[] morseAnswerContainers;

    Vector2 moveStartPos = new Vector2(960f, 0f);

    Vector2 moveEndPos = new Vector2(-960f, 0f);

    public SequenceScript sequenceScript;


    Coroutine moveOrderCoroutine = null;



    void Start()
    {
        morseAnswerContainers = GetComponentsInChildren<MorseAnswerContainer>();
    }

    public void ProgramStart()
    {
        if (moveOrderCoroutine != null)
        {
            StopCoroutine(moveOrderCoroutine);
            moveOrderCoroutine = null;
        }
        moveOrderCoroutine = StartCoroutine(MoveOrderCoroutine());
    }


    public IEnumerator MoveOrderCoroutine()
    {
        Queue<string> temp = UserDataManager.Instance.GetPlayer().PartnerAnswerData;

        Debug.Log($"받은 모스 수 temp count : {temp.Count}");

        temp.Dequeue(); //첫번째는 테스트코드



        while (temp.Count > 0) //마지막은 암호코드라 무시 
        {
            NetworkManager.Instance.SendData($"M");
            foreach (MorseAnswerContainer morseAnswerContainer in morseAnswerContainers)
            {
                if (morseAnswerContainer.IsMove() == false)

                {
                    morseAnswerContainer.MoveStart(moveStartPos, moveEndPos, 400f);
                    yield return CoroutineReturnManager.WaitForFixedUpdate;
                    morseAnswerContainer.SetMorse(temp.Dequeue());

                    yield return CoroutineReturnManager.GetWaitForSeconds(1.0f);

                    break;
                }
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.2f);

            GameManager.Instance.GoToIdleCheck();
        }
        bool isEnd = false;
        while (isEnd == false) //마지막은 암호코드라 무시 
        {
            for (int i = 0; i < morseAnswerContainers.Length; i++)
            {
                if (morseAnswerContainers[i].IsMove())
                {
                    break;
                }
                if (i == morseAnswerContainers.Length - 1)
                {
                    isEnd = true;
                }
            }
            yield return CoroutineReturnManager.GetWaitForSeconds(0.4f);
            GameManager.Instance.GoToIdleCheck();
        }

        //TODO 엑션 추가 end 이벤트
        sequenceScript?.TriggerFroceOn();
        moveOrderCoroutine = null;
    }



}
