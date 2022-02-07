using UnityEngine;

public class Engranaje : MonoBehaviour
{
    Vector2 inicio;
    Vector2 fin;
    float speed = 2;
    float rotationSpeed = -180;
    int dirFacing = 1;
    public int tilesLong;
    


    void Start()
    {        
        inicio = transform.position;
        fin = new Vector2(inicio.x + tilesLong * .5f * dirFacing, inicio.y);
    }


    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
            return;

        float moveDir = dirFacing * Time.deltaTime * speed;        
        transform.position = new Vector3(transform.position.x + moveDir, transform.position.y, 0);

        if (dirFacing == -1 && transform.position.x < inicio.x)
            Flip();

        if (dirFacing == 1 && transform.position.x > fin.x)
            Flip();

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);        
    }


    void Flip()
    {
        dirFacing *= -1;
        rotationSpeed *= -1;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(1);
        }
    }
}
