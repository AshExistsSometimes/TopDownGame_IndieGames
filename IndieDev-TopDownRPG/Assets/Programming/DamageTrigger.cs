using UnityEngine;
using UnityEngine.Rendering.UI;

public class DamageTrigger : MonoBehaviour
{
    public float Damage = 1f;

    private void OnTriggerEnter(Collider collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage(Damage);
        }
    }
}
