// using UnityEngine;

// public class GrabbableObject : MonoBehaviour
// {
//     public bool isGrabbed = false;
//     private Rigidbody rb;

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody>();
//     }

//     public void Grab(float triggerPress)
//     {
//         if (triggerPress > 0.1f)
//         {
//             isGrabbed = true;

//             if (rb != null)
//             {
//                 rb.isKinematic = true;

// #if UNITY_6000_0_OR_NEWER
//                 rb.linearVelocity = Vector3.zero;
// #else
//                 rb.velocity = Vector3.zero;
// #endif

//                 rb.angularVelocity = Vector3.zero;
//             }
//         }
//         else
//         {
//             isGrabbed = false;

//             if (rb != null)
//             {
//                 rb.isKinematic = false;
//             }
//         }
//     }
// }


///// Work No color
// using UnityEngine;

// public class GrabbableObject : MonoBehaviour
// {
//     public bool isGrabbed = false;

//     private Rigidbody rb;
//     private Transform followTarget;

//     private Vector3 lastTargetPosition;
//     private Quaternion lastTargetRotation;

//     private Vector3 releaseVelocity;
//     private Vector3 releaseAngularVelocity;

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody>();

//         if (rb != null)
//         {
//             rb.useGravity = true;
//         }
//     }

//     private void FixedUpdate()
//     {
//         if (!isGrabbed || followTarget == null || rb == null)
//             return;

//         Vector3 newPosition = followTarget.position;
//         Quaternion newRotation = followTarget.rotation;

//         releaseVelocity = (newPosition - lastTargetPosition) / Time.fixedDeltaTime;

//         Quaternion deltaRotation = newRotation * Quaternion.Inverse(lastTargetRotation);
//         deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 axis);

//         if (angleInDegrees > 180f)
//             angleInDegrees -= 360f;

//         if (Mathf.Abs(angleInDegrees) < 0.001f || axis == Vector3.zero)
//             releaseAngularVelocity = Vector3.zero;
//         else
//             releaseAngularVelocity = axis * angleInDegrees * Mathf.Deg2Rad / Time.fixedDeltaTime;

//         rb.MovePosition(newPosition);
//         rb.MoveRotation(newRotation);

//         lastTargetPosition = newPosition;
//         lastTargetRotation = newRotation;
//     }

//     public void Grab(Transform target)
//     {
//         if (rb == null)
//             return;

//         isGrabbed = true;
//         followTarget = target;

//         rb.useGravity = false;
//         rb.isKinematic = true;

//         lastTargetPosition = followTarget.position;
//         lastTargetRotation = followTarget.rotation;

//         releaseVelocity = Vector3.zero;
//         releaseAngularVelocity = Vector3.zero;
//     }

//     public void Release()
//     {
//         if (rb == null)
//             return;

//         isGrabbed = false;
//         followTarget = null;

//         rb.isKinematic = false;
//         rb.useGravity = true;

//         rb.linearVelocity = releaseVelocity;
//         rb.angularVelocity = releaseAngularVelocity;
//     }
// }


using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool isGrabbed = false;

    private Rigidbody rb;
    private Transform followTarget;

    private Vector3 lastTargetPosition;
    private Quaternion lastTargetRotation;

    private Vector3 releaseVelocity;
    private Vector3 releaseAngularVelocity;

    private Renderer[] renderers;
    private Color[][] originalColors;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = true;
        }

        renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers != null && renderers.Length > 0)
        {
            originalColors = new Color[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].materials;
                originalColors[i] = new Color[mats.Length];

                for (int j = 0; j < mats.Length; j++)
                {
                    if (mats[j].HasProperty("_BaseColor"))
                        originalColors[i][j] = mats[j].GetColor("_BaseColor");
                    else if (mats[j].HasProperty("_Color"))
                        originalColors[i][j] = mats[j].GetColor("_Color");
                    else
                        originalColors[i][j] = mats[j].color;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isGrabbed || followTarget == null || rb == null)
            return;

        Vector3 newPosition = followTarget.position;
        Quaternion newRotation = followTarget.rotation;

        releaseVelocity = (newPosition - lastTargetPosition) / Time.fixedDeltaTime;

        Quaternion deltaRotation = newRotation * Quaternion.Inverse(lastTargetRotation);
        deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 axis);

        if (angleInDegrees > 180f)
            angleInDegrees -= 360f;

        if (Mathf.Abs(angleInDegrees) < 0.001f || axis == Vector3.zero)
            releaseAngularVelocity = Vector3.zero;
        else
            releaseAngularVelocity = axis * angleInDegrees * Mathf.Deg2Rad / Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
        rb.MoveRotation(newRotation);

        lastTargetPosition = newPosition;
        lastTargetRotation = newRotation;
    }

    public void Grab(Transform target)
    {
        if (rb == null)
            return;

        isGrabbed = true;
        followTarget = target;

        rb.useGravity = false;
        rb.isKinematic = true;

        lastTargetPosition = followTarget.position;
        lastTargetRotation = followTarget.rotation;

        releaseVelocity = Vector3.zero;
        releaseAngularVelocity = Vector3.zero;

        SetHighlighted(true);
    }

    public void Release()
    {
        if (rb == null)
            return;

        isGrabbed = false;
        followTarget = null;

        rb.isKinematic = false;
        rb.useGravity = true;

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = releaseVelocity;
#else
        rb.velocity = releaseVelocity;
#endif
        rb.angularVelocity = releaseAngularVelocity;

        SetHighlighted(false);
    }

    private void SetHighlighted(bool highlighted)
    {
        if (renderers == null || renderers.Length == 0)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].materials;

            for (int j = 0; j < mats.Length; j++)
            {
                Color targetColor = highlighted ? Color.red : originalColors[i][j];

                if (mats[j].HasProperty("_BaseColor"))
                    mats[j].SetColor("_BaseColor", targetColor);

                if (mats[j].HasProperty("_Color"))
                    mats[j].SetColor("_Color", targetColor);

                mats[j].color = targetColor;
            }
        }
    }
}