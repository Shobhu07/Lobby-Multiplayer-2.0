using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    PhotonView view;

    Vector2 movement;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void Update()
    {

        if(view.IsMine)
        {
            // Input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
           
        }
      
    }

    void FixedUpdate()
    {
        if(view.IsMine)
        {
            // Movement
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
       
    }
}
