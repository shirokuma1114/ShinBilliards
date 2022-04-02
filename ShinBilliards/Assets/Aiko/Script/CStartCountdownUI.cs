using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CStartCountdownUI : MonoBehaviour
{
    [SerializeField] private Text _countDownText;
    [SerializeField] private CTimer _timer;
    private int _time = -1;

    public void Register(CTimer timer)
    {
        _timer = timer;
        _timer._onChanged.AddListener(UpdateText);
    }

    public void UpdateText()
    {
        int time = (int)(_timer.CurrentTime / 1000);

        if (time != _time)
        {
            _time = time;
           
            _countDownText.text = (time + 1).ToString();
        }

        if (_timer.CurrentTime / 1000 <= 0.0f)
        {
            _countDownText.text = "Start!";
        }
    }
}
