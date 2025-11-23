using UnityEngine;
using UnityEngine.UI;

public class RankText : MonoBehaviour
{
    public Text rankText;     // 순위 표시
    public Text nameText;     // 닉네임 표시
    public Text scoreText;    // 점수 표시

    // 랭크 데이터를 받아 UI를 업데이트하는 함수
    public void SetData(int rank, string userName, float score)
    {
        rankText.text = $"#{rank}";
        nameText.text = userName;
        scoreText.text = $"{score:F1} m";
    }
}