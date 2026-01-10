using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using System.Collections.Generic;
using Unity.VisualScripting;


public class UIController : MonoBehaviour
{
    // Script references
    private PlayerController player;
    private WeaponSystem weaponSystem;

    // UI References
    // Hotbar
    [Header("Hotbar")]
    [SerializeField] private List<GameObject> hotbarIcons;
    [SerializeField] private List<Text> ammoDiplay = new List<Text>();
    [SerializeField] private List<Text> inputKeyDisplay = new List<Text>();
    [SerializeField] private List<Image> weaponIcons = new List<Image>();
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

        //SetupHotbar();
    }
    private void OnEnable()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
            weaponSystem = player.GetComponent<WeaponSystem>();
        }

        SetupHotbar();

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
}
