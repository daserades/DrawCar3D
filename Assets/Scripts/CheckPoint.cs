using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] CheckPointManager checkPointManager;

    private void Awake()
    {
        checkPointManager = GetComponentInParent<CheckPointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<DrawnBody>())
        {
            checkPointManager.SetActiveCheckPoint(this.transform);
        }
    }
}
