using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasButton : MonoBehaviour {

    private Button button;
    private Text text;
    private RectTransform rectTransform;

    public string Text
    {
        get
        {
            return text.text;
        }
        set
        {
            text.text = value; 
        }
    }

    protected Button Button
    {
        get
        {
            return button;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            return rectTransform;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {

    }
}
