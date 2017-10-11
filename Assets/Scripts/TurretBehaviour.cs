using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurretBehaviour : MonoBehaviour
{
    [Range(0f, 0.1f)]
    public float MaxError;
    Transform tf;
    Transform cannonTf;

    public Transform target;

    Tween tween;

    void Start()
    {
        tf = transform;
        cannonTf = tf.Find("CannonPivot");
    }

    void Update()
    {
        var line = target.position - cannonTf.position;
        var dir = line.normalized;
        var rot = Quaternion.LookRotation(dir);

        cannonTf.rotation = Quaternion.LookRotation(dir + new Vector3(Random.Range(-MaxError, MaxError), Random.Range(-MaxError, MaxError), Random.Range(-MaxError, MaxError)));

        Color color = Color.red;
        RaycastHit hit;
        if (Physics.Raycast(cannonTf.position, cannonTf.forward, out hit))
        {
            if (hit.transform.name == "Fighter")
            {
                color = Color.green;
            }
        }

        Debug.DrawRay(cannonTf.position, cannonTf.forward * (line.magnitude + 1), color);
    }
}