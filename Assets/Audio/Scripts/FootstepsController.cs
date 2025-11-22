using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FootstepsController : MonoBehaviour
{
    [SerializeField] private EventReference footstepEvent;
    public float stepInterval = 0.5f; // time between steps
    private float stepTimer = 0f;

    private CharacterController controller;
    private EventInstance footstepInstance;

    public int currentSurface = 0; // 0 = Grass, 1 = Wood, etc.

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
    }

    void PlayFootstep()
    {
        footstepInstance = RuntimeManager.CreateInstance(footstepEvent);
        footstepInstance.setParameterByName("Terrain", currentSurface);
        footstepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        footstepInstance.start();
        footstepInstance.release();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Example: detect surface by tag
        if (hit.collider.CompareTag("Grass"))
            currentSurface = 0;
        else if (hit.collider.CompareTag("Wood"))
            currentSurface = 1;
        else if (hit.collider.CompareTag("Stone"))
            currentSurface = 2;
        else if (hit.collider.CompareTag("Carpet"))
            currentSurface = 3;
        else
            currentSurface = 0; // default to grass
    }
}

