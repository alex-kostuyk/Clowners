using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using Assets.SimpleLocalization.Scripts;
public class CameraLookAtEventTrigger : MonoBehaviour
{
    [SerializeField] private float timeBetweenCheck = 0.2f, maxDistance = 2, sliderSpeed = 0.1f, eventProgressUpdateRate = 0.04f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject[] EventUI;
    [SerializeField] private Slider progressBarr;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerInventory cachedInventory;

    private string defaultDescription = "USE/INTERACT",handsFullDescription = "[HANDS FULL]";
    public Event lastEvent;
    private Coroutine eventProgressCoroutine;

    private void OnEnable()
    {
        StartCoroutine(checkForAction());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
            EventAction();
    }

    private PlayerInventory GetInventory()
    {
        if (cachedInventory == null)
        {
            cachedInventory = PlayerInventory.Instance;
            if (cachedInventory == null)
            {
                cachedInventory = FindObjectOfType<PlayerInventory>();
            }
        }
        return cachedInventory;
    }

    private void setActiveForArray(bool state)
    {
         foreach (GameObject obj in EventUI)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }

    private void handleRayCastMiss()
    {
        setActiveForArray(false);
        StopEventProgress();
        description.text = defaultDescription;
        description.gameObject.SetActive(false);

    }

    private bool IsPickupEvent(Event evt)
    {
        if (evt == null) return false;
        return evt.GetComponentInChildren<ActionPickupItem>() != null;
    }

    public void EventAction()
    {
        PlayerInventory inventory = GetInventory();

        if (lastEvent != null && IsPickupEvent(lastEvent))
        {
            lastEvent.Action();
            if (!lastEvent.IsWaitEvent())
                lastEvent = null;
            return;
        }

        if (inventory != null && inventory.HasItem)
        {
            Vector3 throwDir = playerCamera != null ? playerCamera.forward : transform.forward;
            inventory.ThrowItem(throwDir);
            return;
        }

        if (lastEvent == null)//|| (lastEvent.limitForHeavy && inventory.CarryHeavyItem))
            return;

        lastEvent.Action();
        if(!lastEvent.IsWaitEvent())
            lastEvent = null;
    }

    public bool IsActionReady()
    {
        if(progressBarr.value == progressBarr.maxValue)
            return true;

        progressBarr.value += sliderSpeed;
        return false;
    }

    public void StartEventProgress()
    {
        if(!lastEvent.IsWaitEvent())//|| (lastEvent.limitForHeavy && inventory.CarryHeavyItem))
            return;
        progressBarr.gameObject.SetActive(true);
        eventProgressCoroutine = StartCoroutine( eventProgress());
    }

    public void StopEventProgress()
    {
         progressBarr.gameObject.SetActive(false);
         progressBarr.value = progressBarr.minValue;
         if (eventProgressCoroutine != null)
        {
            StopCoroutine(eventProgressCoroutine);
            eventProgressCoroutine = null; 
        }
         lastEvent = null;
    }

    IEnumerator checkForAction()
    {
        Vector3 raycastOrigin;
        Vector3 raycastDirection;
        RaycastHit hit;
        Ray ray;

        while (true)
        {
            transform.rotation = playerCamera.rotation;

            raycastOrigin = transform.position;
            raycastDirection = transform.forward;
            ray = new Ray(raycastOrigin, raycastDirection);
      
            if (Physics.Raycast(ray, out hit, maxDistance, hitLayer))
            {
                Event targetEvent = hit.collider.GetComponent<Event>();

                if (targetEvent != null)
                {

                    setActiveForArray(true);
                    lastEvent = targetEvent;
                       /* if (lastEvent.limitForHeavy && inventory.CarryHeavyItem)
                            discription.text = handsFullDiscription;
                        else*/
                    description.text =  lastEvent.Discription==""?defaultDescription: lastEvent.Discription;

                    description.gameObject.SetActive(true);
                }
                else
                    handleRayCastMiss();
             
            }
            else
                handleRayCastMiss();
           
                yield return new WaitForSeconds(timeBetweenCheck);
        }
    }

     IEnumerator eventProgress()
    {
        while (lastEvent.IsWaitEvent())
        {
            lastEvent.Action();

            if(progressBarr.value == progressBarr.maxValue)
            {
                lastEvent.Action();
                StopEventProgress();
            }
                
            yield return new WaitForSeconds(eventProgressUpdateRate);
        }
        StopEventProgress();
    }
}
