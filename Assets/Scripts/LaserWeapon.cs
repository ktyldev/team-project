﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LaserWeapon : Weapon
{
    [Tooltip("how often damage is dealt, per second")]
    public float damageFrequency = 5f;

    public GameObject laser;
    public GameObject glow;

    private float _damageInterval;
    private Laser _laser;
    private SpriteRenderer _glow;

    protected override string SFXName => GameConstants.SFX_LomgLaser;

    private void Awake()
    {
        _damageInterval = 1f / damageFrequency;
    }

    protected override void Start()
    {
        base.Start();

        _glow = glow.GetComponent<SpriteRenderer>();
        _glow.color = Color.clear;
        _laser = laser.GetComponent<Laser>();
        _laser.SetActive(false);
    }

    protected override bool CancelSFX() => !IsFiring;

    protected override IEnumerator DoFire(Func<bool> getFiring)
    {
        _laser.SetActive(true);
        _laser.SetColour(Ship.WeaponColour);
        _glow.color = Ship.WeaponColour;

        float elapsed = 0;
        float chunk = damage * _damageInterval;

        //int cycles = 0;
        while (getFiring())
        {
            //if (cycles % 20 == 0)
            //{
            PlaySFX(true, true);
            //}
            //cycles++;

            yield return new WaitForEndOfFrame();

            elapsed += Time.deltaTime;
            if (elapsed < _damageInterval)
                continue;

            // go back so the interval check will fail next time
            elapsed -= _damageInterval;
            Ship.Heat.Add(heat);

            // damage interval has passed, look for a health component and smack it
            var health = _laser.Occluder?.GetComponent<Health>();
            if (health == null)
                continue;

            health.TakeDamage(chunk);
        }

        _laser.SetActive(false);
        _glow.color = Color.clear;
    }
}
