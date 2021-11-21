using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    private float xAxisClamp;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.Locked;
            RotateCamera(3);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void RotateCamera(float mouseSensitivity)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotateAmountX = mouseX * mouseSensitivity;
        float rotateAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotateAmountY;

        Vector3 rotatePlayer = Camera.main.transform.rotation.eulerAngles;

        rotatePlayer = new Vector3(rotatePlayer.x - rotateAmountY, rotatePlayer.y + rotateAmountX, 0);

        if (xAxisClamp > 90)
        {
            xAxisClamp = 90;
            rotatePlayer = new Vector3(90, rotatePlayer.y, rotatePlayer.z);
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90;
            rotatePlayer = new Vector3(270, rotatePlayer.y, rotatePlayer.z);

        }
        Camera.main.transform.rotation = Quaternion.Euler(rotatePlayer);
    }
}
