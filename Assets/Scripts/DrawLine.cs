using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public delegate void NewBodyCreated(DrawnBody NewDrawnBody);
    public static event NewBodyCreated newBodyCreated;

    [SerializeField] Material lineMaterial;
    [SerializeField] Camera cam;
    [SerializeField] Transform meshPartPrefab;

    GameObject line;
    GameObject car;
    [SerializeField] Transform lastInstantiatedCarPart;
    GameObject oldCar;
    SlowMotion slowMotion;

    LineRenderer lineRenderer;
    [SerializeField] DrawnBody drawnCar;

    bool startDrawing;
    int currentIndex;

    Vector3 lastCarPosition;

    [SerializeField] float distanceBetweenMeshParts;

    Vector3 mousePos;

    private void Awake()
    {
        slowMotion = new SlowMotion();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        slowMotion.StartSlowMotionEffect();

        line = new GameObject();
        line.name = "Line";

        if (car)
        {
            lastCarPosition = car.transform.position;
            oldCar = car;
            car = new GameObject();
            car.name = "Body";
            drawnCar = car.AddComponent<DrawnBody>();
            car.transform.position = new Vector3(lastCarPosition.x, lastCarPosition.y, lastCarPosition.z);
        }
        else
        {
            car = new GameObject();
            car.name = "Body";
            drawnCar = car.AddComponent<DrawnBody>();
        }

        startDrawing = true;
        mousePos = Input.mousePosition;

        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = .1f;
        lineRenderer.material = lineMaterial;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        slowMotion.StopSlowMotionEffect();
        startDrawing = false;

        // check, if car has sufficient amount body part
        if (drawnCar != null && car.transform.childCount > 2)
        {
            Rigidbody meshRB = car.AddComponent<Rigidbody>();
            meshRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
            meshRB.mass = 400f;

            drawnCar.ActivateBodyParts();
            car.transform.position = new Vector3(car.transform.position.x, car.transform.position.y + 5, 0);

            Destroy(lastInstantiatedCarPart.gameObject);
        }

        lineRenderer.useWorldSpace = false;

        if (line)
        {
            Destroy(line);
        }
        if (oldCar)
        {
            Destroy(oldCar);
        }

        currentIndex = 0;

        if (newBodyCreated != null)
        {
            newBodyCreated(drawnCar);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        startDrawing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // startDrawing = true;
    }

    void Update()
    {
        StartDrawing();
    }

    private void StartDrawing()
    {
        if (startDrawing)
        {
            Vector3 distance = mousePos - Input.mousePosition;
            float distanceSqrMagnitude = distance.sqrMagnitude;

            if (distanceSqrMagnitude > distanceBetweenMeshParts)
            {
                lineRenderer.SetPosition(currentIndex, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 8f)));

                if (lastInstantiatedCarPart != null)
                {
                    drawnCar.CarParts.Add(lastInstantiatedCarPart);
                }

                lastInstantiatedCarPart = Instantiate(meshPartPrefab, lineRenderer.GetPosition(currentIndex), Quaternion.identity, car.transform);

                lastInstantiatedCarPart.gameObject.SetActive(false);

                mousePos = Input.mousePosition;

                currentIndex++;

                lineRenderer.positionCount = currentIndex + 1;

                lineRenderer.SetPosition(currentIndex, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 8f)));
            }
        }
    }
}
