using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Pig"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
