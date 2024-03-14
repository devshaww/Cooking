using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    // 因为不能设置KitchenObject即NetworkBehaviour的transform为player的transform 因为player为动态生成
    // 所以利用FollowTransform来让KitchenObject跟着Player移动
    private Transform targetTransform;

    public void SetTargetTransform(Transform transform) {
        targetTransform = transform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) {
            return;
        }
        transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
    }
}
