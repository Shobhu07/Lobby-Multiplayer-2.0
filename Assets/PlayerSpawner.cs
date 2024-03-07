using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {
        int randomNumber = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomNumber];
        GameObject playerToSpawn = PlayerPrefab;
        PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position,Quaternion.identity);
    }
}
