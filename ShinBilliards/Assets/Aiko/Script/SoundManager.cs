using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �V���O���g��&DontDestroyOnLoad
// ���F�S�V�[���ɓ����v���n�u��u���Ă���
public class SoundManager : CSingletonMonoBehaviour<SoundManager>
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip audioClip;
    }

    // �o�^�p
    [SerializeField]
    private SoundData[] _setBGM;
    [SerializeField]
    private SoundData[] _setSE;

    // �����p
    private Dictionary<string, SoundData> _bgm;
    private Dictionary<string, SoundData> _se;

    // AudioSource
    private AudioSource _asBGM;// BGM�p��AudioSource
    private AudioSource _asSE;// SE�P�̃v���C�p��AudioSource
    private Dictionary<string, AudioSource> _asSEs = new Dictionary<string, AudioSource>();    // SE�Đ��p�̌�AudioSource

    // ���ʊ֌W
    [SerializeField, Range(0, 1)]
    public float _maxVolume = 1; //�ő剹��
    [SerializeField]
    private float _fadeTime = 0.5f;     // �t�F�[�h���鎞��
    private Coroutine _BGMFadeCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        // �Z�b�g����Ă���f�[�^�������p�Ɏ����^�ɕϊ�����
        // �i�ȍ~_setBGM��_setSE�͎g�p���Ȃ��j
        _bgm = new Dictionary<string, SoundData>();
        foreach(SoundData soundData in _setBGM)
        {
            _bgm.Add(soundData.name, soundData);
        }

        _se = new Dictionary<string, SoundData>();
        foreach (SoundData soundData in _setSE)
        {
            _se.Add(soundData.name, soundData);
        }

        // BGM�p��AudioSource���Z�b�g
        if(_asBGM == null)
        {
            _asBGM = gameObject.AddComponent<AudioSource>();
        }
        _asBGM.volume = _maxVolume;

        // SE�p��AudioSource���Z�b�g
        if (_asSE == null)
        {
            _asSE = gameObject.AddComponent<AudioSource>();
        }
        _asSE.volume = _maxVolume;

        DontDestroyOnLoad(gameObject);
    }

    // SE���Đ�
    public void PlaySE(string name, bool loop = false, float volumeScale = 1.0f)
    {
        if (!_se.TryGetValue(name, out var soundData)) //�Ǘ��pDictionary ����T��
        {
            Debug.LogWarning($"SE�͓o�^����Ă��܂���:{name}");
            return;
        }

        if (loop)
        {
            if (!_asSEs.ContainsKey(name) || _asSEs[name] == null)
            {
                // AudioSource�𐶐�
                _asSEs[name] = gameObject.AddComponent<AudioSource>();

                _asSEs[name].clip = _se[name].audioClip;// clip��bgm����
                _asSEs[name].loop = loop;// ���[�v���邩
                _asSEs[name].volume = _maxVolume * volumeScale;
                _asSEs[name].Play();// �Đ�
            }
            else
            {// ���ɐ����ς�
                _asSEs[name].Play();// �čĐ�
            }
        }
        else
        {
            if (_asSE != null)     // PlayOneShot����
            {
                _asSE.PlayOneShot(_se[name].audioClip, volumeScale);//�Đ�
            }
        }

    }

    // ���[�v����SE���~�߂�
    public void StopSE(string name)
    {
        // ���݂��������
        if (_asSEs.ContainsKey(name) && _asSEs[name] != null)
        {
            _asSEs[name].Stop();//��~
            Destroy(_asSEs[name]);
            _asSEs.Remove(name);
        }
    }

    // BGM�Đ�
    public void PlayBGM(string name, bool loop = false)
    {
        if (!_bgm.TryGetValue(name, out var soundData)) //�Ǘ��pDictionary ����T��
        {
            Debug.LogWarning($"BGM�͓o�^����Ă��܂���:{name}");
            return;
        }

        // ���Ƀt�F�[�h���̏ꍇ�͑O�̂𒆒f����
        if (_BGMFadeCoroutine != null)
        {
            StopCoroutine(_BGMFadeCoroutine);
        }

        _BGMFadeCoroutine = StartCoroutine(BGMFade(_bgm[name], loop));
    }
    
    // BGM��~
    public void StopBGM()
    {
        // ���Ƀt�F�[�h���̏ꍇ�͑O�̂𒆒f����
        if (_BGMFadeCoroutine != null)
        {
            StopCoroutine(_BGMFadeCoroutine);
        }

        _BGMFadeCoroutine = StartCoroutine(BGMFade(null, false));
    }

    IEnumerator BGMFade(SoundData nestSoundData, bool loop = false)
    {
        float fadeSpeed = _maxVolume / _fadeTime;

        // ���̉��������Ă���
        for (; _asBGM.volume > 0.0f; _asBGM.volume -= Time.deltaTime * fadeSpeed)
        {
            yield return null;
        }

        _asBGM.volume = 0.0f;
        _asBGM.Stop();

        // ����BGM���Ȃ��ꍇ�͏I��
        if(nestSoundData == null)
        {
            _BGMFadeCoroutine = null;
            yield break;
        }

        _asBGM.clip = nestSoundData.audioClip; // ���̉�
        _asBGM.loop = loop;//���[�v���邩
        _asBGM.Play();//�Đ�

        // ���̉����o���Ă���
        for (; _asBGM.volume < _maxVolume; _asBGM.volume += Time.deltaTime * fadeSpeed)
        {
            yield return null;
        }

        _asBGM.volume = _maxVolume;

        _BGMFadeCoroutine = null;
    }
}