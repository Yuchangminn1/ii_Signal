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


        if (GameManager.Instance.IsDebugMode)
        {
            Debug.LogWarning("Debug Mode: Using test Morse code sequence.");
            temp = new Queue<string>();
            temp.Enqueue("0001");
            temp.Enqueue("0010");
            temp.Enqueue("0100");
            temp.Enqueue("1000");
            temp.Enqueue("0011");
            temp.Enqueue("0101");
            temp.Enqueue("1001");
            temp.Enqueue("1011");
            temp.Enqueue("1111");
            temp.Enqueue("1001");
            temp.Enqueue("0101");
            temp.Enqueue("0101");
            temp.Enqueue("1001");
            temp.Enqueue("1001");
            temp.Enqueue("0101");
            temp.Enqueue("1001");
        }
        temp.Dequeue(); //첫번째는 테스트코드



        while (temp.Count > 0) //마지막은 암호코드라 무시 
        {
            NetworkManager.Instance.SendData($"M");
            foreach (MorseAnswerContainer morseAnswerContainer in morseAnswerContainers)
            {
                if (morseAnswerContainer.IsMove() == false)

                {
                    morseAnswerContainer.MoveStart(moveStartPos, moveEndPos, 300f);
                    yield return CoroutineReturnManager.WaitForFixedUpdate;
                    morseAnswerContainer.SetMorse(temp.Dequeue());

                    yield return CoroutineReturnManager.GetWaitForSeconds(1.5f);

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
