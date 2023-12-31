using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using System;
using UnityEngine.InputSystem.LowLevel;

public class RunClient2 : MonoBehaviour
{
    private string input;
    private Client client;

    public Button hangUpButton;
    //public Button overlayButton;
    public Button handUpButton;
    public Button thumbsUpButton;
    public Button exitOverlayButton;

    public bool clientMode = false;
    private bool alert = false;
    private bool thumbsUp = false;
    private float time = 0;

    public GameObject alertPopup;
    public GameObject thumbsUpPopup;
    public GameObject overlappingElement;
    //public GameObject exitOverlayPopup;

    public SwitchScreen screen;
    public CameraScript cameraScript;

    //Create this to keep track of if something has just been sent
    public bool justSent = false;

    

    private void Update()
    {
        //If the client has started
        if (clientMode)
        {
            //Button obtn = exitOverlayButton.GetComponent<Button>();
            //obtn.onClick.AddListener(Overlay);
            Button hubtn = hangUpButton.GetComponent<Button>();
            hubtn.onClick.AddListener(HangUpButtonClicked);
            //Hand up button
            Button handbtn = handUpButton.GetComponent<Button>();
            handbtn.onClick.AddListener(ClientSendHandUp);
            //Thumbs up button
            Button thumbbtn = thumbsUpButton.GetComponent<Button>();
            thumbbtn.onClick.AddListener(ClientSendThumbsUp);
            
            if (client.returnGameState() == "alert")
            {
                //Call alertPopup then reset the game state
                AlertPopup();
                client.changeGameState();
            } else if (client.returnGameState() == "thumbs")
            {
                ThumbsUpPopup();
                client.changeGameState();
            } else if (client.returnGameState() == "disconnected")
            {
                screen.changeScreen(screenSelector.login);
                screen.endCall();
                client.changeGameState();
            } else if (client.returnGameState() == "exit")
            {
                //Receives an exit when the overlay is finished, renable buttons and change the screen
                enableButtons();
                screen.exitOverlay();
                client.changeGameState();
            }

            if (screen.currentScreen == "overlay")
            {
                disableButtons();
            } else
            {
                enableButtons();
            }
        }

        //Need this so the button doesn't activate 2000 times
        if (justSent && time < 0.1)
        {
            time = time + Time.deltaTime;
        }
        else if (justSent && time > 0.1)
        {
            justSent = false;
            time = 0;
        }

        //Alert function
        if (alert)
        {
            overlappingElement.transform.GetChild(2).gameObject.SetActive(true);
        }

        if (time < 1 && alert)
        {
            time = time + Time.deltaTime;
        }
        else if (time > 1 && alert)
        {
            alert = false;
            time = 0;
            overlappingElement.transform.GetChild(2).gameObject.SetActive(false);
            time = 0;
            //Alert popup is finished, now call overlay
            screen.OverlayMode();
            disableButtons();
        }

        if (thumbsUp)
        {
            thumbsUpPopup.SetActive(true);
        }
        if (time < 1 && thumbsUp)
        {
            time = time + Time.deltaTime;
        }
        else if (time > 1 && thumbsUp)
        {
            thumbsUp = false;
            time = 0;
            thumbsUpPopup.SetActive(false);
            time = 0;
        }

    }

    public void disableButtons()
    {
        thumbsUpButton.enabled = false;
        handUpButton.enabled = false;
    }

    public void enableButtons()
    {
        handUpButton.enabled = true;
        thumbsUpButton.enabled = true;
    }
    
    //Alert popup, activated after receiving alert from teacher
    public void AlertPopup()
    {
        alert = true;
    }

    public void ThumbsUpPopup()
    {
        Debug.Log("thumbs up");
        thumbsUp = true;
    }

    //Function closes connection after hang up button is clicked
    public void HangUpButtonClicked()
    {
        if (!justSent)
        {
            client.CloseConnection();
            print("Connection closed");
        }
        justSent = true;
    }

    //Function to read the IP input
    public void ReadStringInput(string ip)
    {
        clientMode = true;
        //Delete all print statements before final product - just for testing
        input = ip;
        print(ip);
        IPAddress serverIpAddress;
        if (IPAddress.TryParse(ip, out serverIpAddress))
        {
            print("IPAddress object: " + serverIpAddress.ToString());
        }
        else
        {
            print("Invalid IP address format");
        }
        //Create new client instance
        client = new Client(ip);
        Debug.Log("Client has started");
        cameraScript.StartStopCam_Clicked();
        screen.showCameraScreen();
    }

    public Client getClient()
    {
        return client;
    }

    //Need this function because couldn't call a method function directly on a button click
    //event for whatever reason

    public void ClientSendHandUp()
    {
        if (!justSent)
        {
            client.SendMessage("hand");
        }
        justSent = true;
    }

    //Sending a thumbs up
    public void ClientSendThumbsUp()
    {
        if (!justSent)
        {
            client.SendMessage("thumbs");
        }
        justSent = true;
    }

    //Call the CloseConnection method for the Client
    public void CloseClientConnection()
    {
        getClient().CloseConnection();
        clientMode = false;
    }
}



public class Client
{
    private TcpClient client;
    private Thread receiveThread;
    private NetworkStream stream;
    private bool threadRunning = true;
    //This string keeps track of the game state, like if an alert is pressed
    public string gameState = "";


    //New client object, ip address is passed in
    public Client(string ip)
    {
        //Use port 11111 to connect
        client = new TcpClient(ip, 11111);
        stream = client.GetStream();
        receiveThread = new Thread(ConnectionThread);
        receiveThread.Start(gameState);
    }


    //Keeping the connection alive to read inputs from the trainer
    public void ConnectionThread(object state)
    {
        //Keep listening for any inputs
        while (threadRunning)
        {
            try
            {
                Debug.Log("In while loop");
                byte[] data = new byte[1024];
                int bytesRead = stream.Read(data, 0, data.Length);
                string response = Encoding.ASCII.GetString(data, 0, bytesRead);
                Debug.Log(response);
                gameState = response;
                response = "";
            } catch (SocketException e) when (e.ErrorCode == 10004)
            {
                //Do nothing idk why this was being thrown
                Debug.Log("Exception thrown " + e.ToString());
            } catch (ThreadAbortException e)
            {
                //Do nothing
                Debug.Log("Exception thrown " + e.ToString());
            }
            //TODO end of communication, switch back to login screen
        }
    }

    //Returns the gameState
    public string returnGameState()
    {
        return gameState;
    }

    //Changes the gameState back 
    public void changeGameState()
    {
        gameState = "";
    }

    //Send message to the trainer/server
    public void SendMessage(string message)
    {
        //Package the string to send to the trainer
        Debug.Log("sent " + message);
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    //This will be triggered by the disconnect button
    public void CloseConnection()
    {
        gameState = "disconnected";
        //Need this or else the thread closed with an error wasn't sure why
        threadRunning = false;
        receiveThread.Abort();
        //Close stream and client object
        stream.Close();
        client.Close();
    }
}
