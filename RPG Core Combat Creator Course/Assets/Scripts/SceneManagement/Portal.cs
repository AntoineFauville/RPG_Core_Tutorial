using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.SceneManagement;

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

        yield return fader.FadeOut(fadeOutTime);
        yield return SceneManager.LoadSceneAsync(_sceneToLoad);

        Portal otherPortal = GetOtherPortal();
        UpdatePlayer(otherPortal);

        yield return new WaitForSeconds(fadeWaitTime);
        yield return fader.FadeIn(fadeOutTime);

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
