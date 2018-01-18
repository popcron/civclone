using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPanelInfo : MonoBehaviour
{
    public Text science;
    public Text sciencePerTurn;
    public Text culture;
    public Text culturePerTurn;
    public Text faith;
    public Text faithPerTurn;
    public Text gold;
    public Text goldPerTurn;

    private void Update()
    {
        sciencePerTurn.rectTransform.anchoredPosition = new Vector2(science.preferredWidth + 3, 0);
        culturePerTurn.rectTransform.anchoredPosition = new Vector2(culture.preferredWidth + 3, 0);
        faithPerTurn.rectTransform.anchoredPosition = new Vector2(faith.preferredWidth + 3, 0);
        goldPerTurn.rectTransform.anchoredPosition = new Vector2(gold.preferredWidth + 3, 0);
    }
}
