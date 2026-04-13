using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private Renderer[] renderers;
    private Color[][] originalColors;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>(true);

        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning(gameObject.name + " has no Renderer.");
            return;
        }

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

    public void SetHighlighted(bool highlighted)
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
    
    public void Highlight()
    {
        Debug.Log("Highlight called on: " + gameObject.name);

        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
        {
            r.material.color = Color.red;
        }
    }        
}


// using UnityEngine;

// public class SelectableObject : MonoBehaviour
// {
//     private Renderer[] renderers;
//     private bool isHighlighted = false;

//     // Store original colors so we can restore them correctly
//     private Color[][] originalColors;

//     private void Start()
//     {
//         renderers = GetComponentsInChildren<Renderer>(true);

//         if (renderers == null || renderers.Length == 0)
//         {
//             Debug.LogWarning(gameObject.name + " has no Renderer.");
//             return;
//         }

//         originalColors = new Color[renderers.Length][];

//         for (int i = 0; i < renderers.Length; i++)
//         {
//             // Force unique material instances
//             Material[] mats = renderers[i].materials;
//             originalColors[i] = new Color[mats.Length];

//             for (int j = 0; j < mats.Length; j++)
//             {
//                 if (mats[j].HasProperty("_BaseColor"))
//                     originalColors[i][j] = mats[j].GetColor("_BaseColor");
//                 else if (mats[j].HasProperty("_Color"))
//                     originalColors[i][j] = mats[j].GetColor("_Color");
//                 else
//                     originalColors[i][j] = mats[j].color;
//             }
//         }
//     }

//     public void Highlight()
//     {
//         if (renderers == null || renderers.Length == 0)
//         {
//             Debug.LogWarning(gameObject.name + " has no Renderer to highlight.");
//             return;
//         }

//         isHighlighted = !isHighlighted;

//         for (int i = 0; i < renderers.Length; i++)
//         {
//             Material[] mats = renderers[i].materials;

//             for (int j = 0; j < mats.Length; j++)
//             {
//                 Color targetColor = isHighlighted ? Color.red : originalColors[i][j];

//                 if (mats[j].HasProperty("_BaseColor"))
//                     mats[j].SetColor("_BaseColor", targetColor);

//                 if (mats[j].HasProperty("_Color"))
//                     mats[j].SetColor("_Color", targetColor);

//                 mats[j].color = targetColor;
//             }
//         }

//         Debug.Log(gameObject.name + " highlight toggled: " + isHighlighted);
//     }
// }