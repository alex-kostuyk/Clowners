using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeteoriteMouthTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onTrigger,_onEatPlayer;

    private RequestType _rememberedFoodType;
    private bool _lastWasFood = false;

    private void OnTriggerEnter(Collider other)
    {
        _onTrigger?.Invoke();

        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            _onEatPlayer?.Invoke();
            Destroy(player.transform.root.gameObject);
            return;
        }

        if (other.TryGetComponent(out MeteoriteFoodTag foodTag))
        {
            _rememberedFoodType = foodTag.FoodType;
            SendRequest();
            _lastWasFood = true;
            Destroy(other.gameObject);
        }
    }

    public void SendRequest()
    {
        if (MeteoriteEntity.Instance != null && _lastWasFood)
        {
            MeteoriteEntity.Instance.TryToSatisfyRequest(_rememberedFoodType);
            _lastWasFood = false;
        }
    }
}
