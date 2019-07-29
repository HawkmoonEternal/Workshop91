using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class OnClicks : MonoBehaviour
{
    public Text StatsText;
    public GameObject mainPanel;
    public GameObject statsPanel;

    // Start is called before the first frame update
    public void OnStartGame() {
        SceneManager.LoadScene("Level");
    }
    public void OnLoadStats() {
        StatsText.text = "Level: " + GameData.getLevel() + "\n\n\nXP: " + GameData.getXP() + "\n\n\n₽: " + GameData.getCash();
        mainPanel.SetActive(false);
        statsPanel.SetActive(true);

    }
    public void OnBackToMainPanel() {
        statsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
