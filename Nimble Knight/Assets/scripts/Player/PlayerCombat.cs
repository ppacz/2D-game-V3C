using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.3f;
    public LayerMask enemyLayers;
    public int dmg = 20;
    [SerializeField]
    private Camera _cam;
    [SerializeField]
    private Transform _centerTransform;
    private double timer;
    [SerializeField]
    private Animator attackAnimator;

    /// <summary>
    /// rotates hit area of player to the position of mouse
    /// </summary>
    private void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null) dmg = data.dmg;
    }
    void Update()
    {
        Vector3 _mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (_mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        _centerTransform.eulerAngles = new Vector3(0, 0, angle);
        if (timer < Time.time)
        {
            if (Input.GetMouseButtonDown(0) && gameObject.GetComponent<PlayerControler>().canAttack)
            {
                Attack();
            }
        }
    }

    /// <summary>
    /// damages enemies in the radius
    /// </summary>
    void Attack()
    { 
        Collider2D[] hitEnemies =Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHP>().getsDamaged(dmg);
            Debug.Log("just attacked" + enemy.name + " for " + dmg);
        }
        gameObject.GetComponent<PlayerSoundManager>().playAttack();
        attackAnimator.Play("attackAnim");
        timer = Time.time + 0.40f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
