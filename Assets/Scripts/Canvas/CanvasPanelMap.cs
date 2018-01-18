using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPanelMap : MonoBehaviour
{
    public Sprite sprite;
    public RectTransform frame;

    private static float TileSize
    {
        get
        {
            float mp = 1f + (1f - WorldManager.InnerRadius);
            return (240f / Config.GameSave.width) * mp;
        }
    }

    public static void Refresh()
    {
        CanvasPanelMap map = FindObjectOfType<CanvasPanelMap>();

        Image[] images = map.GetComponentsInChildren<Image>();
        for(int i = 0; i < images.Length;i++)
        {
            if(images[i].transform.parent == map.frame.transform)
            {
                Destroy(images[i].gameObject);
            }
        }

        for (int i = 0; i < WorldManager.Tiles.Length; i++)
        {
            Image image = new GameObject("Tile").AddComponent<Image>();
            image.transform.SetParent(map.frame.transform);

            Vector2 position = new Vector2
            {
                x = WorldManager.Tiles[i].transform.position.x,
                y = WorldManager.Tiles[i].transform.position.z
            };

            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.zero;

            image.rectTransform.pivot = new Vector2(0, 1);
            image.rectTransform.anchoredPosition = position * TileSize * 0.5f + (Vector2.one * TileSize * 0.5f);
            image.rectTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
            image.rectTransform.sizeDelta = new Vector2(TileSize, TileSize);
            image.color = WorldManager.Tiles[i].color;
            image.raycastTarget = false;
            image.sprite = map.sprite;
        }
    }
}
