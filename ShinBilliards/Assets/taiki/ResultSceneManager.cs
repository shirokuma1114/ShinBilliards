using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject chara1, chara2;

    private bool winner = true;//true=>自分の勝利、false=>相手の勝利

    private GameObject myCharacter;
    private GameObject otherCharacter;

    [SerializeField] private GameObject winLight;
    [SerializeField] private GameObject loseLight;

    [SerializeField] private Text resultText;

    [SerializeField] private Text playerName;
    [SerializeField] private Text playerScore;

    [SerializeField] private Text otherName;
    [SerializeField] private Text otherScore;

    private bool inputEnable = false;   

    private void Awake()
    {
        //ゲームシーンのキャラクタープレハブを生成
        myCharacter = (GameObject)Resources.Load("ResultBoy");
        otherCharacter = (GameObject)Resources.Load("ResultGirl");
        myCharacter = Instantiate(myCharacter, new Vector3(3.5f, -2.5f, 0), Quaternion.Euler(0, 180, 0));
        otherCharacter = Instantiate(otherCharacter, new Vector3(-3.5f, -1.0f, 4), Quaternion.Euler(0, 180, 0));
        otherCharacter.transform.localScale = new Vector3(2, 2, 2);

        //データ取得
        winner = true;// MasterData.Instance._myPlayerWin;
        playerName.text = "ひろむ";// MasterData.Instance._myPlayerName;
        otherName.text = "たいき";// MasterData.Instance._otherPlayerName;

        playerScore.text = "90";// MasterData.Instance._myPlayerScore.ToString();
        otherScore.text = "9";// MasterData.Instance._otherPlayerScore.ToString();


        winLight.SetActive(false);
        loseLight.SetActive(false);
        resultText.enabled = false;
        playerScore.enabled = false;
        otherScore.enabled = false;
    }

    void Start()
    {
        StartCoroutine(Waiting());
    }

    void Update()
    {
        if(inputEnable)
        {
            if(Input.GetMouseButtonDown(0))
            {
                //タイトルシーン遷移

            }
        }
    }

    private IEnumerator Waiting()
    {
        //ドラムロール的な音があっても良い
 
        //3秒停止
        yield return new WaitForSeconds(3);

        Result();
     }

    private void Result()
    {
        //自分の勝利
        if (winner)
        {
            myCharacter.GetComponent<Animator>().SetBool("Win", true);
            otherCharacter.GetComponent<Animator>().SetBool("Lose", true);
            resultText.text = "Win!!";
            winLight.SetActive(true);
        }
        //相手の勝利
        else
        {
            myCharacter.GetComponent<Animator>().SetBool("Lose", true);
            otherCharacter.GetComponent<Animator>().SetBool("Win", true);
            resultText.text = "Lose...";
            loseLight.SetActive(true);
        }

        resultText.enabled = true;
        playerScore.enabled = true;
        otherScore.enabled = true;

        inputEnable = true;
    }
}
