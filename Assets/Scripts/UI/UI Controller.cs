using TMPro;
using UnityEngine;
using StarterAssets;

public class UIController : MonoBehaviour
{
    public SpawnManager SpawnManager;
    public TextMeshProUGUI RoundsText;
    public TextMeshProUGUI ZombiesLeft;
    public FirstPersonController First;
    public GameObject HealthPerkUI;
    public GameObject SpeedPerkUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HealthPerkUI.SetActive(false);
        SpeedPerkUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
            RoundsText.text = $"{SpawnManager.Rounds}";
            ZombiesLeft.text = $"{SpawnManager.ZombiesLeft}";

        if(First.HealthPerkBool)
        {
            HealthPerkUI.SetActive(true); 
        }

        if (First.SpeedPerkBool)
        {
            SpeedPerkUI.SetActive(true);
        }


    }
}
