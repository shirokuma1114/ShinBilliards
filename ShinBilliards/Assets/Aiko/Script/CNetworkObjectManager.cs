using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CNetworkObjectManager : MonoBehaviourPunCallbacks
{
    public CGameManager _gameManager = null;
    //public CPlayer _player = null;
    //public CDebugBall[] _balls = new CDebugBall[9];
    //public CDebugMainBall _mainBall = null;
    //public CStickManager _cueMng = null;
    

    public void CreateRoomObj()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // �Q�[���}�l�[�W���[����
            GameObject gameManager = PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity, 0);
            _gameManager = gameManager.GetComponent<CGameManager>();

            // �{�[������
            GameObject ball = PhotonNetwork.Instantiate("Balls", Vector3.zero, Quaternion.identity, 0);
            //_balls = ball.GetComponentsInChildren<CDebugBall>();
            //_mainBall = ball.GetComponentInChildren<CDebugMainBall>();

            // �L���[�}�l�[�W���[����
            GameObject cueMng = PhotonNetwork.Instantiate("CueManager", Vector3.zero, Quaternion.identity, 0);
            //_cueMng = cueMng.GetComponent<CStickManager>();

        }

    }

    public void CreatePlayer(int num)
    {
        //�L�����N�^�[�𐶐�
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(-1.0f + (num - 1) * 3.0f, 0, -5.0f), Quaternion.identity, 0);
        //��������������ł���悤�ɃX�N���v�g��L���ɂ���
        CPlayer playerScript = player.GetComponent<CPlayer>();
        playerScript.enabled = true;

        //_player = playerScript;

    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _gameManager.StartGame();
        }
    }

}
