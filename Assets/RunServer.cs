using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Text;
using UnityEngine.UI;
using UnityEditor.PackageManager;

public class RunServer : MonoBehaviour
{
    public TextMeshProUGUI IPDisplay;
    public Button hangUpButton;
    public Button thumbsUpButton;
    public Button alertButton;
    public Server server;

    public bool serverMode = false;
    public SwitchScreen screen;
    public CameraScript cameraScript;

    public GameObject thumbsUpPopup;
    public GameObject overlappingElement;

    private float time = 0;

    public bool clientJoined = false;
    public bool thumbsUp = false;
    public bool alert = false;
    public bool cameraBeingDisplayed = false;

    //Create this to keep track of if something has just been sent
    public bool justSent = false;
    private void Update()
    {
        Debug.Log(justSent);
        //Make sure it's in server mode before connecting functionality to buttons
        if (serverMode)
        {
            Button hubtn = hangUpButton.GetComponent<Button>();
            hubtn.onClick.AddListener(HangUpButtonClicked);
            Button tuBtn = thumbsUpButton.GetComponent<Button>();
            tuBtn.onClick.AddListener(ServerSendsThumbs);
            Button aBtn = alertButton.GetComponent<Button>();
            aBtn.onClick.AddListener(ServerSendsAlert);

            //Display the IP address if the server is waiting and client hasn't joined
            if (server.getGameState() == "waiting" && !clientJoined)
            {
                Debug.Log("displaying IP, waiting");
                IPDisplay.enabled = true;
                displayIP();
            } else
            {
                IPDisplay.enabled = false;
            }
            if (server.getGameState() != "waiting")
            {
                Debug.Log("hiding IP, client joined");
                clientJoined = true;
            }

            if (clientJoined && !cameraBeingDisplayed)
            {
                displayCamera();
                cameraBeingDisplayed = true;
            }

            //Call thumps up
            if (server.getGameState() == "thumbs")
            {
                ThumbsUpPopup();
                server.changeGameState();
            } else if (server.getGameState() == "disconnected")
            {
                screen.changeScreen(screenSelector.login);
                screen.endCall();
                server.changeGameState();
            } else if (server.getGameState() == "alert")
            {
                AlertPopup();
                server.changeGameState();
            }

        }

        if (justSent && time < 0.1)
        {
            time = time + Time.deltaTime;
        } else if (justSent && time > 0.1)
        {
            justSent = false;
            time = 0;
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
        }

    }

    public void displayCamera()
    {
        cameraScript.StartStopCam_Clicked();
        screen.showCameraScreen();
    }

    //Getting local IP address for display
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                //Have this in a popup button with an exit button
                print("My IP address is: " + ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public void displayIP()
    {
        IPDisplay.text = GetLocalIPAddress() + "\n waiting for student to join...";
        print(IPDisplay.text);
    }

    //Start the server
    public void StartServer()
    {
        serverMode = true;
        server = new Server();
    }

    public void ThumbsUpPopup()
    {
        Debug.Log("thumbs up");
        thumbsUp = true;
    }

    public void AlertPopup()
    {
        alert = true;
    }

    public void HangUpButtonClicked()
    {
        server.CloseConnection();
        serverMode = false;
        clientJoined = false;
    }

    public void ServerSendsAlert()
    {
        if (!justSent)
        {
            server.SendMessage("alert");
            Debug.Log("Function activated");
        }
        justSent = true;
    }

    public void ServerSendsThumbs()
    {
        if (!justSent)
        {
            server.SendMessage("thumbs");
        }
        justSent= true;
    }

}

public class Server
{
    private static TcpListener server;
    private Thread listenThread;
    private NetworkStream stream;
    private TcpClient client;
    private string gameState = "waiting";

    public Server()
    {
        server = new TcpListener(IPAddress.Any, 11111);
        server.Start();
        Debug.Log("Server listening on port 11111");
        //Thread to listen for incoming connections
        listenThread = new Thread(new ThreadStart(ListenForConnections));
        listenThread.Start();
    }

    //Thread function to listen for the initial connection, ends when student joins
    void ListenForConnections()
    {
        while (true)
        {
            client = server.AcceptTcpClient();
            //Change the screen here now
            Debug.Log("Reached client");
            gameState = "";
            stream = client.GetStream();
            //TODO DONT I HAVE TO MOVE THIS????
            byte[] buffer = new byte[1024];
            int bytesRead;
            //Read bytes from the incoming stream
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received " + dataReceived);
                gameState = dataReceived;
            }
            break;
        }
        Debug.Log("client disconnected");
        //Shut down server, send it back to login page
        CloseConnection();
        gameState = "disconnected";
    }

    public string getGameState()
    {
        return gameState;
    }

    public void changeGameState()
    {
        gameState = "";
    }

    //Send message to the student/client
    public void SendMessage(string message)
    {
        //Package the string to send to the trainer
        Debug.Log("sent " + message);
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    public void CloseConnection()
    {
        if (client != null)
        {
            client.Close();
        }
        if (stream != null)
        {
            stream.Close();
        }
        if (server != null)
        {
            server?.Stop();
        }
        Debug.Log("Server stopped");
    }
} 
