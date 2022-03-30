using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class CTimer : MonoBehaviourPunCallbacks, IPunObservable
{

    private float _fStartTime = 0.0f;

    [SerializeField]
    private float _fGameTime = 120000.0f;
    private bool _isFinish = false;

    public UnityEvent _onChanged = new UnityEvent();
    public UnityEvent _onFinish = new UnityEvent();
    private float _currentTime = 0;
    public float CurrentTime
    {
        get
        {
            return _currentTime;
        }
        set
        {
            _currentTime = value;

            _onChanged.Invoke();
        }
    }

    private void Start()
    {
        _fStartTime = PhotonNetwork.ServerTimestamp;

    }

    private void Update()
    {
        if (_isFinish) return;

        float time = unchecked((_fStartTime + _fGameTime) - PhotonNetwork.ServerTimestamp);

        if (time <= 0.0f)
        {
            time = 0.0f;
            _onFinish.Invoke();
            _isFinish = true;
        }
        CurrentTime = time;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_fStartTime);
        }
        else
        {
            _fStartTime = (float)stream.ReceiveNext();
        }
    }


}