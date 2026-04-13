using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [Header("References")]
    public Transform rayOrigin;
    public Transform grabAnchor;
    
    [Header("Selection")]
    public GameObject selectedObject;

    [Header("Near Grab")]
    public GameObject grabbedObject;
    public float grabRadius = 0.05f;
    public float triggerThreshold = 0.1f;

    [Header("Ray Grab")]
    public GameObject rayGrabbedObject;
    public float rayGrabMaxDistance = 20f;
    public float rayGrabDistance = 2f;
    public float rayMoveSpeed = 2f;
    public float minRayGrabDistance = 0.3f;

    [Header("Raycast Settings")]
    public LayerMask interactionMask = ~0; // Exclude virtual hand layer in Inspector

    public Vector3 GetPointingDir()
    {
        if (rayOrigin != null)
            return rayOrigin.forward.normalized;

        return transform.forward.normalized;
    }

    public Vector3 GetPosition()
    {
        if (rayOrigin != null)
            return rayOrigin.position;

        return transform.position;
    }

    // -----------------------------
    // Interaction #1: Selection
    // -----------------------------
    public void GetButtonPress()
    {
        // Left controller X button
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            Debug.Log("X pressed: casting selection ray.");
            CastRay();
        }
    }

    // public void CastRay()
    // {
    //     RaycastHit hit;
    //     Vector3 origin = GetPosition();
    //     Vector3 direction = GetPointingDir();

    //     Debug.DrawRay(origin, direction * 100f, Color.red, 2f);

    //     if (Physics.Raycast(origin, direction, out hit, 100f, interactionMask, QueryTriggerInteraction.Ignore))
    //     {
    //         selectedObject = hit.collider.gameObject;
    //         Debug.Log("Selected object: " + selectedObject.name);

    //         SelectableObject selectable = selectedObject.GetComponent<SelectableObject>();
    //         if (selectable != null)
    //         {
    //             selectable.Highlight();
    //         }
    //         else
    //         {
    //             Debug.Log("Hit object does not have SelectableObject.");
    //         }
    //     }
    //     else
    //     {
    //         selectedObject = null;
    //         Debug.Log("Selection ray hit nothing.");
    //     }
    // }

    ///// 
    // public void CastRay()
    // {
    //     RaycastHit hit;
    //     Vector3 origin = GetPosition();
    //     Vector3 direction = GetPointingDir();

    //     Debug.DrawRay(origin, direction * 100f, Color.red, 2f);

    //     if (Physics.Raycast(origin, direction, out hit, 100f, interactionMask, QueryTriggerInteraction.Ignore))
    //     {
    //         selectedObject = hit.collider.gameObject;
    //         Debug.Log("Selected object: " + selectedObject.name);

    //         SelectableObject selectable = selectedObject.GetComponent<SelectableObject>();

    //         if (selectable == null)
    //         {
    //             selectable = selectedObject.GetComponentInParent<SelectableObject>();
    //         }

    //         if (selectable != null)
    //         {
    //             selectable.Highlight();
    //         }
    //         else
    //         {
    //             Debug.Log("Hit object does not have SelectableObject.");
    //         }
    //     }
    //     else
    //     {
    //         selectedObject = null;
    //         Debug.Log("Selection ray hit nothing.");
    //     }
    // }


    // Work
    // public void CastRay()
    // {
    //     RaycastHit hit;
    //     Vector3 origin = GetPosition();
    //     Vector3 direction = GetPointingDir();

    //     Debug.DrawRay(origin, direction * 100f, Color.red, 2f);
    //     Debug.Log("Casting ray from: " + origin + " direction: " + direction);

    //     if (Physics.Raycast(origin, direction, out hit, 100f, interactionMask, QueryTriggerInteraction.Ignore))
    //     {
    //         selectedObject = hit.collider.gameObject;
    //         Debug.Log("Selected object: " + selectedObject.name);

    //         SelectableObject selectable = selectedObject.GetComponent<SelectableObject>();

    //         if (selectable == null)
    //         {
    //             selectable = selectedObject.GetComponentInParent<SelectableObject>();
    //         }

    //         if (selectable != null)
    //         {
    //             Debug.Log("SelectableObject found on: " + selectable.gameObject.name);
    //             selectable.Highlight();
    //         }
    //         else
    //         {
    //             Debug.Log("Hit object does not have SelectableObject.");
    //         }
    //     }
    //     else
    //     {
    //         selectedObject = null;
    //         Debug.Log("Selection ray hit nothing.");
    //     }
    // }

    public void CastRay()
    {
        RaycastHit hit;
        Vector3 origin = GetPosition();
        Vector3 direction = GetPointingDir();

        Debug.DrawRay(origin, direction * 100f, Color.red, 2f);
        Debug.Log("Casting ray from: " + origin + " direction: " + direction);

        if (Physics.Raycast(origin, direction, out hit, 100f, interactionMask, QueryTriggerInteraction.Ignore))
        {
            selectedObject = hit.collider.gameObject;
            Debug.Log("Selected object: " + selectedObject.name);

            SelectableObject selectable = selectedObject.GetComponent<SelectableObject>();

            if (selectable == null)
                selectable = selectedObject.GetComponentInParent<SelectableObject>();

            if (selectable == null)
                selectable = selectedObject.GetComponentInChildren<SelectableObject>();

            if (selectable != null)
            {
                Debug.Log("SelectableObject found on: " + selectable.gameObject.name);
                selectable.SetHighlighted(true);
            }
            else
            {
                Debug.Log("Hit object does not have SelectableObject.");
            }
        }
        else
        {
            selectedObject = null;
            Debug.Log("Selection ray hit nothing.");
        }
    }

    // -----------------------------
    // Interaction #2: Near Grab
    // -----------------------------
    public void GetTriggerPress()
    {
        // Left controller grip / hand trigger
        float pressedValue = OVRInput.Get(
            OVRInput.Axis1D.PrimaryHandTrigger,
            OVRInput.Controller.LTouch
        );

        if (pressedValue > triggerThreshold)
        {
            GrabObject(pressedValue);
        }
        else
        {
            ReleaseObject();
        }
    }

    // public void GrabObject(float pressedValue)
    // {
    //     // Do not near-grab while ray-grabbing
    //     if (rayGrabbedObject != null)
    //         return;

    //     // Keep current object grabbed while grip is held
    //     if (grabbedObject != null)
    //     {
    //         GrabbableObject current = grabbedObject.GetComponent<GrabbableObject>();
    //         if (current != null)
    //         {
    //             current.Grab(pressedValue);
    //         }
    //         return;
    //     }

    //     Collider[] nearbyColliders = Physics.OverlapSphere(
    //         GetPosition(),
    //         grabRadius,
    //         interactionMask,
    //         QueryTriggerInteraction.Ignore
    //     );

    //     GameObject closestObject = null;
    //     float closestDistance = Mathf.Infinity;

    //     foreach (Collider col in nearbyColliders)
    //     {
    //         GrabbableObject grabbable = col.GetComponent<GrabbableObject>();
    //         if (grabbable == null)
    //             continue;

    //         float distance = Vector3.Distance(GetPosition(), col.transform.position);
    //         if (distance < closestDistance)
    //         {
    //             closestDistance = distance;
    //             closestObject = col.gameObject;
    //         }
    //     }

    //     if (closestObject != null)
    //     {
    //         grabbedObject = closestObject;

    //         GrabbableObject grabbedScript = grabbedObject.GetComponent<GrabbableObject>();
    //         if (grabbedScript != null)
    //         {
    //             grabbedScript.Grab(pressedValue);
    //         }

    //         Transform parentTarget = rayOrigin != null ? rayOrigin : transform;
    //         grabbedObject.transform.SetParent(parentTarget, true);

    //         Debug.Log("Near grabbed object: " + grabbedObject.name);
    //     }
    // }

    public void GrabObject(float pressedValue)
    {
        if (rayGrabbedObject != null)
            return;

        if (grabbedObject != null)
        {
            // static
            // GrabbableObject current = grabbedObject.GetComponent<GrabbableObject>();
            // if (current != null)
            // {
            //     current.Grab(pressedValue);
            // }
            // return;
            //dynamic
            return;
        }

        Collider[] nearbyColliders = Physics.OverlapSphere(
            GetPosition(),
            grabRadius,
            interactionMask,
            QueryTriggerInteraction.Ignore
        );

        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in nearbyColliders)
        {
            GrabbableObject grabbable = col.GetComponent<GrabbableObject>();
            if (grabbable == null)
                continue;

            float distance = Vector3.Distance(GetPosition(), col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = col.gameObject;
            }
        }

        if (closestObject != null)
        {
            grabbedObject = closestObject;

            /////// color change
            // SelectableObject selectable = grabbedObject.GetComponent<SelectableObject>();
            // if (selectable == null)
            //     selectable = grabbedObject.GetComponentInParent<SelectableObject>();
            // if (selectable == null)
            //     selectable = grabbedObject.GetComponentInChildren<SelectableObject>();

            // if (selectable != null)
            //     selectable.SetHighlighted(false);
            ////////
            ///
            /// static 
            // GrabbableObject grabbedScript = grabbedObject.GetComponent<GrabbableObject>();
            // if (grabbedScript != null)
            // {
            //     grabbedScript.Grab(pressedValue);
            // }

            // Transform parentTarget = grabAnchor != null ? grabAnchor : transform;
            // grabbedObject.transform.SetParent(parentTarget, true);
            // dynamic
            GrabbableObject grabbedScript = grabbedObject.GetComponent<GrabbableObject>();
            Transform followTarget = grabAnchor != null ? grabAnchor : transform;

            if (grabbedScript != null)
            {
                grabbedScript.Grab(followTarget);
            }
            Debug.Log("Near grabbed object: " + grabbedObject.name);
        }
    }

    public void ReleaseObject()
    {
        if (grabbedObject == null)
            return;

        // GrabbableObject grabbedScript = grabbedObject.GetComponent<GrabbableObject>();
        // if (grabbedScript != null)
        // {
        //     grabbedScript.Grab(0f);
        // }
        // grabbedObject.transform.SetParent(null, true);

        // dynamic
        GrabbableObject grabbedScript = grabbedObject.GetComponent<GrabbableObject>();
        if (grabbedScript != null)
        {
            // SelectableObject selectable = grabbedObject.GetComponent<SelectableObject>();
            // if (selectable == null)
            //     selectable = grabbedObject.GetComponentInParent<SelectableObject>();
            // if (selectable == null)
            //     selectable = grabbedObject.GetComponentInChildren<SelectableObject>();

            // if (selectable != null)
            //     selectable.SetHighlighted(true);        
            grabbedScript.Release();
        }

        Debug.Log("Released near grabbed object: " + grabbedObject.name);

        ///////
        /////////
        grabbedObject = null;
    }

    // -----------------------------
    // Extra Credit: Ray Grab
    // -----------------------------
    public void GetRayGrabInput()
    {
        // Hold Y to ray-grab from distance
        bool isHoldingRayGrab = OVRInput.Get(OVRInput.RawButton.Y);

        if (isHoldingRayGrab)
        {
            if (rayGrabbedObject == null)
            {
                StartRayGrab();
            }
            else
            {
                UpdateRayGrab();
                UpdateRayGrabDistance();
            }
        }
        else
        {
            ReleaseRayGrab();
        }
    }

    public void StartRayGrab()
    {
        // Do not ray-grab while near-grabbing
        if (grabbedObject != null)
            return;

        RaycastHit hit;
        Vector3 origin = GetPosition();
        Vector3 direction = GetPointingDir();

        Debug.DrawRay(origin, direction * rayGrabMaxDistance, Color.blue, 2f);

        if (Physics.Raycast(origin, direction, out hit, rayGrabMaxDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            GrabbableObject grabbable = hit.collider.GetComponent<GrabbableObject>();

            if (grabbable != null)
            {
                rayGrabbedObject = hit.collider.gameObject;
                rayGrabDistance = Vector3.Distance(origin, rayGrabbedObject.transform.position);

                // grabbable.Grab(1f);
                Transform followTarget = grabAnchor != null ? grabAnchor : transform;
                grabbable.Grab(followTarget);

                Debug.Log("Ray grabbed object: " + rayGrabbedObject.name);
                Debug.Log("Initial ray grab distance: " + rayGrabDistance);
            }
            else
            {
                Debug.Log("Ray hit object, but it is not grabbable.");
            }
        }
        else
        {
            Debug.Log("Ray grab hit nothing.");
        }
    }

    public void UpdateRayGrab()
    {
        if (rayGrabbedObject == null)
            return;

        Vector3 targetPosition = GetPosition() + GetPointingDir() * rayGrabDistance;
        rayGrabbedObject.transform.position = targetPosition;
    }

    public void UpdateRayGrabDistance()
    {
        if (rayGrabbedObject == null)
            return;

        Vector2 joystick = OVRInput.Get(
            OVRInput.Axis2D.PrimaryThumbstick,
            OVRInput.Controller.LTouch
        );

        float pushPullInput = joystick.y;

        if (Mathf.Abs(pushPullInput) > 0.1f)
        {
            rayGrabDistance += pushPullInput * rayMoveSpeed * Time.deltaTime;
            rayGrabDistance = Mathf.Clamp(rayGrabDistance, minRayGrabDistance, rayGrabMaxDistance);
        }
    }

    public void ReleaseRayGrab()
    {
        if (rayGrabbedObject == null)
            return;

        GrabbableObject grabbedScript = rayGrabbedObject.GetComponent<GrabbableObject>();
        if (grabbedScript != null)
        {
            // grabbedScript.Grab(0f);
            grabbedScript.Release();
        }

        Debug.Log("Released ray grabbed object: " + rayGrabbedObject.name);
        rayGrabbedObject = null;
    }

    private void Update()
    {
        OVRInput.Update();

        GetButtonPress();
        GetTriggerPress();
        GetRayGrabInput();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = rayOrigin != null ? rayOrigin.position : transform.position;
        Vector3 forward = rayOrigin != null ? rayOrigin.forward : transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, grabRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(origin, forward * rayGrabMaxDistance);
    }
}