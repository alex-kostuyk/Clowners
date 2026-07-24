using UnityEngine;

public class ActionPickupItem : MonoBehaviour, IAction
{
    [SerializeField] private PickupItem targetItem;
    [SerializeField] private PlayerInventory cachedInventory;
    [SerializeField] private Transform throwDirectionTransform;

    private void Awake()
    {
        if (targetItem == null)
        {
            targetItem = GetComponent<PickupItem>();
        }
    }

    public void StartAction()
    {
        if (targetItem == null)
            return;

        if (cachedInventory == null)
        {
            cachedInventory = PlayerInventory.Instance;
            if (cachedInventory == null)
            {
                cachedInventory = FindObjectOfType<PlayerInventory>();
            }
        }

        if (cachedInventory == null)
            return;

        if (cachedInventory.HasItem)
        {
            Vector3 throwDir = throwDirectionTransform != null
                ? throwDirectionTransform.forward
                : cachedInventory.transform.forward;

            cachedInventory.TrySwap(targetItem, throwDir);
        }
        else
        {
            cachedInventory.Pickup(targetItem);
        }
    }
}
