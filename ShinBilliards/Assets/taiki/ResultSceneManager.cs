using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject chara1, chara2;

    private bool winner = true;//true=>�����̏����Afalse=>����̏���

    private GameObject myCharacter;
    private GameObject otherCharacter;

    [SerializeField] private GameObject light;

    private void Awake()
    {
        //�Q�[���V�[���̃L�����N�^�[�v���n�u�𐶐�
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
        }
        //����̏���
        else
        {
            myCharacter.GetComponent<Animator>().SetBool("Lose", true);
            otherCharacter.GetComponent<Animator>().SetBool("Win", true);
        }

        light.SetActive(true);
    }
}
