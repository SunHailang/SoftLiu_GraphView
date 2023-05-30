using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveMono : MonoBehaviour
{
    public float Speed = 100;
    
    private Rigidbody m_player = null;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GetComponent<Rigidbody>();
    }

    private bool draging = false;
    private float startPosX = 0;
    // Update is called once per frame
    void Update()
    {
        if (!draging && Input.GetMouseButtonDown(0))
        {
            draging = true;
            startPosX = Input.mousePosition.x;
        }

        if (draging)
        {
            float delta = Input.mousePosition.x - startPosX;
            this.transform.Rotate(Vector3.up, delta);
            startPosX = Input.mousePosition.x;
        }

        if (draging && Input.GetMouseButtonUp(0))
        {
            draging = false;
        }
        
        //m_player.Rotate(Vector3.up, );
        
        float vertical = Input.GetAxis("Vertical");
        vertical = vertical > 0 ? 1 : vertical < 0 ? -1 : 0;
        Vector3 forward = transform.forward.normalized;
        Vector3 dir = forward * vertical;
        float horizontal = Input.GetAxis("Horizontal");
        horizontal = horizontal > 0 ? -1 : horizontal < 0 ? 1 : 0;
        float angle = Mathf.Deg2Rad * 90f;
        Vector3 dir1 = new Vector3(forward.x * Mathf.Cos(angle) - forward.z * Mathf.Sin(angle),
            0,
            forward.x * Mathf.Sin(angle) + forward.z * Mathf.Cos(angle));
        dir1 = dir1.normalized * horizontal;
        Vector3 targetDir = (dir + dir1).normalized;

        m_player.velocity = targetDir * Speed;;
    }
}