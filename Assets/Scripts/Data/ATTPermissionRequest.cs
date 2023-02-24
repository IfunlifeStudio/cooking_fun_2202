using UnityEngine;
using System.Collections;
#if UNITY_IOS
// Include the IosSupport namespace if running on iOS:
using Unity.Advertisement.IosSupport;
#endif
public class ATTPermissionRequest : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
#if UNITY_IOS
        // Check the user's consent status.
        // If the status is undetermined, display the request request: 
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}