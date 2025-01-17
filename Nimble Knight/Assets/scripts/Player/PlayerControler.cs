using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private LayerMask dashLayerMask;
    private const float MOVEMENTSPEED = 90f ;
    private Rigidbody2D rigidBody2D;
    private Vector3 moveDirection;
    private Vector3 dashPosition;
    private Vector3 lastDirection;
    private float moveY, moveX;
    private float dashAmount;
    private SkillUnlocking skillsSet;
    private Animator animator;
    private bool isWalking;
    public bool canAttack;
    
    
    /// <summary>
    /// sets skill if there are none and thase needed references
    /// </summary>
    private void Start()
    {
        canAttack = true;
        skillsSet = gameObject.GetComponent<SkillUnlocking>();
        skillsSet.setSkills("Dash");
        skillsSet.setSkills("Damage");
        skillsSet.setSkills("Health regeneration");
        skillsSet.setSkills("Max health");
        skillsSet.setSkills("Max stamina");
        skillsSet.setSkills("Stamina regeneration");
        rigidBody2D = GetComponent<Rigidbody2D>();
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            rigidBody2D.MovePosition(new Vector2(data.position[0], data.position[1]));
        }
        animator = gameObject.GetComponent<Animator>();
    }
    /// <summary>
    /// manages inputs and takes care if other instant actions
    /// </summary>
    private void Update()
    {
        if (gameObject.GetComponent<PlayerHP>().alive)
        {
            isWalking = false;
            moveX = 0f;
            moveY = 0f;
            if (Input.GetKey(KeyCode.W))
            {
                moveY = 1;
                isWalking = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveY = -1f;
                isWalking = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
                isWalking = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveX = 1f;
                isWalking = true;
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                canAttack = !canAttack;
                PlayerManager.instance.skillTree.SetActive(!PlayerManager.instance.skillTree.activeSelf);
            }
            if (Input.GetKeyUp(KeyCode.P))
            {
                SaveSystem.SavePlayer(gameObject);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dash();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveSystem.SavePlayer(gameObject);
                SceneManager.LoadScene(0);
            }
            moveDirection = new Vector3(moveX, moveY).normalized;
        }
        else
        {
            rigidBody2D.velocity = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// physics update
    /// </summary>
    private void FixedUpdate()
    {
        if (gameObject.GetComponent<PlayerHP>().alive)
        {
            rigidBody2D.velocity = 4 * MOVEMENTSPEED * Time.deltaTime * moveDirection;
            if (moveDirection != new Vector3(0, 0))
            {
                lastDirection = moveDirection;
            }
            if (moveDirection.x != 0f)
            {
                animator.SetFloat("horizontalMovement", moveDirection.x);
            }
            if (moveDirection.x == 0)
            {
                animator.SetFloat("horizontalMovementLast", lastDirection.x);
            }
            if (rigidBody2D.velocity != new Vector2(0, 0)) animator.SetBool("isMoving", true);
            else animator.SetBool("isMoving", false);
            if (!gameObject.GetComponent<AudioSource>().isPlaying && isWalking)
            {
                gameObject.GetComponent<PlayerSoundManager>().playWalk();
            }
            if (!isWalking)
            {
                gameObject.GetComponent<AudioSource>().volume = 0;
            }
        }
    }
    /// <summary>
    /// dash ability that uses stamine and needs to be unlocked in skillTree
    /// </summary>
    private void Dash()
    {
        
        if (skillsSet.getState("Dash") && PlayerManager.instance.player.GetComponent<PlayerStamina>().useAbility(10))
        {
            dashAmount = 5f;
            Vector3 centerOfHero = new Vector3(transform.position.x, transform.position.y + 1.2f);
            bool hit = Physics2D.CircleCast(centerOfHero, .5f, lastDirection, dashAmount, dashLayerMask);
            // Casting circular rayCast, only returns bool if it hit something in specified layer, might be used to varify result;
            while (hit)
            {
                dashAmount -= .1f;
                hit = Physics2D.CircleCast(centerOfHero, .5f, lastDirection, dashAmount, dashLayerMask);
                if (dashAmount <= 0) break;
            }
            Debug.Log(centerOfHero);
            dashPosition = transform.position + lastDirection * dashAmount;
            rigidBody2D.MovePosition(dashPosition);
            gameObject.GetComponent<PlayerSoundManager>().playDash();
            return;

        }
        else
        {

            Debug.Log("Skill is not unlocked yet!");
            return;
        }
    }

    public void newUpgrade(string skill)
    {
        gameObject.GetComponent<PlayerSoundManager>().playAbilityUpgrade();
        Debug.Log("works");
        if (skill=="Dash") return;
        else if (skill == "Damage") GetComponent<PlayerCombat>().dmg+=5;
        else if (skill == "Health regeneration") GetComponent<PlayerHP>().regen +=2 ;
        else if (skill == "Max health")GetComponent<PlayerHP>().maxHealth += 50;
        else if (skill == "Max stamina") GetComponent<PlayerStamina>().maxStamina += 30;
        else if (skill == "Stamina regeneration") GetComponent<PlayerStamina>().regen += 1;
    }
}
