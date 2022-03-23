using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterData : CSingletonMonoBehaviour<MasterData>
{

    public string _myPlayerPrefabName = "";
    public string _otherPlayerPrefabName = "";
    public string _myPlayerName = "";
    public string _otherPlayerName = "";
    public int _myPlayerScore = 0;
    public int _otherPlayerScore = 0;
    public bool _myPlayerWin = false;
    

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

}
