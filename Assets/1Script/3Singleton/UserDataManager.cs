using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using Unity.VisualScripting;

using Random = UnityEngine.Random;
using TMPro;

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
    public const int defaultValue = -1;
    public Player(string lastName, string firstName, Direction direction, string colorCode, int pieceCount, bool isAllContentPlayed)
    {
        _firstName = firstName;
        _lastName = lastName;

        _direction = direction;
        _colorBallType = (ColorBallType)Enum.Parse(typeof(ColorBallType), colorCode);
        _pieceCount = pieceCount;
        _addPiece = defaultValue;
        partnerPassCode = "";
        IsAllContentPlayed = isAllContentPlayed;
        //SetAnswers();
        Debug.Log($"{_lastName} {_firstName}의 색상 타입이{colorCode} /  {_colorBallType}로 설정되었습니다. {_pieceCount}개의 피스를 가지고 있습니다. 모든 콘텐츠 플레이 여부: {IsAllContentPlayed}");

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

    string _morseTotalData;
    string _morsePartnerTotalData;


    string passCode;
    string partnerPassCode;


    Direction _direction;

    #region Properties
    public ColorBallType ColorBallType
    {
        get { return _colorBallType; }
        set { _colorBallType = value; }
    }

    // public void SetAnswers()
    // {
    //     Debug.Log("질문 수에 맞춰 답변 배열 초기화: " + QuestionManager.Instance.QuestionInfos.Count);
    //     _answers = new int[QuestionManager.Instance.QuestionInfos.Count];
    // }
    public string[] CartridgeContent { get; set; }

    public string MorseTotalData
    {
        get { return _morseTotalData; }
        set { _morseTotalData = value; }
    }

    public string MorsePartnerTotalData
    {
        get { return _morsePartnerTotalData; }
        set { _morsePartnerTotalData = value; }
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
    public string PartnerPassCode
    {
        get { return partnerPassCode; }
        set { partnerPassCode = value; }
    }
    public Queue<string> AnswerData = new Queue<string>();

    public Queue<string> PartnerAnswerData = new Queue<string>();


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
            Debug.Log($"{_firstName}의 점수가 {_score}로 설정되었습니다.");
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

public class UserDataManager : MonoBehaviour, IJsonGenericTarget
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

    public string DebugUID = "";

    public bool IsTestData = false;

    bool _isUsingRoom = false;

    public bool IsUsingRoom
    {
        get { return _isUsingRoom; }
    }

    const int PlayAbleContentNum = 4;
    private Dictionary<string, string> userDataCache = null;
    Player[] player = new Player[2];

    Coroutine userInitializeCoroutine = null;

    Coroutine contentEndCoroutine = null;

    public bool IsContentEnd = false;


    private Action onUserUIDSet;

    public Direction CurrentDirection = Direction.Left;

    bool _isLeftplayer = false;

    const int contentNum = 4;

    public int ContentNum { get { return contentNum; } }

    public List<int[]> GoalIndexint = new List<int[]>();
    JsonGenericUpData _genericData = new JsonGenericUpData();

    string[] contentCodes;

    public int[] stamp { get; private set; } = new int[contentNum];

    public int deviceNum = 1;


    public bool IsLastContent = false;

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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.LogWarning("T Key Pressed");
            TestKey();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.LogWarning("Y Key Pressed");

            TestKey2();
        }
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

    public IEnumerator UploadImageRequest()
    {
        string url = $"http://192.168.0.252:8500/api/uploadFile.cfm?idx_user={userDataCache["IDX_USER"]}&uid={userDataCache["UID_LEFT"]}&code={ServerData.Instance.Code}&type=jpg&count1";

        Debug.Log("UploadImageRequest URL: " + url);

        yield return ServerData.Instance.RequestDataCoroutine($"url", RoomUsingTest);

    }
    public IEnumerator IsUserTagRequest()
    {
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/checkRoomState.cfm?code={ServerData.Instance.Code}", RoomUsingTest);
    }
    public IEnumerator ResetUserCoroutine()
    {
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/resetStart.cfm?idx_user={userDataCache["IDX_USER"]}&code={ServerData.Instance.Code}", Answer);

    }
    public IEnumerator RequestUserTagAll()
    {
        yield return StartCoroutine(IsUserTagRequest());
        yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        if (_isUsingRoom)
            yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/getCurrentRoomUser.cfm?code={ServerData.Instance.Code}", ParseCurrentSessionData);
    }


    public IEnumerator RequestInitializeUserData(string userUID)
    {
        yield return ServerData.Instance.RequestDataCoroutine("http://211.110.44.104:8500/api/" + $"checkIDX.cfm?uid={userUID}&device={ServerData.Instance.DeviceNum}&Code={ServerData.Instance.Code}", ParseJsonData);
    }

    public IEnumerator RequestInitializeUserDataTest(string userUID)
    {
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/getUser.cfm?uid={userUID}", ParseJsonData);
    }
    public IEnumerator RequestCartridgeInfo()
    {
        if (userDataCache == null) yield break;
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/getCartridgeContent.cfm?cartridge={userDataCache["CARTRIDGE"]}", SetBlock);
    }


    public IEnumerator RequestExitRoom()
    {
        if (userDataCache == null) yield break;
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/exitRoom.cfm?code={ServerData.Instance.Code}&idx_user={userDataCache["IDX_USER"]}", Answer);

    }

    public IEnumerator RequestPieceDataUpdate()
    {
        if (userDataCache == null) yield break;
        yield return ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/updatePiece.cfm?idx_user={userDataCache["IDX_USER"]}&code={ServerData.Instance.Code}&value={player[0].AddPiece}", Answer);

    }
    public void ResetUserData()
    {
        if (userDataCache == null) return;
        if (NetworkManager.Instance.IsServer)
            StartCoroutine(ResetUserCoroutine());
        ClearRoom();

    }

    public void ClearRoom()
    {
        if (userDataCache == null) return;
        if (NetworkManager.Instance.IsServer)
            StartCoroutine(RequestExitRoom());

        Reset();

    }


    public bool IsUser()
    {
        return userDataCache != null;
    }

    public void EndRequest()
    {
        if (userDataCache == null) return;
        if (NetworkManager.Instance.IsServer)
            StartCoroutine(ServerData.Instance.RequestDataCoroutine($"http://192.168.0.252:8500/api/updateTime.cfm?idx_user={userDataCache["IDX_USER"]}&option=end&code={ServerData.Instance.Code}", Answer));

        ClearRoom();
    }



    public void Answer(string _an)
    {
        Debug.Log("Server : " + _an);
    }

    // public bool IsLastContentChecker()
    // {

    //     string[] contentCodes = { "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3", "D1", "D2", "D3" };
    //     int clearContentCount = 0;

    //     foreach (var code in contentCodes)
    //     {
    //         if (code == ServerData.Instance.Code)
    //         {
    //             continue; // 현재 콘텐츠는 검사에서 제외
    //         }
    //         string pieceValue = FindValue("PIECE_" + code);
    //         if (pieceValue != null && pieceValue != "null")
    //         {
    //             Debug.Log($"IsLastContent: END_{code} 값 = {pieceValue}");
    //             clearContentCount++;
    //         }
    //     }
    //     Debug.Log($"IsLastContent: 현재까지 클리어된 콘텐츠 수 = {clearContentCount} / {PlayAbleContentNum}");
    //     IsLastContent = PlayAbleContentNum - 1 == clearContentCount;
    //     return IsLastContent; // 현재 콘텐츠를 제외한 나머지 콘텐츠가 모두 클리어된 경우 마지막 콘텐츠로 간주
    // }

    public void RoomUsingTest(string message)
    {
        string[] lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        string trimmed = null;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.StartsWith("<!--") && line.EndsWith("-->"))
            {
                continue;
            }

            trimmed = line;
            break;
        }
        if (trimmed == "EMPTY")
        {
            //Debug.Log("현재 세션 사용자 없음 (EMPTY)");
        }
        else
        {
            _isUsingRoom = true;
            //Debug.Log("현재 세션 사용자 있음 (HAS_USER)");

        }

    }


    public void ParseCurrentSessionData(string responseText)
    {
        if (userDataCache != null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(responseText))
        {
            Debug.LogError("현재 세션 응답이 비어 있습니다.");
            return;
        }

        string[] lines = responseText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        string trimmed = null;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.StartsWith("<!--") && line.EndsWith("-->"))
            {
                continue;
            }

            trimmed = line;
            break;
        }

        if (string.IsNullOrEmpty(trimmed))
        {
            Debug.LogError("현재 세션 응답에서 유효한 데이터 라인을 찾지 못했습니다.");
            return;
        }

        if (trimmed.Equals("EMPTY", StringComparison.OrdinalIgnoreCase))
        {
            // isCurrentSessionEmpty = true;
            // userDataCache = new Dictionary<string, string>
            // {
            //     { "STATE", "EMPTY" }
            // };
            //Debug.Log("현재 세션 사용자 없음 (EMPTY)");
            return;
        }

        string[] parts = trimmed.Split(',');
        if (parts.Length < 3)
        {
            Debug.LogError("현재 세션 응답 형식 오류: " + trimmed);
            return;
        }

        string idxContentText = parts[2].Trim();
        if (!int.TryParse(idxContentText, out int idxContent))
        {
            Debug.LogError("현재 세션 IDX_CONTENT 파싱 오류: " + idxContentText);
            return;
        }

        userDataCache = new Dictionary<string, string>
        {
            { "UID", parts[0].Trim() },
            { "CODE", parts[1].Trim() },
            { "IDX_CONTENT", idxContent.ToString() },
            { "STATE", "HAS_USER" }
        };

        userInitializeCoroutine = StartCoroutine(RequestInitializeUserDataTest(userDataCache["UID"]));

        Debug.Log($"현재 세션 캐시 완료: uid={userDataCache["UID"]}, code={userDataCache["CODE"]}, idx_content={userDataCache["IDX_CONTENT"]}");
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

            //StartCoroutine(RequestCartridgeInfo());


            SetBlock(userDataCache["BLOCK_CODE"]);


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

    public void Reset()
    {
        IsTestData = false;
        IsLastContent = false;
        player[0] = null;
        player[1] = null;
        _isUsingRoom = false;
        userDataCache = null;
        IsContentEnd = false;
        ResultManager.Instance.Reset();
        if (contentEndCoroutine != null)
        {
            StopCoroutine(contentEndCoroutine);
            contentEndCoroutine = null;
        }

    }
    public void TestKey()
    {
        Debug.Log("TestKey Pressed");
        Reset();
        IsTestData = true;
        StartCoroutine(RequestInitializeUserDataTest("2C39C73258"));
        //SetPlayers("길동");
    }
    public void TestKey2()
    {

        Debug.Log("TestKey Pressed");
        Reset();
        IsTestData = true;
        StartCoroutine(RequestInitializeUserDataTest("58F1E83169"));
        //SetPlayers("길동");
    }
    public void TestKey3()
    {
        Reset();
        IsTestData = true;
        StartCoroutine(RequestInitializeUserDataTest("710AE1CE10"));
        //SetPlayers("길동");
    }
    public void TestKey4()
    {
        Reset();
        IsTestData = true;
        StartCoroutine(RequestInitializeUserDataTest("F1701367EE"));
        //SetPlayers("길동");
    }
    public void TestKey5()
    {
        Reset();
        IsTestData = true;
        StartCoroutine(RequestInitializeUserDataTest("56731063CB"));
        //SetPlayers("길동");
    }
    public void SetBlock(string _an)
    {
        contentCodes = _an.Split(',');
        for (int i = 0; i < contentCodes.Length; i++)
            contentCodes[i] = contentCodes[i].Trim();

        Debug.Log("BlockContent : " + string.Join(", ", contentCodes));

        SetPlayers();
    }

    public void SetPlayers()
    {
        int pieceCount = 0;

        string pieceValue;

        bool isLastContent = true;


        foreach (var code in contentCodes)
        {
            pieceValue = FindValue("PIECE_" + code);

            if (string.IsNullOrEmpty(pieceValue) || pieceValue == "null")
            {
                Debug.Log($"코드 {code}의 PIECE_{code} 값이 없습니다. 피스 수 계산에서 0으로 간주됩니다.");
                continue;
            }

            Debug.Log($"코드 {code}의 피스 {pieceValue}");

            if (code == ServerData.Instance.Code)
            {
                continue;
            }
            pieceCount += int.TryParse(pieceValue, out int result) ? result : 0;
        }

        foreach (var code in contentCodes)
        {
            if (code == ServerData.Instance.Code)
            {
                continue;
            }
            string endValue = FindValue("END_" + code);
            if (string.IsNullOrEmpty(endValue) || endValue == "null")
            {
                Debug.Log($"코드 {code}의 END_{code} 값이 없습니다.마지막 체험이 아님");
                isLastContent = false;
                break;
            }

        }


        string relationValue = FindValue("RELATION");

        string cartrigValue = FindValue("CARTRIDGE");




        Debug.Log($"SetPlayers: relationValue={relationValue}, pieceCount={pieceCount}, isLastContent={isLastContent}");

        int relation = 1;
        if (!string.IsNullOrWhiteSpace(relationValue) && !relationValue.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            int.TryParse(relationValue.Trim(), out relation);
        }

        Debug.Log($"Parsed relation value: {relation} from relationValue: '{relationValue}'");


        if (cartrigValue == "A")
        {

            Debug.Log("A 카트리지입니다.");
            relation += 0;

        }
        else if (cartrigValue == "B")
        {
            Debug.Log("B 카트리지입니다.");
            relation += 5;

        }
        else if (cartrigValue == "C")
        {
            Debug.Log("C 카트리지입니다.");
            relation += 10;

        }
        else if (cartrigValue == "D")
        {
            Debug.Log("D 카트리지입니다.");
            relation += 15;
        }


        QuestionManager.Instance.SetRelationship(relation);



        player[0] = new Player(FindValue("RESERVATION_LAST_NAME_LEFT"), FindValue("RESERVATION_FIRST_NAME_LEFT"), Direction.Left, FindValue("COLOR_LEFT"), pieceCount, isLastContent);
        player[1] = new Player(FindValue("RESERVATION_LAST_NAME_RIGHT"), FindValue("RESERVATION_FIRST_NAME_RIGHT"), Direction.Right, FindValue("COLOR_RIGHT"), pieceCount, isLastContent);
        TCPAddPiece();

        QuestionManager.Instance.CurrentIndex = 0;

    }


    public void TCPAddPiece()
    {
        Debug.Log("TCPAddPiece");
        if (player[0].AddPiece == Player.defaultValue)
        {

            player[0].AddPiece = 5;
            player[1].AddPiece = player[0].AddPiece;
            string addPieceData = "S" + player[0].AddPiece;
            NetworkManager.Instance.SendData(addPieceData);
        }


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

    public Player GetPartnerPlayer()
    {
        if (CurrentDirection == Direction.Left)
        {
            return player[1];
        }
        else
        {
            return player[0];
        }
    }

    public void Initialize(JsonGenericUpData data)
    {
        _genericData = data;
        data.boolParams.TryGetValue("isLeftPlayer", out _isLeftplayer);
        if (_isLeftplayer)
        {
            CurrentDirection = Direction.Left;
        }
        else
        {
            CurrentDirection = Direction.Right;
        }
    }
    public JsonGenericUpData Data()
    {
        _genericData.intParams = new Dictionary<string, int>();
        _genericData.floatParams = new Dictionary<string, float>();
        _genericData.boolParams = new Dictionary<string, bool>();
        _genericData.stringParams = new Dictionary<string, string>();


        _genericData.boolParams["isLeftPlayer"] = _isLeftplayer;
        return _genericData;
    }

    public void StartEndWait()
    {
        if (NetworkManager.Instance.IsServer == false)
        {
            IsContentEnd = true;
        }
        if (NetworkManager.Instance.IsServer && contentEndCoroutine == null)
            contentEndCoroutine = StartCoroutine(EndWaitCoroutine());
    }

    public IEnumerator EndWaitCoroutine()
    {
        const float endWaitTimeout = 30f;
        float endWaitStartTime = Time.time;

        while (IsContentEnd == false)
        {
            if (Time.time - endWaitStartTime >= endWaitTimeout)
            {
                Debug.LogWarning($"EndWait timed out after {endWaitTimeout} seconds. Continuing with reset.");
                break;
            }

            NetworkManager.Instance.SendData("EReset");

            yield return CoroutineReturnManager.GetWaitForSeconds(1.5f);
        }
        EndRequest();
        yield return CoroutineReturnManager.GetWaitForSeconds(1.5f);




        NetworkManager.Instance.ResetRequested = true;



        contentEndCoroutine = null;


    }
}