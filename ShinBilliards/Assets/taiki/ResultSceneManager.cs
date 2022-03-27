using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject chara1, chara2;

    private bool winner = true;//true=>自分の勝利、false=>相手の勝利

    private GameObject myCharacter;
    private GameObject otherCharacter;

    [SerializeField] private GameObject light;

    private void Awake()
    {
        //ゲームシーンのキャラクタープレハブを生成
        myCharacter = Instantiate(chara1, new Vector3(3.5f,-2.5f,0), Quaternion.Euler(0, 180, 0));
        otherCharacter = Instantiate(chara2, new Vector3(-3.5f,-2.5f,0), Quaternion.Euler(0, 180, 0));

        light.SetActive(false);

    }

    void Start()
    {
        StartCoroutine(Waiting());
    }

    void Update()
    {
        
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
        }
        //相手の勝利
        else
        {
            myCharacter.GetComponent<Animator>().SetBool("Lose", true);
            otherCharacter.GetComponent<Animator>().SetBool("Win", true);
        }

        light.SetActive(true);
    }
}
