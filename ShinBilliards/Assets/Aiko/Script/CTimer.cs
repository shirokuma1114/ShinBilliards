using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class CTimer : MonoBehaviourPunCallbacks, IPunObservable
{

    private float _fStartTime = 0.0f;
    [SerializeField]
    private float _fTime = 0.0f;

    private bool _isStart = false;
    private bool _isFinish = false;
    private bool _isStop = false;

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

    private void Awake()
    {
        _fTime *= 1000; // m‚©‚çms‚Ö–ß‚·
        if (!_isStart)
        {
            _fStartTime = PhotonNetwork.ServerTimestamp;
        }
    }

    public void TimerStart()
    {
        if (!_isStart)
        {
            _fStartTime = PhotonNetwork.ServerTimestamp;
            _isStart = true;
        }
    }

    public void TimerStop()
    {
        _isStop = true;
    }

    private void FixedUpdate()
    {
        if (_isStop) return;

        float time = unchecked((_fStartTime + _fTime) - PhotonNetwork.ServerTimestamp);

        if (_isStart && !_isFinish)
        {
            if (time <= 0.0f)
            {
                time = 0.0f;
                _onFinish.Invoke();
                _isFinish = true;
            }
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