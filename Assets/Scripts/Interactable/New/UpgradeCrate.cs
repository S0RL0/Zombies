using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class UpgradeCrate : WeaponCrate
{
    [Header("Stats")]
    public int upgradeCost = 5000;
    private State state = State.ready;
    private WeaponProfile upgradedProfile;

    // Lid Animation
    [Header("Lid variables")]
    [SerializeField] private Transform lid;
    private Vector3 lidClosedLocalPos;
    [SerializeField] private float lidOpenHeight = 0.5f;
    [SerializeField] private float lidOpenAnimationDuration = 0.35f;
    [SerializeField] private float lidCloseAnimationDuration = 0.25f;

    // Weapon Animation
    [Header("Weapon variables")]
    [SerializeField] private Vector3 weaponStartPosition = new Vector3(0f,0.8f,1.2f);
    [SerializeField] private Vector3 weaponReadyPosition = new Vector3(0f, 0.8f, 0f);
    [SerializeField] private Vector3 weaponClosedPosition = new Vector3(0f, 0.1f, 0f);
    [SerializeField] private float weaponDepositAnimationDuration = 5f;
    [SerializeField] private float weaponRecieveAnimationDuration = 10f;

    // Chest Shake Animation
    [Header("Shake variables")]
    [SerializeField] private float riseHeight = 1.2f;
    [SerializeField] private float riseDuration = 0.35f;
    [SerializeField] private float lowerDuration = 1.2f;
    [SerializeField] private float shakeDuration = 0.6f;
    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private int shakeVibrato = 20;

    // FX references
    [Header("FX references")]
    [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    [SerializeField] private GameObject glow;

    protected override void Start()
    {
        lidClosedLocalPos = lid.localPosition;
        player = FindFirstObjectByType<PlayerController>();
        weaponDisplayPoint.transform.localPosition = weaponStartPosition;
        glow.SetActive(false);
    }

    public override InteractionResult Interact()
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerController not found in the scene.");
            return new InteractionResult(false, null, null, InteractionType.Upgrade);
        }
        switch (state)
        {
            case State.ready:
                if (player == null)
                {
                    Debug.Log("Player reference is null");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
                }
                if (!player.HasWeapon())
                {
                    Debug.Log("No weapon to upgrade.");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
                }
                if (player.GetCurrentWeaponProfile().upgradedProfile == null)
                {
                    Debug.Log("Current weapon has no upgrade available.");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
                }
                if (player.money < upgradeCost)
                {
                    Debug.Log("Not enough money for upgrade.");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
                }
                model = player.GetDropedWeapon();
                upgradedProfile = model.GetComponent<Weapon>().weaponProfile.upgradedProfile;

                // set model parent to weapon display position
                model.transform.SetParent(weaponDisplayPoint.transform);
                TweenUtils.LerpTween(model, Vector3.zero, Quaternion.identity, 0.5f);
                SwitchFX(true);
                state = State.recieving;
                AnimateLid(true);
                return new InteractionResult(true, upgradeCost, null, InteractionType.Upgrade);
            case State.recieving:
                    Debug.Log("Currently receiving a weapon, do nothing.");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
            case State.returning:
                    Debug.Log("Currently returning a weapon, do nothing.");
                    return new InteractionResult(false, null, null, InteractionType.Upgrade);
            case State.collection:
                Debug.Log("Upgrade ready for collection.");
                state = State.ready;
                SwitchFX(false);
                AnimateLid(false);
                return new InteractionResult(true, upgradedProfile, model, InteractionType.Weapon);
            default:
                Debug.LogError("Unknown state.");
                return new InteractionResult(false, null, null, InteractionType.Upgrade);
        }
    }

    private void UpgradeWeapon()
    {
        if (model == null || upgradedProfile == null)
        {
            Debug.LogError("Model or UpgradedWeapon is null.");
            return;
        }

        Destroy(model);
        CreateWeapon(upgradedProfile);
    }

    private void AnimateGun(bool deposit)
    {

        state = deposit ? State.recieving : State.collection;
        weaponDisplayPoint.transform.DOKill();

        Vector3 targetLocalPos = deposit
            ? weaponClosedPosition
            : weaponReadyPosition;

        float duration = deposit
            ? weaponDepositAnimationDuration
            : weaponRecieveAnimationDuration;

        weaponDisplayPoint.transform
            .DOLocalMove(targetLocalPos, duration)
            .SetEase(Ease.InCubic) // accelerates → hard stop
            .OnComplete(() =>
            {
                // Safety snap
                weaponDisplayPoint.transform.localPosition = targetLocalPos;

                // Close lid after gun finishes moving
                if (state == State.recieving)
                    AnimateLid(false);
            });
    }

    private void AnimateLid(bool open)
    {
        if (!open)
            glow.SetActive(false);

        lid.DOKill();

        Vector3 targetPos = open
            ? lidClosedLocalPos + Vector3.up * lidOpenHeight
            : lidClosedLocalPos;

        float duration = open
            ? lidOpenAnimationDuration
            : lidCloseAnimationDuration;

        Ease ease = open ? Ease.InCubic : Ease.OutCubic;

        lid.DOLocalMove(targetPos, duration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (open)
                    glow.SetActive(true);
                switch (state)
                {
                    case State.recieving:
                        if (open)
                            AnimateGun(true);
                        else
                            AnimateChestShake();
                        break;

                    case State.returning:
                        if (open)
                            AnimateGun(false);
                        else
                        {
                            state = State.ready;
                        }
                        break;
                }
            });
    }

    private void AnimateChestShake()
    {
        Transform crate = transform;
        crate.DOKill();

        Vector3 startPos = crate.localPosition;
        Vector3 raisedPos = startPos + Vector3.up * riseHeight;

        Vector3 spinUp = new Vector3(0f, 360f, 0f);
        Vector3 spinDown = new Vector3(0f, -360f, 0f);

        Sequence seq = DOTween.Sequence();

        // === RISE + SPIN (snappy) ===
        seq.Append(
            crate.DOLocalMove(raisedPos, riseDuration)
                .SetEase(Ease.InCubic)
        );

        seq.Join(
            crate.DOLocalRotate(spinUp, riseDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InCubic)
        );

        // === SHAKE AT TOP ===
        seq.Append(
            crate.DOShakePosition(
                shakeDuration,
                shakeStrength,
                shakeVibrato,
                90f,
                false,
                true
            )
        );

        // === LOWER + SPIN (gradual) ===
        seq.Append(
            crate.DOLocalMove(startPos, lowerDuration)
                .SetEase(Ease.OutCubic)
        );

        seq.Join(
            crate.DOLocalRotate(spinDown, lowerDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
        );

        seq.OnComplete(() =>
        {
            crate.localPosition = startPos;
            crate.localRotation = Quaternion.identity; // safety snap
            state = State.returning;

            UpgradeWeapon();
            AnimateLid(true);
        });
    }

    private void SwitchFX(bool on)
    {
        if (particleSystems == null || particleSystems.Count == 0) return;
        if (on)
        {
            foreach (var ps in particleSystems)
            {
                ps.Play();
            }
        }
        else
        {
            foreach (var ps in particleSystems)
            {
                ps.Stop();
            }
        }
    }


    private enum State
    {
        ready,
        recieving,
        returning,
        collection
    }
}
