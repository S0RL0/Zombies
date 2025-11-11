using UnityEngine;

public class MovePanelToPos : MonoBehaviour
{
    public Vector3 initialpos;
    public Quaternion initialrot;
    private bool moving;
    private bool fallen = false;
    private Vector3 moveFrompos;
    private Quaternion moveFromrot;
    private float moveTimer = 2f;
    public float moveTimerMax = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialpos = transform.position;
        initialrot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            moveTimer -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, initialpos, 1F * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, initialrot, 15F * Time.deltaTime);
           
            if(transform.position == moveFrompos || moveTimer <= 0)
            {
                moving = false;
                fallen = false;
                transform.position = initialpos;
                transform.rotation = initialrot;
                GetComponent<Rigidbody>().isKinematic = true;
            
            }
        }

        
    }
    public void FallDown()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        fallen = true;
    }

    public void MoveToStart()
    {
        if (!moving && fallen)
        {
            moveFrompos = transform.position;
            moveFromrot = transform.rotation;
            moving = true;
            GetComponent<Rigidbody>().useGravity = false;
            moveTimer = moveTimerMax;
           
        }
    }
}
