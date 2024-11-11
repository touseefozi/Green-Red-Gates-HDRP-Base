using DG.Tweening;
using UnityEngine;

public class SizeController : MonoBehaviour
{
    public float minSize, maxSize;
    public float time;
    public Animator playernimatior;
    private void OnEnable()
    {
        GateCollider.SizeChanged += OnSizeChange;
    }

    private void OnDisable()
    {
        GateCollider.SizeChanged -= OnSizeChange;
    }

    private void OnSizeChange(float delta)
    {
        // Calculate the new target scale
        Vector3 targetScale = transform.localScale + new Vector3(delta, delta, delta);

        // Clamp each axis to be within the min and max size
        float clampedX = Mathf.Clamp(targetScale.x, minSize, maxSize);
        float clampedY = Mathf.Clamp(targetScale.y, minSize, maxSize);
        float clampedZ = Mathf.Clamp(targetScale.z, minSize, maxSize);

        // Apply the clamped scale using DOScale
        transform.DOScale(new Vector3(clampedX, clampedY, clampedZ), time);
    }
    public bool tweenDie;

    [ContextMenu("Die")]

    public void Die()
    {
        if (!tweenDie)
        {
            playernimatior.Play("die");
            return;
        }

        // Move the spider slightly upward (Y axis) and backward (Z axis) to simulate falling back
        // Move up by 1.5 units over 1 second
        transform.DOMoveY(transform.position.y + 4f, .5f).OnComplete(() =>
        {
            // Move back down by 1 unit over 1 second
            transform.DOMoveY(transform.position.y - 1.5f, .5f).SetEase(Ease.OutBounce);
        }).SetEase(Ease.Linear);


        // Rotate the spider upside down over 2 seconds
        transform.DORotate(new Vector3(-180, 0, 0), .8f).SetEase(Ease.Linear);
            

        transform.DOMoveZ(transform.position.z - 0.3f, 2)
            .SetEase(Ease.OutQuad); // Smooth backward movement
    }


}
