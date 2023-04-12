using Mirror;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkClient.isConnected) NetworkManager.singleton.StopClient();
        else if (NetworkServer.active) NetworkManager.singleton.StopServer();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}