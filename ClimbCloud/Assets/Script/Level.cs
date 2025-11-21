using UnityEngine;
using UnityEngine.SceneManagement;
public class Level : MonoBehaviour

{
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void Infinity()
   {
        SceneManager.LoadScene("GameScene");
   }

    public void Rank()
    {
        SceneManager.LoadScene("RankScene");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void CreateAccount()
    {
        SceneManager.LoadScene("CreateAccountScene");
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("StartScene");
    }
}
