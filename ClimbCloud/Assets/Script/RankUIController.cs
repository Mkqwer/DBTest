using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;

public class RankUIController : MonoBehaviour
{
    // 싱글톤 Instance는 유지하되, 파괴 로직은 제거합니다.
    public static RankUIController Instance;

    public Transform contentParent; 
    public GameObject rankItemPrefab; 

    void Awake()
    {
        Instance = this; 
        
    }
    
    void Start()
    {
        // 시작하자마자 랭킹 요청
        StartCoroutine(InitializeRanking());
    }

    private IEnumerator InitializeRanking()
    {
        // RankManager(DBManager)가 준비될 때까지 기다림
        while (RankManager.Instance == null)
        {
            yield return null; 
        }

        // 준비되면 바로 랭킹 데이터 요청
        Debug.Log("RankUISceneManager: 초기화 및 LoadRanking 요청.");
        RankManager.Instance.LoadRanking();
    }
    
    public void DisplayRanking(List<RankManager.RankEntry> rankingList)
    {
        Debug.Log($"D. 랭킹 항목 생성 시작. (개수: {rankingList.Count})");

        if (contentParent == null) return;

        // 기존 항목 청소
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        
        if (rankingList.Count == 0) return;

        // 항목 생성
        foreach (var entry in rankingList)
        {
            GameObject item = Instantiate(rankItemPrefab, contentParent);
            RankText rankItemScript = item.GetComponent<RankText>();

            if (rankItemScript != null)
            {
                rankItemScript.SetData(entry.rank, entry.userName, entry.score);
            }
        }
    }
}