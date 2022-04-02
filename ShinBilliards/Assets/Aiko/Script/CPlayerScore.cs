using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class CPlayerScore : MonoBehaviourPunCallbacks, IPunObservable
{
    public UnityEvent _onChanged = new UnityEvent();

    private int _score = 0;
    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;

            _onChanged.Invoke();
        }
    }

    private void Start()
    {
        
        if (photonView.IsMine)
        {
            CScoreUI.Instance.RegisterRight(this, photonView.Owner.NickName);
        }
        else
        {
            CScoreUI.Instance.RegisterLeft(this, photonView.Owner.NickName);
        }
        
    }
    
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Score);
        }
        else
        {
            Score = (int)stream.ReceiveNext();
        }
    }
    

}
