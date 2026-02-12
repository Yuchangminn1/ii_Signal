using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultStampContainer : MonoBehaviour
{
    AnswerStamp[] answerStamps = new AnswerStamp[5];

    public Texture emptyStampTexture;
    public Texture correctStampTexture;

    Coroutine showStampCoroutine = null;
    void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        showStampCoroutine = null;

    }
    void Start()
    {
        answerStamps = GetComponentsInChildren<AnswerStamp>();

        foreach (var stamp in answerStamps)
        {
            stamp.SetTextures(emptyStampTexture, correctStampTexture);
        }
    }

    public void ShowStamp()
    {
        if (showStampCoroutine == null)
            showStampCoroutine = StartCoroutine(ShowStampCoroutine());
    }

    public IEnumerator ShowStampCoroutine()
    {
        int stampCount = PlayerData.Instance.GetPlayer().StampCount;
        Debug.Log($"스탬프 개수: {stampCount}");
        for (int i = 0; i < stampCount; i++)
        {
            answerStamps[i].SetCorrectStamp();
            yield return CoroutineReturnManager.GetWaitForSeconds(0.8f);
        }
        showStampCoroutine = null;
    }
}
