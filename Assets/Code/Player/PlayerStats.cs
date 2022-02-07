using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Singleton<PlayerStats>
{
    public bool canTakeDamage = true;
    public Transform lifePanel;
    public Transform lifeRecipe;
    public Sprite[] recipes;
    int currentRecipe = 0;
    int maxRecipes = 0;
    public int maxHealth;
    int currentHealth;
    public float swordForce;
    bool isDead = false;
    public Events.EventChangeGameState OnGameStateChanged;


    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleOnGameStateChanged);
        Initialize();
    }


    void Initialize()
    {
        currentHealth = maxHealth;
        currentRecipe = 0;
        maxRecipes = recipes.Length;
        lifeRecipe.GetComponent<Image>().sprite = recipes[currentRecipe];
        for (int i = 0; i < maxHealth; i++)
        {
            lifePanel.GetChild(i).gameObject.SetActive(true);
        }
        isDead = false;        
    }


    public void TakeDamage(int amount)
    {
        if (!canTakeDamage || isDead)
            return;

        StartCoroutine(GetComponent<PlayerController>().PlayerBlink());
        currentHealth -= amount;
        UpdateLifeUI();        

        canTakeDamage = false;

        if (currentHealth <= 0)
        {
            isDead = true;
            AudioManager.Instance.Play("Death");
            GameManager.Instance.UpdateState(GameManager.GameState.DEATH);
        }
    }

    private void UpdateLifeUI()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (currentHealth - 1 >= i)
                lifePanel.GetChild(i).gameObject.SetActive(true);
            else
                lifePanel.GetChild(i).gameObject.SetActive(false);
        }
    }


    public void GetSoul()
    {
        currentRecipe++;
        if (currentRecipe < maxRecipes)
        {
            lifeRecipe.GetComponent<Image>().sprite = recipes[currentRecipe];
        }
        else
        {
            if (currentHealth < maxHealth)
            {
                currentRecipe = 0;
                lifeRecipe.GetComponent<Image>().sprite = recipes[currentRecipe];
                currentHealth++;
                UpdateLifeUI();
            }
        }
    }



    void HandleOnGameStateChanged(GameManager.GameState state, GameManager.GameState previousState)
    {
        switch (state)
        {
            case GameManager.GameState.INITIALIZING:
                Initialize();
                break;

            case GameManager.GameState.RUNNING:
                break;

            case GameManager.GameState.DEATH:
                break;

            default:
                break;
        }
    }
}
