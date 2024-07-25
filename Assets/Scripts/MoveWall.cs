using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    AudioSource audioSource;
    public void BeginMovement()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(MoveWallCoroutine());
    }
    private void MoveMe()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, 7.7f), 0.1f);
    }

    IEnumerator MoveWallCoroutine()
    {
        audioSource.Play();
        while (transform.position.z < 7.7f)
        {
            MoveMe();
            yield return new WaitForSeconds(0.01f);
        }
        audioSource.Stop();
    }
}
