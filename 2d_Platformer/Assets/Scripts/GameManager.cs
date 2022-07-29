using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
            //GameManager : ������ �������� ����
{
    //������ �������� ���� ���� ����
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;

    public PlayerMove player;
    public GameObject[] Stages;

    //UI
    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject UIReBtn;

     void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //�������� �ٲٱ�
        if(stageIndex < Stages.Length-1)        //�������� ���� Ȯ���� ���� �������� �̵�/���� ����
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "stage" + (stageIndex + 1);
        }
        else //���� ��
        {
            Time.timeScale = 0;
            
            //restart button ui
            TextMeshProUGUI btnText = UIReBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "game clear";
            UIReBtn.SetActive(true);
        }

        //���� ���
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            player.PlaySound("DAMAGED");
            UIhealth[health].color = new Color(1, 1, 1, 0.2f); //���İ� �������ν� �ش� �̹��� ���� ��Ӱ� ����
        }
            
        else 
        {
            Invoke("Deathstop", 1.1f);
            UIhealth[0].color = new Color(1, 1, 1, 0.3f);
            player.OnDie();

            Debug.Log("die...");

            UIReBtn.SetActive(true);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   
            if(health>1) 
                PlayerReposition();  //�÷��̾� ����ġ��

            HealthDown();  //���� ����
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4, 1, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void Deathstop()
    {
        Time.timeScale = 0;
    }
}
