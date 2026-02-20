using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayDirection
{
    Left,
    Right
}

public class Player
{
    string _name;
    bool _isReady = false;

    bool isAllContentPlayed = false;

    int _ledTagIndx = 0;

    int _score = 0;

    int _playedContentCount = 0;

    int _stampCount = 0;

    string passCode;

    PlayDirection _direction;

    #region Properties

    public int StampCount
    {
        get { return _stampCount; }
        set { _stampCount = value; }
    }

    public string PassCode
    {
        get { return passCode; }
        set { passCode = value; }
    }

    public Queue<string> QuestionAnswerData = new Queue<string>();

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public PlayDirection Direction
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
            Debug.Log($"{_name}의 점수가 {_score}로 설정되었습니다.");
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

    public Player(string name, PlayDirection direction)
    {
        _name = name;
        _direction = direction;
    }


}

public class Pair
{
    int _first;
    int _second;

    public int First
    {
        get { return _first; }
    }
    public int Second
    {
        get { return _second; }
    }

    public Pair(int first, int second)
    {
        _first = first;
        _second = second;
    }
}


public class PlayerData : MonoBehaviour
{

    private static PlayerData instance;

    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerData>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerDatas");
                    instance = singletonObject.AddComponent<PlayerData>();
                }
            }
            return instance;
        }
    }
    Player player;


    public List<Pair> GoalIndexint = new List<Pair>();

    public void SetLedPair()
    {
        GoalIndexint.Clear();
        for (int i = 0; i < 50; i++)
        {
            int first = Random.Range(0, 6);
            int second = Random.Range(6, 12);
            GoalIndexint.Add(new Pair(first, second));
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {

    }

    public void Reset()
    {
        player = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestKey();
        }
    }

    public void TestKey()
    {
        Reset();
        SetPlayers("길동");
    }
    public void SetPlayers(string Name)
    {
        Debug.Log($"플레이어 추가 요청: {Name}");
        if (player == null)
        {
            player = new Player(Name, PlayDirection.Left);
        }
        player.StampCount = UnityEngine.Random.Range(1, 6);

        QuestionManager.Instance.CurrentIndex = 0;

    }

    public Player GetPlayer()
    {
        return player;
    }
}
