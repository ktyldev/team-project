﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipMovement : MonoBehaviour
{
    // TODO: 
    // make movement more sophisticated. it should be faster for the ship
    // to accelerate forwards, not equal in every direction
    public float acceleration;
    [Range(0, 1)]
    public float rotationSpeed;
    public GameObject[] weapons;
    [Range(0, 1)]
    public float weaponRotationSpeed;

    private Rigidbody2D _rb;
    private IInputProvider _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); 
    }

    private void Start()
    {
        var ship = GetComponent<Ship>();
        _input = ship.InputProvider;
    }

    private void FixedUpdate()
    {
        Movement();
        RotateShip();
        RotateWeapons();
    }

    private void Movement()
    {
        _rb.AddForce(_input.AccelerationDir * acceleration);
    }

    private void RotateShip()
    {
        var aimDir = (_input.TargetPosition - (Vector2)transform.position).normalized;
        var lookDir = Vector3.Lerp(transform.up, aimDir, rotationSpeed);

        transform.rotation *= Quaternion.FromToRotation(transform.up, lookDir);
    }

    private void RotateWeapons()
    {
        var targetPos = _input.TargetPosition;

        foreach (var w in weapons)
        {
            var aimDir = (targetPos - (Vector2)w.transform.position).normalized;

            var target = Quaternion.LookRotation(transform.forward, aimDir);

            w.transform.rotation = Quaternion.Lerp(w.transform.rotation, target, weaponRotationSpeed);
        }
    }
}
