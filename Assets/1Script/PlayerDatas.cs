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
    PlayDirection _direction;

    #region Properties
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


public class PlayerDatas : MonoBehaviour
{

    private static PlayerDatas instance;

    public static PlayerDatas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerDatas>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerDatas");
                    instance = singletonObject.AddComponent<PlayerDatas>();
                }
            }
            return instance;
        }
    }
    Player[] players = new Player[2];


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
    public void AddLedPair()
    {
        for (int i = 0; i < 10; i++)
        {
            int first = Random.Range(0, 6);
            int second = Random.Range(6, 12);
            GoalIndexint.Add(new Pair(first, second));
        }
    }
    // public Pair GetPlayerLEDPair(int playerIndex)
    // {
    //     if (playerIndex == 0 && GoalIndexint.Count == 0)
    //     {
    //         SetLedPair();
    //     }
    //     if (players[playerIndex].LedTagIndex + 1 >= GoalIndexint.Count)
    //     {
    //         Debug.Log("사용 다 해서 다시 생성");
    //         AddLedPair();
    //     }
    //     Debug.Log($"플레이어 {playerIndex}의 현재 LED 태그 인덱스: {players[playerIndex].LedTagIndex} / {GoalIndexint.Count}");
    //     return GoalIndexint[players[playerIndex].LedTagIndex];
    // }

    public void AddPlayerLEDIndex()
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                player.LedTagIndex++;
            }
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

    void Player1ScoreUp()
    {
        if (players[1] != null)
        {
            players[1].Score++;
            players[1].LedTagIndex++;
        }
    }
    void Player0ScoreUp()
    {
        if (players[0] != null)
        {
            players[0].Score++;
            players[0].LedTagIndex++;
        }
    }
    void ResetPlayer0Score()
    {
        players[0].Score = 0;

    }
    void ResetPlayer1Score()
    {
        players[1].Score = 0;

    }




    public void Reset()
    {
        players[0] = null;
        players[1] = null;
    }


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TestKey();
        }
    }



    public void TestKey()
    {
        Reset();
        SetPlayers("아영");
        SetPlayers("길동");
    }

    public void SetPlayers(string Name)
    {
        Debug.Log($"플레이어 추가 요청: {Name}");
        if (players[0] == null)
        {
            players[0] = new Player(Name, PlayDirection.Left);
        }
        else if (players[1] == null)
        {
            players[1] = new Player(Name, PlayDirection.Right);
        }
        else
        {
            Debug.LogError("플레이어가 이미 모두 설정되어 있습니다.");
        }
    }


    public int GetCurrentPlayersNum()
    {
        int Length = 0;
        foreach (var player in players)
        {
            if (player != null)
            {
                Length++;
            }
        }
        return Length;
    }

    public Player GetPlayers(int playerIndex)
    {
        return players[playerIndex];
    }



}
