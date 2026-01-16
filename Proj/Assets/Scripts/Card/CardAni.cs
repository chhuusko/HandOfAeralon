using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CardAni : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator MoveTo(Transform destination)
    {
        Vector3.Lerp(transform.position, destination.position, 0.1f);
        yield return new WaitForSeconds(0.01f);
    }
}
