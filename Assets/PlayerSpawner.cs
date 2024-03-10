using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public Transform[] spawnPoints;
    public GameObject usernameTextPrefab;
    private GameObject playerInstance;

    private void Start()
    {
        int randomNumber = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomNumber];
        GameObject playerToSpawn = PlayerPrefab;
        playerInstance = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity);

        // Create a new GameObject for the username text and attach it to the player
        GameObject usernameText = Instantiate(usernameTextPrefab, playerInstance.transform);
        usernameText.transform.localPosition = new Vector3(0f, 1.5f, 0f);

        // Get the TextMeshPro component and set the player's name
        TextMeshPro textMeshPro = usernameText.GetComponent<TextMeshPro>();
        textMeshPro.text = PhotonNetwork.NickName;

    }
}
