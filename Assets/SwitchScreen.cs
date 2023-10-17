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
using UnityEngine.UI;

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

    //To keep track of the current instruction page that the user is at, this is used to return to
    //the last page user accessed before closing the popup
    private bool recip = false;
    private int helpPageNum = 0;

    public GameObject selfVideoScreen;
    public GameObject otherVideoScreen;
    public RawImage selfVideoScreenImage;
    public RawImage otherVideoScreenImage;

    //To keep track of the current screen
    public string currentScreen = "self";


    //Intializes the app by deactivating all UI elements in the program and only showing the login screen
    //this is to prevent unnecessary UI elements from activating and displaying at the wrong time
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

    //This is the function to activate all the relevant UI elements inside the teacher screen, this includes
    //the header and buttons that the teacher can use
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

    public void OverlayMode()
    {
        currentScreen = "overlay";
        selfVideoScreen.SetActive(true);
        otherVideoScreen.SetActive(true);
        float alpha = 0.5f; //1 is opaque, 0 is transparent
        Color currColor = selfVideoScreenImage.color;
        currColor.a = alpha;
        selfVideoScreenImage.color = currColor;
        Color otherCurrColor = otherVideoScreenImage.color;
        otherCurrColor.a = alpha;
        otherVideoScreenImage.color = otherCurrColor;
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

    //This function changes the current screen to the selected screen with the respective UI elements
    // change Screen function is activated by pressing either the end call, student of teacher button
    // where the previously active screen would be deactivated and the new screen becomes activated
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

    //This is the function to activate all the relevant UI elements inside the student screen, this includes
    //the header and buttons that the student can use
    public void studentMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen(screenSelector.student);
    }


    //This is the visual side of the endCall function where the UI elements of the current calling session
    //screen is deactivated and the user is brought back to the login screen. This function only
    //handles the visual side of end call, the function side of the endCall (i.e. disconnect from server, etc)
    //is being handled at server and client script
    public void endCall()
    {
        callingSession.SetActive(false);
        overlappingElement.SetActive(false);
        changeScreen(screenSelector.login);
        displayCamera.SetActive(false);
    }

    //Function to activate and deactivate the recipe popup whenever the recipe button is pressed
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

    //Function to react to the actions related to the instruction page, there are 4 actions in total,
    //opening and closing the page is set to activate and deactivate related UI elements whenever it is called,
    //flip right and left displays the next and previous page inside the manual
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
