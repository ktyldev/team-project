﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public GameObject shipExplosion;
    public GameObject bulletExplosion;
    public Transform bulletParent;
    public GameObject ship;
    private Ship _ship;
    public Ship Ship
    {
        get
        {
            if (_ship == null)
            {
                _ship = ship.GetComponent<Ship>();
            }

            return _ship;
        }
    }

    private void Awake()
    {
        if (Instance != null)
            throw new System.Exception();

        Instance = this;
    }

    public void Restart(float delay = 0) => Instance.StartCoroutine(RestartScene(delay));
    private IEnumerator RestartScene(float delay)
    {
        var currentScene = SceneManager.GetActiveScene();

        yield return new WaitForSecondsRealtime(delay);

        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
