using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultSelectText : MonoBehaviour
{

    public int QuestionNum;

    public Text[] texts;


    void Start()
    {
        QuestionManager.Instance.AddOnQuestionSetup(SetText);
    }

    public void SetText()
    {
        string[] selections = QuestionManager.Instance.GetQuestionInfo(QuestionNum).Selection;




        for (int i = 0; i < texts.Length && i < selections.Length; i++)
        {

            texts[i].text = selections[i].Replace("\n", "");
        }
    }
}
