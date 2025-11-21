using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; 

public class Move : MonoBehaviour
{

    float fMaxPositionX = 4.0f; 
    float fMinPositionX = -4.0f; 
    float fPositionX = 0.0f; 

    public Transform player; 
    public Text resultText; 
    private float maxHeight = 0f; 


    //Cat 오브젝트의 Rigidbody2D 컴포넌트를 갖는 멤버변수(m_)
    Rigidbody2D m_rigid2DCat = null;
    Animator m_animatorcat = null;
    float fjumpForce = 380.0f;
    float fwalkForce = 20.0f;
    float fmaxWalkSpeed = 2.0f;
    int nLeftRightKeyValue = 0;
    float fthreshold = 0.2f;

    void Start()
    {
        Application.targetFrameRate = 60;
        m_rigid2DCat = GetComponent<Rigidbody2D>();
        m_animatorcat = GetComponent<Animator>();
        
        // 최고 높이 측정 시작
        maxHeight = player.position.y;
        if (resultText != null)
        {
            resultText.text = ""; 
        }
    }

    void Update()
    {
        // 점프
        // 🚨 이전에 오류가 났던 linearVelocity 대신 velocity 사용
        if (Input.GetKey(KeyCode.Space) && m_rigid2DCat.velocity.y == 0) 
        {
            m_animatorcat.SetTrigger("JumpTrigger");
            m_rigid2DCat.AddForce(transform.up * fjumpForce);
        }

        // 좌우이동
        if (Input.GetKey(KeyCode.LeftShift)) { nLeftRightKeyValue = 0; }
        if (Input.GetKey(KeyCode.RightArrow)) { nLeftRightKeyValue = 1; }
        if (Input.GetKey(KeyCode.LeftArrow)) { nLeftRightKeyValue = -1; }
        
        // X좌표 값 제한
        fPositionX = Mathf.Clamp(transform.position.x, fMinPositionX, fMaxPositionX);
        transform.position = new Vector3(fPositionX, transform.position.y, transform.position.z);


        // 플레이어 스피드 및 스피드 제한 (velocity 사용)
        float speedx = Mathf.Abs(m_rigid2DCat.velocity.x);

        if (speedx < fmaxWalkSpeed)
        {
            m_rigid2DCat.AddForce(transform.right * fwalkForce * nLeftRightKeyValue);
        }

        // 움직이는 방향에 따라 반전 및 애니메이션 속도 설정
        if (nLeftRightKeyValue != 0)
        {
            transform.localScale = new Vector3(nLeftRightKeyValue, 1, 1);
        }
        if (m_rigid2DCat.velocity.y == 0)
        {
            m_animatorcat.speed = speedx / 2.0f;
        }
        else
        {
            m_animatorcat.speed = 1.0f;
        }

        if (player.position.y > maxHeight) // 최고 기록 갱신
        {
            maxHeight = player.position.y;
        }

        // 플레이어가 화면 밖으로 나갔다면 게임 오버
        if (transform.position.y < -10)
        {
            GameOver();
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("골");
        // GameOver(); // 골 지점에 닿았을 때도 게임 오버 처리 가능
    }
    
    private void GameOver()
    {
        // 이미 게임 오버 상태라면 중복 실행 방지
        if (enabled == false) return;
        
        enabled = false; // 현재 컴포넌트 비활성화
        
        // 2. 최고 높이 기록을 RankManager에 저장 요청
        if (RankManager.Instance != null && RankManager.Instance.IsLoggedIn) 
        {
            RankManager.Instance.SaveMaxHeight(maxHeight);
            RankManager.Instance.LoadRanking(); // 저장 후 랭킹 목록 불러오기
            
             if (resultText != null)
            {
                resultText.text = $"최종 높이: {maxHeight:F1}m\n랭킹 저장 완료!";
            }
        }
        else
        {
             if (resultText != null)
            {
                resultText.text = $"최종 높이: {maxHeight:F1}m\n로그인 상태가 아닙니다. 랭킹 저장을 건너뜁니다.";
            }
        }
        
        // 5. 3초 후 씬 재시작 (테스트용)
        StartCoroutine(RestartSceneAfterDelay(3.0f));
    }
    
    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("LevelScene");
    }
}