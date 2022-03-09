using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ‰¼
public class CScoreUI : CSingletonMonoBehaviour<CScoreUI>
{
    [SerializeField] private Text _scoreTextRight;
    [SerializeField] private Text _scoreTextLeft;

    private CPlayerScore[] _playerScores = new CPlayerScore[2];
    
    void Start()
    {
        // ƒIƒtƒ‰ƒCƒ“
        
        CPlayer[] players = FindObjectsOfType<CPlayer>();
        if(players.Length >= 2)
        {
            _playerScores[0] = players[0].Score;
            _playerScores[1] = players[1].Score;
            _playerScores[0]._onChanged.AddListener(UpdateScoreTextRight);
            _playerScores[1]._onChanged.AddListener(UpdateScoreTextLeft);
        }
        
    }

    public void RegisterRight(CPlayerScore score)
    {
        _playerScores[0] = score;
        _playerScores[0]._onChanged.AddListener(UpdateScoreTextRight);
    }

    public void RegisterLeft(CPlayerScore score)
    {
        _playerScores[1] = score;
        _playerScores[1]._onChanged.AddListener(UpdateScoreTextLeft);
    }

    void UpdateScoreTextRight()
    {
        _scoreTextRight.text = _playerScores[0].Score.ToString();
    }

    void UpdateScoreTextLeft()
    {
        _scoreTextLeft.text = _playerScores[1].Score.ToString();
    }

}
