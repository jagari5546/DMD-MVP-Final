using UnityEngine;
using UnityEngine.Events;

public class DoorScript : MonoBehaviour
{
    [Header("Requirement")]
    public KeyPickup.KeyType requiredKey;

    [Header("Events")]
    public UnityEvent OnUnlocked;
    public UnityEvent OnLockedAttempt;

    private bool _isUnlocked;

    private void OnTriggerEnter(Collider other)
    {
        var inv = other.GetComponentInParent<PlayerInventory>();
        if (inv == null) return;

        if (inv.HasKey(requiredKey))
        {
            if (!_isUnlocked)
            {
                _isUnlocked = true;
                OnUnlocked?.Invoke();
            }
        }
        else
        {
            OnLockedAttempt?.Invoke();
        }
    }

    public bool IsUnlocked => _isUnlocked;
}