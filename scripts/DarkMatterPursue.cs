using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class DarkMatterPursue : MonoBehaviour
{
    [SerializeField] Transform spiral;
    [SerializeField] Transform target;
    [SerializeField] float minLagDistance = 50f;
    [SerializeField] float smoothMove = 0.5f;
    private Animator animator;
    private float moveSpeed;
    private Vector3 direction;
    private Vector3 desiredSpiralScale;
    private float spiralScaleXY;
    private float spiralScaleZ;
    private float scaleInterpolation;
    private const float MIN_SPEED = 4f;
    private const float MAX_SPEED = 20f;
    private const float SPEED_INCREMENT = 2f;
    private const float SPEED_DECREMENT = 0.4f;
    private const float SCALE_DURATION = 1f;
    private const float SCALE_INFLUENCE_REDUCTION = 2f;

    void Start()
    {
        PlayerController.OnPlayerDied += PlayerController_OnPlayerDied;
        PickupLife.OnPickupWasted += Pickup_OnPickupWasted;
        PickupLife.OnPickupCollected += PickupLife_OnPickupCollected;

        moveSpeed = MIN_SPEED;
        spiralScaleXY = spiral.localScale.x;
        spiralScaleZ = spiral.localScale.z;

        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerDied -= PlayerController_OnPlayerDied;
        PickupLife.OnPickupWasted -= Pickup_OnPickupWasted;
        PickupLife.OnPickupCollected -= PickupLife_OnPickupCollected;
    }

    private void PlayerController_OnPlayerDied()
    {
        // Stop pursuing.
        enabled = false;

        // Sound FX off (abruptly so far).
        GetComponent<AudioSource>().enabled = false;
    }

    private void Pickup_OnPickupWasted()
    {
        // After pickup has been wasted change speed of dark matter
        // and scale spiral gameobject appropriately.
        moveSpeed += SPEED_INCREMENT;

        MoveAndScale();

        animator.SetTrigger("t_anger");
    }

    private void PickupLife_OnPickupCollected(int obj)
    {
        moveSpeed -= SPEED_DECREMENT;

        MoveAndScale();

        // No calmdown animation when it's moving with minimal speed (maybe).
        if (moveSpeed > MIN_SPEED)
            animator.SetTrigger("t_calmdown");
    }

    private void MoveAndScale()
    {
        moveSpeed = Mathf.Clamp(moveSpeed, MIN_SPEED, MAX_SPEED);

        float factor = 1 + (moveSpeed - MIN_SPEED) / MIN_SPEED / SCALE_INFLUENCE_REDUCTION;
        desiredSpiralScale.Set(spiralScaleXY * factor, spiralScaleXY * factor, spiralScaleZ);

        // Make sure there are no other coroutines on this behavior.
        StopAllCoroutines();
        scaleInterpolation = 0f;
        StartCoroutine(ScaleGradually(desiredSpiralScale));
    }

    private IEnumerator ScaleGradually(Vector3 newScale)
    {
        while (scaleInterpolation < 1f)
        {
            scaleInterpolation += Time.deltaTime / SCALE_DURATION;
            spiral.localScale = Vector3.Lerp(spiral.localScale, newScale, scaleInterpolation);

            yield return null;
        }
    }

    void Update()
    {
        direction = target.position - transform.position;
        if (direction.magnitude > minLagDistance)
        {
            direction.Normalize();
            Vector3 desiredPos = target.position - direction * minLagDistance;
            desiredPos.y = 0;
            Vector3 smoothedPos = Vector3.Slerp(transform.position, desiredPos, smoothMove);
            transform.position = smoothedPos;
        }
        else
        {
            direction.Normalize();
            direction.y = 0;
        }

        transform.Translate(direction * Time.deltaTime * moveSpeed);
    }
}
