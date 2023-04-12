using Mirror;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResetManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI winnerText;

    private bool isRestartingNow = false;

    public void Restart(ThirdPersonController[] players, float resetTime, PlayerNick winner)
    {
       if (!isRestartingNow) StartCoroutine(ResetRoutine(players, resetTime, winner));
    }

    private IEnumerator ResetRoutine(ThirdPersonController[] players, float resetTime, PlayerNick winner)
    {
        isRestartingNow = true;
        ShowWinner(winner);
        yield return new WaitForSecondsRealtime(resetTime);
        RestartMatch(players);
        canvas.gameObject.SetActive(false);
        isRestartingNow = false;
    }

    private void ShowWinner(PlayerNick winner)
    {
        canvas.gameObject.SetActive(true);
        canvas.worldCamera = Camera.main;
        winnerText.text = "Winner is " + winner.nickname;
    }

    private void RestartMatch(ThirdPersonController[] players)
    {
        var positions = FindObjectsOfType<NetworkStartPosition>().ToList();

        foreach (ThirdPersonController player in players)
        {
            player.OnRestart();

            int positionPoint = Random.Range(0, positions.Count);
            player.transform.position = positions[positionPoint].transform.position;
            if (positions.Count > 1) positions.RemoveAt(positionPoint);
        }
        
    }
}