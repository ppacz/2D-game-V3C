using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int maxHealt = 100;
    [SerializeField] private string nameOfEnemy;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealt;
    }
    
    public void getsDamaged(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log(nameOfEnemy+" died!");
        Spawner.instance.deleteEnemy(gameObject);
        if(gameObject)  Destroy(gameObject);
    }
}
