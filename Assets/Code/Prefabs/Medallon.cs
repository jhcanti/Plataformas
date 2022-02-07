using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medallon : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.Play("Orb Collect");
            AudioManager.Instance.Play("Collect Voice");
            Destroy(gameObject);
        }
    }
}
