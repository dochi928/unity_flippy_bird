using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private static int highscore = 0;
    public TextMeshProUGUI playInfoText;
    public Button restartButton;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        restartButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playInfoText.text = $"Best Score : {highscore}\n Score : {score}";
    }

    public void AddScore(int value = 1)
    {
        score += value;
        highscore = (score > highscore) ? score : highscore;
        PipeMovement.PipeSpeed += 0.25f;
    }

    public void GameOver()
    {
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        PipeMovement.PipeSpeed = 4f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
