using UnityEngine;

namespace _Project.Scripts.Impact
{
    public class ImpactEffect : MonoBehaviour
    {
        public void SetDefaultPosition(RaycastHit hit)
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }
}