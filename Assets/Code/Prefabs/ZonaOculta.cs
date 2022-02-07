using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZonaOculta : MonoBehaviour
{
    public BoxCollider2D col;
    public Tilemap tilemap;
    public int offset;
    float amount = 0.5f;

 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ActivarZonaOculta();
        }    
    }


    void ActivarZonaOculta()
    {
        for (int i = offset; i <= col.bounds.size.x / .5f; i++)
        {
            Vector3 pos = new Vector3(0,0,0);
            pos.x = transform.position.x - col.bounds.size.x / 2 + amount * i;

            for (int j = 0; j <= col.bounds.size.y / .5f; j++)
            {
                pos.y = transform.position.y - col.bounds.size.y / 2 + amount * j;
                pos.z = 0;
                Vector3Int posCell = tilemap.WorldToCell(pos);
                tilemap.SetTile(posCell, null);
            }
        }

        Destroy(gameObject);
    }
}
