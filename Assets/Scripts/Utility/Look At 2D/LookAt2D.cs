using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LookAt2D : MonoBehaviour
{
    public FlippedPoint[] AdditionalPointsToFlip;
    public bool FlipX;
    public bool FlipY;
    public bool FlipZ;

    // Private variables..
    private Vector3 LocalScale;

    private void Awake() {
        LocalScale = this.transform.localScale;
    }

    public float PointTorwards(Vector2 Target, float Offset, bool EnableFlipping)
    {
        Vector3 LookDirection = (new Vector3(Target.x, Target.y, 0f) - this.transform.position).normalized;
        float Angle = Mathf.Atan2(LookDirection.y, LookDirection.x) * Mathf.Rad2Deg + Offset;

        if(EnableFlipping)
        {
            if(Angle > 90f || Angle < -90f)
            {
                this.transform.localScale = new Vector3(((FlipX) ? -LocalScale.x : LocalScale.x), ((FlipY) ? -LocalScale.y : LocalScale.y), ((FlipZ) ? -LocalScale.z : LocalScale.z));

                for (int I = 0; I < AdditionalPointsToFlip.Length; I++)
                {
                    AdditionalPointsToFlip[I].Point.localEulerAngles = AdditionalPointsToFlip[I].FlippedEulerAngles;
                }
            }
            
            else {
                this.transform.localScale = LocalScale;

                for (int I = 0; I < AdditionalPointsToFlip.Length; I++)
                {
                    AdditionalPointsToFlip[I].Point.localEulerAngles = AdditionalPointsToFlip[I].NormalEulerAngles;
                }
            }
        }

        this.transform.eulerAngles = new Vector3(0, 0, Angle);
        return Angle;
    }



    [System.Serializable] public struct FlippedPoint
    {
        public Transform Point;
        public Vector3 FlippedEulerAngles;
        public Vector3 NormalEulerAngles;
    }
}
