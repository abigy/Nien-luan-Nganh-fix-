using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPower : MonoBehaviour
{
    public GameObject[] powerPrefaps;
    private float spawnPosX = 8.0f;
    private float spawnPosZ = 11.0f;
    private float powerCount;
    public int secondToSpawn;
    public bool readyToSpawn = true;


    /*void Start()
    {
        int powerRandom = Random.Range(0, powerPrefaps.Length);
        Instantiate(powerPrefaps[powerRandom], SpawnSomeThing(), powerPrefaps[powerRandom].transform.rotation);
    }*/

    // Update is called once per frame
    void Update()
    {
        powerCount = FindObjectsOfType<PowerUp>().Length;
        if (powerCount == 0 && readyToSpawn)
        {
            int powerRandom = Random.Range(0, powerPrefaps.Length);
            Instantiate(powerPrefaps[powerRandom], SpawnSomeThing(), powerPrefaps[powerRandom].transform.rotation);
            readyToSpawn = false;
            StartCoroutine(countDownToSpawn());
        }
        
    }

    private Vector3 SpawnSomeThing()
    {
        float spawnX = Random.Range(-spawnPosX, spawnPosX);
        float spawnZ = Random.Range(-spawnPosZ, spawnPosZ);
        Vector3 randomSpawn = new Vector3(spawnX, 20, spawnZ);
        return randomSpawn;
    }

    IEnumerator countDownToSpawn()
    {
        yield return new WaitForSeconds(secondToSpawn);
        readyToSpawn = true;
    }
}
