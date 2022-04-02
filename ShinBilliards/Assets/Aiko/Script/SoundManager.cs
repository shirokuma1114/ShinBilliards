using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シングルトン&DontDestroyOnLoad
// 注：全シーンに同じプレハブを置いておく
public class SoundManager : CSingletonMonoBehaviour<SoundManager>
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip audioClip;
    }

    // 登録用
    [SerializeField]
    private SoundData[] _setBGM;
    [SerializeField]
    private SoundData[] _setSE;

    // 検索用
    private Dictionary<string, SoundData> _bgm;
    private Dictionary<string, SoundData> _se;

    // AudioSource
    private AudioSource _asBGM;// BGM用のAudioSource
    private AudioSource _asSE;// SE単体プレイ用のAudioSource
    private Dictionary<string, AudioSource> _asSEs = new Dictionary<string, AudioSource>();    // SE再生用の個別AudioSource

    // 音量関係
    [SerializeField, Range(0, 1)]
    public float _maxVolume = 1; //最大音量
    [SerializeField]
    private float _fadeTime = 0.5f;     // フェードする時間
    private Coroutine _BGMFadeCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        // セットされているデータを検索用に辞書型に変換する
        // （以降_setBGMと_setSEは使用しない）
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

        // BGM用のAudioSourceをセット
        if(_asBGM == null)
        {
            _asBGM = gameObject.AddComponent<AudioSource>();
        }
        _asBGM.volume = _maxVolume;

        // SE用のAudioSourceをセット
        if (_asSE == null)
        {
            _asSE = gameObject.AddComponent<AudioSource>();
        }
        _asSE.volume = _maxVolume;

        DontDestroyOnLoad(gameObject);
    }

    // SEを再生
    public void PlaySE(string name, bool loop = false, float volumeScale = 1.0f)
    {
        if (!_se.TryGetValue(name, out var soundData)) //管理用Dictionary から探索
        {
            Debug.LogWarning($"SEは登録されていません:{name}");
            return;
        }

        if (loop)
        {
            if (!_asSEs.ContainsKey(name) || _asSEs[name] == null)
            {
                // AudioSourceを生成
                _asSEs[name] = gameObject.AddComponent<AudioSource>();

                _asSEs[name].clip = _se[name].audioClip;// clipにbgmを代入
                _asSEs[name].loop = loop;// ループするか
                _asSEs[name].volume = _maxVolume * volumeScale;
                _asSEs[name].Play();// 再生
            }
            else
            {// 既に生成済み
                _asSEs[name].Play();// 再再生
            }
        }
        else
        {
            if (_asSE != null)     // PlayOneShotする
            {
                _asSE.PlayOneShot(_se[name].audioClip, volumeScale);//再生
            }
        }

    }

    // ループするSEを止める
    public void StopSE(string name)
    {
        // 存在したら消す
        if (_asSEs.ContainsKey(name) && _asSEs[name] != null)
        {
            _asSEs[name].Stop();//停止
            Destroy(_asSEs[name]);
            _asSEs.Remove(name);
        }
    }

    // BGM再生
    public void PlayBGM(string name, bool loop = false)
    {
        if (!_bgm.TryGetValue(name, out var soundData)) //管理用Dictionary から探索
        {
            Debug.LogWarning($"BGMは登録されていません:{name}");
            return;
        }

        // 既にフェード中の場合は前のを中断する
        if (_BGMFadeCoroutine != null)
        {
            StopCoroutine(_BGMFadeCoroutine);
        }

        _BGMFadeCoroutine = StartCoroutine(BGMFade(_bgm[name], loop));
    }
    
    // BGM停止
    public void StopBGM()
    {
        // 既にフェード中の場合は前のを中断する
        if (_BGMFadeCoroutine != null)
        {
            StopCoroutine(_BGMFadeCoroutine);
        }

        _BGMFadeCoroutine = StartCoroutine(BGMFade(null, false));
    }

    IEnumerator BGMFade(SoundData nestSoundData, bool loop = false)
    {
        float fadeSpeed = _maxVolume / _fadeTime;

        // 元の音を消していく
        for (; _asBGM.volume > 0.0f; _asBGM.volume -= Time.deltaTime * fadeSpeed)
        {
            yield return null;
        }

        _asBGM.volume = 0.0f;
        _asBGM.Stop();

        // 次のBGMがない場合は終了
        if(nestSoundData == null)
        {
            _BGMFadeCoroutine = null;
            yield break;
        }

        _asBGM.clip = nestSoundData.audioClip; // 次の音
        _asBGM.loop = loop;//ループするか
        _asBGM.Play();//再生

        // 次の音を出していく
        for (; _asBGM.volume < _maxVolume; _asBGM.volume += Time.deltaTime * fadeSpeed)
        {
            yield return null;
        }

        _asBGM.volume = _maxVolume;

        _BGMFadeCoroutine = null;
    }
}