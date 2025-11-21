
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Classic : MonoBehaviour
{

    float fMaxPositionX = 4.0f; //플레이어가 좌, 우 이동시 게임 창을 벗어나지 않도록 Vector 최대값을 설정 변수
    float fMinPositionX = -4.0f; //플레이어가 좌, 우 이동시 게임 창을 벗어나지 않도록 Vector 최솟값을 설정 변수
    float fPositionX = 0.0f; //플레이어가 좌, 우 이동할 수 있는 X 좌표 저장 변수

    public Transform player; // 최대 높이를 체크하기위한 플레이어 오브젝트 설정
    public TextMeshProUGUI resultText; // 게임오버시 최종높이를 보여주는 텍스트 오브젝트
    private float maxHeight = 0f; // 최고 높이저장하는 변수


    //Cat 오브젝트의 Rigidbody2D 컴포넌트를 갖는 멤버변수(m_)
    Rigidbody2D m_rigid2DCat = null;
    Animator m_animatorcat = null;
    //플레이어에 가할 힘 값을 저장할 변수
    float fjumpForce = 380.0f;
    //플레이어 좌, 우로 움직이는 가속도
    float fwalkForce = 20.0f;
    //플레이어의 이동속도가 지정한 최고 속도
    float fmaxWalkSpeed = 2.0f;
    //플레이어 좌우 움직임 키 값: 오른쪽 화살 키 -> 1,왼쪽 화살 키 -> 1, 움직이지 않을 때 -> 0
    int nLeftRightKeyValue = 0;
    //
    float fthreshold = 0.2f;
    void Start()
    {
        Application.targetFrameRate = 60;
        m_rigid2DCat = GetComponent<Rigidbody2D>();
        m_animatorcat = GetComponent<Animator>();
    }

    void Update()
    {
        // 점프

        if (Input.GetKey(KeyCode.Space) && m_rigid2DCat.velocity.y == 0)
        {
            m_animatorcat.SetTrigger("JumpTrigger");
            m_rigid2DCat.AddForce(transform.up * fjumpForce);
        }

        // 좌우이동
        // 플레이어를 멈추게하는 코드
        if (Input.GetKey(KeyCode.LeftShift))
        {
            nLeftRightKeyValue = 0;
        }
        // 플레이어를 오른쪽으로 이동시키는 코드
        if (Input.GetKey(KeyCode.RightArrow))
        {
            nLeftRightKeyValue = 1;
        }
        // 플레이어를 왼쪽으로 이동시키는 코드
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            nLeftRightKeyValue = -1;
        }
        // X좌표 값을 일정 이상 벗어나지 않게하는 코드
        fPositionX = Mathf.Clamp(transform.position.x, fMinPositionX, fMaxPositionX);
        transform.position = new Vector3(fPositionX, transform.position.y, transform.position.z);


        // 플레이어 스피드
        float speedx = Mathf.Abs(m_rigid2DCat.velocity.x);

        // 스피드 제한
        if (speedx < fmaxWalkSpeed)
        {
            m_rigid2DCat.AddForce(transform.right * fwalkForce * nLeftRightKeyValue);
        }

        // 움직이는 방향에 따라 반전한다.
        if (nLeftRightKeyValue != 0)
        {
            transform.localScale = new Vector3(nLeftRightKeyValue, 1, 1);
        }
        // 플레이어 속도에 맞춰 애니메이션 속도를 바꾼다.
        if (m_rigid2DCat.velocity.y == 0)
        {
            m_animatorcat.speed = speedx / 2.0f;
        }
        else
        {
            m_animatorcat.speed = 1.0f;
        }

        if (player.position.y > maxHeight) // 플레이어가 지난번 최고 높이보다 높이 올라가면 최고기록 갱신
        {
            maxHeight = player.position.y;
        }

        // 플레이어가 화면 밖으로 나갔다면 처음부터
        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("LevelScene");
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("골");
    }
}