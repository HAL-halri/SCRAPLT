using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationSpot : MonoBehaviour
{
    private RectTransform rectTransform;
    private SeabedExplorationSystem system;

    public Vector2 Position => rectTransform.anchoredPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(SeabedExplorationSystem system)
    {
        this.system = system;
    }
}
