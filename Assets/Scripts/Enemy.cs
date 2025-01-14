﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float rotationSpeed;
    public float rotationTime = 0.5f;
    public float dashSpeed;
    public float dashTime = 2.0f;
    public float damage = 1;
    [Range(0, 1)]
    public float pickupDropChance = 0.1f;

    private Health _health;
    private Transform _target;
    private LightSource _source;
    private Rigidbody2D _rb;

    private bool _hidden = true;

    private static List<Enemy> _all = new List<Enemy>();
    public static IEnumerable<Enemy> All => _all;

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.death.AddListener(Explode);

        _rb = GetComponent<Rigidbody2D>();
        _all.Add(this);
    }

    private void Start()
    {
        var ship = Game.Instance.Ship;

        _target = ship.transform;
        _source = ship.LightSource;

        StartCoroutine(WaitInShadow());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var ship = collision.gameObject.GetComponent<Ship>();
        if (ship == null)
            return;

        var health = ship.GetComponent<Health>();
        health.TakeDamage(damage);
        Explode();
    }

    private IEnumerator WaitInShadow()
    {
        while (_hidden && Game.IsRunning)
        {
            var player = Game.Instance.Ship;
            if (Vector3.Distance(transform.position, player.transform.position) > 50)
            {
                Destroy(gameObject);
            }

            _hidden = LightEngine.Instance[_source].ContainsPoint(transform.position);
            yield return new WaitForSeconds(0.5f);
        }

        yield return Dash();
    }

    private IEnumerator Dash()
    {
        // turn to face player
        var toTarget = (_target.position - transform.position).normalized;
        var targetRotation = Quaternion.FromToRotation(transform.up, toTarget);

        var start = Time.time;
        float elapsed = 0;
        while (elapsed < 1)
        {
            elapsed = (Time.time - start) / rotationTime;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                elapsed);

            yield return new WaitForEndOfFrame();
        }

        // accelerate a bunch
        start = Time.time;
        elapsed = 0;
        while (elapsed < 1)
        {
            elapsed = (Time.time - start) / dashTime;
            _rb.AddForce(toTarget * dashSpeed);

            yield return new WaitForFixedUpdate();
        }

        // TODO: come around for another try?
    }

    private void Explode()
    {
        Explosion.New(Game.Instance.shipExplosion, transform.position);

        if (Random.Range(0.0f, 1.0f) < pickupDropChance)
        {
            Instantiate(Game.Instance.healthPickup, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _all.Remove(this);
    }
}
