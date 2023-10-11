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

public class RunClient2 : MonoBehaviour
{
    private string input;
    private Client client;

    public Button hangUpButton;
    public Button overlayButton;
    public Button handUpButton;
    public Button thumbsUpButton;

    public bool clientMode = false;
    
    public GameObject alertPopup;
    public GameObject thumbsUpPopup;


    private void Update()
    {
        if (clientMode)
        {
            //Hang up button
            Button hubtn = hangUpButton.GetComponent<Button>();
            hubtn.onClick.AddListener(HangUpButtonClicked);
            //Overlap button
            Button obtn = overlayButton.GetComponent<Button>();
            obtn.onClick.AddListener(Overlay);
            //Hand up button
            Button handbtn = handUpButton.GetComponent<Button>();
            handbtn.onClick.AddListener(ClientSendHandUp);
            //Thumbs up button
            Button thumbbtn = thumbsUpButton.GetComponent<Button>();
            thumbbtn.onClick.AddListener(ClientSendThumbsUp);

            //alertPopup.SetActive(false);
            //thumbsUpPopup.SetActive(false);
        }
        
    }
    //Function for the Overlay
    //TODO
    public void Overlay()
    {
        print("its overlaying time");
    }

    
    //Alert popup, activated after receiving alert from teacher
    public void AlertPopup()
    {
        Debug.Log("alert popup yo");
        //This is where the error comes in
        //alertPopup.SetActive(true);
    }

    //Function closes connection after hang up button is clicked
    public void HangUpButtonClicked()
    {
        client.CloseConnection();
        print("Connection closed");
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
        //Could pass in gameobjects here???
        client = new Client(ip);
        print("Client has started");
    }

    public Client getClient()
    {
        return client;
    }

    //Need this function because couldn't call a method function directly on a button click
    //event for whatever reason

    public void ClientSendHandUp()
    {
        client.SendMessage("hand");
    }

    //Sending a thumbs up
    public void ClientSendThumbsUp()
    {
        client.SendMessage("thumbs");
    }

    //Call the CloseConnection method for the Client
    public void CloseClientConnection()
    {
        getClient().CloseConnection();
    }
}

public class Client
{
    private TcpClient client;
    private Thread receiveThread;
    private NetworkStream stream;
    private bool threadRunning = true;

    //New client object, ip address is passed in
    public Client(string ip)
    {
        //Use port 11111 to connect
        client = new TcpClient(ip, 11111);
        stream = client.GetStream();
        receiveThread = new Thread(new ThreadStart(ConnectionThread));
        receiveThread.Start();
    }


    //Keeping the connection alive to read inputs from the trainer
    void ConnectionThread()
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
                
                //Alert popup
                if (response == "alert")
                {
                    //RunClient2.AlertPopup();
                }
                else if (response == "thumbs")
                {
                    //Thumbs up popup
                }
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
        //Need this or else the thread closed with an error wasn't sure why
        threadRunning = false;
        receiveThread.Abort();
        //Close stream and client object
        stream.Close();
        client.Close();
    }
}
