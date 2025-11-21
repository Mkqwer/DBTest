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
        Debug.Log("1. UIManager: 로그인 버튼 클릭 감지."); 
        
        string id = idInputField.text;
        string pwd = passwordInputField.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
        {
            UpdateStatusText("ID와 비밀번호를 모두 입력하세요.", false);
            return;
        }

        if (RankManager.Instance != null)
        {
            Debug.Log($"2. UIManager: 입력 값 유효함. ID: {id}"); 
            Debug.Log("3. UIManager: RankManager에게 DB 요청 전송."); 

            // DB 통신 요청
            RankManager.Instance.LoginUser(id, pwd);
            statusText.text = "로그인 요청 중...";
        }
        else
        {
            Debug.LogError("UIManager: [B] RankManager.Instance가 Null입니다. 싱글톤 오류.");
            UpdateStatusText("RankManager가 준비되지 않았습니다.", false);
        }
    }
    
    // 회원가입 버튼의 OnClick 이벤트에 연결할 함수
    public void OnRegisterButtonClicked()
    {
        Debug.Log("1. UIManager: 회원가입 버튼 클릭 감지."); 
        
        string id = idInputField.text;
        string pwd = passwordInputField.text;
        
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
        {
            UpdateStatusText("ID와 비밀번호를 모두 입력하세요.", false);
            return;
        }
        
        if (RankManager.Instance != null)
        {
            Debug.Log($"2. UIManager: 입력 값 유효함. ID: {id}"); 
            Debug.Log("3. UIManager: RankManager에게 DB 요청 전송."); 

            // DB 통신 요청
            RankManager.Instance.RegisterUser(id, pwd);
            statusText.text = "회원가입 요청 중...";
        }
        else
        {
             Debug.LogError("UIManager: [B] RankManager.Instance가 Null입니다. 싱글톤 오류.");
             UpdateStatusText("RankManager가 준비되지 않았습니다.", false);
        }
    }
    
    public void OnLoginSuccess()
    {
        Debug.Log("9. UIManager: 로그인 성공 콜백 수신. 씬 전환 준비 중."); 

        UpdateStatusText($"로그인 성공! ({RankManager.Instance.CurrentLoggedInUser})", true);
        
        // 게임 씬으로 이동
        SceneManager.LoadScene("LevelScene");
    }

    public void UpdateStatusText(string message, bool isSuccess)
    {
        statusText.text = message;
    }
}