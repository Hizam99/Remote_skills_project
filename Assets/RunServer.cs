using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Text;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class RunServer : MonoBehaviour
{
    public TextMeshProUGUI IPDisplay;
    public Button hangUpButton;
    public Button thumbsUpButton;
    public Button alertButton;
    public Button exitOverlayButton;
    public Server server;

    public bool serverMode = false;
    public SwitchScreen screen;
    public CameraScript cameraScript;

    public GameObject thumbsUpPopup;
    public GameObject overlappingElement;
    public GameObject exitOverlayPopup;

    private float time = 0;

    public bool clientJoined = false;
    public bool thumbsUp = false;
    public bool alert = false;
    public bool cameraBeingDisplayed = false;

    //Create this to keep track of if something has just been sent
    public bool justSent = false;


    //New objects

    WebCamTexture webCam;
    public RawImage displayOtherVideo;
    public bool enableLog = false;
    public TcpClient videoClient;

    Texture2D currentTexture;

    private TcpListener listner;
    private const int port = 8010;
    private bool stop = false;

    const int SEND_RECEIVE_COUNT = 15;

    private void Start()
    {
        //this could mess some stuff up?
        Application.runInBackground = true;

        //Start WebCam coroutine
        StartCoroutine(initAndWaitForWebCamTexture());
    }
    private void Update()
    {
        displayOtherVideo.texture = webCam;
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
            Button obtn = exitOverlayButton.GetComponent<Button>();
            obtn.onClick.AddListener(ServerSendsExitOverlay);

            //Display the IP address if the server is waiting and client hasn't joined
            if (server.getGameState() == "waiting" && !clientJoined)
            {
                Debug.Log("displaying IP, waiting");
                IPDisplay.enabled = true;
                displayIP();
                //Send
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
        if (!justSent)
        {
            server.CloseConnection();
            serverMode = false;
            clientJoined = false;

            if (webCam != null && webCam.isPlaying)
            {
                webCam.Stop();
                stop = true;
            }

            if (listner != null)
            {
                listner.Stop();
            }
        }
        justSent = true;
    }

    public void ServerSendsAlert()
    {
        if (!justSent)
        {
            server.SendMessage("alert");
            Debug.Log("Function activated");
        }
        justSent = true;
        exitOverlayPopup.SetActive(true);
    }

    public void ServerSendsThumbs()
    {
        if (!justSent)
        {
            server.SendMessage("thumbs");
        }
        justSent = true;
    }


    public void ServerSendsExitOverlay()
    {
        if (!justSent)
        {
            server.SendMessage("exit");
        }
        justSent = true;
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

    IEnumerator initAndWaitForWebCamTexture()
    {
        // Open the Camera on the desired device, in my case IPAD pro
        webCam = new WebCamTexture();
        // Get all devices , front and back camera
        webCam.deviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;

        // request the lowest width and heigh possible
        webCam.requestedHeight = 10;
        webCam.requestedWidth = 10;

        displayOtherVideo.texture = webCam;

        webCam.Play();

        currentTexture = new Texture2D(webCam.width, webCam.height);

        // Connect to the server
        listner = new TcpListener(IPAddress.Any, port);

        listner.Start();

        while (webCam.width < 100)
        {
            yield return null;
        }

        //Start sending coroutine
        StartCoroutine(senderCOR());
    }

    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    IEnumerator senderCOR()
    {

        bool isConnected = false;
        TcpClient videoClient = null;
        NetworkStream stream = null;

        // Wait for client to connect in another Thread 
        Loom.RunAsync(() =>
        {
            while (!stop)
            {
                // Wait for client connection
                videoClient = listner.AcceptTcpClient();
                // We are connected

                isConnected = true;
                stream = videoClient.GetStream();
            }
        });

        //Wait until client has connected
        while (!isConnected)
        {
            yield return null;
        }

        LOG("Connected!");

        bool readyToGetFrame = true;

        byte[] frameBytesLength = new byte[SEND_RECEIVE_COUNT];

        while (!stop)
        {
            //Wait for End of frame
            yield return endOfFrame;

            currentTexture.SetPixels(webCam.GetPixels());
            byte[] pngBytes = currentTexture.EncodeToPNG();
            //Fill total byte length to send. Result is stored in frameBytesLength
            byteLengthToFrameByteArray(pngBytes.Length, frameBytesLength);

            //Set readyToGetFrame false
            readyToGetFrame = false;

            Loom.RunAsync(() =>
            {
                //Send total byte count first
                stream.Write(frameBytesLength, 0, frameBytesLength.Length);
                LOG("Sent Image byte Length: " + frameBytesLength.Length);

                //Send the image bytes
                stream.Write(pngBytes, 0, pngBytes.Length);
                LOG("Sending Image byte array data : " + pngBytes.Length);

                //Sent. Set readyToGetFrame true
                readyToGetFrame = true;
            });

            //Wait until we are ready to get new frame(Until we are done sending data)
            while (!readyToGetFrame)
            {
                LOG("Waiting To get new frame");
                yield return null;
            }
        }
    }


    void LOG(string messsage)
    {
        if (enableLog)
            Debug.Log(messsage);
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

public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    static int numThreads;

    private static Loom _current;
    private int _count;
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize()
    {
        if (!initialized)
        {

            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }

    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }

    }


    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }



    // Use this for initialization
    void Start()
    {

    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }



    }
}
