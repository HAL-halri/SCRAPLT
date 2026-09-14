using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    [SerializeField] private GameObject craftPanel;
    [SerializeField] private List<Button> footerButtons;

    public void ToggleCraftButton()
    {
        bool newState = !craftPanel.activeSelf;
        craftPanel.SetActive(!craftPanel.activeSelf);

        foreach (var button in footerButtons)
        {
            //craftPanelÇ∆îΩëŒÇÃèÛë‘Ç…Ç∑ÇÈ
            button.interactable = !newState;
        }
    }
}
