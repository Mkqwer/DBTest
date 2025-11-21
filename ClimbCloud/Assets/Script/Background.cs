using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform[] backgrounds; 
        public float backgroundHeight;  

    public Transform player;

    void Update()
    {
        foreach (Transform bg in backgrounds)  // 배경 오브젝트 반복
        {
            if (player.position.y - bg.position.y > backgroundHeight) // 플레이어가 배경보다 1단계 높이 올라갔을 때
            {
                // 배경 오브젝트를 위로 이동시켜 backgrounds.Length만큼 반복해서 배경을 이어붙인다.
                bg.position += new Vector3(0, backgroundHeight * backgrounds.Length, 0); 
            }
        }
    }
}
