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
using Unity.WebRTC;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Unity.VisualScripting;

public class RunClient2 : MonoBehaviour
{
    private string input;
    private Client client;

    public Button hangUpButton;
    //public Button overlayButton;
    public Button handUpButton;
    public Button thumbsUpButton;
    public Button exitOverlayButton;
    public Button studentButton;

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

    public RawImage displayOtherVideo;
    public bool enableLog = false;

    const int port = 8010;
    public string IP = "10.132.3.74";
    TcpClient videoClient;

    Texture2D tex;

    private bool stop = false;

    //This must be the-same with SEND_COUNT on the server
    const int SEND_RECEIVE_COUNT = 15;

    private void Start()
    {
        Button btn = studentButton.GetComponent<Button>();
        btn.onClick.AddListener(CallIPInput);
    }

    private void Update()
    {
        //If the client has started
        if (clientMode)
        {

            //Hang up button
            /*
            Button hubtn = hangUpButton.GetComponent<Button>();
            hubtn.onClick.AddListener(HangUpButtonClicked);
            //Hand up button
            Button handbtn = handUpButton.GetComponent<Button>();
            handbtn.onClick.AddListener(ClientSendHandUp);
            //Thumbs up button
            Button thumbbtn = thumbsUpButton.GetComponent<Button>();
            thumbbtn.onClick.AddListener(ClientSendThumbsUp);
            //Button obtn = exitOverlayButton.GetComponent<Button>();
            //obtn.onClick.AddListener(Overlay);
            Debug.Log("Current game state: " + client.returnGameState());


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
                screen.changeCameraScreen();
                client.changeGameState();
            } else if (client.returnGameState() != "")
            {
                //This is the videoStreamTrack
                Debug.Log("received video track");
            }

            if (screen.currentScreen == "overlay")
            {
                disableButtons();
            } else
            {
                enableButtons();
            }*/
        }

        //Need this so the button doesn't activate 2000 times
        /*
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
        }*/

    }

    public void CallIPInput()
    {
        ReadStringInput(IP);
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
            videoClient.Close();
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
        /*
        Debug.Log("Client has started");
        cameraScript.StartStopCam_Clicked();
        screen.showCameraScreen();


        Application.runInBackground = true;

        tex = new Texture2D(0, 0);
        videoClient = new TcpClient();

        //Connect to server from another Thread
        Loom.RunAsync(() =>
        {
            LOGWARNING("Connecting to server...");
            videoClient.Connect(IPAddress.Parse(IP), port);
            LOGWARNING("Connected!");

            imageReceiver();
        });*/
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
        videoClient.Close();
    }


    /*
    void imageReceiver()
    {
        //While loop in another Thread is fine so we don't block main Unity Thread
        Loom.RunAsync(() =>
        {
            while (!stop)
            {
                //Read Image Count
                int imageSize = readImageByteSize(SEND_RECEIVE_COUNT);
                LOGWARNING("Received Image byte Length: " + imageSize);

                //Read Image Bytes and Display it
                readFrameByteArray(imageSize);
            }
        });
    }


    //Converts the data size to byte array and put result to the fullBytes array
    void byteLengthToFrameByteArray(int byteLength, byte[] fullBytes)
    {
        //Clear old data
        Array.Clear(fullBytes, 0, fullBytes.Length);
        //Convert int to bytes
        byte[] bytesToSendCount = BitConverter.GetBytes(byteLength);
        //Copy result to fullBytes
        bytesToSendCount.CopyTo(fullBytes, 0);
    }

    
    //Converts the byte array to the data size and returns the result
    int frameByteArrayToByteLength(byte[] frameBytesLength)
    {
        int byteLength = BitConverter.ToInt32(frameBytesLength, 0);
        return byteLength;
    }


    /////////////////////////////////////////////////////Read Image SIZE from Server///////////////////////////////////////////////////
    private int readImageByteSize(int size)
    {
        bool disconnected = false;

        NetworkStream serverStream = videoClient.GetStream();
        byte[] imageBytesCount = new byte[8192];
        var total = 0;
        do
        {
            var read = serverStream.Read(imageBytesCount, total, size - total);
            //Debug.LogFormat("Client recieved {0} bytes", total);
            if (read == 0)
            {
                disconnected = true;
                break;
            }
            total += read;
        } while (total != size);

        int byteLength;

        if (disconnected)
        {
            byteLength = -1;
        }
        else
        {
            byteLength = frameByteArrayToByteLength(imageBytesCount);
        }
        return byteLength;
    }

    /////////////////////////////////////////////////////Read Image Data Byte Array from Server///////////////////////////////////////////////////
    private void readFrameByteArray(int size)
    {
        bool disconnected = false;

        NetworkStream serverStream = videoClient.GetStream();
        byte[] imageBytes = new byte[8192];
        var total = 0;
        do
        {
            var read = serverStream.Read(imageBytes, total, size - total);
            //Debug.LogFormat("Client recieved {0} bytes", total);
            if (read == 0)
            {
                disconnected = true;
                break;
            }
            total += read;
        } while (total != size);

        bool readyToReadAgain = false;

        //Display Image
        if (!disconnected)
        {
            //Display Image on the main Thread
            Loom.QueueOnMainThread(() =>
            {
                displayReceivedImage(imageBytes);
                readyToReadAgain = true;
            });
        }

        //Wait until old Image is displayed
        while (!readyToReadAgain)
        {
            System.Threading.Thread.Sleep(1);
        }
    }


    void displayReceivedImage(byte[] receivedImageBytes)
    {
        tex.LoadImage(receivedImageBytes);
        displayOtherVideo.texture = tex;
    }



    void LOG(string messsage)
    {
        if (enableLog)
            Debug.Log(messsage);
    }

    void LOGWARNING(string messsage)
    {
        if (enableLog)
            Debug.LogWarning(messsage);
    }*/

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
