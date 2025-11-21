using UnityEngine;
using UnityEngine.SceneManagement;

public class FlagClear : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("ClearScene");
    }
}
