using UnityEngine;

public class SaveCP : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameData data = new GameData(transform.position.x, transform.position.y);            
            SaveManager.Instance.SaveData(data);
        }
    }
}
