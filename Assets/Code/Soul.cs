using UnityEngine;

public class Soul : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.GetSoul();
            Destroy(gameObject);
        }
    }


}
