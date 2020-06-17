using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    private Vector3 spawnPos = new Vector3(17f,0.5f,0f);
    private float startDelay = 2f;
    private float repeatRate = .8f;
    private PlayerController playerControllerScript;
    private System.Random rnd;
    private int lastLane = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        InvokeRepeating("SpawnObstacle", startDelay, repeatRate);
        rnd = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObstacle()
    {
        if (!playerControllerScript.gameOver && !playerControllerScript.levelDone)
        {
            Vector3 iPos = spawnPos;
            int lane = (rnd.Next(100) % 3);
            while (lane == lastLane) lane = (rnd.Next(100) % 3);
            lastLane = lane;
            switch (lane)
            {
                case 0:
                    iPos.z = -2.5f;
                    break;
                case 1:
                    iPos.z = 0f;
                    break;
                case 2:
                    iPos.z = 2.5f;
                    break;
            }

            Instantiate(obstaclePrefab, iPos, obstaclePrefab.transform.rotation);
        }
    }
}
