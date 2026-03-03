using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using Unity.VisualScripting;

using Random = UnityEngine.Random;

public class UserJsonData
{
    public List<string> COLUMNS { get; set; }
    public List<List<object>> DATA { get; set; }
}
public enum Direction
{
    Left,
    Right
}
public enum ColorBallType
{
    Orange,
    Red,
    Mint,
    Green,
    Pink,
    Yellow
}

public class Player
{
    public Player(string name, Direction direction, string colorCode, int pieceCount)
    {
        _lastName = name;
        _direction = direction;
        _colorBallType = (ColorBallType)Enum.Parse(typeof(ColorBallType), colorCode);
        _pieceCount = pieceCount;
        SetAnswers();
        Debug.Log($"{_lastName}의 색상 타입이{colorCode} /  {_colorBallType}로 설정되었습니다. {_pieceCount}개의 피스를 가지고 있습니다.");

    }
    string _firstName;

    ColorBallType _colorBallType;
    string _lastName;
    bool _isReady = false;

    bool isAllContentPlayed = false;

    int _ledTagIndx = 0;


    int _score = 0;

    int _playedContentCount = 0;

    int _pieceCount = 0;

    int _addPiece = 0;

    int[] _answers;



    string passCode;

    Direction _direction;

    #region Properties
    public ColorBallType ColorBallType
    {
        get { return _colorBallType; }
        set { _colorBallType = value; }
    }

    public void SetAnswers()
    {
        Debug.Log("질문 수에 맞춰 답변 배열 초기화: " + QuestionManager.Instance.QuestionInfos.Count);
        _answers = new int[QuestionManager.Instance.QuestionInfos.Count];
    }

    public int[] Answers
    {
        get { return _answers; }
        set { _answers = value; }
    }
    public int AddPiece
    {
        get { return _addPiece; }
        set { _addPiece = value; }
    }

    public int PieceCount
    {
        get { return _pieceCount; }
        set { _pieceCount = value; }
    }

    public string PassCode
    {
        get { return passCode; }
        set { passCode = value; }
    }

    public Queue<string> QuestionAnswerData = new Queue<string>();

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; }
    }

    public string LastName
    {
        get { return _lastName; }
        set { _lastName = value; }
    }
    public Direction Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }
    public bool IsReady
    {
        get { return _isReady; }
        set { _isReady = value; }
    }

    public bool IsAllContentPlayed
    {
        get { return isAllContentPlayed; }
        set { isAllContentPlayed = value; }
    }
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            Debug.Log($"{_lastName}의 점수가 {_score}로 설정되었습니다.");
        }
    }
    public int LedTagIndex
    {
        get { return _ledTagIndx; }
        set { _ledTagIndx = value; }
    }

    public int PlayedContentCount
    {
        get { return _playedContentCount; }
        set { _playedContentCount = value; }
    }
    #endregion




}

public class UserDataManager : MonoBehaviour
{

    private static UserDataManager instance;

