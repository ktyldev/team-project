﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // this means something different for each weapon type - leave it to the
    // implementor
    public float damage;
    public float heat;
    public Color colour;

    protected abstract string SFXName { get;}
    protected bool IsFiring { get; private set; }
    protected Ship Ship { get; private set; }

    private bool _stop;

    protected virtual void Start()
    {
        Ship = GetComponentInParent<Ship>();
    }

    public void Fire(IInputProvider input)
    {
        // can't fire a weapon that's already firing
        if (IsFiring)
            return;

        StartCoroutine(DoFire(input));
    }

    private IEnumerator DoFire(IInputProvider input)
    {
        IsFiring = true;

        yield return DoFire(() => input.IsFiring && !_stop);

        IsFiring = false;
        _stop = false;
    }

    protected abstract IEnumerator DoFire(Func<bool> getFiring);
    protected virtual bool CancelSFX() => false;
    
    public void PlaySFX(bool loop = false, bool singular = true)
    {
        //Game.Instance.Audio.
        if (!ShouldPlaySFX)
            return;

        Game.Instance.Audio.PlaySFX(SFXName, CancelSFX, false, loop, singular);
    }

    public bool ShouldPlaySFX { get; set; }

    public void Stop()
    {
        _stop = true;
    }
}
