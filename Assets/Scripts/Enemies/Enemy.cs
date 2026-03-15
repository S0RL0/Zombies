using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public Transform textOrigin;
    public GameObject floatingText;
    [SerializeField] protected EnemyState currentState;

    //private Vector3 targetPosition;
    private GameObject model;
    public Transform target;
    private Transform obstacle;
    private NavMeshAgent navMeshAgent;
    public ParticleSystem damageEffect;
    private PlayerController player;

    // Movement configuration
    public float speed = 3f;

    // Attack configuration
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float attackDamage = 10f;
    public float attackDelay = 0.1f;
    private float attackTimer = 0.5f;
    private bool readyToAttack = true;
    public float rotationSpeed = 10f;
    public float arrivalThreshold = 0.5f;
    public bool isJumping = false;



    protected virtual void Start()
    {
        currentState = EnemyState.Spawn;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;

        player = FindFirstObjectByType<PlayerController>();
        // pick a random model from children with skinned mesh renderer
        SkinnedMeshRenderer[] models = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (models.Length > 0)
        {
            int randomIndex = Random.Range(0, models.Length);
            model = models[randomIndex].gameObject;
            // Disable all other models
            foreach (SkinnedMeshRenderer smr in models)
            {
                if (smr.gameObject != model)
                {
                    smr.gameObject.SetActive(false);
                }
            }
        }
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            float randomAnimStart = Random.Range(0.9f, 1.1f);
            animator.SetFloat("Randomiser", randomAnimStart);
        }
    }

    public virtual void Update()
    {
        // Basic state machine logic
        switch (currentState)
        {
            case EnemyState.Spawn:
                {
                    Spawn();
                }
                break;
            case EnemyState.Chase:
                {
                    Chase();
                }
                break;
            case EnemyState.Attack:
                {
                    Attack();
                }
                break;
        }
    }
    public void Spawn()
    {
        Vector3 currentTargetPos = target.position;
        // Move towards target position
        Vector3 prevPos = transform.position;
        Vector3 newPos = Vector3.MoveTowards(prevPos, currentTargetPos, speed * Time.deltaTime);
        transform.position = newPos;

        // Rotate model to face movement direction
        Vector3 moveDir = (currentTargetPos - prevPos);
        if (model != null && moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // If close enough to the target, switch to Chase
        if (Vector3.Distance(transform.position, currentTargetPos) <= arrivalThreshold)
        {
            target = player.transform;
            navMeshAgent.enabled = true;
            currentState = EnemyState.Chase;
        }

        // If close to obstacle, play jump animation (left as a placeholder)
        if (obstacle != null && Vector3.Distance(transform.position, obstacle.position) <= 0.1 || !isJumping)
        {
            Jump();
        }
    }
    public virtual void Chase()
    {
        Vector3 dir = navMeshAgent.velocity.normalized;
        Vector3 forward = transform.forward;
        Vector3 deltaDir = dir - forward;
        Debug.Log("Enemy velocity: " + dir + "|Object forward: " + forward + "|Delta: " + deltaDir);
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("x", deltaDir.x);
            animator.SetFloat("z", 1 - deltaDir.z);
        }

        if (!target) return;
        bool reachedGoal = false;
        if (Vector3.Distance(transform.position, target.position) < attackRange)
        {
            reachedGoal = true;
        }
        if (reachedGoal && readyToAttack)
        {
            if (navMeshAgent)
                StopMovement();
            currentState = EnemyState.Attack;
        }
        else
        {
            navMeshAgent.destination = target.position;
        }
    }

    public virtual void Attack()
    {
        // start attack animation then wait, then apply damage to player if still in range
        if (!target) return;

        // Rotate to face the target while attacking
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                Animator animator = GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                Debug.Log("Enemy attacks the player for " + attackDamage + " damage.");
                //player.TakeDamage(attackDamage);

                readyToAttack = false;
                attackTimer = attackDelay;

                Invoke("AttackCooldown", attackCooldown);
                if (navMeshAgent)
                    ResumeMovement();
                currentState = EnemyState.Chase;
            }
            else
            {
                attackTimer = attackDelay;
                if (navMeshAgent)
                    ResumeMovement();
                currentState = EnemyState.Chase;
                return;
            }
        }

    }

    public void AttackCooldown()
    {
        readyToAttack = true;
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (textOrigin == null)
            textOrigin = transform;
        GameObject Text = Instantiate(floatingText, textOrigin.position, Quaternion.identity);
        if (Text.GetComponent<FloatingText>())
            Text.GetComponent<FloatingText>().SetText(amount.ToString(), false);

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
    public virtual void TakeDamage(float amount, bool headshot)
    {
        health -= amount;
        if (textOrigin == null)
            textOrigin = transform;
        GameObject Text = Instantiate(floatingText, textOrigin.position, Quaternion.identity);
        if (Text.GetComponent<FloatingText>())
            Text.GetComponent<FloatingText>().SetText(amount.ToString(), false);

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Jump()
    {
        isJumping = true;
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

    }

    public virtual void init(Transform target)
    {
        this.target = target;
    }

    public virtual void init(Transform target, Transform obstacle)
    {
        // Store transform so the enemy can follow a moving target
        this.target = target;
        this.obstacle = obstacle;
    }

    void StopMovement()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }

    void ResumeMovement()
    {
        navMeshAgent.isStopped = false;
    }

    protected enum EnemyState
    {
        Spawn,
        Chase,
        Attack
    }
}