    public static UserDataManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<UserDataManager>();
            return instance;
        }
    }

    private Dictionary<string, string> userDataCache = null;
    Player[] player = new Player[2];



    private Action onUserUIDSet;

    public Direction CurrentDirection = Direction.Left;

    const int contentNum = 4;

    public int ContentNum { get { return contentNum; } }

    public List<int[]> GoalIndexint = new List<int[]>();


    public int[] stamp { get; private set; } = new int[contentNum];

    public int deviceNum = 1;

    public void AddUserUIDSet(Action action)
    {
        onUserUIDSet += action;
    }

    public string FindValue(string _Key)
    {
        if (userDataCache == null)
        {
            Debug.Log(" userDataCache = null");
            return null;
        }
        return userDataCache[_Key];
    }
    //http://192.168.0.252:8500/api/getUser.cfm?uid=2270AE4A-ABFC-E349-1A0A5A69999CC1A8

    // public IEnumerator RequestUserDataUpdate(int _question, string _value)
    // {
    //     //http://192.168.0.252:8500/api/getUser.cfm?uid=
    //     if (userDataCache == null) yield break;
    //     yield return ServerData.Instance.RequestDataCoroutine("http://211.110.44.104:8500/api/" + $"updateValue.cfm?idx_user={userDataCache["IDX_USER"]}&uid={userDataCache["UID"]}&code={ServerData.Instance.Code}&question={_question}&value={_value}&device={ServerData.Instance.DeviceNum}", Answer);
    // }
    public IEnumerator RequestUserDataUpdate(int _question, int _value, Direction direction)
    {
        string contentCode = "D1";
        string side = "";
        //http://192.168.0.252:8500/api/getUser.cfm?uid=
        if (userDataCache == null) yield break;
        if (direction == Direction.Left)
        {
            side = "left";
        }
        else
        {
            side = "right";
        }
        Debug.Log($"RequestUserDataUpdate: question={_question}, value={_value}, direction={direction}, contentCode={contentCode}");
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/updateValue.cfm?idx_user={userDataCache["IDX_USER"]}&q_no={_question}&side={side}&code={contentCode}&value={_value}", Answer);
    }



    public IEnumerator RequestInitializeUserData(string userUID)
    {
        yield return ServerData.Instance.RequestDataCoroutine("http://211.110.44.104:8500/api/" + $"checkIDX.cfm?uid={userUID}&device={ServerData.Instance.DeviceNum}&Code={ServerData.Instance.Code}", ParseJsonData);
    }

    public IEnumerator RequestInitializeUserDataTest(string userUID)
    {
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/getUser.cfm?uid={userUID}", ParseJsonData);
    }



    public IEnumerator RequestUserContentEnd()
    {
        if (userDataCache == null) yield break;


        yield return ServerData.Instance.RequestDataCoroutine("http://211.110.44.104:8500/api/" + $"updateValue.cfm?idx_user={userDataCache["IDX_USER"]}&uid={userDataCache["UID"]}&code={ServerData.Instance.Code}&question={33}&value={1}&device={ServerData.Instance.DeviceNum}", Answer);

    }

    public void Answer(string _an)
    {
        Debug.Log("Server : " + _an);
    }

    public bool IsUser()
    {
        if (userDataCache != null && userDataCache.Count > 0)
        {
            return true;
        }
        return false;
    }

    public void ParseJsonData(string jsonText)
    {
        Debug.Log("ParseJsonData : " + jsonText);
        try
        {
            // 우선 클래스로 파싱
            UserJsonData parsedData = JsonConvert.DeserializeObject<UserJsonData>(jsonText);

            if (parsedData == null || parsedData.COLUMNS == null || parsedData.DATA == null || parsedData.DATA.Count == 0)
            {
                Debug.LogError("JSON 구조가 잘못되었습니다.");
                userDataCache = null;
                return; //false;
            }

            List<string> columns = parsedData.COLUMNS;
            List<object> dataRow = parsedData.DATA[0]; // 첫번째 데이터 행을 사용한다고 가정

            if (columns.Count != dataRow.Count)
            {
                Debug.LogError("COLUMNS와 DATA의 개수가 맞지 않습니다.");
                userDataCache = null;
                return;//false;
            }

            // Dictionary 생성
            userDataCache = new Dictionary<string, string>();

            for (int i = 0; i < columns.Count; i++)
            {
                string key = columns[i];
                string value = dataRow[i]?.ToString() ?? "null";
                userDataCache[key] = value;
            }
            if (userDataCache != null && userDataCache.Count > 0)
            {
                if (onUserUIDSet != null) onUserUIDSet.Invoke();
            }

            SetPlayers();



            //SetPlayer



        }
        catch (JsonException ex)
        {
            Debug.LogError("JSON 파싱 중 에러 발생: " + ex.Message);
            userDataCache = null;
            return;// false;
        }
    }

    public int GetStamp(int _contentIDX)
    {
        if (stamp.Length > _contentIDX)
        {
            return stamp[_contentIDX];
        }

        else
        {
            Debug.Log("GetStamp Error ");
            return -1;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestKey();
        }
    }
    public void Reset()
    {
        player[0] = null;
        player[1] = null;
    }
    public void TestKey()
    {
        Reset();
        StartCoroutine(RequestInitializeUserDataTest("2270AE4A-ABFC-E349-1A0A5A69999CC1A8"));

        //SetPlayers("길동");
    }

    public void SetPlayers()
    {
        string[] contentCodes = { "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3", "D1", "D2", "D3" };

        int pieceCount = 0;

        foreach (var code in contentCodes)
        {
            Debug.Log($"코드 {code}의 피스 {FindValue("PIECE_" + code)}");

            pieceCount += int.TryParse(FindValue("PIECE_" + code), out int result) ? result : 0;

        }

        player[0] = new Player(FindValue("RESERVATION_LAST_NAME_LEFT"), Direction.Left, FindValue("COLOR_LEFT"), pieceCount);
        player[1] = new Player(FindValue("RESERVATION_LAST_NAME_RIGHT"), Direction.Right, FindValue("COLOR_RIGHT"), pieceCount);



        player[0].AddPiece = UnityEngine.Random.Range(1, 6);
        player[1].AddPiece = player[0].PieceCount;


        QuestionManager.Instance.CurrentIndex = 0;

    }
    public Player GetPlayer(Direction direction)
    {
        if (direction == Direction.Left)
        {
            return player[0];
        }
        else
        {
            return player[1];
        }
    }
    public Player GetPlayer()
    {
        if (CurrentDirection == Direction.Left)
        {
            return player[0];
        }
        else
        {
            return player[1];
        }
    }

    public void ResetUser()
    {
        userDataCache = null;
    }


}