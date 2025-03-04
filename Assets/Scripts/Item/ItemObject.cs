using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}


public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.decription}";
        return str;
    }

    public void OnInteract()
    {
        Player player = CharacterManager.Instance.Player;
        player.itemData = data;
        player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
