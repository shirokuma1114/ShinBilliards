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
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }


    //ルームに入室前に呼び出される
    public override void OnConnectedToMaster()
    {
        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
    }

    //ルームに入室後に呼び出される
    public override void OnJoinedRoom()
    {
        int number = PhotonNetwork.CountOfPlayersInRooms;//ルーム内人数

        //キャラクターを生成
        GameObject monster = PhotonNetwork.Instantiate("PlayerSample", new Vector3(-1.0f  + number,0,-5.0f), Quaternion.identity, 0);
        //自分だけが操作できるようにスクリプトを有効にする
        PlayerController monsterScript = monster.GetComponent<PlayerController>();
        monsterScript.enabled = true;
    }
}
