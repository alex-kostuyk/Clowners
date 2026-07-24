using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(1000)]
public class PlayerInventory : MonoBehaviour
{
    public enum InventoryState
    {
        Empty,
        PickingUp,
        Holding,
        Throwing
    }

    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform collisionRoot;
    [SerializeField] private KinematicCharacterMotor characterMotor;

    [Header("Throw & Launch Settings")]
    [FormerlySerializedAs("throwForce")]
    [SerializeField] private float throwSpeed = 15f;
    [SerializeField] private float throwUpwardModifier = 1.5f;

    [Header("Pickup Animation Settings")]
    [SerializeField] private float pickupDuration = 0.25f;
    [SerializeField] private float pickupPositionTolerance = 0.05f;
    [SerializeField] private float pickupRotationTolerance = 2f;

    [Header("Procedural Motion Settings")]
    [SerializeField] private float bobSpeedMultiplier = 6.5f;
    [SerializeField] private float bobVerticalAmount = 0.035f;
    [SerializeField] private float bobHorizontalAmount = 0.02f;
    [SerializeField] private float bobMovementThreshold = 0.1f;
    [SerializeField] private float bobFadeSharpness = 10f;
    [SerializeField] private float swayPosAmount = 0.0008f;
    [SerializeField] private float swayRotAmount = 0.08f;
    [SerializeField] private float swayMaxPosOffset = 0.04f;
    [SerializeField] private float swayMaxRotOffset = 4f;
    [SerializeField] private float swaySmoothing = 12f;

    [Header("Anti-Clipping Settings")]
    [SerializeField] private LayerMask obstacleLayerMask = 1; // Default layer. Set this to include your walls/floors.
    [SerializeField] private float itemCollisionRadius = 0.15f; // How thick the item is to prevent clipping.

    private InventoryState currentState = InventoryState.Empty;
    private PickupItem currentlyHeldItem;

    // Saved Rigidbody properties to restore on drop/throw/release
    private bool savedIsKinematic;
    private bool savedUseGravity;
    private float savedDrag;
    private float savedAngularDrag;
    private RigidbodyConstraints savedConstraints;
    private RigidbodyInterpolation savedInterpolation;
    private CollisionDetectionMode savedCollisionDetectionMode;
    private float savedMaxAngularVelocity;

    // Colliders cached to disable during hold & restore on release
    private readonly List<KeyValuePair<Collider, bool>> savedHeldColliders = new List<KeyValuePair<Collider, bool>>();

    // Pickup interpolation state
    private Vector3 pickupStartPos;
    private Quaternion pickupStartRot;
    private float pickupElapsedTime;

    // Colliders cached to ignore/restore player collisions
    private Collider[] cachedOwnerColliders;
    private readonly List<KeyValuePair<Collider, Collider>> ignoredColliderPairs = new List<KeyValuePair<Collider, Collider>>();

    // Procedural Motion Tracking
    private Vector3 lastTargetPos;
    private Quaternion lastTargetRot;
    private float bobTimer;
    private float smoothedSpeed;
    private float currentBobWeight;
    private Vector3 currentSwayPosOffset;
    private Vector3 currentSwayRotOffset;

