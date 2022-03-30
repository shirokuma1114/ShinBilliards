using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CGameManager : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public CStickManager _cueManager;
    private PlayerController _player;
    private List<CDebugBall> _cDebugBalls = new List<CDebugBall>();
    private CDebugMainBall _cDebugMainBall;
    private CTimer _timer;

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
                    Debug.LogError("CGameManager����");
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
            // �l�b�g���[�N�I�u�W�F�N�g�̐U�蕪��
            foreach (var obj in PhotonNetwork.PhotonViewCollection)
            {
                //�v���C���[�擾
                PlayerController cPlayer = obj.GetComponent<PlayerController>();
                if(cPlayer != null)
                {
                    // ���������肩
                    if (obj.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        _player = cPlayer;
                    }
                    else
                    {
                        // �ǉ�����̃v���C���[��F���ł��Ȃ�
                        //_playerOther = cPlayer;
                    }
                }

                // �{�[���擾
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

                // �L���[�}�l�[�W���[�擾
                CStickManager cueMng = obj.GetComponent<CStickManager>();
                if (cueMng != null)
                {
                    _cueManager = cueMng;
                    continue;
                }

                // �^�C�}�[�擾
                if (_timer == null)
                {
                    CTimer timer = obj.GetComponent<CTimer>();
                    if(timer != null)
                    {
                        _timer = timer;
                    }
                }
            }

            _isInitialized = true;
        }
        _cueManager.Init();
        FindObjectOfType<CCountDownUI>().Register(_timer);
    }

    public void StartGame()
    {
        // �J�E���g�_�E���^�C�}�[����
        GameObject timer = PhotonNetwork.Instantiate("Timer", Vector3.zero, Quaternion.identity, 0);
        _timer = timer.GetComponent<CTimer>();
        _timer._onFinish.AddListener(GameTimeFinish);

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

        SoundManager.Instance.PlayBGM("Game", true);
        SoundManager.Instance.PlaySE("Start");

        StartCoroutine(StartCountDown());
    }

    private IEnumerator StartCountDown()
    {

        yield return new WaitForSeconds(3.0f);


        if (_state == State.Finish)
        {
            MasterData.Instance._myPlayerPrefabName = PhotonNetwork.LocalPlayer.GetPrefabName();
            MasterData.Instance._otherPlayerPrefabName = null;
            yield break;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal)
            {
                MasterData.Instance._myPlayerPrefabName = player.GetPrefabName();
            }
            else
            {
                MasterData.Instance._otherPlayerPrefabName = player.GetPrefabName();
            }
        }

        Debug.Log("my : " + MasterData.Instance._myPlayerPrefabName + "other : " + MasterData.Instance._otherPlayerPrefabName);
    }

    /// <summary>
    /// ���g�p
    /// </summary>
    //public void Initialize()
    //{
    //    //_cueManager = CStickManager.Instance;

    //    //��
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
            // �֐��Ăяo�����Ǝ��g����v���Ă��邩�`�F�b�N
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNum)
            {
                //��v���Ă���ꍇ�L���[���v���C���[���擾����(�L���[�̃I�[�i�[�ύX���s��)
                _cueManager.Cue().CueUse(_player);
                _player.CueUse();

                // �{�[���̃I�[�i�[��ύX����
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
        Invoke("FinishBilliards", 5.0f);    // ��

        photonView.RPC(nameof(HitBallRPC), RpcTarget.All);

    }

    [PunRPC]
    private void HitBallRPC()
    {
        _state = State.Billiards;

        // �L���[�V���b�g���i���łɂ����Ɂj
        SoundManager.Instance.PlaySE("Cue_Shot");

        // �L���[����
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

        // �{�[�������X�g�b�v
        foreach (CDebugBall ball in _cDebugBalls)
        {
            ball.MoveStop();
        }
        _cDebugMainBall.MoveStop();

        // �L���[�쐬
        if (PhotonNetwork.IsMasterClient)
        {
            _cueManager.CreateCue(false);
        }

        //_player.MoveStart();

    }

    public void GoalBall(CDebugBall ball, Vector3 goalPos)
    {
        if(_state == State.Billiards)
        {
            _player.Score.Score += ball.Get_num();
        }
        Debug.Log(_state);
        //�r�����[�h���ȊO�ɃS�[������ꍇ�͖����O��

        photonView.RPC(nameof(GoalBallRPC), RpcTarget.All, ball.Get_num());

        PhotonNetwork.Instantiate("GoalEffect", goalPos, Quaternion.AngleAxis(-90f, Vector3.right), 0);

    }

    [PunRPC]
    private void GoalBallRPC(int num)
    {
        if (_state == State.Finish) return;

        // �Ώۂ̃{�[�����폜����
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
                // �Q�[���I��
                _state = State.Finish;

                photonView.RPC(nameof(GameFinishRPC), RpcTarget.AllViaServer, false);

            }

        }
    }

    // �}�X�^�[�N���C�A���g�̂�
    private void GameTimeFinish()
    {
        if (_state == State.Finish) return;

        // �Q�[���I��
        _state = State.Finish;

        photonView.RPC(nameof(GameFinishRPC), RpcTarget.AllViaServer, false);
    }

    // ���v���C���[�����[������ޏo�������ɌĂ΂��R�[���o�b�N
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �Q�[���I������
        if (_state != State.Finish)
        {
            _state = State.Finish;
            GameFinishRPC(true);
        }
    }

    [PunRPC]
    private void GameFinishRPC(bool forceFinish)
    {

        // �v���C���[�X�g�b�v
        //_player.MoveStop();
        //���C���{�[���X�g�b�v
        _cDebugMainBall.MoveStop();
        
        MasterData.Instance._myPlayerScore = _player.Score.Score;
        
        if(forceFinish)
        {// �����I��������
            MasterData.Instance._otherPlayerScore = 0;
            MasterData.Instance._myPlayerWin = true;
        }
        else
        {
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach(PlayerController player in players)
            {
                if (player.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    MasterData.Instance._otherPlayerScore = player.Score.Score;
                    MasterData.Instance._myPlayerWin = _player.Score.Score >= player.Score.Score;
                }
            }
        }

        SoundManager.Instance.PlaySE("Finish");

        FindObjectOfType<CResultCanvas>().SetWinLose(MasterData.Instance._myPlayerWin);    //TODO

        //Time.timeScale = 0.0f;  //TODO

        Invoke("GoToResult", 2.0f);

    }

    private void GoToResult()
    {
        Time.timeScale = 1.0f;  //TODO
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }
}
