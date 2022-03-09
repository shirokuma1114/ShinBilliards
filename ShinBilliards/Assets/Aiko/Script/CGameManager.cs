using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameManager : CSingletonMonoBehaviour<CGameManager>
{
    private CStickManager _stickManager;
    private CPlayer[] _cPlayers;
    private CPlayer _useCuePlayer;
    private List<CDebugBall> _cDebugBalls = new List<CDebugBall>();
    private CDebugMainBall _cDebugMainBall;
    [SerializeField] Text _resultText;


    enum State
    {
        Idle,
        BeachFlags,
        Fight,
        Billiards,
        Finish
    }

    State _state;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _stickManager = CStickManager.Instance;
        _stickManager.Init();

        //仮
        _cPlayers = FindObjectsOfType<CPlayer>();
        _cDebugBalls.AddRange(FindObjectsOfType<CDebugBall>());
        _cDebugMainBall = FindObjectOfType<CDebugMainBall>();

        _state = State.BeachFlags;
    }

    public void GetCue()
    {
        _state = State.Fight;

        // ボールみんなにオーナーを委譲する
        _cDebugMainBall = FindObjectOfType<CDebugMainBall>();
        foreach (CDebugBall ball in _cDebugBalls)
        {
            ball.ChangeOwner();
        }
        FindObjectOfType<CDebugMainBall>().ChangeOwner();
    }

    public void HitBall(CPlayer whichPlayer)
    {
        _state = State.Billiards;
        _useCuePlayer = whichPlayer;
        Invoke("FinishBilliards", 5.0f);    // 仮

        // プレイヤーストップ
        //foreach (CPlayer player in _cPlayers)
        //{
        //    player.MoveStop();
        //}
    }

    public void FinishBilliards()
    {
        _state = State.BeachFlags;
        _stickManager.CreateCue(false);

        // ボール強制ストップ
        foreach (CDebugBall ball in _cDebugBalls)
        {
            ball.MoveStop();
        }

        // プレイヤー動けるように
        foreach (CPlayer player in _cPlayers)
        {
            player.MoveStart();
        }
    }

    public void GoalBall(CDebugBall ball)
    {
        if(_state == State.Billiards)
        {
            _useCuePlayer.Score.Score += ball.Get_num();
        }
        _cDebugBalls.Remove(ball);
        if(_cDebugBalls.Count <= 0)
        {
            // ゲーム終了
            _state = State.Finish;

            // プレイヤーストップ
            //foreach (CPlayer player in _cPlayers)
            //{
            //    player.MoveStop();
            //}
            // メインボールストップ
            //_cDebugMainBall.MoveStop();

            //if (_cPlayers[0].Score.Score < _cPlayers[1].Score.Score)
            //{
            //    _resultText.text = "Player 2 Win!";
            //}
            //else
            //{
            //   _resultText.text = "Player 1 Win!";
            //}
            //_resultText.gameObject.SetActive(true);

        }

        //ビリヤード中以外にゴールする場合は無し前提
    }
}
