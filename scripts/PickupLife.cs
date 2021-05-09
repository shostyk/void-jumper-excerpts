using System;
using System.Collections;
using UnityEngine;
using OlegShostyk;

[RequireComponent(typeof(Animator))]
public class PickupLife : MonoBehaviour
{
    // Player collects this pickup.
    public static event Action<int> OnPickupCollected;
    // End of life or devoured by dark matter.
    public static event Action OnPickupWasted;

    [SerializeField] int score;
    [SerializeField] int lifeTime;
    [SerializeField] float rotationSpeed;
    [Header("Event Sounds")]
    [SerializeField] AudioClip collectedSound;
    [SerializeField] AudioClip wastedSound;

    void OnEnable()
    {
        StartCoroutine(Live());
    }

    void Update()
    {
        // Innocent rotation.
        transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectPickup();
        }

        else if (other.CompareTag("Dark Void"))
        {
            WasteLife();
        }
    }

    private IEnumerator Live()
    {
        yield return new WaitForSeconds(lifeTime);

        GetComponent<Animator>().SetTrigger("t_wasteLife");
    }

    private void CollectPickup()
    {
        // Fire up event.
        OnPickupCollected?.Invoke(score);
        // Show particle effect.
        HandyTool.CreateInstanceFromPool(MyTag.CollectFX, transform.position);
        // Play sound effect.
        GameObject soundClip = HandyTool.CreateInstanceFromPool(MyTag.SoundClip, transform.position);
        soundClip.GetComponent<SoundLife>().PlaySound(collectedSound, 0.3f, 1.8f);
        // Return to pool.
        Deactivate();
    }

    private void WasteLife()
    {
        // Fire up event.
        OnPickupWasted?.Invoke();
        // Show particle effect.
        HandyTool.CreateInstanceFromPool(MyTag.DevourFX, transform.position);
        // Play sound effect.
        GameObject soundClip = HandyTool.CreateInstanceFromPool(MyTag.SoundClip, transform.position);
        soundClip.GetComponent<SoundLife>().PlaySound(wastedSound, 1f, 0.6f);
        // Return to pool.
        Deactivate();
    }

    private void Deactivate()
    {
        // Free this object.
        ObjectsPooler.Instance.Free(MyTag.Pickup, gameObject);
    }
}
