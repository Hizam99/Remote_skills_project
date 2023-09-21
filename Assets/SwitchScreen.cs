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
        current = loginScreen;
        current.SetActive(true);
    }

    public void teacherMode()
    {
        callingSession.SetActive(true);
        current.SetActive(false);
        current = teacherScreen;
        overlappingElement.SetActive(true);
        changeScreen();
    }

    public void changeScreen()
    {
        current.SetActive(true);
    }

    public void studentMode()
    {
        callingSession.SetActive(true);
        current.SetActive(false);
        current = studentScreen;
        overlappingElement.SetActive(true);
        changeScreen();
    }

    public void endCall()
    {
        callingSession.SetActive(false);
        overlappingElement.SetActive(false);
        current.SetActive(false);
        current = loginScreen;
        changeScreen();
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
