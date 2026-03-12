using System.Collections.Generic;
using UnityEngine;
public class MorseTranslatorData
{

    public void SetData(string morseData, float pressTimes)
    {
        _morseData = morseData;
        _pressTimes = pressTimes;
    }

    string _morseData;
    public string MorseData
    {
        get { return _morseData; }
    }
    float _pressTimes = 0f;

    public float PressTimes
    {
        get { return _pressTimes; }
    }

}
public static class MorseTranslator
{



    public const float DefaultDotTime = 0.15f;

    public const float DefaultDashTime = 0.8f;
    public const float MaxDotTime = 0.49f;

    public const float MaxDashTime = 3.0f;

    public const float InputResetTime = 3.1f;

    public const float OverInputTime = 5f;

    public static MorseTranslatorData _MorseTranslatorData = new MorseTranslatorData();
    public static int CurrentDataIndex
    {
        get { return _currentDataIndex; }
    }
    static int _currentDataIndex = 0;

    public static string CurrentData
    {
        get { return _currentData; }
    }
    static string _currentData = "";

    public static float Accuracy(string morseData, float[] pressTimes)
    {

        Debug.Log("체크 모스 호출 : " + morseData);
        Debug.Log("체크 프레스 타임 : " + string.Join(", ", pressTimes));
        float[] outputPressTimes = new float[4];
        float accuracy = 100f;

        for (int i = 0; i < outputPressTimes.Length; i++)
        {
            float difference;
            if (morseData[i] == '0')
            {
                difference = Mathf.Abs(pressTimes[i] - DefaultDotTime);
            }
            else if (morseData[i] == '1')
            {
                difference = Mathf.Abs(pressTimes[i] - DefaultDashTime);
            }
            else
            {
                difference = 10f;
                Debug.LogWarning("Invalid Morse Data: " + morseData[i]);
            }
            Debug.Log($"Difference for index {i}: {difference}");

            if (difference < 0.5f)
            {
                ;
            }
            else if (difference < 1.0f)
            {
                accuracy -= 5f;
            }
            else
            {
                accuracy -= 10f;
            }
        }
        return accuracy;

    }


    public static string Translate(string morseData)
    {
        Debug.Log("MorseTranslator Translate 호출 : " + morseData);
        string[] morsePatterns;
        int index = -1;
        if (PageController.Instance.CurrentPage == 4)
        {
            morsePatterns = QuestionManager.Instance.CurrentMorsePattern;
            index = System.Array.IndexOf(morsePatterns, morseData);
        }
        else if (PageController.Instance.CurrentPage == 5)
        {
            morsePatterns = new string[]
              {
                "0100", "0000", "1100", "1000", "1101",
                "1110", "0111", "0011", "1011", "1010",
                "0010", "0001", "0101", "0110", "1111"
              };
            index = System.Array.IndexOf(morsePatterns, morseData);
        }

        if (index != -1)
        {
            _currentData = morseData;
            _currentDataIndex = index;
        }
        else
        {
            _currentData = "";
            _currentDataIndex = -1;
        }
        return _currentData;
    }


}
