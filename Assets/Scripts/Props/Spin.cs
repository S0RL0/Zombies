using UnityEngine;

public class Spin : MonoBehaviour
{
    public Transform Transform;
    public float x;
    public float y;
    public float z; 
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform.Rotate(new Vector3(x, y, z) * Time.deltaTime * 100);
    }
}
