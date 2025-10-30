using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly HashSet<KeyPickup.KeyType> keys = new();

    public void AddKey(KeyPickup.KeyType type) => keys.Add(type);
    public bool HasKey(KeyPickup.KeyType type) => keys.Contains(type);
}