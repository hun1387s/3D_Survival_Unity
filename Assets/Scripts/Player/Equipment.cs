using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    PlayerController controller;
    PlayerCondition condition;

    void Start()
    {
        condition = GetComponent<PlayerCondition>();
        controller = GetComponent<PlayerController>();
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed 
            && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput();
        }
    }
}
