using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCountDownUI : MonoBehaviour
{
    [SerializeField] private Text _countDownText;
    private CTimer _timer;
    private int _time = -1;

    public void Register(CTimer timer)
    {
        _timer = timer;
        _timer._onChanged.AddListener(UpdateText);
    }

    void UpdateText()
    {
        int time = (int)(_timer.CurrentTime / 1000);

        if(time != _time)
        {
            _time = time;
            _countDownText.text = time.ToString();

            if(time < 10)
            {
                _countDownText.color = Color.red;
            }
        }
    }
}
