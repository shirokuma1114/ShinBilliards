using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CNetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private int MAX_PLAYER_NUM = 2; // ルーム最大人数（固定値）

    [SerializeField]
    private CNetworkObjectManager _objMng = null;

    [SerializeField]
    private GameObject _disconnectCanvas = null;

    [SerializeField]
    private GameObject _otherDisconnectCanvas = null;

    [SerializeField]
    private bool _isRandomMatch = false;

    [SerializeField]
    private string _roomName = "neko";

    void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

        SoundManager.Instance.PlayBGM("Wait", true);
    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }


    //ルームに入室前に呼び出される
    public override void OnConnectedToMaster()
    {
        if (_isRandomMatch)
        {
            // ランダムなルームに参加
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //ルームの参加人数を2人に設定する
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)MAX_PLAYER_NUM;

            //_roomNameという名前のルームに参加する（ルームが無ければ作成してから参加する）
            PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, TypedLobby.Default);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (_isRandomMatch)
        {
            // ルームの参加人数を2人に設定する
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)MAX_PLAYER_NUM;

            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    //ルームに入室後にnullれる
    public override void OnJoinedRoom()
    {
        // プレイヤー作成
        _objMng.CreatePlayer(PhotonNetwork.CurrentRoom.PlayerCount);

        // ルームオブジェクト作成(マスタークライアントのみ)
        _objMng.CreateRoomObj();
        
        // ルームが満員になったら、以降そのルームへの参加を不許可にする
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // ゲーム開始の処理を実行
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _objMng.StartGame();
            }
        }
    }

    // サーバー切断時に呼ばれる
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"サーバーとの接続が切断されました: {cause.ToString()}");

        Time.timeScale = 0.0f;
        //タイトルに戻る
        if (_disconnectCanvas != null)
        {
            _disconnectCanvas.SetActive(true);
        }

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //タイトルに戻る
        if (_otherDisconnectCanvas != null)
        {
            _otherDisconnectCanvas.SetActive(true);
        }
    }
    
    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }


}
