using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private static CanvasManager instance;
    private string layer;
    private CanvasLayer[] layers;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private string defaultLayer = "Menu";

	public static string Layer
    {
        get
        {
            if (!instance) instance = FindObjectOfType<CanvasManager>();
            return instance.layer;
        }
        set
        {
            if (!instance) instance = FindObjectOfType<CanvasManager>();
            instance.layer = value;
            Refresh();
        }
    }

    public static CanvasLayer[] Layers
    {
        get
        {
            if (!instance) instance = FindObjectOfType<CanvasManager>();
            if (instance.layer == null) Refresh();
            return instance.layers;
        }
    }

    private void Awake()
    {
        instance = this;
        Layer = defaultLayer;
    }

    private void OnEnable()
    {
        instance = this;
    }

    public static void Refresh()
    {
        if (!instance) instance = FindObjectOfType<CanvasManager>();
        instance.layers = instance.canvas.GetComponentsInChildren<CanvasLayer>(true);
        for (int i = 0; i < instance.layers.Length; i++)
        {
            instance.layers[i].gameObject.SetActive(Layer == instance.layers[i].name);
        }
    }
}
