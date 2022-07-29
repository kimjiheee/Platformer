using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
            //GameManager : 점수와 스테이지 관리
{
    //점수와 스테이지 전역 변수 생성
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
        //스테이지 바꾸기
        if(stageIndex < Stages.Length-1)        //스테이지 갯수 확인해 다음 스테이지 이동/종료 구현
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "stage" + (stageIndex + 1);
        }
        else //게임 끝
        {
            Time.timeScale = 0;
            
            //restart button ui
            TextMeshProUGUI btnText = UIReBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "game clear";
            UIReBtn.SetActive(true);
        }

        //점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            player.PlaySound("DAMAGED");
            UIhealth[health].color = new Color(1, 1, 1, 0.2f); //알파값 줄임으로써 해당 이미지 색상 어둡게 변경
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
                PlayerReposition();  //플레이어 원위치로

            HealthDown();  //생명 깎음
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
