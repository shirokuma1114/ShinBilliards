using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayer : MonoBehaviour, ITouche
{
    [SerializeField]
    private CPlayerMover _cPlayerMover = null;

    [SerializeField]
    private CStick _cCue = null;

    [SerializeField]
    private IGamePlayerInput _iGamePlayerInput = null;
    
    private CPlayerScore _score = null;
    public CPlayerScore Score
    {
        get { return _score; }
    }

    enum State
    {
        Idle,
        Damaged,
        Stop
    }

    State _state;

    void Awake()
    {
        _score = gameObject.AddComponent<CPlayerScore>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.Damaged || _state == State.Stop) return;


        _cPlayerMover.Move(_iGamePlayerInput.Move(), _iGamePlayerInput.DirMove());

        if(_iGamePlayerInput.Action())
        {
            if (_cCue != null)    _cCue.Attack();
        }
    }

    public void Damaged()
    {
        if (_state == State.Idle)
        {
            StartCoroutine(DamagedAnim());
        }
    }

    IEnumerator DamagedAnim()
    {
        _state = State.Damaged;

        float damagedTime = 1.0f;
        float blinkingInterval = 0.1f;

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        bool isVisual = true;   // 点滅用 falseで透明
        float startTime = Time.time;
        while(Time.time - startTime < damagedTime)
        {
            isVisual = !isVisual;
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.enabled = isVisual;
            }

            yield return new WaitForSeconds(blinkingInterval);
        }

        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.enabled = true;
        }

        _state = State.Idle;
    }

    public void TouchedEnter(GameObject other, Collider collider)
    {
        // スティックに触れられた
        CStick stick = other.GetComponent<CStick>();
        if (stick != null)
        {
            Damaged();
            Vector3 dir = transform.position - other.transform.position;
            dir.Normalize();
            GetComponent<Rigidbody>().AddForce(dir * 20.0f, ForceMode.Impulse); // 仮定数
        }
    }

    public void GetCue(CStick cue)
    {
        _cCue = cue;
        CGameManager.Instance.GetCue();
    }

    public void MoveStop()
    {
        _state = State.Stop;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void MoveStart()
    {
        _state = State.Idle;
    }

}
