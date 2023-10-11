using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public enum screenSelector
{
    login,
    student,
    teacher
}
//Please work
public class SwitchScreen : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject teacherScreen;
    public GameObject studentScreen;
    public GameObject callingSession;
    public GameObject overlappingElement;
    public GameObject buttonCover;
    public GameObject IPTextBox;
    public TextMeshProUGUI IPDisplay;
    private GameObject current;
    private bool alert;
    private float time = 0;


    public void Start()
    {
        callingSession.SetActive(false);
        teacherScreen.SetActive(false);
        studentScreen.SetActive(false);
        buttonCover.SetActive(false);
        current = loginScreen;
        current.SetActive(true);
    }


    public void teacherMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen(screenSelector.teacher);
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
    }

    public void Alert()
    {
        alert = true;
    }

    public void test()
    {
        print("working");
    }

    private void Update()
    {
        if (alert)
        {
            overlappingElement.transform.GetChild(2).gameObject.SetActive(true);
        }

        if (time < 1 && alert)
        {
            time = time + Time.deltaTime;
        } else if (time > 1 && alert)
        {
            alert = false;
            time = 0;
            overlappingElement.transform.GetChild(2).gameObject.SetActive(false);
            time = 0;
        }
    }

}
