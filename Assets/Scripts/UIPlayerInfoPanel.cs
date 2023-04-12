using UnityEngine;
using TMPro;

public class UIPlayerInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerNameText;
    [SerializeField] private TextMeshProUGUI PlayerScoreText;

    public void UpdateText(string playerName, int playerScore)
    {
        PlayerNameText.text = playerName;
        PlayerScoreText.text = playerScore.ToString();
    }
}