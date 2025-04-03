using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private InventoryScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI itemDisplayNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI statText1;
    [SerializeField] private TextMeshProUGUI statText2;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;
    [SerializeField] private InventorySO inventorySO;

    private Button firstSelectedButton;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onInventoryTogglePressed += InventoryTogglePressed;
        GameEventsManager.instance.inventoryEvents.onItemAdded += ItemAdded;
        GameEventsManager.instance.inventoryEvents.onItemRemoved += ItemRemoved;
        GameEventsManager.instance.inventoryEvents.onInventoryUpdated += InventoryUpdated;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onInventoryTogglePressed -= InventoryTogglePressed;
        GameEventsManager.instance.inventoryEvents.onItemAdded -= ItemAdded;
        GameEventsManager.instance.inventoryEvents.onItemRemoved -= ItemRemoved;
        GameEventsManager.instance.inventoryEvents.onInventoryUpdated -= InventoryUpdated;
    }

    private void InventoryTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        InventoryUpdated();

        // Start a coroutine to set the selection after the UI is fully updated
        StartCoroutine(SetInitialSelection());
    }

    private IEnumerator SetInitialSelection()
    {
        yield return new WaitForEndOfFrame();

        if (firstSelectedButton == null && scrollingList.HasButtons())
        {
            InventoryButton firstButton = scrollingList.GetFirstButton();
            if (firstButton != null && firstButton.button != null)
            {
                firstSelectedButton = firstButton.button;
            }
        }

        if (firstSelectedButton != null && firstSelectedButton.gameObject.activeInHierarchy && firstSelectedButton.interactable)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
        else
        {
            Debug.LogWarning("No valid button to select when opening the inventory.");
        }
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        scrollingList.ClearList();
    }

    private void InventoryUpdated()
    {
        scrollingList.ClearList();
        foreach (ItemSO item in inventorySO.inventoryItems)
        {
            ItemAdded(item);
        }
    }

    private void ItemAdded(ItemSO item)
    {
        //Debug.Log("ItemAdded ran from InventoryUI");
        InventoryButton inventoryButton = scrollingList.CreateButton(item, () =>
        {
            SetInventoryInfo(item);
        });


        if (firstSelectedButton == null)
        {
            firstSelectedButton = inventoryButton.button;
        }

        SetInventoryInfo(item);
    }

    private void ItemRemoved(ItemSO item)
    {
        // Start a coroutine to wait for InventoryUpdated to finish
        StartCoroutine(HandleItemRemovedAfterUpdate(item));
    }

    private IEnumerator HandleItemRemovedAfterUpdate(ItemSO item)
    {
        // Wait for the end of the frame to ensure InventoryUpdated has completed
        yield return new WaitForEndOfFrame();

        // Check if the stack still exists
        InventoryButton button = scrollingList.GetButtonFromItem(item);
        if (button != null)
        {
            // Stack still exists, keep the selector on the same button
            firstSelectedButton = button.button;

            if (firstSelectedButton != null && firstSelectedButton.gameObject.activeInHierarchy && firstSelectedButton.interactable)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
            }
            else
            {
                Debug.LogWarning("Cannot set selected GameObject. Button is either null, inactive, or not interactable.");
            }
        }
        else
        {
            // Stack was removed, move to the next available button
            InventoryButton nextButton = scrollingList.GetFirstButton();
            if (nextButton != null && nextButton.button != null && nextButton.button.gameObject.activeInHierarchy && nextButton.button.interactable)
            {
                firstSelectedButton = nextButton.button;
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
            }
            else
            {
                // No buttons remain, clear the selection
                firstSelectedButton = null;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    /*private void ItemRemoved(ItemSO item)
    {
        Debug.Log("ItemRemoved ran from InventoryUI");
        //InventoryButton removedButton = scrollingList.GetButtonFromItem(item);
        //scrollingList.RemoveButton(removedButton);

        //GameEventsManager.instance.inventoryEvents.ItemRemoved(item);

        // Update the firstSelectedButton to the next available button

        Debug.Log("Setting firstSelectedButton");
        firstSelectedButton = scrollingList.GetFirstButton().button;
        if (firstSelectedButton != null)
        {
            //EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject); // Explicitly set the selection
        }
        
     
    }*/

    private void SetInventoryInfo(ItemSO item)
    {
        itemDisplayNameText.text = item.itemName;
        itemDescriptionText.text = item.itemName; //placeholder until I add description
        levelRequirementsText.text = "Level " + item.itemName; //placeholder
        questRequirementsText.text = "Quest Requirements: " + item.itemName; //placeholder
        statText1.text = "stat1"; //placeholder
        statText2.text = "stat2"; //placeholder
    }
}