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
    
    enum PlayerPrefabList
    {
        Player_Girl_1 = 0,
        Player_Girl_2 = 1,
        Player_Boy_1 = 2,
        Player_Boy_2 = 3,
        PrefabCount,
    }

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
        GameObject player;

        while (true)
        {
            PlayerPrefabList number = (PlayerPrefabList)Random.Range(0, (int)PlayerPrefabList.PrefabCount - 1);

            Debug.Log(number.ToString());
            if (GameObject.Find(number.ToString()) == null)
            {
                //�L�����N�^�[�𐶐�
                player = PhotonNetwork.Instantiate(number.ToString(), new Vector3(-1.0f + (num - 1) * 3.0f, 0, -12.0f), Quaternion.identity, 0);
                //��������������ł���悤�ɃX�N���v�g��L���ɂ���
                PlayerController playerScript = player.GetComponent<PlayerController>();
                playerScript.enabled = true;
                break;
            }
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _gameManager.StartGame();
        }
    }

}
