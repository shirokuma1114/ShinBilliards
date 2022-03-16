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

    private CPlayer _touchPlayer = null;
    public CPlayer GetHavePlayer()
    {
        return _touchPlayer;
    }

    enum State
    {
        Placed,
        Idle,
        Charge,
        Attack
    }

    State _state;
    

    void Start()
    {
        _collider = GetComponent<Collider>();
        _state = State.Placed;
    }


    public void Init(CStickManager mgr)
    {
        _mgr = mgr;
    }


    public void Attack()
    {
        if(_state == State.Idle)
        {
            StartCoroutine(AttackAnim());
        }
    }


    IEnumerator AttackAnim()
    {
        _state = State.Charge;
        
        for(float time = 0.0f; time < 0.25f; time += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(_offset, _offset - Vector3.forward, time * 4.0f);
            yield return null;
        }

        _state = State.Attack;
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

    public void Picked(CPlayer player)
    {
        ChangeOwner();
        _touchPlayer = player;
        transform.parent = player.transform;
        _collider.enabled = false;
        _state = State.Idle;
        _offset = new Vector3(0.64f, 0.25f, 1.28f);
        transform.localPosition = _offset;
        transform.localRotation = Quaternion.Euler(-81.7f, 0.0f, 0.0f);
        player.GetCue(this);
        Debug.Log("Picked");
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
