using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawnBody : MonoBehaviour
{
    public delegate void NewBodyCreated(DrawnBody NewDrawnBody);
    public static event NewBodyCreated newBodyCreated;

    [SerializeField] private List<Transform> bodyParts;
    public List<Transform> BodyParts { get { return bodyParts; } }

    Transform firstPart, lastPart;

    [SerializeField] GameObject wheelPrefab;
    [SerializeField] float motorPower = 30000f;

    GameObject backRightWheel, backLeftWheel, frontRightWheel, frontLeftWheel;
    WheelCollider backRightWC, backLeftWC, frontRightWC, frontLeftWC;

    [SerializeField] Rigidbody rigidBody;

    public bool isDead = false;

    void Awake()
    {
        bodyParts = new List<Transform>();
        wheelPrefab = Resources.Load<GameObject>("Wheel");

        if (newBodyCreated != null)
        {
            newBodyCreated(this);
        }
    }

    public void ActivateBodyParts()
    {
        rigidBody = GetComponent<Rigidbody>();

        foreach (Transform part in bodyParts)
        {
            part.gameObject.SetActive(true);
            part.position = new Vector3(part.transform.position.x, part.transform.position.y + 5, 0);
        }
        if (bodyParts.Last() && bodyParts.First())
        {
            lastPart = bodyParts.Last();
            firstPart = bodyParts.First();

            // TODO: edit magic numbers
            Vector3 frontLeftWheelPosition = new Vector3(lastPart.position.x, lastPart.position.y - lastPart.localScale.y, lastPart.position.z - wheelPrefab.transform.localScale.z * .5f);
            Vector3 frontRightWheelPosition = new Vector3(lastPart.position.x, lastPart.position.y - lastPart.localScale.y, lastPart.position.z + wheelPrefab.transform.localScale.z * .5f);
            Vector3 backLeftWheelPosition = new Vector3(firstPart.position.x, firstPart.position.y - firstPart.localScale.y, lastPart.position.z - wheelPrefab.transform.localScale.z * .5f);
            Vector3 backRightWheelPosition = new Vector3(firstPart.position.x, firstPart.position.y - firstPart.localScale.y, lastPart.position.z + wheelPrefab.transform.localScale.z * .5f);

            frontLeftWheel = Instantiate(wheelPrefab, frontLeftWheelPosition, Quaternion.identity, transform);
            frontRightWheel = Instantiate(wheelPrefab, frontRightWheelPosition, Quaternion.identity, transform);
            backRightWheel = Instantiate(wheelPrefab, backRightWheelPosition, Quaternion.identity, transform);
            backLeftWheel = Instantiate(wheelPrefab, backLeftWheelPosition, Quaternion.identity, transform);

            frontLeftWC = frontLeftWheel.GetComponent<WheelCollider>();
            frontRightWC = frontRightWheel.GetComponent<WheelCollider>();
            backRightWC = backRightWheel.GetComponent<WheelCollider>();
            backLeftWC = backLeftWheel.GetComponent<WheelCollider>();

            backLeftWC.steerAngle = 90f;
            backRightWC.steerAngle = 90f;
            frontLeftWC.steerAngle = 90f;
            frontRightWC.steerAngle = 90f;
        }

    }

    private void FixedUpdate()
    {
        if (backRightWC)
        {
            MoveForward();
        }
    }

    private void MoveForward()
    {
        backRightWC.motorTorque = motorPower * Time.fixedDeltaTime;
        backLeftWC.motorTorque = motorPower * Time.fixedDeltaTime;
        frontRightWC.motorTorque = motorPower * Time.fixedDeltaTime;
        frontLeftWC.motorTorque = motorPower * Time.fixedDeltaTime;
    }

    public Vector3 GetDrawnBodyVelocity()
    {
        if (rigidBody != null)
        {
            return new Vector3(rigidBody.velocity.x, 0, 0);
        }
        return Vector3.zero;
    }

    public void ReturnCheckPoint(Transform checkPointPosition)
    {
        float offsetY = 2.5f;
        transform.position = new Vector3(checkPointPosition.position.x, checkPointPosition.position.y + offsetY, transform.position.z);
        rigidBody.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
