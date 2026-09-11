using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allPanels;
    public static PanelManager Instance { get; private set; }

    public void Awake()
    {
        //Šù‚É‘¶İ‚µ‚Ä‚¢‚éê‡A©g‚ğ”j‰ó
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void OpenPanel(GameObject targetPanel)
    {
        bool isAlreadyOpen = targetPanel.activeSelf;

        foreach (var panel in allPanels)
        {
            panel.SetActive(false);
        }
        if (!isAlreadyOpen)
        {
            targetPanel.SetActive(true);
        }
    }
}
