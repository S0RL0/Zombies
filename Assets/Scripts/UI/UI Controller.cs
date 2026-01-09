using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using System.Collections.Generic;


public class UIController : MonoBehaviour
{
    // UI References
    // Hotbar
    [Header("Hotbar")]
    [SerializeField] private List<Text> ammoDiplay;
    [SerializeField] private List<Text> inputKeyDisplay;
    [SerializeField] private List<Image> weaponIcons;

    // Topright counters
    [Header("Topright Counters")]
    [SerializeField] private Text zombieCounter;
    [SerializeField] private Text roundCounter;
    [SerializeField] private Text pointCounter;

    // Scoreboard
    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreboard;

    // Interaction Prompts
    [Header("Interaction Prompts")]
    [SerializeField] private Text interactionPrompt;
    [SerializeField] private Text interactionIcon;
    [SerializeField] private ParticleSystem hitScreenEffect;
    //[SerializeField] private ParticleSystem healScreenEffect;
    [SerializeField] private GameObject hitDirectionEffect; // Rotate the z axis based on hit direction
    [SerializeField] private Animator hitDirectionAnimator;
    [SerializeField] private GameObject hitVignetteEffect;
    [SerializeField] private Animator hitVignetteAnimator;


}
