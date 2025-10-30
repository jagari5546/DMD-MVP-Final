using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public enum KeyType { Red, Green, Blue, Yellow, Purple }

    public KeyType keyType;
    [SerializeField] private AudioSource sfx; // opcional

    private void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        inv.AddKey(keyType);
        if (sfx) sfx.Play();
        Destroy(gameObject); // o desactiva si quieres VFX antes
    }
}