using System;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{

    [Header("Physics")] 
    public float jumpforce = 5f;
    
    
    private Rigidbody _rigidbody;
    
    private QLearning<(int, int, int)> _qLearning;
    
    private Vector3 _initialPosition;
    
    private (int, int, int) _currentState;
    private int _currentAction;
    private float _currentReward;
    private int _currentScore;
    
    private float _timeSinceLastDecision;
    private int _stepCount;
    
    private bool _isDead = false;
    
    [Header("Decision-making interval")]
    public float decisionInterval = 0.5f;
    
    [Header("Q-Learning")]
    public float learningRate = 0.3f;
    public float discountFactor = 0.5f;
    public float initialEpsilon = 0.1f;
    public int Score => _currentScore;
        
    [Header("Reward")] public float surviveReward = 0.05f;
    public float pipepassReward = 100.0f;
    public float collisionPenalty = -1000.0f;
    
    [Header("gap close reward")] public float gapeNearThreshold = 0.8f;
    public float gapNearReward = 0.5f;
    public float gapMidThreshold = 2.0f;
    public float gapMidReward = 0.5f;
    public float gapFarPenalty = -0.1f;

    [Header("Action restriction")] public float maxYPosition = 4.5f;
    public float minYPosition = 2.0f;
    public float maxUpVelocity = 1.0f;
    
    public float Epsilon
    {
        get => _qLearning.Epsilon;
        set => _qLearning.Epsilon = value;
    }
    
    void Awake()
    {
        AgentManager.Instance.agent = this;
        _rigidbody = GetComponent<Rigidbody>();
        _qLearning = new QLearning<(int, int, int)>(2,learningRate, discountFactor, initialEpsilon);
        _initialPosition = _rigidbody.position;
    }

    

    void FixedUpdate()
    {
        if (_isDead) return;
        
        _timeSinceLastDecision += Time.fixedDeltaTime;
        if (_timeSinceLastDecision >= decisionInterval)
        {
            Step();
            _timeSinceLastDecision = 0f;
        }
    }

    void Step()
    {
        _stepCount++;

        CaculateReward();
        _currentReward += surviveReward;
        
        var nextState = GetState();
        
        _qLearning.Update(_currentState, _currentAction, _currentReward, nextState);
        
        _currentAction = _qLearning.GetAction(nextState);
        _currentAction = ApplyActionConstraints(_currentAction);
        
        if(_currentAction == 1) Jump();
        
        _currentState = nextState;
        _currentReward = 0f;

    }

    int ApplyActionConstraints(int action)
    {
        float y = transform.position.y;
        float veloctiy = _rigidbody.linearVelocity.y;
        
        if(y > maxYPosition || veloctiy > maxUpVelocity) return 0;
        if(y < maxYPosition && veloctiy < 0f) return 1;
        return action;
    }
    
    void Jump()
    {
        _rigidbody.linearVelocity = Vector3.up * jumpforce;
    }

    public void Reset()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        transform.position = _initialPosition;
        
        _isDead = false;
        _currentScore = 0;
        _currentReward = 0f;
        _timeSinceLastDecision = 0f;
        
        _stepCount = 0;

        _currentAction = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        _isDead = true;
        _currentReward = collisionPenalty;
        _qLearning.Update(_currentState, _currentAction, (int)(_currentReward+100f), GetState());
        AgentManager.Instance.EndEpisode();
        //TODO = QLearning Qtable 게임오버 상태 업데이트
    }

    private void OnTriggerExit(Collider other)
    {
        _currentReward += pipepassReward;
        _currentScore++;
    }
    
    //파이프 간격과 Y좌표 차, 파이프와 X좌표 차
    (int, int, int) GetState()
    {
        //파이프 가져오기
        GameObject closestPipe = FindClosestPipe();
        
        int dxBin = 0; //현재 X
        int dyBin = 0; //현재 Y
        int velBin = Mathf.RoundToInt(_rigidbody.linearVelocity.y); //현재 속도

        if (closestPipe != null)
        {
            float gapCenterY = GetGapCenterY(closestPipe);
            float dx = closestPipe.transform.position.x - transform.position.x;
            float dy = gapCenterY - transform.position.y;
            
            dxBin = Mathf.RoundToInt(dx);
            dyBin = Mathf.RoundToInt(dy*2f);
        }
        
        return (dxBin, dyBin, velBin);
    }

    void CaculateReward()
    {
        GameObject closestPipe = FindClosestPipe();
        if (closestPipe == null) return;
        
        float gapCenterY = GetGapCenterY(closestPipe);
        float absDy = Mathf.Abs(transform.position.y - gapCenterY);
        
        if(absDy < gapeNearThreshold) _currentReward += gapNearReward;
        if (absDy < gapMidThreshold) _currentReward += gapMidReward;
        else _currentReward += gapFarPenalty;
    }

    float GetGapCenterY(GameObject closestPipe)
    {
        float topY = closestPipe.transform.GetChild(0).position.y;
        float bottomY = closestPipe.transform.GetChild(1).position.y;
        return (topY + bottomY) / 2f;
    }
    
    

    GameObject FindClosestPipe()
    {
        PipeMovement[] pipes = FindObjectsByType<PipeMovement>(FindObjectsSortMode.None);

        GameObject closest = null;

        float minDist = Mathf.Infinity;

        foreach (var pipe in pipes)
        {
            float dist = pipe.transform.position.x - transform.position.x;
            if (dist > 0f && dist < minDist)
            {
                minDist = dist;
                closest = pipe.gameObject;
            }
        }
        
        return closest;
    }
}
