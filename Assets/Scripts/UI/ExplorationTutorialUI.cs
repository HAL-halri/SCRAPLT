using UnityEngine;

public class ExplorationTutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;

    private const string TUTORIAL_SEEN_KEY = "Tutorial_Exploration_Seen";

    public void ShowIfFirstTime()
    {
        if (PlayerPrefs.GetInt(TUTORIAL_SEEN_KEY, 0) == 0)
        {
            tutorialPanel.SetActive(true);
            PlayerPrefs.SetInt(TUTORIAL_SEEN_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    public void ShowManual()
    {
        tutorialPanel.SetActive(true);
    }

    public void OnCloseButtonClicked()
    {
        tutorialPanel.SetActive(false);
    }
}