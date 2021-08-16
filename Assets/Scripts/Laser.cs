using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ToggleLaser());
    }

    //Toggle on and off the lasers randomly in an unpredictable manner
    IEnumerator ToggleLaser()
    {
        yield return new WaitForSeconds(Random.Range(1f, 5f));
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;

        yield return new WaitForSeconds(Random.Range(1f, 5f));
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;

        StartCoroutine(ToggleLaser());
    }
}
