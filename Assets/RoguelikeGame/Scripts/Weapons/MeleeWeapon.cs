using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public enum MeleeDamageAreaShapes { Box, Sphere }

    [Header("DamageArea")]
    public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Box;
    public Vector3 AreaSize = new Vector3(1, 1);
    public Vector3 AreaOffset = new Vector3(1, 0);

    [Header("Damage Area Timing")]
    public float InitialDelay = 0f;
    public float ActiveDuration = 1f;

    [Header("Damage Caused")]
    public LayerMask TargetLayerMask;
    public int DamageCaused = 10;
    public DamageOnTouch.KnockbackStyles Knockback;
    public Vector2 KnockbackForce = new Vector2(10, 2);
    public float InvincibilityDuration = 0.5f;
    public bool CanDamageOwner = false;

    protected Collider _damageAreaCollider;
    protected bool _attackInProgress = false;
    protected Color _gizmosColor;
    protected Vector3 _gizmoSize;
    protected BoxCollider _boxCollider;
    protected SphereCollider _sphereCollider;
    protected Vector3 _gizmoOffset;
    protected DamageOnTouch _damageOnTouch;
    protected GameObject _damageArea;

    public override void Initialization()
    {
        base.Initialization();

        if (_damageArea == null)
        {
            CreateDamageArea();
            DisableDamageArea();
        }
        if (Owner != null)
        {
            _damageOnTouch.Owner = Owner.gameObject;
        }
    }

    protected virtual void CreateDamageArea()
    {
        _damageArea = new GameObject();
        _damageArea.name = $"{this.name}DamageArea";
        _damageArea.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        _damageArea.transform.SetParent(this.transform);
        _damageArea.layer = this.gameObject.layer;

        if (DamageAreaShape == MeleeDamageAreaShapes.Box)
        {
            _boxCollider = _damageArea.AddComponent<BoxCollider>();
            _boxCollider.center = AreaOffset;
            _boxCollider.size = AreaSize;
            _damageAreaCollider = _boxCollider;
            _damageAreaCollider.isTrigger = true;
        }

        if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
        {
            _sphereCollider = _damageArea.AddComponent<SphereCollider>();
            _sphereCollider.center = this.transform.position + this.transform.rotation * AreaOffset;
            _sphereCollider.radius = AreaSize.x / 2;
            _damageAreaCollider = _sphereCollider;
            _damageAreaCollider.isTrigger = true;
        }

        Rigidbody rigidbody = _damageArea.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();
        _damageOnTouch.SetGizmoSize(AreaSize);
        _damageOnTouch.SetGizmoOffset(AreaOffset);
        _damageOnTouch.TargetLayerMask = TargetLayerMask;
        _damageOnTouch.DamageCaused = DamageCaused;
        _damageOnTouch.DamageCausedKnockbackType = Knockback;
        _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
        _damageOnTouch.InvincibilityDuration = InvincibilityDuration;

        if (!CanDamageOwner)
        {
            _damageOnTouch.IgnoreGameObject(Owner.gameObject);
        }
    }

    /// <summary>
    /// Enables the damage area.
    /// </summary>
    protected virtual void EnableDamageArea()
    {
        if (_damageAreaCollider != null)
        {
            _damageAreaCollider.enabled = true;
        }
    }


    /// <summary>
    /// Disables the damage area.
    /// </summary>
    protected virtual void DisableDamageArea()
    {
        if (_damageAreaCollider != null)
        {
            Debug.Log($"{this.GetType()}.DisableDamageArea: Disabling _damageAreaCollider.", gameObject);
            _damageAreaCollider.enabled = false;
        }
        else
        {
            Debug.LogError($"{this.GetType()}.DisableDamageArea: _damageAreaCollider not found.", gameObject);
        }
    }

    public override void WeaponUse()
    {
        base.WeaponUse();
        StartCoroutine(MeleeWeaponAttackCoroutine());
    }

    protected virtual IEnumerator MeleeWeaponAttackCoroutine()
    {
        if (_attackInProgress) { yield break; }

        _attackInProgress = true;
        yield return new WaitForSeconds(InitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableDamageArea();
        _attackInProgress = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            DrawGizmos();
        }
    }

    protected virtual void DrawGizmos()
    {
        switch (DamageAreaShape)
        {
            case MeleeDamageAreaShapes.Box:
                Gizmos.DrawWireCube(this.transform.position + AreaOffset, AreaSize);
                break;

            case MeleeDamageAreaShapes.Sphere:
                Gizmos.DrawWireSphere(this.transform.position + AreaOffset, AreaSize.x / 2);
                break;
        }
    }
}
