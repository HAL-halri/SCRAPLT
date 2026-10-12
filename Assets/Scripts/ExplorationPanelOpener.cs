using UnityEngine;

public class ExplorationPanelOpener : MonoBehaviour
{
    void OnEnable()
    {
        if (SeabedExplorationSystem.Instance != null)
        {
            SeabedExplorationSystem.Instance.OnPanelOpened();
        }
    }

    void OnDisable()
    {
        if (SeabedExplorationSystem.Instance != null)
        {
            SeabedExplorationSystem.Instance.OnPanelClosed();
        }
    }
}