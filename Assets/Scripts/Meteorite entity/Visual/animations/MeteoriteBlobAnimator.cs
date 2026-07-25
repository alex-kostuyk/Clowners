using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteBlobAnimator : MonoBehaviour
{
    public Transform[] Bones;
    public float Speed = 2f;
    public float MaxOffset = 0.5f;

    [Header("Choppiness Settings")]
    [Tooltip("Lower values make the movement choppier (e.g., 5 to 12 for a stop-motion feel).")]
    public float StepsPerSecond = 8f;

    private struct BoneData
    {
        public Transform Bone;
        public Vector3 InitialLocalPosition;
        public Vector3 RandomAxis;
        public float TimeOffset;
    }

    private BoneData[] _boneDataArray;

    private void Start()
    {
        if (Bones == null || Bones.Length == 0) return;

        _boneDataArray = new BoneData[Bones.Length];

        for (int i = 0; i < Bones.Length; i++)
        {
            if (Bones[i] != null)
            {
                _boneDataArray[i] = new BoneData
                {
                    Bone = Bones[i],
                    InitialLocalPosition = Bones[i].localPosition,
                    RandomAxis = Random.onUnitSphere,
                    TimeOffset = Random.Range(0f, Mathf.PI * 2f)
                };
            }
        }
    }

    private void FixedUpdate()
    {
        if (_boneDataArray == null) return;

        // "Step" the time so it updates in discrete chunks rather than a smooth flow
        float steppedTime = Mathf.Floor(Time.time * StepsPerSecond) / StepsPerSecond;

        for (int i = 0; i < _boneDataArray.Length; i++)
        {
            var data = _boneDataArray[i];

            if (data.Bone != null)
            {
                // Calculate the sine wave using the stepped time instead of Time.time
                float sineWave = Mathf.Sin((steppedTime * Speed) + data.TimeOffset);
                Vector3 currentOffset = data.RandomAxis * (sineWave * MaxOffset);

                data.Bone.localPosition = data.InitialLocalPosition + currentOffset;
            }
        }
    }
}