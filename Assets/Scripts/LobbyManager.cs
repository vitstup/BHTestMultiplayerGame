using UnityEngine;
using Mirror;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private NetworkManager manager;

    [SerializeField] private TMP_InputField nickInput;
    [SerializeField] private TMP_InputField ipInput;

    public static string Nick;
    private string Ip;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        ResetInfo();
    }
    public void OnNickChanged(string value)
    {
        Nick = value;
    }

    public void OnIpChanged(string value)
    {
        Ip = value;
        manager.networkAddress = Ip;
    }

    public void StartAsHost()
    {
        manager.StartHost();
    }

    public void StartAsClient()
    {
        manager.StartClient();
    }

    private void ResetInfo()
    {
        nickInput.text = string.Format("Ubivator{0}", Random.Range(2000, 2023));
        ipInput.text = "localhost";
    }
}