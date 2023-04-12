using Mirror;

public class PlayerNick : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnNicknameChanged))] public string nickname;

    private void Start()
    {
        if (!isLocalPlayer) return;
        SetNickName();
    }

    private void SetNickName()
    {
        CmdSetNickname(LobbyManager.Nick);
    }

    [Command]
    private void CmdSetNickname(string nickname)
    {
        this.nickname = nickname;
    }

    private void OnNicknameChanged(string oldNickname, string newNickname)
    {
        nickname = newNickname;
        ScoreManager.instance.UpdateInfo();
    }
}