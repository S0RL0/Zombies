using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public GameObject floatingText;
    protected EnemyState currentState;

    private Vector3 targetPosition;
    public GameObject model;
    public Transform targetTransform;
    public Transform obstacleTransform;

    // Movement configuration
    public float speed = 3f;
    public float rotationSpeed = 10f;
    public float arrivalThreshold = 0.5f;
    public bool isJumping = false;

    protected virtual void Start()
    {
        currentState = EnemyState.Spawn;

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
    }

    public virtual void Update()
    {
        // Determine the current target position (follow moving target if provided)
        Vector3 currentTargetPos = targetTransform != null ? targetTransform.position : targetPosition;

        // Basic state machine logic
        switch (currentState)
        {
            case EnemyState.Spawn:
                {
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
                        currentState = EnemyState.Chase;
                    }

                    // If close to obstacle, play jump animation (left as a placeholder)
                    if (obstacleTransform != null && Vector3.Distance(transform.position, obstacleTransform.position) <= 0.1 || !isJumping)
                    {
                        Jump();
                    }
                }
                break;
            case EnemyState.Chase:
                {
                }
                break;
            case EnemyState.Attack:
                // Handle attack logic (left as a placeholder)
                break;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        GameObject Text = Instantiate(floatingText, transform.position, Quaternion.identity);
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
        // Store transform so the enemy can follow a moving target
        targetTransform = target;
        targetPosition = target.position;
    }

    public virtual void init(Transform target, Transform obstacle)
    {
        // Store transform so the enemy can follow a moving target
        targetTransform = target;
        targetPosition = target.position;
        obstacleTransform = obstacle;
    }

    protected enum EnemyState
    {
        Spawn,
        Chase,
        Attack
    }
}