    public InventoryState CurrentState => currentState;
    public bool HasItem => currentlyHeldItem != null && currentState != InventoryState.Empty;
    public PickupItem CurrentlyHeldItem => currentlyHeldItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        CacheOwnerColliders();
    }

    private void CacheOwnerColliders()
    {
        Transform targetRoot = collisionRoot != null ? collisionRoot : transform.root;
        cachedOwnerColliders = targetRoot.GetComponentsInChildren<Collider>(true);

        if (characterMotor == null)
        {
            characterMotor = targetRoot.GetComponentInChildren<KinematicCharacterMotor>(true);
        }
    }

    private void LateUpdate()
    {
        if (currentlyHeldItem == null || currentState == InventoryState.Empty)
            return;

        if (!currentlyHeldItem.gameObject.activeInHierarchy)
        {
            ClearHeldStateInternal(restorePhysics: true);
            return;
        }

        Transform targetTransform = holdPoint != null ? holdPoint : transform;
        Rigidbody rb = currentlyHeldItem.Rb;

        if (rb == null)
        {
            ClearHeldStateInternal(restorePhysics: false);
            return;
        }

        Vector3 rawTargetPos = targetTransform.position;
        Quaternion rawTargetRot = targetTransform.rotation;
        Vector3 desiredPos = rawTargetPos;

        if (currentState == InventoryState.Holding)
        {
            float dt = Time.deltaTime;
            if (dt > 0f)
            {
                float frameSpeed = (rawTargetPos - lastTargetPos).magnitude / dt;
                smoothedSpeed = Mathf.Lerp(smoothedSpeed, frameSpeed, dt * 10f);

                float targetBobWeight = smoothedSpeed > bobMovementThreshold ? 1f : 0f;
                currentBobWeight = Mathf.Lerp(currentBobWeight, targetBobWeight, dt * bobFadeSharpness);
                if (currentBobWeight < 0.0001f) currentBobWeight = 0f;

                if (currentBobWeight > 0f)
                {
                    float speedFactor = Mathf.Clamp(smoothedSpeed / 4f, 0.5f, 1.5f);
                    bobTimer += dt * bobSpeedMultiplier * speedFactor;
                }

                Quaternion rotDelta = rawTargetRot * Quaternion.Inverse(lastTargetRot);
                rotDelta.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f) angle -= 360f;

                Vector3 angularVel = (axis * (angle * Mathf.Deg2Rad)) / dt;
                Vector3 localAngularVel = Quaternion.Inverse(rawTargetRot) * angularVel;

                Vector3 targetSwayPos = new Vector3(
                    Mathf.Clamp(-localAngularVel.y * swayPosAmount, -swayMaxPosOffset, swayMaxPosOffset),
                    Mathf.Clamp(-localAngularVel.x * swayPosAmount, -swayMaxPosOffset, swayMaxPosOffset),
                    0f
                );

                Vector3 targetSwayRot = new Vector3(
                    Mathf.Clamp(localAngularVel.x * swayRotAmount, -swayMaxRotOffset, swayMaxRotOffset),
                    Mathf.Clamp(localAngularVel.y * swayRotAmount, -swayMaxRotOffset, swayMaxRotOffset),
                    Mathf.Clamp(-localAngularVel.z * swayRotAmount, -swayMaxRotOffset, swayMaxRotOffset)
                );

                currentSwayPosOffset = Vector3.Lerp(currentSwayPosOffset, targetSwayPos, dt * swaySmoothing);
                currentSwayRotOffset = Vector3.Lerp(currentSwayRotOffset, targetSwayRot, dt * swaySmoothing);
            }
        }

        lastTargetPos = rawTargetPos;
        lastTargetRot = rawTargetRot;

        Vector3 finalPos = rawTargetPos;
        Quaternion finalRot = rawTargetRot;

        if (currentState == InventoryState.Holding)
        {
            Vector3 localBobOffset = Vector3.zero;
            if (currentBobWeight > 0f)
            {
                localBobOffset.x = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmount * currentBobWeight;
                localBobOffset.y = Mathf.Sin(bobTimer) * bobVerticalAmount * currentBobWeight;
            }

            Vector3 totalLocalOffset = localBobOffset + currentSwayPosOffset;
            desiredPos = rawTargetPos + targetTransform.TransformVector(totalLocalOffset);
            finalRot = rawTargetRot * Quaternion.Euler(currentSwayRotOffset);

            // --- ANTI-CLIPPING LOGIC ---
            // Raycast from the camera down to the desired position to stop clipping
            Vector3 origin = Camera.main != null ? Camera.main.transform.position : (transform.position + Vector3.up * 1.5f);
            Vector3 direction = desiredPos - origin;
            float distance = direction.magnitude;

            // If a wall is in the way, pull the item inward based on the hit point
            if (Physics.SphereCast(origin, itemCollisionRadius, direction.normalized, out RaycastHit hit, distance, obstacleLayerMask, QueryTriggerInteraction.Ignore))
            {
                finalPos = hit.point + (hit.normal * itemCollisionRadius);
            }
            else
            {
                finalPos = desiredPos;
            }
        }

        if (currentState == InventoryState.PickingUp)
        {
            pickupElapsedTime += Time.deltaTime;
            float t = pickupDuration > 0f ? Mathf.Clamp01(pickupElapsedTime / pickupDuration) : 1f;
            float smoothT = t * t * (3f - 2f * t);

            Vector3 interpolatedPos = Vector3.Lerp(pickupStartPos, finalPos, smoothT);
            Quaternion interpolatedRot = Quaternion.Slerp(pickupStartRot, finalRot, smoothT);

            rb.position = interpolatedPos;
            rb.rotation = interpolatedRot;
            currentlyHeldItem.transform.SetPositionAndRotation(interpolatedPos, interpolatedRot);

            float posDist = Vector3.Distance(interpolatedPos, finalPos);
            float angleDist = Quaternion.Angle(interpolatedRot, finalRot);

            if (t >= 1f || (posDist <= pickupPositionTolerance && angleDist <= pickupRotationTolerance))
            {
                currentState = InventoryState.Holding;
            }
        }
        else if (currentState == InventoryState.Holding)
        {
            rb.position = finalPos;
            rb.rotation = finalRot;
            currentlyHeldItem.transform.SetPositionAndRotation(finalPos, finalRot);
        }
    }

    public bool Pickup(PickupItem item)
    {
        if (item == null || currentState != InventoryState.Empty || !item.CanBePickedUp())
            return false;

        Rigidbody rb = item.Rb;
        if (rb == null)
            return false;

        currentlyHeldItem = item;
        currentState = InventoryState.PickingUp;

        Transform targetTransform = holdPoint != null ? holdPoint : transform;
        lastTargetPos = targetTransform.position;
        lastTargetRot = targetTransform.rotation;
        bobTimer = 0f;
        smoothedSpeed = 0f;
        currentBobWeight = 0f;
        currentSwayPosOffset = Vector3.zero;
        currentSwayRotOffset = Vector3.zero;

        savedIsKinematic = rb.isKinematic;
        savedUseGravity = rb.useGravity;
        savedDrag = rb.drag;
        savedAngularDrag = rb.angularDrag;
        savedConstraints = rb.constraints;
        savedInterpolation = rb.interpolation;
        savedCollisionDetectionMode = rb.collisionDetectionMode;
        savedMaxAngularVelocity = rb.maxAngularVelocity;

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        pickupStartPos = rb.position;
        pickupStartRot = rb.rotation;
        pickupElapsedTime = 0f;

        DisableHeldItemColliders(item);
        IgnorePlayerCollisions(item);
        item.OnPickedUp(this);
        return true;
    }

    public bool TrySwap(PickupItem nextItem, Vector3 throwDirection)
    {
        if (nextItem == null || !HasItem || currentState == InventoryState.Throwing || nextItem == currentlyHeldItem || !nextItem.CanBePickedUp())
            return false;

        if (!ThrowItem(throwDirection))
            return false;

        return Pickup(nextItem);
    }

    public bool ThrowItem(Vector3 throwDirection)
    {
        if (!HasItem || currentState == InventoryState.Throwing)
            return false;

        currentState = InventoryState.Throwing;
        PickupItem itemToThrow = currentlyHeldItem;
        Rigidbody rb = itemToThrow != null ? itemToThrow.Rb : null;

        // Force an update to a safe position BEFORE restoring colliders
        if (rb != null)
        {
            rb.position = currentlyHeldItem.transform.position;
        }

        RestorePlayerCollisions();
        RestoreHeldItemColliders();
        RestoreRigidbodyProperties(rb);

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (itemToThrow != null)
        {
            itemToThrow.OnReleased();
            itemToThrow.SetCooldown();
        }

        currentlyHeldItem = null;
        currentState = InventoryState.Empty;

        if (rb != null)
        {
            Vector3 playerVelocity = characterMotor != null ? characterMotor.Velocity : Vector3.zero;
            Vector3 throwDirNorm = throwDirection.sqrMagnitude > 0.0001f ? throwDirection.normalized : transform.forward;

            Vector3 finalLaunchVelocity = playerVelocity + (throwDirNorm * throwSpeed) + (Vector3.up * throwUpwardModifier);
            rb.velocity = finalLaunchVelocity;
        }

        return true;
    }

    public PickupItem ConsumeOrReleaseItem()
    {
        if (!HasItem)
            return null;

        PickupItem item = currentlyHeldItem;
        Rigidbody rb = item != null ? item.Rb : null;

        RestorePlayerCollisions();
        RestoreRigidbodyProperties(rb);
        if (item != null)
        {
            item.OnReleased();
        }

        currentlyHeldItem = null;
        currentState = InventoryState.Empty;
        return item;
    }

    public void NotifyItemDisabledOrDestroyed(PickupItem item)
    {
        if (currentlyHeldItem == item)
        {
            ClearHeldStateInternal(restorePhysics: true);
        }
    }

    public void NotifyItemConsumed(PickupItem item)
    {
        if (currentlyHeldItem == item)
        {
            ClearHeldStateInternal(restorePhysics: false);
            Destroy(item.gameObject);
        }
    }

    private void OnDisable()
    {
        ClearHeldStateInternal(restorePhysics: true);
    }

    private void OnDestroy()
    {
        ClearHeldStateInternal(restorePhysics: true);
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void DisableHeldItemColliders(PickupItem item)
    {
        savedHeldColliders.Clear();
        if (item == null)
            return;

        Collider[] itemColliders = item.GetComponentsInChildren<Collider>(true);
        if (itemColliders == null || itemColliders.Length == 0)
            return;

        for (int i = 0; i < itemColliders.Length; i++)
        {
            Collider col = itemColliders[i];
            if (col == null) continue;

            savedHeldColliders.Add(new KeyValuePair<Collider, bool>(col, col.enabled));
            if (col.enabled)
            {
                col.enabled = false;
            }
        }
    }

    private void RestoreHeldItemColliders()
    {
        for (int i = 0; i < savedHeldColliders.Count; i++)
        {
            Collider col = savedHeldColliders[i].Key;
            bool wasEnabled = savedHeldColliders[i].Value;

            if (col != null)
            {
                col.enabled = wasEnabled;
            }
        }
        savedHeldColliders.Clear();
    }

    private void IgnorePlayerCollisions(PickupItem item)
    {
        ignoredColliderPairs.Clear();
        if (item == null)
            return;

        if (cachedOwnerColliders == null || cachedOwnerColliders.Length == 0)
        {
            CacheOwnerColliders();
        }

        Collider[] itemColliders = item.GetComponentsInChildren<Collider>(true);
        if (itemColliders == null || itemColliders.Length == 0 || cachedOwnerColliders == null)
            return;

        foreach (Collider ownerCol in cachedOwnerColliders)
        {
            if (ownerCol == null) continue;
            foreach (Collider itemCol in itemColliders)
            {
                if (itemCol == null || !itemCol.enabled || !ownerCol.enabled) continue;
                Physics.IgnoreCollision(ownerCol, itemCol, true);
                ignoredColliderPairs.Add(new KeyValuePair<Collider, Collider>(ownerCol, itemCol));
            }
        }
    }

    private void RestorePlayerCollisions()
    {
        for (int i = 0; i < ignoredColliderPairs.Count; i++)
        {
            Collider colA = ignoredColliderPairs[i].Key;
            Collider colB = ignoredColliderPairs[i].Value;
            if (colA != null && colB != null)
            {
                Physics.IgnoreCollision(colA, colB, false);
            }
        }
        ignoredColliderPairs.Clear();
    }

    private void RestoreRigidbodyProperties(Rigidbody rb)
    {
        if (rb == null)
            return;

        rb.isKinematic = savedIsKinematic;
        rb.useGravity = savedUseGravity;
        rb.drag = savedDrag;
        rb.angularDrag = savedAngularDrag;
        rb.constraints = savedConstraints;
        rb.interpolation = savedInterpolation;
        rb.collisionDetectionMode = savedCollisionDetectionMode;
        rb.maxAngularVelocity = savedMaxAngularVelocity;
    }

    private void ClearHeldStateInternal(bool restorePhysics)
    {
        RestorePlayerCollisions();
        RestoreHeldItemColliders();
        PickupItem item = currentlyHeldItem;
        currentlyHeldItem = null;
        currentState = InventoryState.Empty;

        bobTimer = 0f;
        smoothedSpeed = 0f;
        currentBobWeight = 0f;
        currentSwayPosOffset = Vector3.zero;
        currentSwayRotOffset = Vector3.zero;

        if (item != null)
        {
            item.OnReleased();
            if (restorePhysics)
            {
                RestoreRigidbodyProperties(item.Rb);
            }
        }
    }
}