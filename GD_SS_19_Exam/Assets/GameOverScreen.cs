using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Score score;
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        scoreText.text = score.lastScore.ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
