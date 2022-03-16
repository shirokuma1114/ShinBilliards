using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PunServer : MonoBehaviourPunCallbacks
{

    void Start()
    {
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
        // "room"�Ƃ������O�̃��[���ɎQ������i���[����������΍쐬���Ă���Q������j
        PhotonNetwork.JoinOrCreateRoom("room2", new RoomOptions(), TypedLobby.Default);
    }

    //���[���ɓ�����ɌĂяo�����
    public override void OnJoinedRoom()
    {
        int number = PhotonNetwork.CountOfPlayersInRooms;//���[�����l��

        //�L�����N�^�[�𐶐�
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(-1.0f  + number,0,-5.0f), Quaternion.identity, 0);
        //��������������ł���悤�ɃX�N���v�g��L���ɂ���
        PlayerController playerScript = player.GetComponent<PlayerController>();
        playerScript.enabled = true;

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    // ���ۂ̏���
        //    GameObject ball = PhotonNetwork.Instantiate("Balls", new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, 0);
        //}

    }
}
