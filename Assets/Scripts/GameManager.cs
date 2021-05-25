using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public AudioClip soundPickUp;
    public AudioClip soundCollision;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public void OnPropPickup(GameObject prop)
    {
        PlayPickupSound();
        // use the prop in the UI
    }

    public void OnPropRejection()
    {
        PlayCollisionSound();
    }

    public void PlayPickupSound()
    {
        _audioSource.PlayOneShot(soundPickUp);
    }

    public void PlayCollisionSound()
    {
        // _audioSource.PlayOneShot(soundCollision);
    }
}