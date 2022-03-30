using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject chara1, chara2;

    private bool winner = true;//true=>�����̏����Afalse=>����̏���

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
        //�Q�[���V�[���̃L�����N�^�[�v���n�u�𐶐�
        myCharacter = (GameObject)Resources.Load("ResultBoy");
        otherCharacter = (GameObject)Resources.Load("ResultGirl");
        myCharacter = Instantiate(myCharacter, new Vector3(3.5f, -2.5f, 0), Quaternion.Euler(0, 180, 0));
        otherCharacter = Instantiate(otherCharacter, new Vector3(-3.5f, -1.0f, 4), Quaternion.Euler(0, 180, 0));
        otherCharacter.transform.localScale = new Vector3(2, 2, 2);

        //�f�[�^�擾
        winner = true;// MasterData.Instance._myPlayerWin;
        playerName.text = "�Ђ��";// MasterData.Instance._myPlayerName;
        otherName.text = "������";// MasterData.Instance._otherPlayerName;

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
                //�^�C�g���V�[���J��

            }
        }
    }

    private IEnumerator Waiting()
    {
        //�h�������[���I�ȉ��������Ă��ǂ�
 
        //3�b��~
        yield return new WaitForSeconds(3);

        Result();
     }

    private void Result()
    {
        //�����̏���
        if (winner)
        {
            myCharacter.GetComponent<Animator>().SetBool("Win", true);
            otherCharacter.GetComponent<Animator>().SetBool("Lose", true);
            resultText.text = "Win!!";
            winLight.SetActive(true);
        }
        //����̏���
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
