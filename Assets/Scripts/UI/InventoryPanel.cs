using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private Button buttonOpen;
    [SerializeField] private RectTransform rectTransformPanel;

    [Inject] private readonly GameArea gameArea;

    private bool isOpened;

    void Awake()
    {
        buttonOpen.onClick.AddListener(ChangePanel);
        gameArea.OnPointerClickEvent += ClosePanel;
    }

    public void ClosePanel()
    {
        rectTransformPanel.DOAnchorPosX(0, 0.5f);
        isOpened = false;
    }

    private void ChangePanel()
    {
        if (isOpened)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }

    }

    private void OpenPanel()
    {
        rectTransformPanel.DOAnchorPosX(rectTransformPanel.rect.width, 0.5f);
        isOpened = true;
    }
}
