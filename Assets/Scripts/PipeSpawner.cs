using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float SpawnRateSec = 2f;
    private float _timer = 0f;
    public float RandomYMaxoffset = 10f;
    public float RandomYMinoffset = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= SpawnRateSec)
        {
            float y = Random.Range(RandomYMaxoffset, RandomYMinoffset);
            Instantiate(pipePrefab, new Vector3(12f, y, 0f), Quaternion.identity);
            _timer = 0f;
        }
    }
}
