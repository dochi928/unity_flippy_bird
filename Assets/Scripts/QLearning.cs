using System.Collections.Generic;
using UnityEngine;

//경험을 통해 최적의 행동 학습시 사용하는 알고리즘

public class QLearning<TState>
{
    private Dictionary<TState, float[]> _qTable;
    
    private float _learningRate; //학습률(a)
    private float _discountFactor; //할인률(y)
    private float _epsilon; //탐험률(e)

    private int _actionCount = 2; //(stay, jump)
    public float Epsilon => _epsilon;

    public void setEpsilon(float epsilon)
    {
        _epsilon = epsilon;
    }

    public QLearning(int actionCount, float learningRate, float discountFactor, float epsilon)
    {
        _actionCount = actionCount;
        _learningRate = learningRate;
        _discountFactor = discountFactor;
        _epsilon = epsilon;
        _qTable = new Dictionary<TState, float[]>();
    }

    public int GetAction(TState state)
    {
        if (!_qTable.ContainsKey(state))
        {
            _qTable[state] = new float[_actionCount];
        }

        if (Random.value < _epsilon)
            return Random.Range(0, _actionCount);

        return _qTable[state][0] >= _qTable[state][1] ? 0 : 1;
    }

    public void Update(TState state, int action, float reward, TState nextState)
    {
        if(!_qTable.ContainsKey(state))
            _qTable[state] = new float[_actionCount];
        if(!_qTable.ContainsKey(nextState))
            _qTable[nextState] = new float[_actionCount];
        
        //점프 vs 대기 중 큰 값을 뽑아옴
        float maxNextQ = Mathf.Max(_qTable[state]);

        _qTable[state][action] += _learningRate * (reward + _discountFactor * maxNextQ - _qTable[state][action]);
    }
}
