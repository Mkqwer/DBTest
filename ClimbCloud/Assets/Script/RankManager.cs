using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RankManager : MonoBehaviour
{
    private const string ConnectionString = 
        "Server=localhost;" +         // MySQL 서버 주소 외부서버 사용시 IP값 입력
        "Database=my_rank_game;" +    // 데이터베이스 이름
        "Uid=root;" +                 // MySQL 사용자 아이디
        "Pwd=Password;";             // MySQL 사용자 비밀번호

    // 현재 로그인된 유저의 닉네임을 저장
    public string CurrentLoggedInUser { get; private set; } 
    public bool IsLoggedIn => !string.IsNullOrEmpty(CurrentLoggedInUser);

    // 랭킹 정보를 저장할 구조체 (생략)
    public struct RankEntry 
    { 
        public int rank;
        public string userName; 
        public float score;
    }
    
    // --- 싱글톤 패턴 ---
    public static RankManager Instance;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }
    
    //회원가입 & 로그인

    public void RegisterUser(string userId, string password)
    {
        StartCoroutine(RegisterUserCoroutine(userId, password));
    }

    private IEnumerator RegisterUserCoroutine(string userId, string password)
    {
        // ... (로그 및 쿼리 생략) ...
        string query = 
            $"INSERT INTO user_accounts (user_id, password) VALUES ('{userId}', '{password}');";

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Debug.Log($"[회원가입] 성공: {userId}");
                
                // 회원가입 성공 메시지를 UIManager로 전달
                UIManager.Instance.UpdateStatusText("회원가입 성공! 로그인해주세요.", true);
            }
            catch (MySqlException ex)
            {
                // ... (에러 처리 및 UIManager 호출) ...
                if (ex.Number == 1062) 
                    UIManager.Instance.UpdateStatusText($"회원가입 실패: 이미 존재하는 닉네임입니다.", false);
                else
                    UIManager.Instance.UpdateStatusText($"DB 오류 발생: {ex.Message}", false);
            }
        }
        yield break;
    }

    public void LoginUser(string userId, string password)
    {
        StartCoroutine(LoginUserCoroutine(userId, password));
    }

    private IEnumerator LoginUserCoroutine(string userId, string password)
    {
        string query = 
            $"SELECT user_id FROM user_accounts WHERE user_id='{userId}' AND password='{password}';";

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows) 
                    {
                        CurrentLoggedInUser = userId; 
                        Debug.Log($"[로그인] 성공: {CurrentLoggedInUser}");
                        
                        // ⭐️ [해결책] 로그인 성공 시점에만 오브젝트를 유지하도록 호출!
                        DontDestroyOnLoad(gameObject);
                        
                        UIManager.Instance.OnLoginSuccess();
                    }
                    else
                    {
                        CurrentLoggedInUser = null;
                        UIManager.Instance.UpdateStatusText($"로그인 실패: 닉네임 또는 비밀번호 불일치.", false);
                    }
                }
            }
            catch (MySqlException ex)
            {
                UIManager.Instance.UpdateStatusText($"DB 오류 발생: {ex.Message}", false);
            }
        }
        yield break;
    }

    // =========================================================================
    // B. 랭킹 시스템 로직 (나머지 랭킹, 저장 로직은 유지)
    // =========================================================================
    
    public void SaveMaxHeight(float newHeight)
    {
        if (!IsLoggedIn)
        {
            Debug.LogError("[랭킹] 로그인 상태가 아닙니다. 점수 저장을 건너뜁니다.");
            return;
        }
        StartCoroutine(SaveScoreCoroutine(CurrentLoggedInUser, newHeight));
    }

    private IEnumerator SaveScoreCoroutine(string userName, float newHeight)
    {
        string query = 
            $"INSERT INTO ranking (user_name, score) VALUES ('{userName}', {newHeight:F1}) " +
            $"ON DUPLICATE KEY UPDATE score = GREATEST(score, {newHeight:F1});";

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Debug.Log($"[랭킹] 점수 저장/갱신 성공: {userName}, {newHeight:F1}m");
            }
            catch (MySqlException ex)
            {
                Debug.LogError($"[랭킹] 점수 저장 중 MySQL 에러: {ex.Message}");
            }
        }
        yield break;
    }
    
    public void LoadRanking()
    {
        StartCoroutine(LoadRankingCoroutine());
    }

    private IEnumerator LoadRankingCoroutine()
    {
        string query = "SELECT user_name, score FROM ranking ORDER BY score DESC LIMIT 10;";
        List<RankEntry> rankingList = new List<RankEntry>();

        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rankingList.Add(new RankEntry 
                        { 
                            rank = rankingList.Count + 1,
                            userName = reader.GetString("user_name"),
                            score = reader.GetFloat("score")
                        });
                    }
                }
                
                // DB 조회 성공 로그는 여기 (B)입니다.
                Debug.Log($"B. DB 조회 완료. {rankingList.Count}개 데이터 수신.");

                DisplayRankingUI(rankingList); 
            }
            catch (MySqlException ex)
            {
                Debug.LogError($"[랭킹] 조회 중 MySQL 에러: {ex.Message}");
            }
        }
        yield break;
    }

    private void DisplayRankingUI(List<RankManager.RankEntry> list)
    {
        RankUIController uiController = FindObjectOfType<RankUIController>();
        
        if (uiController != null)
        {
            Debug.Log("C. RankManager가 RankUIController 호출 성공.");
            uiController.DisplayRanking(list);
        }
        else
        {
            // ... (콘솔 출력 로직 유지) ...
            Debug.Log("--- RankUIController를 찾을 수 없습니다. ---");
            Debug.Log("--- 랭킹 데이터 (테스트 출력) ---");
            foreach (var entry in list)
            {
                Debug.Log($"#{entry.rank}: {entry.userName} ({entry.score:F1}m)");
            }
            Debug.Log("------------------------------------");
        }
    }
}