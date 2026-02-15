using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float speed = 4f;
    public static float PipeSpeed = 4f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -15f)
        {
            Destroy(this.gameObject);
        }
        transform.position += Vector3.left * PipeSpeed * Time.deltaTime;
    }
}
