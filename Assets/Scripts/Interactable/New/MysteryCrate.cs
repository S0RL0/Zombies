using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;


public class MysteryCrate : WeaponCrate
{
    #region Variables
    [SerializeField] private GameObject lid;
    private Vector3 openAngle = new Vector3(-160f, 0, 0);
    private Vector3 closeAngle = Vector3.zero;
    [SerializeField] private float lidOpenAnimationDuration = 0.6f;
    [SerializeField] private float lidCloseAnimationDuration = 0.3f;
    [SerializeField] private float weaponRiseAnimationDuration = 5f;
    [SerializeField] private float weaponLowerAnimationDuration = 10f;
    [SerializeField] private GameObject glow;
    [SerializeField] private ParticleSystem sparkle;

    public int cost = 1000;
    [SerializeField] private List<WeaponProfile> possibleWeapons;
    private List<GameObject> weaponModels = new List<GameObject>();
    private WeaponProfile currentWeaponProfile;
    private Coroutine weaponCycleCoroutine;

    private enum State { closed, animation, open, cooldown }
    [SerializeField] private State state = State.closed;
    #endregion

    #region Start
    protected override void Start()
    {
        base.Start();
        glow.SetActive(false);
        sparkle.Stop();
        lid.transform.localRotation = Quaternion.Euler(closeAngle);
        interactPromptText = $"for a Random Weapon for [Cost:${cost}]";

        // Instantiate all models except the final model
        foreach (WeaponProfile wp in possibleWeapons)
        {
            if (wp.prefab == model) continue; // skip final model
            GameObject obj = Instantiate(wp.prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
            obj.GetComponent<Rigidbody>().ToggleRB(false);
            obj.SetActive(false);
            weaponModels.Add(obj);
        }
    }
    #endregion

    #region Interaction
    public override InteractionResult Interact()
    {
        if (player == null) return new InteractionResult(false, null, null, InteractionType.Buy);
        switch (state)
        {
            case State.closed:
                if (player.GetMoney() < cost)
                    return new InteractionResult(false, null, null, InteractionType.Buy);

                // Game Feel: Sound and VFX
                state = State.animation;
                glow.SetActive(true);
                sparkle.Play();
                AnimateLid(true);
                AnimateWeaponRise(true);

                // Start cycling weapons
                if (weaponCycleCoroutine != null)
                    StopCoroutine(weaponCycleCoroutine);

                weaponCycleCoroutine = StartCoroutine(CycleWeapons());

                return new InteractionResult(true, 1000, this.gameObject, InteractionType.Mystery);
            case State.animation:
                // Crate is in animation, do nothing
                return new InteractionResult(false, null, null, InteractionType.Buy);
            case State.open:
                AnimateLid(false);
                // Give weapon to player if they don't have it, otherwise give ammo
                if (player.HasWeapon(profile))
                {
                    Destroy(model);
                    return new InteractionResult(true, profile, null, InteractionType.Ammo);
                }
                return new InteractionResult(true, profile, model, InteractionType.Weapon);
            case State.cooldown:
                // Crate is in transition or cooldown, do nothing
                return new InteractionResult(false, null, null, InteractionType.Buy);
            default:
                // Fallback case
                return new InteractionResult(false, null, null, InteractionType.Buy);
        }
    }

    protected override void CreateWeapon()
    {
        /*
        model = Instantiate(possibleWeapons[0].prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
        MeshCollider col = model.GetComponent<MeshCollider>();
        col.enabled = false;
        model.GetComponent<Rigidbody>().ToggleRB(false);
        */
    }

    private void SelectFinalWeapon()
    {
        // Hide all cycling models
        foreach (GameObject obj in weaponModels)
        {
            obj.SetActive(false);
        }

        // Pick final weapon
        profile = possibleWeapons[Random.Range(0, possibleWeapons.Count)];

        // Destroy previous model if exists
        if (model != null) Destroy(model);

        // Instantiate final weapon and assign to inherited model
        model = Instantiate(profile.prefab, weaponDisplayPoint.transform.position, weaponDisplayPoint.transform.rotation, weaponDisplayPoint.transform);
        MeshCollider col = model.GetComponent<MeshCollider>();
        if (col != null) col.enabled = false;
        model.GetComponent<Rigidbody>().ToggleRB(false);
    }

    public override string GetInteractionText()
    {
        switch (state)
        {
            case State.closed:
                return interactPromptText;
            case State.open:
                if (player != null && player.HasWeapon(profile))
                    return $"to collect Ammo";
                else
                    return $"to collect weapon";
            default:
                return null;
        }
    }

    public override Sprite GetInteractionIcon()
    {
        return null;
    }
    #endregion

    #region Animation
    private System.Collections.IEnumerator CycleWeapons()
    {
        int currentIndex = -1;

        while (true)
        {
            if (weaponModels.Count == 0) yield break;

            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, weaponModels.Count);
            } while (nextIndex == currentIndex && weaponModels.Count > 1);

            // Hide previous weapon
            if (currentIndex != -1)
                weaponModels[currentIndex].SetActive(false);

            // Show next weapon
            weaponModels[nextIndex].SetActive(true);

            currentIndex = nextIndex;
            currentWeaponProfile = possibleWeapons[nextIndex];

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void AnimateWeaponRise(bool up)
    {
        if (weaponDisplayPoint == null) return;

        weaponDisplayPoint.transform.DOKill();

        Vector3 targetPos = weaponDisplayPoint.transform.localPosition;
        float riseAmount = 1.0f; // height of rise
        targetPos.y = up ? riseAmount : 0f;

        float duration = up ? weaponRiseAnimationDuration : weaponLowerAnimationDuration;
        Ease ease = up ? Ease.OutCubic : Ease.InCubic;

        Tween tween = weaponDisplayPoint.transform.DOLocalMove(targetPos, duration).SetEase(ease);

        tween.OnComplete(() =>
        {
            if (up)
            {
                // Stop weapon cycling
                if (weaponCycleCoroutine != null)
                {
                    StopCoroutine(weaponCycleCoroutine);
                    weaponCycleCoroutine = null;
                }
                SelectFinalWeapon();
                // Lower the weapon after a short delay
                AnimateWeaponRise(false);
            }
            else
            {
                // Weapon finished lowering, close lid
                AnimateLid(false);

                // Optional: destroy final model if needed
                // Destroy(model); // keep it if you want it persistent
            }
        });
    }

    private void AnimateLid(bool open)
    {
        lid.transform.DOKill();

        Vector3 targetRotation = open
            ? openAngle
            : closeAngle;

        float duration = open ? lidOpenAnimationDuration : lidCloseAnimationDuration;

        Ease ease = open ? Ease.InCubic : Ease.OutCubic;

        // Animate
        Tween tween = lid.transform.DOLocalRotate(
            targetRotation,
            duration,
            RotateMode.Fast
        ).SetEase(ease);

        // Update state when done
        tween.OnComplete(() =>
        {
            if (!open)
            {
                glow.SetActive(false);
                sparkle.Stop();
                if (profile != null) profile = null;

            }
            state = open ? State.open : State.closed;
        });
    }
    #endregion
}
