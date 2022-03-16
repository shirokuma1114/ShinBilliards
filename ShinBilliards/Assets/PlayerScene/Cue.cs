using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cue : MonoBehaviourPunCallbacks
{
    private bool isUse = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CueUse()
    {
        isUse = true;
    }

    public bool GetCueIsUse => isUse;

    public void CueRelease()
    {
        isUse = false;
    }

    public void ChangeOwner()
    {
        photonView.RequestOwnership();
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

    public void Charge(GameObject player, float power)
    {
        transform.position = player.transform.position + (power * -(player.transform.rotation * Vector3.forward));
        //transform.localPosition = new Vector3(0.5f, 1, 0) + (power * (player.transform.rotation * new Vector3(-1, 0, 0)));
    }

    public void Hit(GameObject player)
    {
        transform.position = player.transform.position;

    }
}
