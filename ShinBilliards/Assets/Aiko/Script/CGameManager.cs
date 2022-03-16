using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CGameManager : MonoBehaviourPunCallbacks
{
    private CStickManager _cueManager;
    private CPlayer _player;
    private List<CDebugBall> _cDebugBalls = new List<CDebugBall>();
    private CDebugMainBall _cDebugMainBall;
    [SerializeField] Text _resultText;

    private bool _isInitialized = false;

    private static CGameManager _instance;
    public static CGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CGameManager>();
                if (_instance == null)
                {
                    Debug.LogError("CGameManager無し");
                }
            }

            return _instance;
        }
    }


    enum State
    {
        StartIdle,
        BeachFlags,
        Fight,
        Billiards,
        Finish
    }

    State _state;

    public void Init()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (!_isInitialized)
        {
            // ネットワークオブジェクトの振り分け
            foreach (var obj in PhotonNetwork.PhotonViewCollection)
            {
                //プレイヤー取得
                CPlayer cPlayer = obj.GetComponent<CPlayer>();
                if(cPlayer != null)
                {
                    // 自分のみ取得
                    if (obj.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        _player = cPlayer;
                    }
                }

                // ボール取得
                CDebugBall ball = obj.GetComponent<CDebugBall>();
                if (ball != null)
                {
                    _cDebugBalls.Add(ball);
                    continue;
                }
                CDebugMainBall mainball = obj.GetComponent<CDebugMainBall>();
                if (mainball != null)
                {
                    _cDebugMainBall = mainball;
                    continue;
                }

                // キューマネージャー取得
                CStickManager cueMng = obj.GetComponent<CStickManager>();
                if (cueMng != null)
                {
                    _cueManager = cueMng;
                    continue;
                }
            }

            _isInitialized = true;
        }
        _cueManager.Init();
    }

    public void StartGame()
    {
        photonView.RPC(nameof(StartGameRPC), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void StartGameRPC()
    {
        _state = State.BeachFlags;

        Init();

        if (PhotonNetwork.IsMasterClient)
        {
            _cueManager.CreateCue(false);
        }
    }

    /// <summary>
    /// 未使用
    /// </summary>
    //public void Initialize()
    //{
    //    //_cueManager = CStickManager.Instance;

    //    //仮
    //    _player = FindObjectsOfType<CPlayer>();
    //    _cDebugBalls.AddRange(FindObjectsOfType<CDebugBall>());
    //    _cDebugMainBall = FindObjectOfType<CDebugMainBall>();

    //    _state = State.BeachFlags;
    //}

    public void GetCue()
    {
        photonView.RPC(nameof(GetCueRPC), RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log("Get");
    }

    [PunRPC]
    private void GetCueRPC(int actorNum)
    {
        if (_state == State.BeachFlags)
        {
            // 関数呼び出し元と自身が一致しているかチェック
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
            {
                //一致している場合キューをプレイヤーが取得する(キューのオーナー変更も行う)
                _cueManager.Cue().Picked(_player);

                // ボールのオーナーを変更する
                foreach (CDebugBall ball in _cDebugBalls)
                {
                    ball.ChangeOwner();
                }
                _cDebugMainBall.ChangeOwner();

                Debug.Log("GetCue");
            }
        }

        _state = State.Fight;
    }

    public void LostCue()
    {

        photonView.RPC(nameof(LostCueRPC), RpcTarget.AllViaServer);

    }

    [PunRPC]
    private void LostCueRPC()
    {
        _state = State.BeachFlags;
    }

    public void HitBall()
    {
        Invoke("FinishBilliards", 5.0f);    // 仮

        photonView.RPC(nameof(HitBallRPC), RpcTarget.All);

    }

    [PunRPC]
    private void HitBallRPC()
    {
        _state = State.Billiards;

        // プレイヤーストップ
        _player.MoveStop();

        // キュー消去
        _cueManager.Cue().Destroy();
    }

    public void FinishBilliards()
    {
        photonView.RPC(nameof(FinishBilliardsRPC), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void FinishBilliardsRPC()
    {
        _state = State.BeachFlags;

        // ボール強制ストップ
        foreach (CDebugBall ball in _cDebugBalls)
        {
            ball.MoveStop();
        }
        _cDebugMainBall.MoveStop();

        // キュー作成
        if (PhotonNetwork.IsMasterClient)
        {
            _cueManager.CreateCue(false);
        }

        _player.MoveStart();

    }

    public void GoalBall(CDebugBall ball)
    {
        if(_state == State.Billiards)
        {
            _player.Score.Score += ball.Get_num();
        }

        photonView.RPC(nameof(GoalBallRPC), RpcTarget.All, ball.Get_num());

        //ビリヤード中以外にゴールする場合は無し前提
    }

    [PunRPC]
    private void GoalBallRPC(int num)
    {
        // 対象のボールを削除する
        CDebugBall removeBall = null;

        foreach(CDebugBall ball in _cDebugBalls)
        {
            if(ball.Get_num() == num)
            {
                removeBall = ball;
                ball.gameObject.SetActive(false);
                break;
            }
        }
        Debug.Log(removeBall);
        _cDebugBalls.Remove(removeBall);

        if(PhotonNetwork.IsMasterClient)
        {

            if (_cDebugBalls.Count <= 0)
            {
                // ゲーム終了
                _state = State.Finish;

                photonView.RPC(nameof(GameFinishRPC), RpcTarget.AllViaServer);

            }

        }
    }

    [PunRPC]
    private void GameFinishRPC()
    {

        // プレイヤーストップ
        _player.MoveStop();
        //メインボールストップ
        _cDebugMainBall.MoveStop();

        CPlayer[] players = FindObjectsOfType<CPlayer>();   //TODO

        CPlayer winner = players[0].Score.Score > players[1].Score.Score ? players[0] : players[1];

        FindObjectOfType<CResultCanvas>().SetWinLose(winner == _player);    //TODO

    }
}
