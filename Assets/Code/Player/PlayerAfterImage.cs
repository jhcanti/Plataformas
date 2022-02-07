using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    [SerializeField] float activeTime = 0.1f;
    float timeActivated;
    float alpha;
    float alphaSet = 0.8f;
    [SerializeField] float alphaMultiplier = 0.85f;
    Color color;

    Transform player;
    SpriteRenderer rend;
    SpriteRenderer playerRend;


    void OnEnable()
    {
        rend = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRend = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        rend.sprite = playerRend.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        transform.localScale = player.localScale;
        timeActivated = Time.time;
    }


    void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        rend.color = color;

        if (Time.time >= timeActivated + activeTime)
        {
            // desactivar el gameobject
            gameObject.SetActive(false);
        }
    }
}
