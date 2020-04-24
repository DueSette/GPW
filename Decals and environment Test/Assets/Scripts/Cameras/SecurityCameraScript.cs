﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraScript : MonoBehaviour
{
    private float startAngY;
    private float time;

    public bool camMove;
    public float camSpeed;

    private int maxDistance;
    private int minDistance;


    private void Start()
    {
        camMove = true;

        // Getting the starting position of the camera;
        startAngY = this.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (camMove)
        {
            time += (Time.deltaTime * camSpeed);
            if (time > 360)
            {
                time -= 360;
            }

            float newRot = Mathf.Cos(time);
            newRot *= -1;
            newRot += 1;

            newRot *= 45;
            newRot += startAngY;

            this.transform.eulerAngles = new Vector3(0, newRot, 0);
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Drawing Cone Of Movement
        Vector3 camPos = this.transform.position;
        Vector3 camStartLook = this.transform.position + this.transform.forward;
        Vector3 camEndLook = this.transform.position + (Quaternion.AngleAxis(90, Vector3.up) * this.transform.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(camPos, camStartLook);
        Gizmos.DrawLine(camPos, camEndLook);
        Gizmos.DrawLine(camStartLook, camEndLook);
    }
}
