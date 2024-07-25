using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiation : MonoBehaviour
{

    public GameObject Canvas, SpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        GameObject instance = Instantiate(Canvas, new Vector3(0, 0, 0), Quaternion.identity);
        instance.transform.SetParent(SpawnPoint.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
