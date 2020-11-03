using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.SceneManagement;
using RPG.Control;

public class Portal : MonoBehaviour
{
    enum DestinationIdentifier
    {
        A, B, C, D, E, F, G
    }

    [SerializeField] private int _sceneToLoad = -1;
    [SerializeField] private Transform _portalSpawnPoint;
    [SerializeField] private DestinationIdentifier _destinationIdentifier;

    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeWaitTime = 1f;
    [SerializeField] private float fadeInTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Transition());
        }
    }

    private IEnumerator Transition()
    {
        if (_sceneToLoad < 0)
        {
            Debug.Log("Scene to load not set");
            yield break;
        }
        
        DontDestroyOnLoad(gameObject);

        Fader fader = FindObjectOfType<Fader>();
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

        PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.enabled = false;


        yield return fader.FadeOut(fadeOutTime);

        // Save Level
        wrapper.Save();

        yield return SceneManager.LoadSceneAsync(_sceneToLoad);

        PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        newPlayerController.enabled = false;

        Portal otherPortal = GetOtherPortal();

        // Load Level
        wrapper.Load();

        UpdatePlayer(otherPortal);

        // Save Again with the new position
        wrapper.Save();

        yield return new WaitForSeconds(fadeWaitTime);
        fader.FadeIn(fadeInTime);

        newPlayerController.enabled = true;

        Destroy(gameObject);
    }

    private void UpdatePlayer(Portal otherPortal)
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<NavMeshAgent>().Warp(otherPortal._portalSpawnPoint.position);
        player.transform.rotation = _portalSpawnPoint.transform.rotation;
    }

    private Portal GetOtherPortal()
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (portal == this) continue;
            if (portal._destinationIdentifier != _destinationIdentifier) continue;

            return portal;
        }

        return null;
    }
}
