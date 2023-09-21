using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScreen : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject teacherScreen;
    public GameObject studentScreen;
    public GameObject callingSession;
    public GameObject overlappingElement;
    private GameObject current;


    public void Start()
    {
        callingSession.SetActive(false);
        teacherScreen.SetActive(false);
        studentScreen.SetActive(false);
        current = loginScreen;
        current.SetActive(true);
    }

    public void teacherMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen("teacher");
    }

    public void changeScreen(string screen)
    {
        current.SetActive(false);

        if (screen == "login")
        {
            current = loginScreen;
        } else if (screen == "student")
        {
            current = studentScreen;
        } else if (screen == "teacher")
        {
            current = teacherScreen;
        }

        current.SetActive(true);
    }

    public void studentMode()
    {
        callingSession.SetActive(true);
        overlappingElement.SetActive(true);
        changeScreen("student");
    }

    public void endCall()
    {
        callingSession.SetActive(false);
        overlappingElement.SetActive(false);
        changeScreen("login");
    }

    public void Alert()
    {
        overlappingElement.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void test()
    {
        print("working");
    }

}
