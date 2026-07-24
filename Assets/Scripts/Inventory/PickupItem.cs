using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    [SerializeField] private float cooldownTime = 0.5f;

    private Rigidbody rb;
    private float lastDropOrThrowTime = -100f;
    private PlayerInventory owningInventory;

    public Rigidbody Rb => rb;
    public PlayerInventory OwningInventory => owningInventory;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool CanBePickedUp()
    {
        return owningInventory == null && Time.time >= lastDropOrThrowTime + cooldownTime;
    }

    public void SetCooldown()
    {
        lastDropOrThrowTime = Time.time;
    }

    public void OnPickedUp(PlayerInventory inventory)
    {
        owningInventory = inventory;
    }

    public void OnReleased()
    {
        owningInventory = null;
    }

    public void NotifyConsumed()
    {
        if (owningInventory != null)
        {
            owningInventory.NotifyItemConsumed(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (owningInventory != null)
        {
            owningInventory.NotifyItemDisabledOrDestroyed(this);
        }
    }

    private void OnDestroy()
    {
        if (owningInventory != null)
        {
            owningInventory.NotifyItemDisabledOrDestroyed(this);
        }
    }
}
