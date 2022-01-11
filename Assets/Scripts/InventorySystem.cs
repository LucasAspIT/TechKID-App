using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate() {
            OnClick(param);
        });
    }
}

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Button buttonGreen;
    [SerializeField] private Button buttonRed;

    [Serializable] public struct Item
    {
        public string Name;
        public string Type;
        public string Date;
        public Button Status;
    }

    [SerializeField] Item[] allItems;

    void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject; // Select the child of the panel (the button)
        GameObject gO;
        int n = allItems.Length;

        for (int i = 0; i < n; i++)
        {
            gO = Instantiate(buttonTemplate, transform);
            gO.transform.GetChild(0).GetComponent<Button>().image.sprite = allItems[i].Status.image.sprite;
            gO.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Name: " + allItems[i].Name;
            gO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Type: " + allItems[i].Type;
            gO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Last change: " + allItems[i].Date;

            gO.transform.GetChild(0).GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

        Destroy(buttonTemplate); // Remove the template once it has been used for the items.
    }

    /// <summary>
    /// Item status button click event.
    /// </summary>
    /// <param name="itemIndex"></param>
    void ItemClicked(int itemIndex)
    {
        Debug.Log($"Item {itemIndex} was clicked.");
    }
}
