using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private GameManager _gameManager;

    private float jumpforce = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnJump()
    {
        _rigidbody.linearVelocity = Vector3.up * jumpforce;
        //_rigidbody.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        _gameManager.GameOver();
        //Debug.Log("collision enter");
    }

    void OnTriggerEnter(Collider other)
    {
        _gameManager.AddScore();
    }
}
