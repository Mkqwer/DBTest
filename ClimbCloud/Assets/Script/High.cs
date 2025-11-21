using UnityEngine;
using UnityEngine.UI;

public class High : MonoBehaviour
{
    public Transform player;
    public Text highText; 

    private float maxHeight = 0f; 

    void Update()
    {
        if (player.position.y > maxHeight)
        {
            maxHeight = player.position.y;
            highText.text = maxHeight.ToString("F1") + "m";
        }
    }
}
