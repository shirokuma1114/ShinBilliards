using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITouche
{
    void TouchedEnter(GameObject other, Collider collider);
}

public class CStick : MonoBehaviour
{
    private Collider _collider = null;
    private Vector3 _offset;

    enum State
    {
        Idle,
        Charge,
        Attack
    }

    State _state;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _state = State.Idle;
        _offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
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


    private void OnTriggerEnter(Collider ohter)
    {
        ITouche tocheObj = ohter.transform.GetComponent<ITouche>();

        if(tocheObj != null)
        {
            tocheObj.TouchedEnter(gameObject, ohter);
        }
    }

}
