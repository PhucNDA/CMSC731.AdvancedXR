using UnityEngine;

public class ToggleAmbientSound : MonoBehaviour
{
    public AudioSource ambientSource;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (ambientSource.isPlaying)
                ambientSource.Stop();
            else
                ambientSource.Play();
        }
    }
}
