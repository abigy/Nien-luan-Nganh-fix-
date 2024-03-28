using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPower : MonoBehaviour
{
    public GameObject powerPrefaps;
    private float spawnPosX = 8.0f;
    private float spawnPosZ = 11.0f;
    private float powerCount;

    void Start()
    {
        Instantiate(powerPrefaps, SpawnSomeThing(), powerPrefaps.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        powerCount = FindObjectsOfType<PowerUp>().Length;
        if (powerCount == 0)
        {
            Instantiate(powerPrefaps, SpawnSomeThing(), powerPrefaps.transform.rotation);
        }
    }

    private Vector3 SpawnSomeThing()
    {
        float spawnX = Random.Range(-spawnPosX, spawnPosX);
        float spawnZ = Random.Range(-spawnPosZ, spawnPosZ);
        Vector3 randomSpawn = new Vector3(spawnX, 1, spawnZ);
        return randomSpawn;
    }
}
