using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CameraScript : MonoBehaviour
{
    int currentCamIndex = 0;

    WebCamTexture tex;

    public RawImage display;


    public void SwapCam_Clicked()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;

            //if tex is not null;
            // stop the webcam;
            //start the webcam;

            /* if (tex != null)
            {
                StopWebcam();
                StartStopCam_Clicked();
            } */
        }

    }

    public void StartStopCam_Clicked()
    {
        if (tex != null) // stop the cam
        {
            StopWebcam();
        }
        else //start the cam
        {
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            display.texture = tex;

            tex.Play();
        }

    }

    private void StopWebcam()
    {
        display.texture = null;
        tex.Stop();
        tex = null;
    }
}
