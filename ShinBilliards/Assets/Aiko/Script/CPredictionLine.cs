using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPredictionLine : MonoBehaviour
{

    [SerializeField]
    private Transform _spherePref = null;
    
    [SerializeField]
    private int _maxSphereCount;
    
    private List<Transform> _sphereList = new List<Transform>();

    private bool _show = false;

    private bool active = true;

    [SerializeField]
    private LayerMask _hitLayerMask = 0;

    void Start()
    {
        //ó\ë™ì_Çê∂ê¨
        for (int i = 0; i < _maxSphereCount; i++)
        {
            Transform obj = Instantiate(_spherePref, transform);
            _sphereList.Add(obj);
        }
    }

    void Update()
    {
        if (active && !_show)
        {
            foreach (Transform obj in _sphereList)
            {
                obj.gameObject.SetActive(false);
            }
            active = false;
        }

        _show = false;
    }

    public void Show(Vector3 dir)
    {
        _show = true;

        if(!active)
        {
            foreach (Transform obj in _sphereList)
            {
                obj.gameObject.SetActive(true);
            }
            active = true;
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position, dir * 10, Color.green);
        if (Physics.SphereCast(transform.position, 0.15f, dir, out hit, 10))
        {
            int hitNum = Mathf.Min((int)(hit.distance * 4), _maxSphereCount);
            // îΩéÀëOÇÃó\ë™ê¸
            for (int i = 0; i < hitNum; i++)
            {
                Vector3 pos = transform.position + dir * (i + 1) * 0.25f;
                _sphereList[i].position = pos;
            }

            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ball"))
            {//à»ç~ÇÃó\ë™ê¸Ç»Çµ
                for (int i = hitNum; i < _maxSphereCount; i++)
                {
                    Vector3 pos = new Vector3(100.0f, -100.0f, 100.0f);     // âìÇ≠Ç…îÚÇŒÇµÇ∆Ç≠
                    _sphereList[i].position = pos;
                }
                return;
            }

            // îΩéÀå„ÇÃó\ë™ê¸
            Vector3 refrectVec = Vector3.Reflect(dir, hit.normal);//îΩéÀÉxÉNÉgÉãåvéZ
            refrectVec.y = 0.0f;
            refrectVec.Normalize();
            Debug.DrawRay(hit.point, refrectVec * (10 - hit.distance), Color.yellow);
            RaycastHit nextHit;
            if (Physics.SphereCast(hit.point, 0.15f, refrectVec, out nextHit, 10 - hit.distance))
            {
                int nextHitNum = Mathf.Min((int)(nextHit.distance * 4) + hitNum, _maxSphereCount);
                for (int i = hitNum; i < nextHitNum; i++)
                {
                    Vector3 pos = hit.point + refrectVec * ((i - hitNum) + 1) * 0.25f;
                    _sphereList[i].position = pos;
                }
                for (int i = nextHitNum; i < _maxSphereCount; i++)
                {
                    Vector3 pos = new Vector3(100.0f, -100.0f, 100.0f);     // âìÇ≠Ç…îÚÇŒÇµÇ∆Ç≠
                    _sphereList[i].position = pos;
                }
            }
            else
            {
                for (int i = hitNum; i < _maxSphereCount; i++)
                {
                    Vector3 pos = hit.point + refrectVec * ((i - hitNum) + 1) * 0.25f;
                    _sphereList[i].position = pos;
                }
            }

        }
        else
        {
            for (int i = 0; i < _maxSphereCount; i++)
            {
                Vector3 pos = dir * (i + 1) * 0.25f;
                _sphereList[i].localPosition = pos;
            }
        }
    }
    public void Hide()
    {
        if(_show)
        {
            foreach(Transform obj in _sphereList)
            {
                obj.gameObject.SetActive(false);
            }
            _show = false;
        }
    }
}