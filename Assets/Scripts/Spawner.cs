using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class Spawner : MonoBehaviour
{

    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public const byte DestroyObjectEventCode = 1;

    public float minSpawnDelay = 0.4f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 15f;
    public float maxForce = 19f;

    public float maxLifetime = 5f;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);
        while (enabled)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject prefab = fruitPrefabs[UnityEngine.Random.Range(0, fruitPrefabs.Length)];
                Vector3 position = new Vector3();
                position.x = UnityEngine.Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                position.y = UnityEngine.Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
                position.z = UnityEngine.Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

                Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minAngle, maxAngle));

                GameObject fruit = PhotonNetwork.Instantiate(prefab.name, position, rotation);
                StartCoroutine(DestroyFruit(fruit));

                float force = UnityEngine.Random.Range(minForce, maxForce);
                fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);
            }


            yield return new WaitForSeconds(UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    private IEnumerator DestroyFruit(GameObject fruit)
    {
        yield return new WaitForSeconds(5f);

        if (fruit)
        {
            PhotonNetwork.Destroy(fruit);
        }
    }
}
