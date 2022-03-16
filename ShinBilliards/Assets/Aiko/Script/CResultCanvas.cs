using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CResultCanvas : MonoBehaviour
{
    [SerializeField]
    Text _text = null;

    public void SetWinLose(bool win)
    {
        if (win)
        {
            _text.text = "You Win!";
            _text.color = Color.yellow;
        }
        else
        {
            _text.text = "You Lose...";
            _text.color = Color.blue;
        }

        _text.gameObject.SetActive(true);
    }
}
