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
    public const float DefaultDotTime = 0.5f;

    public const float DefaultDashTime = 2f;
    public const float MaxDotTime = 1.0f;

    public const float MaxDashTime = 2.8f;

    public const float InputResetTime = 3f;

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


    public static MorseTranslatorData Translate(string morseData, float[] pressTimes)
    {
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
                difference = 0f;
                Debug.LogError("MorseTranslator Translate 오류 : morseData가 0또는 1이 아님");
            }

            if (difference < 0.5f)
            {
                ;
            }
            else if (difference < 1f)
            {
                accuracy -= 5f;
            }
            else
            {
                accuracy -= 10f;
            }

            //TODO 아래가 정확한 판단 기획서랑 달라서 일단 주석
            // if (morseData[i] == '0')
            // {

            //     if (pressTimes[i] < DefaultDotTime)
            //     {
            //         outputPressTimes[i] = pressTimes[i] / DefaultDotTime;

            //     }
            //     else if (pressTimes[i] > DefaultDotTime)
            //     {
            //         outputPressTimes[i] = pressTimes[i] / DefaultDotTime;
            //         if (outputPressTimes[i] > 2f)
            //             outputPressTimes[i] = 0f;
            //         else
            //         {
            //             outputPressTimes[i] -= 1f;
            //         }
            //     }
            //     else
            //     {
            //         outputPressTimes[i] = 1f;
            //     }
            // }
            // else if (morseData[i] == '1')
            // {

            //     if (pressTimes[i] < DefaultDashTime)
            //     {
            //         outputPressTimes[i] = pressTimes[i] / DefaultDashTime;

            //     }
            //     else if (pressTimes[i] > DefaultDashTime)
            //     {
            //         outputPressTimes[i] = pressTimes[i] / DefaultDashTime;
            //         if (outputPressTimes[i] > 2f)
            //             outputPressTimes[i] = 0f;
            //         else
            //         {
            //             outputPressTimes[i] -= 1f;
            //         }
            //     }
            //     else
            //     {
            //         outputPressTimes[i] = 1f;
            //     }
            // }
        }

        if (PageController.Instance.CurrentPage == 4)
        {
            switch (morseData)
            {
                case "0010":
                    _currentData = "0010";
                    _currentDataIndex = 0;
                    break;
                case "0100":
                    _currentData = "0100";
                    _currentDataIndex = 1;

                    break;
                case "0110":
                    _currentData = "0110";
                    _currentDataIndex = 2;

                    break;
                case "0101":
                    _currentData = "0101";
                    _currentDataIndex = 3;

                    break;
                case "0001":
                    _currentData = "0001";
                    _currentDataIndex = 4;

                    break;
                default:
                    _currentData = "";
                    _currentDataIndex = -1;

                    break;
            }
        }
        else if (PageController.Instance.CurrentPage == 5)
        {
            switch (morseData)
            {
                case "0100":
                    _currentData = "0100";
                    _currentDataIndex = 0;

                    break;
                case "0000":
                    _currentData = "0000";
                    _currentDataIndex = 1;

                    break;
                case "1100":
                    _currentData = "1100";
                    _currentDataIndex = 2;

                    break;
                case "1000":
                    _currentData = "1000";
                    _currentDataIndex = 3;
                    break;
                case "1101":
                    _currentData = "1101";
                    _currentDataIndex = 4;
                    break;
                case "1110":
                    _currentData = "1110";
                    _currentDataIndex = 5;
                    break;
                case "0111":
                    _currentData = "0111";
                    _currentDataIndex = 6;
                    break;
                case "0011":
                    _currentData = "0011";
                    _currentDataIndex = 7;
                    break;
                case "1011":
                    _currentData = "1011";
                    _currentDataIndex = 8;
                    break;
                case "1010":
                    _currentData = "1010";
                    _currentDataIndex = 9;
                    break;
                case "0010":
                    _currentData = "0010";
                    _currentDataIndex = 10;
                    break;
                case "0001":
                    _currentData = "0001";
                    _currentDataIndex = 11;
                    break;
                case "0101":
                    _currentData = "0101";
                    _currentDataIndex = 12;
                    break;
                case "0110":
                    _currentData = "0110";
                    _currentDataIndex = 13;
                    break;
                case "1111":
                    _currentData = "1111";
                    _currentDataIndex = 14;
                    break;
                default:
                    _currentData = "";
                    _currentDataIndex = -1;
                    break;
            }
        }
        if (_currentData != "")
        {
            _MorseTranslatorData.SetData(_currentData, accuracy);

        }
        else
        {
            _MorseTranslatorData.SetData(_currentData, 0f);

        }

        return _MorseTranslatorData;
    }
}
