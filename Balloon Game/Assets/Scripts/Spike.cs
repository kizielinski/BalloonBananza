using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Rigidbody2D rbSpike;
    Collider2D coSpike;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbSpike = GetComponent<Rigidbody2D>();
        coSpike = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log($"{collision.gameObject.tag}");
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
        }
        else
        {
            Destroy(gameObject);    
        }
    }
}
