using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private string interactKey; // Store interact key from input mapping

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        weaponSystem = player.GetComponent<WeaponSystem>();
        cameraTransform = player.GetComponentInChildren<Camera>().transform;

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        InputActionAsset actions = playerInput.actions;
        InputAction interactAction = actions.FindAction("Player/Interact");
        if (interactAction != null && interactAction.bindings.Count > 0)
        {
            // Get the first binding (usually the main one)
            InputBinding binding = interactAction.bindings[0];

            // Convert binding to a readable string
            interactKey = InputControlPath.ToHumanReadableString(
                binding.effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );

            Debug.Log("Jump keybind: " + interactKey);
        }
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
            bool textFound = false;
            bool iconFound = false;
            // Check if looking at interactable
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (interactable.GetInteractionText() != null)
                {
                    interactionPrompt.text = "Press " + interactKey + " " + interactable.GetInteractionText();
                    textFound = true;
                }

                if (interactable.GetInteractionIcon() != null)
                {
                    interactionIcon.sprite = interactable.GetInteractionIcon();
                    iconFound = true;
                }

            }

            // Check if looking at weapon you can pick up
            Weapon weapon = hit.collider.GetComponent<Weapon>();
            if (weapon != null)
            {
                // Show weapon pickup prompt
                interactionPrompt.text = "Press " + interactKey + " " + weapon.GetInteractionText();
                interactionIcon.sprite = weapon.GetInteractionIcon();
                textFound = true;
                iconFound = true;
            }

            //Debug.Log("Interacted: " + interacted);
            interactionPrompt.gameObject.SetActive(textFound);
            interactionIcon.gameObject.SetActive(iconFound);
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
            interactionIcon.gameObject.SetActive(false);
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
