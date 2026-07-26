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

            if (MeteoriteEntity.Instance != null)
                MeteoriteEntity.Instance.TryToSatisfyRequest(_rememberedFoodType);
         
            Destroy(other.gameObject);
        }
    }
}
