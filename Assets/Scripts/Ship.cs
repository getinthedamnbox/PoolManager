using UnityEngine;

public class Ship : MonoBehaviour
{
    private void OnTriggerExit(UnityEngine.Collider other)
    {
        if (other.CompareTag("Boundaries"))
        {
            if (SpawnManager.Instance.UsePoolManager)
            {
                PoolManager.Instance.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
