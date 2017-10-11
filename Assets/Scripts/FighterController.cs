using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterController : MonoBehaviour
{
    public float MaxVelocity = 150;
    public float MaxAcceleration = 30;
    public float RotationSpeed = 5;
    [Range(0f, 1f)]
    public float VelocityCorrector = 0.5f;

    private float velocity;
    private float acceleration;

    private Rigidbody rb;
    private Transform tf;

    private Text velocityText;
    private Text accelerationText;

    private ParticleSystem[] Laser;
    private Light LaserLight;

    private bool Firing;

    private Camera cam;
    
    void Start () {
        rb = GetComponent<Rigidbody>();
        tf = transform;

        velocityText = GameObject.Find("VelocityText").GetComponent<Text>();
        accelerationText = GameObject.Find("AccelerationText").GetComponent<Text>();

        Laser = tf.Find("Laser").GetComponentsInChildren<ParticleSystem>();
        LaserLight = tf.Find("Laser").GetComponentInChildren<Light>();

        cam = Camera.main;
    }
	
	void Update ()
    {
        tf.Rotate(new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Yaw")/10f, -Input.GetAxis("Horizontal")) * RotationSpeed * (1.5f - rb.velocity.magnitude / MaxVelocity));

        if (Input.GetButton("ThrottleUp"))
        {
            acceleration += MaxAcceleration / 50f;
        } else if (Input.GetButton("ThrottleDown"))
        {
            acceleration -= MaxAcceleration / 50f;
        } else if(acceleration < 0)
        {
            acceleration += MaxAcceleration / 25f;
        } else if(acceleration > 0)
        {
            acceleration -= MaxAcceleration / 25f;
        }

        acceleration = Mathf.Clamp(acceleration, -MaxAcceleration, MaxAcceleration);

        Vector3 Force = tf.forward * acceleration;
        rb.AddForce(Force, ForceMode.Acceleration);

        if(rb.velocity.magnitude > MaxVelocity)
        {
            rb.velocity = rb.velocity.normalized * MaxVelocity;
        }

        velocity = rb.velocity.magnitude;

        if (velocity != 0)
        {
            Vector3 forward;

            if (Vector3.Angle(tf.forward, rb.velocity.normalized) < Vector3.Angle(-tf.forward, rb.velocity.normalized))
            {
                forward = tf.forward;
            } else
            {
                forward = -tf.forward;
            }

            Vector3 correction = (rb.velocity.normalized * (1f - VelocityCorrector) + forward * VelocityCorrector).normalized;

            Debug.DrawRay(tf.position, rb.velocity.normalized * 5, Color.red);
            Debug.DrawRay(tf.position, correction * 5, Color.yellow);
            Debug.DrawRay(tf.position, forward * 5, Color.green);

            rb.velocity = correction * velocity;
        }

        if(Input.GetButtonDown("Fire"))
        {
            foreach (var ps in Laser)
                ps.Play();
            LaserLight.enabled = true;
            Firing = true;
        }

        if(Input.GetButtonUp("Fire"))
        {
            foreach (var ps in Laser)
                ps.Stop();
            LaserLight.enabled = false;
            Firing = false;
        }

        if(Firing)
        {
            var ray = cam.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f));
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 1000);

            if(hitInfo.transform != null && hitInfo.transform.CompareTag("Destructible"))
            {
                var target = hitInfo.transform.GetComponent<TargetBehaviour>();
                target.Damage(1, hitInfo.point);
            }
        }

        SetHUD();
    }

    void SetHUD()
    {
        velocityText.text = "Velocity : " + ((int)velocity) + "/" + ((int)MaxVelocity);
        accelerationText.text = "Acceleration : " + ((int)acceleration) + "/" + ((int)MaxAcceleration);
    }
}