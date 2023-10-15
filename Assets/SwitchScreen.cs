using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
    public GameObject help_page;
    private GameObject current;

    private bool alert;
    private bool thumb_up;
    private float time = 0;
    private bool recip = false;
    private int helpPageNum = 0;

    

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
        changeScreen(screenSelector.teacher);
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

        if (thumb_up)
        {
            overlappingElement.transform.GetChild(3).gameObject.SetActive(true);
        }

        if (time < 1 && thumb_up)
        {
            time = time + Time.deltaTime;
        }
        else if (time > 1 && thumb_up)
        {
            thumb_up = false;
            time = 0;
            overlappingElement.transform.GetChild(3).gameObject.SetActive(false);
            time = 0;
        }
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

    public void thumb()
    {
        thumb_up = true;
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
