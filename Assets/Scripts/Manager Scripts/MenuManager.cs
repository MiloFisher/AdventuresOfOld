using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : Singleton<MenuManager>
{
    public GameObject[] menuScenes;
    public AudioSource themePlayer;
    public AudioSource effectPlayer;

    private void Awake()
    {
        //SwapScene(0);
    }

    public void SwapScene(int id)
    {
        effectPlayer.Play();
        if (id >= menuScenes.Length || id < 0)
            return;
        foreach (GameObject g in menuScenes)
            g.SetActive(false);
        menuScenes[id].SetActive(true);
    }

    public void resolutionChange(int resNumber) { // Set resNumber with Button function in Unity Editor Scene
        if (resNumber == 1920) {
            Screen.SetResolution(1920, 1080, true); // True == Fullscreen
        }
        else if (resNumber == 2560) {
            Screen.SetResolution(2560, 1440, true);
        }
    }

    public void creditsScene() {
        SceneManager.LoadScene("Credits");
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    //void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    //    if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    //    {
    //        StartButtons();
    //    }
    //    else
    //    {
    //        StatusLabels();

    //        SubmitNewPosition();
    //    }

    //    GUILayout.EndArea();
    //}

    //static void StartButtons()
    //{
    //    if (GUILayout.Button("Host"))
    //    {
    //        NetworkManager.Singleton.StartHost();
    //    }
    //    if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
    //    if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    //}

    //static void StatusLabels()
    //{
    //    var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

    //    GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
    //    GUILayout.Label("Mode: " + mode);
    //}

    //static void SubmitNewPosition()
    //{
    //    if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
    //    {
    //        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
    //        {
    //        //    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
    //        //        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().Move();
    //        }
    //        else
    //        {
    //            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
    //            var player = playerObject.GetComponent<Player>();
    //            //player.Move();
    //        }
    //    }
    //}
}