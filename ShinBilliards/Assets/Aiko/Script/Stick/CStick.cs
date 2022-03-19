using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface ITouche
{
    void TouchedEnter(GameObject other, Collider collider);
}

public class CStick : MonoBehaviourPunCallbacks
{
    private CStickManager _mgr = null;

    private Collider _collider = null;
    private Vector3 _offset;

    private float _currentCharge = 0.0f;
    

    enum State
    {
        Placed,
        Idle,
        Charge,
        Shot
    }

    State _state;

    public bool GetCueIsUse
    {
        get { return _state == State.Placed; }
    }


    void Start()
    {
        _collider = GetComponent<Collider>();
        _state = State.Placed;
    }


    public void Init(CStickManager mgr)
    {
        _mgr = mgr;
    }


    public void Shot()
    {
        if(_state == State.Idle)
        {
            StartCoroutine(ShotAnim());
        }
    }


    IEnumerator ShotAnim()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
        _state = State.Charge;
        
        for(float time = 0.0f; time < 0.25f; time += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(_offset, _offset - Vector3.forward, time * 4.0f);
            yield return null;
        }

        _state = State.Shot;
        _collider.enabled = true;

        for (float time = 0.0f; time < 0.1f; time += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(_offset - Vector3.forward, _offset, time * 10.0f);
            yield return null;
        }

        _collider.enabled = false;
        _state = State.Idle;
    }


    private void OnTriggerEnter(Collider other)
    {

        // E‚¤Žž‚Ìˆ—
        if(_state == State.Placed)
        {
            /*
            CPlayer touchPlayer = other.transform.GetComponent<CPlayer>();
            if(touchPlayer != null)
            {
                _touchPlayer = touchPlayer;
                transform.parent = touchPlayer.transform;
                _collider.enabled = false;
                _state = State.Idle;
                _offset = new Vector3(0.64f, 0.25f, 1.28f);
                transform.localPosition = _offset;
                transform.localRotation = Quaternion.Euler(-81.7f, 0.0f, 0.0f);
                touchPlayer.GetCue(this);
            }
            */
            if (other.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                CGameManager.Instance.GetCue();
            }
            Debug.Log("Touch");
            return;
        }
        
        if (!photonView.AmOwner) return;

        ITouche tocheObj = other.transform.GetComponent<ITouche>();

        if(tocheObj != null)
        {
            tocheObj.TouchedEnter(gameObject, other);
        }
    }

    public void SetTransform(GameObject parentObj)
    {
        this.gameObject.transform.parent = parentObj.transform;
        //this.gameObject.transform.localPosition = new Vector3(0.5f, 1, 0);// + parentObj.transform.position;
        this.gameObject.transform.localRotation = Quaternion.Euler(0, 90, 90);// * Quaternion.Euler(parentObj.transform.forward);
    }

    public void ResetTransform()
    {
        this.gameObject.transform.parent = null;

        this.gameObject.transform.localPosition += new Vector3(-0.1f, 0, 0);
        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Charge(GameObject player, float late)
    {
        if (_state == State.Placed) return;
        transform.localPosition = Vector3.Lerp(_offset, _offset - Vector3.forward, late);
        //transform.position = player.transform.position + (power * -(player.transform.rotation * Vector3.forward));
        //transform.localPosition = new Vector3(0.5f, 1, 0) + (power * (player.transform.rotation * new Vector3(-1, 0, 0)));
    }

    public void Hit(GameObject player, float currentCharge)
    {
        if (_state == State.Placed) return;

        _currentCharge = currentCharge;

        _state = State.Shot;
        if (_collider == null) _collider = GetComponent<Collider>();
        _collider.enabled = true;
        transform.localPosition = _offset;

        Invoke("FinishAttack", 0.1f);

    }

    private void FinishAttack()
    {
        _state = State.Idle;
        //_collider.enabled = false;
    }

    public void CueUse(PlayerController player)
    {
        ChangeOwner();
        _state = State.Idle;

        // ˆÊ’uˆÚ“®
        transform.parent = player.transform;
        _offset = new Vector3(0.64f, 0.75f, 1.28f);
        transform.localPosition = _offset;
        transform.localRotation = Quaternion.Euler(-81.7f, 0.0f, 0.0f);
        if (_collider == null) _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void CueRelease()
    {
        _state = State.Placed;
        transform.parent = null;

        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
        transform.rotation = Quaternion.Euler(-81.702f, 0, 0);
        if (_collider == null) _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }

    public void Destroy()
    {
        if (!photonView.IsMine) return;

        PhotonNetwork.Destroy(gameObject);
    }

    public void ChangeOwner()
    {
        photonView.RequestOwnership();
    }

}
