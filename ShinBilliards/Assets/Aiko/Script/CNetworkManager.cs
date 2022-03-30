using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CNetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private int MAX_PLAYER_NUM = 2; // ���[���ő�l���i�Œ�l�j

    [SerializeField]
    private CNetworkObjectManager _objMng = null;

    [SerializeField]
    private GameObject _disconnectPrefab = null;

    void Start()
    {
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
    }

    void OnGUI()
    {
        //���O�C���̏�Ԃ���ʏ�ɏo��
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }


    //���[���ɓ����O�ɌĂяo�����
    public override void OnConnectedToMaster()
    {
        // �����_���ȃ��[���ɎQ��
        //PhotonNetwork.JoinRandomRoom();

        // ���[���̎Q���l����2�l�ɐݒ肷��
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MAX_PLAYER_NUM;

        // "room"�Ƃ������O�̃��[���ɎQ������i���[����������΍쐬���Ă���Q������j
        PhotonNetwork.JoinOrCreateRoom("neko", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // ���[���̎Q���l����2�l�ɐݒ肷��
        //var roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = (byte)MAX_PLAYER_NUM;

        //PhotonNetwork.CreateRoom(null, roomOptions);
    }

    //���[���ɓ������null���
    public override void OnJoinedRoom()
    {
        // �v���C���[�쐬
        _objMng.CreatePlayer(PhotonNetwork.CurrentRoom.PlayerCount);

        // ���[���I�u�W�F�N�g�쐬(�}�X�^�[�N���C�A���g�̂�)
        _objMng.CreateRoomObj();
        
        // ���[���������ɂȂ�����A�ȍ~���̃��[���ւ̎Q����s���ɂ���
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // �Q�[���J�n�̏��������s
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _objMng.StartGame();
            }
        }
    }

    // �T�[�o�[�ؒf���ɌĂ΂��
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"�T�[�o�[�Ƃ̐ڑ����ؒf����܂���: {cause.ToString()}");

        Time.timeScale = 0.0f;
        //�^�C�g���ɖ߂�
        //Instantiate(_disconnectPrefab);

    }

}
