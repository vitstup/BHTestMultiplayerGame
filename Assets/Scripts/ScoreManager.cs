using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] private ResetManager resetManager;

    [SerializeField] private Canvas canvas;
    [SerializeField] private UIPlayerInfoPanel[] panels;

    [SerializeField] private int scoreToWin = 3;
    [SerializeField] private float resetTime = 5f;

    private void Awake()
    {
        instance = this;
        StartCoroutine(lifeRoutine());
    }

    public void UpdateInfo()
    {
        var players = FindObjectsOfType<ThirdPersonController>();
        Debug.Log("Connections count: " + players.Length);
        for (int i = 0; i < panels.Length; i++)
        {
            if (i >= players.Length)
            {
                panels[i].gameObject.SetActive(false);
                continue;
            }

            panels[i].gameObject.SetActive(true);

            var playerNickScript = players[i].gameObject.GetComponent<PlayerNick>();
            string playerNick = playerNickScript.nickname;
            if (players[i].isOwned) playerNick += "(you)";
            panels[i].UpdateText(playerNick, players[i].score);

            if (players[i].score >= scoreToWin) resetManager.Restart(players, resetTime, playerNickScript);
        }
    }

    public void SetCamera(Camera camera)
    {
        canvas.worldCamera = camera;
    }

    private IEnumerator lifeRoutine()
    {
        UpdateInfo();
        yield return new WaitForSeconds(3f);
        StartCoroutine(lifeRoutine());
    }
}