using UnityEngine;

public class MovePanelToPos : MonoBehaviour
{
    public Vector3 initialPos;
    public Quaternion initialRot;
    private bool moving;
    private bool fallen = false;
    private Vector3 moveFromPos;
    private Quaternion moveFromRot;
    private float moveTimer = 2f;
    public float moveTimerMax = 2f;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            moveTimer -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, initialPos, 1F * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRot, 15F * Time.deltaTime);
           
            if(Vector3.Distance(transform.position,initialPos)<= 0.15)
            {
                moving = false;
                fallen = false;
                transform.position = initialPos;
                transform.rotation = initialRot;
                rb.isKinematic = true;
            
            }
        }   
    }
    public void FallDown()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        fallen = true;
    }

    public void MoveToStart()
    {
        if (!moving && fallen)
        {
            moveFromPos = transform.position;
            moveFromRot = transform.rotation;
            moving = true;
            rb.useGravity = false;
            moveTimer = moveTimerMax;
           
        }
    }
}
