using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Destructable : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public GameObject floatingText;

    [Header("DestructableObjects")]
    public bool DestructableObject;
    public GameObject Effect; 

    [Header("Sounds")]
    [SerializeField] private EventReference HitEvent;
    [SerializeField] private EventReference DestroyEvent;
    private EventInstance Hitnoise;
    private EventInstance DestroyNoise;
    float fXVolume;

    public void Start()
    {
       fXVolume = AudioSettingsManager.Instance.FXVolume;
    }



    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        GameObject Text = Instantiate(floatingText, transform.position, Quaternion.identity);

        //Hit Noise
        Hitnoise = RuntimeManager.CreateInstance(HitEvent);
        Hitnoise.setVolume(1);
        Hitnoise.start();
        Hitnoise.release();


        if (Text.GetComponent<FloatingText>())
            Text.GetComponent<FloatingText>().SetText(amount.ToString(), false);

        if (health <= 0f)
        {
            DestroyNoise = RuntimeManager.CreateInstance(DestroyEvent);
            //DestroyNoise.set3DAttributes(RuntimeUtils.To3DAttributes(this.transform));
            DestroyNoise.setVolume(1);
            DestroyNoise.start();
            DestroyNoise.release();

            if (DestructableObject)
            {
                GameObject effect = Instantiate(Effect,transform.position,Quaternion.identity);
                Destroy(effect, 2f); 
            }
            
            Destroy(gameObject);
        }
    }
}
