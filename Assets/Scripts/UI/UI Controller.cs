using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;



public class UIController : MonoBehaviour
{
    // Script references
    private PlayerController player;
    private WeaponSystem weaponSystem;
    private Transform cameraTransform;

    // UI References
    // Hotbar
    [Header("Hotbar")]
    [SerializeField] private List<GameObject> hotbarIcons;
    private List<Text> ammoDiplay = new List<Text>();
    private List<Text> inputKeyDisplay = new List<Text>();
    private List<Image> weaponIcons = new List<Image>();
    private List<Animator> weaponIconAnimators = new List<Animator>();

    // Topright counters
    [Header("Topright Counters")]
    [SerializeField] private Text zombieCounter;
    [SerializeField] private Text roundCounter;
    //[SerializeField] private Text pointCounter;

    // Scoreboard
    //[Header("Scoreboard")]
    //[SerializeField] private GameObject scoreboard;

    // Interaction Prompts
    [Header("Interaction Prompts")]
    [SerializeField] private Text interactionPrompt;
    [SerializeField] private Image interactionIcon;
    [SerializeField] private List<ParticleSystem> hitScreenEffect;
    //[SerializeField] private ParticleSystem healScreenEffect;
    [SerializeField] private GameObject hitDirectionEffect; // Rotate the z axis based on hit direction
    [SerializeField] private Animator hitDirectionAnimator;
    [SerializeField] private CanvasGroup hitVignetteGroup;
    [SerializeField] private GameObject hitVignetteEffect;
    [SerializeField] private Animator hitVignetteAnimator;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        weaponSystem = player.GetComponent<WeaponSystem>();
        cameraTransform = player.GetComponentInChildren<Camera>().transform;
    }
    private void OnEnable()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
            weaponSystem = player.GetComponent<WeaponSystem>();
        }

        ApplyEvents();

        SetupHotbar();

    }

    private void OnDisable()
    {
        RemoveEvents();
    }

    private void ApplyEvents()
    {
        WeaponSystem.onAmmoChanged += UpdateAmmo;
        WeaponSystem.onWeaponSwitched += WeaponSwitched;
        WeaponSystem.onInventoryChanged += UpdateInventory;
    }

    void RemoveEvents()
    {
        WeaponSystem.onAmmoChanged -= UpdateAmmo;
        WeaponSystem.onWeaponSwitched -= WeaponSwitched;
        WeaponSystem.onInventoryChanged -= UpdateInventory;
    }

    void SetupHotbar()
    {
        for (int a = 0; a < hotbarIcons.Count; a++)
        {
            Animator anim = hotbarIcons[a].GetComponent<Animator>();
            if (anim != null)
                weaponIconAnimators.Add(anim);

            Image icon = hotbarIcons[a].transform.GetChild(0).GetChild(1).GetComponentInChildren<Image>();
            if (icon != null)
                weaponIcons.Add(icon);

            Text ammo = hotbarIcons[a].transform.GetChild(1).GetComponentInChildren<Text>();
            if (ammo != null)
                ammoDiplay.Add(ammo);

            Text inputKey = hotbarIcons[a].transform.GetChild(2).GetComponentInChildren<Text>();
            if (inputKey != null)
                inputKeyDisplay.Add(inputKey);
        }


        int i = 0;
        for (i = 0; i < weaponSystem.profiles.Count; i++)
        {
            if (i >= hotbarIcons.Count)
                break;
            hotbarIcons[i].SetActive(true);
            weaponIcons[i].sprite = weaponSystem.profiles[i].icon;
            inputKeyDisplay[i].text = (i + 1).ToString();
            List<int> ammoValues = weaponSystem.GetAmmoCount(i);
            ammoDiplay[i].text = ammoValues[0].ToString() + " / " + ammoValues[1].ToString();
        }
        for (; i < hotbarIcons.Count; i++)
        {
            hotbarIcons[i].SetActive(false);
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 3f))
        {
            bool interacted = false;
            // Check if looking at interactable
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log("Looking at interactable: " + interactable.name);
                interactionPrompt.text = "test";//interactable.GetInteractionPrompt();
                //interactionIcon.sprite = interactable.GetInteractionIcon();
                interactionPrompt.gameObject.SetActive(true);
                interactionIcon.gameObject.SetActive(true);
                interacted = true;
            }

            // Check if looking at weapon you can pick up
            Weapon weapon = hit.collider.GetComponent<Weapon>();
            if (weapon != null)
            {
                // Show weapon pickup prompt
                interactionPrompt.text = "Pick up " + weapon.weaponProfile.name;
                interactionIcon.sprite = weapon.weaponProfile.icon;
                interactionPrompt.gameObject.SetActive(true);
                interactionIcon.gameObject.SetActive(true);
                interacted = true;
            }

            //Debug.Log("Interacted: " + interacted);
            if (!interacted)
            {
                interactionPrompt.gameObject.SetActive(false);
                interactionIcon.gameObject.SetActive(false);
            }
        }
    }

    void UpdateAmmo(GameObject sender)
    {
        if (sender != weaponSystem.gameObject)
            return;

        for (int i = 0; i < weaponSystem.profiles.Count; i++)
        {
            List<int> ammoValues = weaponSystem.GetAmmoCount(i);
            ammoDiplay[i].text = ammoValues[0].ToString() + " / " + ammoValues[1].ToString();
        }
    }

    void UpdateWeapons(GameObject sender)
    {
        if (sender != weaponSystem.gameObject)
            return;

        for (int i = 0; i < weaponSystem.profiles.Count; i++)
        {
            weaponIcons[i].sprite = weaponSystem.profiles[i].icon;
        }
        UpdateAmmo(sender);
    }

    void WeaponSwitched(GameObject sender)
    {
        if (sender != weaponSystem.gameObject)
            return;

        // Update selected weapon animation
        for (int i = 0; i < weaponIconAnimators.Count; i++)
        {
            if (i == weaponSystem.currentWeaponIndex)
            {
                weaponIconAnimators[i].SetBool("isSelected", true);
            }
            else
            {
                weaponIconAnimators[i].SetBool("isSelected", false);
            }
        }
        UpdateAmmo(sender);
    }

    void UpdateInventory(GameObject sender)
    {
        if (sender != weaponSystem.gameObject)
            return;

        int i = 0;
        for (i = 0; i < weaponSystem.profiles.Count; i++)
        {
            if (i >= hotbarIcons.Count)
                break;
            hotbarIcons[i].SetActive(true);
            weaponIcons[i].sprite = weaponSystem.profiles[i].icon;
            inputKeyDisplay[i].text = (i + 1).ToString();
            List<int> ammoValues = weaponSystem.GetAmmoCount(i);
            ammoDiplay[i].text = ammoValues[0].ToString() + " / " + ammoValues[1].ToString();
        }
        for (; i < hotbarIcons.Count; i++)
        {
            hotbarIcons[i].SetActive(false);
        }
    }
}
