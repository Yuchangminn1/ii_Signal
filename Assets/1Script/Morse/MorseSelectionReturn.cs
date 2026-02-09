using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorseSelectionReturn : MonoBehaviour
{
    public void ReturnSelectedData()
    {
        Queue<string> temp = PlayerDatas.Instance.GetPlayer().QuestionAnswerData;
        for (int i = 0; i < temp.Count; i++)
        {
            Debug.Log($"선택된 데이터{i} {temp.Dequeue()}");
        }

    }
}
