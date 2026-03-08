using TMPro;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; }

    void Awake()
    {
        //싱글톤 패턴
        if (Instance != null && Instance != this) Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public PlayerAgent agent;
    public TextMeshProUGUI infoText;
    
    public float timeScale = 1f;
    
    public float minEpsilon = 0.05f; //최저 학습 확률 임계치
    public float deacyRate = 0.999f; //엡실론 감소 속도

    private int _episodeCount;
    private float _initialEpsilon;
    private int _bestScore;

    void Start()
    {
        Time.timeScale = timeScale;
        _episodeCount = 0;
        _initialEpsilon = agent.Epsilon;
        
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (infoText == null) return;
        if(agent.Score > _bestScore) _bestScore = agent.Score;
        
        infoText.text = $"Best Score : {_bestScore}\n" 
                        + $"Score : {agent.Score}\n" 
                        + $"Episodes : {_episodeCount}\n" 
                        + $"Epsilon : {agent.Epsilon:F3}";
    }

    public void EndEpisode()
    {
        _episodeCount++;
        DecayEpsilon();
        DestroyAllPipes();
    }

    void DecayEpsilon()
    {
        float newEpsilon = _initialEpsilon*Mathf.Pow(deacyRate, _episodeCount);
        agent.setEpsilon(Mathf.Max(newEpsilon, minEpsilon));
    }

    void DestroyAllPipes()
    {
        PipeMovement[] pipes = FindObjectsByType<PipeMovement>(FindObjectsSortMode.None);
        foreach (PipeMovement pipe in pipes) Destroy(pipe.gameObject);
    }
}
