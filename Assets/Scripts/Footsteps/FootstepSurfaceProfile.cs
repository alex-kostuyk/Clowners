using UnityEngine;

namespace Footsteps
{
    [CreateAssetMenu(fileName = "FootstepProfile_", menuName = "Audio/Footstep Surface Profile")]
    public class FootstepSurfaceProfile : ScriptableObject
    {
        public string TagName;
        public AudioClip[] WalkClips;
        public AudioClip[] LandClips;
        public AudioClip[] JumpClips;
    }
}
