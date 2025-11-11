using UnityEngine;
using UnityEngine.AI;

public class DoorFix : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Healing Settings")]
    public int healAmount = 5;      // How much to heal each tick
    public float healInterval = 1f; // Time in seconds between heals

    private float healTimer = 0f;

    [Header("Damage Settings")]
    public int damagePerTick = 5;      // Damage applied per interval
    public float damageInterval = 1f;  // Time in seconds between damage ticks

    private float damageTimer = 0f;

    [Header("Panel Settings")]
    public GameObject[] panels; // will hold all panels automatically

    public bool Fixable;
    public bool PlayerinRange;
    public bool ZombieinRange;
    public GameObject doorLink;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        Fixable = true;

        currentHealth = maxHealth;

        Transform[] children = GetComponentsInChildren<Transform>(true);
        int count = 0;

        // First count how many panels there are
        foreach (Transform child in children)
        {
            if (child.CompareTag("Panel") && child != this.transform)
            {
                count++;
            }
        }

        // Create the array
        panels = new GameObject[count];
        int index = 0;
        foreach (Transform child in children)
        {
            if (child.CompareTag("Panel") && child != this.transform)
            {
                panels[index] = child.gameObject;
            
                index++;
            }
        }

        UpdatePanels(); // initialize panel state

    }

    // Update is called once per frame
    void Update()
    {

        if (Fixable && PlayerinRange)
        {
            {
                // Automatic healing over time
                if (currentHealth < maxHealth)
                {
                    healTimer += Time.deltaTime;
                    if (healTimer >= healInterval)
                    {
                        Heal(healAmount);
                        healTimer = 0f;
                    }
                }
            }
        }

        if (ZombieinRange)
        {
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    TakeDamage(damagePerTick);
                    damageTimer = 0f;
                }
            }
        }
             
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // prevents overhealing
        Debug.Log("Door healed. Current health: " + currentHealth);
        UpdatePanels();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Door took damage. Current health: " + currentHealth);
        UpdatePanels();


    }

    private void UpdatePanels()
    {
        if (panels == null || panels.Length == 0) return;

        float healthPercent = (float)currentHealth / maxHealth;
        int panelsToShow = Mathf.CeilToInt(healthPercent * panels.Length);

       /* for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null) // safety check
                panels[i].SetActive(i < panelsToShow);
        }*/

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null && i < panelsToShow) // safety check
            {
                panels[i].GetComponent<MovePanelToPos>().MoveToStart();
              
            }
            else if (panels[i] != null)
            {
                panels[i].GetComponent<MovePanelToPos>().FallDown();
            }
        }

        bool pathActive = false;
        if(panelsToShow == 0)
        {
            pathActive = true;
        }
        doorLink.SetActive(pathActive);
    }

}





