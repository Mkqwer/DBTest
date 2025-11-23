using UnityEngine;
using UnityEngine.UI; // Legacy InputField와 Text 사용
using UnityEngine.SceneManagement; 
using System.Collections; 

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public InputField idInputField;
    public InputField passwordInputField;
    public Text statusText; // Legacy Text 컴포넌트

    public void OnLoginButtonClicked()
    {
        
        string id = idInputField.text;
        string pwd = passwordInputField.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
        {
            UpdateStatusText("ID와 비밀번호를 모두 입력하세요.", false);
            return;
        }

        if (RankManager.Instance != null)
        {

            // DB 통신 요청
            RankManager.Instance.LoginUser(id, pwd);
            statusText.text = "로그인 요청 중...";
        }
        else
        {
            UpdateStatusText("RankManager가 준비되지 않았습니다.", false);
        }
    }
    
    // 회원가입 버튼의 OnClick 이벤트에 연결할 함수
    public void OnRegisterButtonClicked()
    {
        
        string id = idInputField.text;
        string pwd = passwordInputField.text;
        
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
        {
            UpdateStatusText("ID와 비밀번호를 모두 입력하세요.", false);
            return;
        }
        
        if (RankManager.Instance != null)
        {
            // DB 통신 요청
            RankManager.Instance.RegisterUser(id, pwd);
            statusText.text = "회원가입 요청 중...";

            statusText.text = "회원가입 성공!";
        }
        else
        {
             UpdateStatusText("RankManager가 준비되지 않았습니다.", false);
        }
    }
    
    public void OnLoginSuccess()
    {

        UpdateStatusText($"로그인 성공! ({RankManager.Instance.CurrentLoggedInUser})", true);
        
        // 게임 씬으로 이동
        SceneManager.LoadScene("LevelScene");
    }

    public void UpdateStatusText(string message, bool isSuccess)
    {
        statusText.text = message;
    }
}