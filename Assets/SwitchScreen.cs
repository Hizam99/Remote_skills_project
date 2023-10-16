using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Device;

public enum screenSelector
{
    login,
    student,
    teacher
}

public class SwitchScreen : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject teacherScreen;
    public GameObject studentScreen;
    public GameObject callingSession;
    public GameObject overlappingElement;
    public GameObject buttonCover;
    public GameObject IPTextBox;
    public GameObject displayCamera;
    public TextMeshProUGUI IPDisplay;
    public GameObject help_page;
    private GameObject current;

    private bool recip = false;
    private int helpPageNum = 0;

    public GameObject selfVideoScreen;
    public GameObject otherVideoScreen;

    //To keep track of the current screen
    private string currentScreen = "self";

    public void Start()
    {
        callingSession.SetActive(false);
        teacherScreen.SetActive(false);
        studentScreen.SetActive(false);
        buttonCover.SetActive(false);
        current = loginScreen;
        current.SetActive(true);
    }

    public void showCameraScreen()
    {
        displayCamera.SetActive(true);
    }

    public void teacherMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen(screenSelector.teacher);
    }

    public void changeCameraScreen()
    {
        if (currentScreen == "self")
        {
            //Switch video screen being displayed
            selfVideoScreen.SetActive(false);
            otherVideoScreen.SetActive(true);
            currentScreen = "other";
        } else
        {
            selfVideoScreen.SetActive(true);
            otherVideoScreen.SetActive(false);
            currentScreen = "self";
        }
    }

    //This function is to show the button cover and the IP text box
    //Activated by pressing student or teacher button
    public void hideButtons()
    {
        buttonCover.SetActive(true);
    }

    //This function is to show the IP text box
    //Activated by pressing the student button so they can enter the IP address of the teacher
    public void showIPTextBox()
    {
        IPTextBox.SetActive(true);
    }

    public void changeScreen(screenSelector screen)
    {
        current.SetActive(false);

        switch (screen)
        {
            case screenSelector.login:
                current = loginScreen;
                break;
            case screenSelector.student:
                current = studentScreen;
                break;
            case screenSelector.teacher:
                current = teacherScreen;
                break;
        }

        current.SetActive(true);
    }

    public void studentMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen(screenSelector.student);
    }

    public void endCall()
    {
        callingSession.SetActive(false);
        overlappingElement.SetActive(false);
        changeScreen(screenSelector.login);
        displayCamera.SetActive(false);
    }

    public void recipe()
    {
        if (recip == false)
        {
            recip = true;
        } else
        {
            recip = false;
        }

        overlappingElement.transform.GetChild(5).gameObject.SetActive(recip);
    }

    public void instructionPage(string action)
    {
        switch (action)
        {
            case "open":
                help_page.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "close":
                help_page.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case "flip left":
                help_page.transform.GetChild(1).GetChild(helpPageNum).gameObject.SetActive(false);
                if (helpPageNum == 0)
                {
                    helpPageNum = 2;
                } else
                {
                    helpPageNum -= 1;
                }
                help_page.transform.GetChild(1).GetChild(helpPageNum).gameObject.SetActive(true);
                break;
            case "flip right":
                help_page.transform.GetChild(1).GetChild(helpPageNum).gameObject.SetActive(false);
                if (helpPageNum == 2)
                {
                    helpPageNum = 0;
                }
                else
                {
                    helpPageNum += 1;
                }
                help_page.transform.GetChild(1).GetChild(helpPageNum).gameObject.SetActive(true);
                break;
        }
    }
}
