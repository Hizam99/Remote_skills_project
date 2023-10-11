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
    public Server server;

    public bool serverMode = false;

    private void Update()
    {
        if (serverMode)
        {
            Button hubtn = hangUpButton.GetComponent<Button>();
            hubtn.onClick.AddListener(HangUpButtonClicked);
            //TODO add buttons for sending messages too 
        }
        
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

    public void HangUpButtonClicked()
    {
        server.CloseConnection();
    }

    public void ServerSendsAlert()
    {
        server.SendMessage("alert");
    }

    public void ServerSendsThumbs()
    {
        server.SendMessage("thumbs");
    }

}

public class Server
{
    private static TcpListener server;
    private Thread listenThread;
    private NetworkStream stream;
    private TcpClient client;

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
            stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            //Read bytes from the incoming stream
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received " + dataReceived);
                if (dataReceived == "hand")
                {
                    //TODO hand up
                } else if (dataReceived == "thumbs")
                {
                    //TODO
                }
            }
            break;
        }
        
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
        server?.Stop();
        Debug.Log("Server stopped");
    }
} 
