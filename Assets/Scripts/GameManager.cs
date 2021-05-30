using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public AudioClip soundPickUp;
    public AudioClip soundCollision;

    private AudioSource collisionAudio;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        collisionAudio = GetComponent<AudioSource>();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        KatamariController.propPickupEvent += OnPropPickup;
    }

    public void OnPropPickup(StickyProp prop)
    {
        PlayPickupSound();
    }

    public void OnPropRejection()
    {
        PlayCollisionSound();
    }

    public void PlayPickupSound()
    {
        collisionAudio.PlayOneShot(soundPickUp);
    }

    public void PlayCollisionSound()
    {
        // collisionAudio.PlayOneShot(soundCollision);
    }
}