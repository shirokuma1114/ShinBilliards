using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    Image gaugeCtrl;
    [SerializeField]
    Color color_1;
    [SerializeField]
    Color color_2;
    [SerializeField]
    Color color_3;
    [SerializeField]
    float num=0;

    void Awake()
    {
        gaugeCtrl = GetComponent<Image>();
        gaugeCtrl.fillAmount = 0f;
    }

    void Update()
    {
    }

    public void SetCharge(float charge,float max)
    {
        gaugeCtrl.fillAmount = charge / max;
        SetColor(charge / max);
    }

    private void SetColor(float comNum)
    {
        if(comNum<0.5f)
        gaugeCtrl.color = color_1 * (1f - comNum*2f) + color_2 * comNum*2f;
        else
            gaugeCtrl.color = color_2 * (1f - (comNum-0.5f)*2f) + color_3 * (comNum - 0.5f) * 2f;
    }
}
