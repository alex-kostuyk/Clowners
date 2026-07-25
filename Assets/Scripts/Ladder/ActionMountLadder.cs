using UnityEngine;
using KinematicCharacterController.Examples;

/// <summary>
/// Action that mounts the player onto a Ladder when triggered by the interaction system (E key).
/// Attach to the same GameObject (or a child) that has the Ladder component.
/// Also requires an Event component on the interactable collider to hook into the E-key system.
/// </summary>
public class ActionMountLadder : MonoBehaviour, IAction
{
    [Tooltip("Reference to the Ladder component. Auto-resolved from this or parent GameObject if left empty.")]
    [SerializeField] private Ladder _ladder;

    private void Awake()
    {
        if (_ladder == null)
            _ladder = GetComponentInParent<Ladder>();
        if (_ladder == null)
            _ladder = GetComponent<Ladder>();
    }

    public void StartAction()
    {
        if (_ladder == null) return;

        // Find the player's character controller
        ExampleCharacterController controller = FindObjectOfType<ExampleCharacterController>();
        if (controller == null) return;

        // Don't mount if already climbing
        if (controller.CurrentCharacterState == CharacterState.Climbing)
            return;

        controller.MountLadder(_ladder);
    }
}
