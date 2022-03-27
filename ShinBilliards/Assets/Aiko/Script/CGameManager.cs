using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CGameManager : MonoBehaviourPunCallbacks
{
    public CStickManager _cueManager;
    private PlayerController _player;
    private PlayerController _playerOther;
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
                PlayerController cPlayer = obj.GetComponent<PlayerController>();
                if(cPlayer != null)
                {
                    // 自分か相手か
                    if (obj.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        _player = cPlayer;
                    }
                    else
                    {
                        _playerOther = cPlayer;
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
        if (_state == State.Finish) return;

        if (_state == State.BeachFlags)
        {
            photonView.RPC(nameof(GetCueRPC), RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
        }
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
                _cueManager.Cue().CueUse(_player);
                _player.CueUse();

                // ボールのオーナーを変更する
                foreach (CDebugBall ball in _cDebugBalls)
                {
                    ball.ChangeOwner();
                }
                _cDebugMainBall.ChangeOwner();

                SoundManager.Instance.PlaySE("Cue_Get");
            }

            _state = State.Fight;

            _cueManager.Cue().Take();

            foreach (CDebugBall ball in _cDebugBalls)
            {
                ball.StartBilliards();
            }
            _cDebugMainBall.StartBilliards();
        }
    }

    public void LostCue()
    {
        photonView.RPC(nameof(LostCueRPC), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void LostCueRPC()
    {
        _state = State.BeachFlags;
        _cueManager.Cue().CueRelease();
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

        // キューショット音（ついでにここに）
        SoundManager.Instance.PlaySE("Cue_Shot");

        // キュー消去
        _cueManager.Cue().Destroy();

        _player.CueNoUse();
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

        //_player.MoveStart();

    }

    public void GoalBall(CDebugBall ball, Transform goal)
    {
        if(_state == State.Billiards)
        {
            _player.Score.Score += ball.Get_num();
        }
        Debug.Log(_state);
        //ビリヤード中以外にゴールする場合は無し前提

        photonView.RPC(nameof(GoalBallRPC), RpcTarget.All, ball.Get_num());

        //PhotonNetwork.Instantiate("GoalEffect", goal.position, Quaternion.AngleAxis(-90f, Vector3.right), 0);

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

        SoundManager.Instance.PlaySE("Ball_Goal");
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

    // 他プレイヤーがルームから退出した時に呼ばれるコールバック
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // ゲーム終了する
        if (_state != State.Finish)
        {
            _state = State.Finish;
            GameFinishRPC(true);
        }
    }

    [PunRPC]
    private void GameFinishRPC(bool forceFinish = false)
    {

        // プレイヤーストップ
        //_player.MoveStop();
        //メインボールストップ
        _cDebugMainBall.MoveStop();
        
        MasterData.Instance._myPlayerScore = _player.Score.Score;
        
        if(forceFinish)
        {// 強制終了時処理
            MasterData.Instance._otherPlayerScore = 0;
            MasterData.Instance._myPlayerWin = true;
        }
        else
        {
            MasterData.Instance._otherPlayerScore = _playerOther.Score.Score;
            MasterData.Instance._myPlayerWin = _player.Score.Score >= _playerOther.Score.Score;
        }

        FindObjectOfType<CResultCanvas>().SetWinLose(MasterData.Instance._myPlayerWin);    //TODO

        Time.timeScale = 0.0f;  //TODO

    }
}
