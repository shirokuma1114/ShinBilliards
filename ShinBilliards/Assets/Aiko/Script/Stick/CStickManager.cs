
using System.Linq;
using UnityEngine;

public class CStickManager : CSingletonMonoBehaviour<CStickManager>
{
    [SerializeField] private CStick _cuePrefab = null;
    [SerializeField] private bool _isCollectGeneratingPoints = true;
    [SerializeField] private Transform[] _generatingPoints = null;

    private CStick _cue = null;


    public void Init()
    {
        // �����|�C���g���W(_generatingPoints�̌��̓��e�͎g�p���Ȃ�)
        if(_isCollectGeneratingPoints)
        {
            Transform pointsParent = transform.Find("GeneratingPoints");
            if (pointsParent == null) return;
            
            _generatingPoints = 
                pointsParent.GetComponentsInChildren<Transform>()
                .Where(c => pointsParent.gameObject != c.gameObject)
                .ToArray();

        }

        CreateCue(false);
    }


    // �L���[���쐬
    // �����@force : �����I�ɍ쐬���邩(�����̂�������)
    public void CreateCue(bool force)
    {

        if (_cuePrefab == null)
        {
            Debug.LogError("�L���[�v���n�u����");
            return;
        }
        if (_generatingPoints == null || _generatingPoints.Length <= 0)
        {
            Debug.LogError("�L���[�����ʒu�w��Ȃ�");
            return;
        }

        if (_cue != null)
        {
            if (!force) return;     // �L���[���܂����݂���ꍇ�V�����쐬���Ȃ�

            Destroy(_cue.gameObject);        // �����쐬�̏ꍇ�ȑO�̂��̂�j������
            _cue = null;
        }

        // �����_���ʒu�ɐV�����z�u
        Transform point = _generatingPoints[Random.Range(0, _generatingPoints.Length)];
        _cue = Instantiate(_cuePrefab, point.position, point.rotation);
        _cue.Init(this);

    }

    
    // �L���[��Destroy���ꂽ�Ƃ��ɌĂяo��
    public void OnDestroyCue(CStick cue)
    {
        if(_cue == cue)
        {
            _cue = null;
        }
    }

}
