using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    Transform tf;
    Transform cannonTf;

    //PID CONTROLLER
    public float P = 0f;
    public float I = 0f;
    public float D = 0f;

    float error_sumX = 0f;
    float error_sumY = 0f;

    public float RotSpeed;
    public float MaxRotSpeed;

    float targetXAngle = 0f;
    float targetYAngle = 0f;
    float lastErrorX = 0f;
    float lastErrorY = 0f;
    float curAngleX = 0f;
    float curAngleY = 0f;

    public Transform target;

    void Start () {
        tf = transform;
        cannonTf = tf.Find("CannonPivot");
        curAngleX = cannonTf.eulerAngles.x;
        curAngleY = cannonTf.eulerAngles.y;
    }
	
	void Update ()
    {
        var rot = Quaternion.LookRotation((target.position - tf.position).normalized);
        targetXAngle = rot.eulerAngles.x;
        targetYAngle = rot.eulerAngles.y;

        Debug.DrawRay(cannonTf.position, cannonTf.forward * 100000f, Color.red);
        Debug.DrawRay(cannonTf.position, (Quaternion.Euler(targetXAngle, targetYAngle, 0) * Vector3.forward) * 5f, Color.green);

        var alphas = GetPIDAlpha();
        float alphaX = alphas.x;
        float alphaY = alphas.y;

        alphaX = Mathf.Clamp(alphaX * Time.deltaTime * RotSpeed, -MaxRotSpeed, MaxRotSpeed);
        alphaY = Mathf.Clamp(alphaY * Time.deltaTime * RotSpeed, -MaxRotSpeed, MaxRotSpeed);

        curAngleY = curAngleY + alphaY;
        curAngleX = curAngleX + alphaX;

        cannonTf.rotation = Quaternion.Euler(curAngleX, curAngleY, 0);

        Debug.Log("X: " + curAngleX + "/" + targetXAngle + " | Y: " + curAngleY + "/" + targetYAngle);
    }

    public Vector3 GetPIDAlpha()
    {
        float alphaX = 0f;
        float alphaY = 0f;

        float errorY = (curAngleY - targetYAngle);
        error_sumY += errorY * Time.deltaTime;
        float derivativeOfErrorY = (errorY - lastErrorY) / Time.deltaTime;
        lastErrorY = errorY;
        alphaY = (errorY * P) + (error_sumY * I) + (derivativeOfErrorY * D);

        float errorX = (curAngleX - targetXAngle);
        error_sumX += errorX * Time.deltaTime;
        float derivativeOfErrorX = (errorX - lastErrorX) / Time.deltaTime;
        lastErrorX = errorX;
        alphaX = (errorX * P) + (error_sumX * I) + (derivativeOfErrorX * D);

        return new Vector3(-alphaX, -alphaY);
    }

    public void SetNewTarget()
    {
        targetXAngle = Random.Range(-75f, 35f);
        targetYAngle = Random.Range(0f, 360f);
        error_sumX = 0f;
        error_sumY = 0f;
        lastErrorY = 0f;
        lastErrorX = 0f;
    }
}
