using UnityEngine;

public class ActionSwapMaterials : MonoBehaviour, IAction
{
    [SerializeField]
    private Material _materialToSwap;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private int _targetMaterialIndex;

    public void StartAction()
    {
        if (_meshRenderer == null || _materialToSwap == null) return;

        Material[] currentMaterials = _meshRenderer.materials;

        if (_targetMaterialIndex >= 0 && _targetMaterialIndex < currentMaterials.Length)
        {
            currentMaterials[_targetMaterialIndex] = _materialToSwap;
            _meshRenderer.materials = currentMaterials;
        }
    }
}
