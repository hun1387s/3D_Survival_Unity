using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;
    public GameObject inventoryWnd;
    public Transform slotPannel;
    public Transform dropPosition;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject useBtn;
    public GameObject equipBtn;
    public GameObject unequipBtn;
    public GameObject dropBtn;

    PlayerController controller;
    PlayerCondition condition;

    ItemData selectedItem;
    int selectedItemIdx = -1;

    int curEquipIdx;

    void Start()
    {
        Player player = CharacterManager.Instance.Player;
        controller = player.controller;
        condition = player.condition;
        dropPosition = player.dropPosition;

        controller.inventory += Toggle;
        player.addItem += AddItem;

        inventoryWnd.SetActive(false);
        slots = new ItemSlot[slotPannel.childCount];

        for (int i = 0; i<slots.Length; i++)
        {
            slots[i] = slotPannel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelectedItemWnd();
    }

    void ClearSelectedItemWnd()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        useBtn.SetActive(false);
        equipBtn.SetActive(false);
        unequipBtn.SetActive(false);
        dropBtn.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
            inventoryWnd.SetActive(false);
        else
            inventoryWnd.SetActive(true);
    }

    public bool IsOpen()
    {
        return inventoryWnd.activeInHierarchy;
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        // 아이템이 중복 가능한지 canStack
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;

                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        // 비어있는 슬롯 가져온다.
        ItemSlot emptySlot = GetEmptySlot();
        // 있다면
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }
        // 없다면
        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    void UpdateUI()
    {
        for (int i = 0; i<slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }
    void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, 
            Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index].item;
        selectedItemIdx = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.decription;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useBtn.SetActive(selectedItem.type == ItemType.Consumable);
        equipBtn.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].equipped);
        unequipBtn.SetActive(selectedItem.type == ItemType.Equipable && slots[index].equipped);
        dropBtn.SetActive(true);

    }

    public void OnUseBtn()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            for (int i=0; i<selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].type)
                {
                    case CounsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].value);
                        break;
                    case CounsumableType.Hunger:
                        condition.Eat(selectedItem.consumables[i].value);
                        break;
                }
            }
            RemoveSelectedItem();
        }
    }
    public void OnDropBtn()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIdx].quantity--;
        if (slots[selectedItemIdx].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIdx].item = null;
            selectedItemIdx = -1;
            ClearSelectedItemWnd();
        }

        UpdateUI();
    }

    public void OnEquipBtn()
    {
        if (slots[curEquipIdx].equipped)
        {
            UnEquip(curEquipIdx);
        }

        slots[selectedItemIdx].equipped = true;
        curEquipIdx = selectedItemIdx;
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem);
        UpdateUI();

        SelectItem(selectedItemIdx);
    }

    void UnEquip(int idx)
    {
        slots[idx].equipped = false;
        CharacterManager.Instance.Player.equip.UnEquip();
        UpdateUI();

        if (selectedItemIdx == idx)
        {
            SelectItem(selectedItemIdx);
        }
    }

    public void OnUnEquipBtn()
    {
        UnEquip(selectedItemIdx);
    }
}