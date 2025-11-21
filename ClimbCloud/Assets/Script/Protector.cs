using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protector : MonoBehaviour
{
  // 랭킹 UI 구조 전체를 관리하는 싱글톤 인스턴스
    public static Protector Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        StartCoroutine(InitializeRanking());
    }

    private System.Collections.IEnumerator InitializeRanking()
    {
        while (RankManager.Instance == null)
        {
            yield return null; 
        }

        if (RankManager.Instance != null)
        {
            Debug.Log("RankUISceneManager: 초기화 성공 및 LoadRanking 요청.");
            RankManager.Instance.LoadRanking();
        }
    }
}
