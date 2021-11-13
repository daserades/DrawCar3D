using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    // Components
    [SerializeField] Material lineMaterial;
    [SerializeField] Camera cam;
    [SerializeField] Transform meshPartPrefab;

    GameObject line;
    GameObject body;
    Transform lastInstantiatedCollider;

    LineRenderer lineRenderer;
    [SerializeField] DrawnBody drawnBody;

    bool startDrawing;
    int currentIndex;

    Vector3 temp;

    [SerializeField] float distanceBetweenMeshParts;

    Vector3 mousePos;

    public void OnPointerDown(PointerEventData eventData)
    {
        line = new GameObject();
        line.name = "Line";

        if (body)
        {
            temp = body.transform.position;
            Destroy(body);
            body = new GameObject();
            body.name = "Body";
            drawnBody = body.AddComponent<DrawnBody>();

        }
        else
        {
            body = new GameObject();
            body.name = "Body";
            drawnBody = body.AddComponent<DrawnBody>();

        }

        startDrawing = true;
        mousePos = Input.mousePosition;

        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = .2f;
        lineRenderer.material = lineMaterial;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        startDrawing = false;

        if (drawnBody != null)
        {
            if (drawnBody.transform.GetChildCount() > 3)
            {

                Rigidbody meshRB = body.AddComponent<Rigidbody>();
                meshRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
                meshRB.mass = 400f;

                drawnBody.ActivateBodyParts();
                drawnBody.transform.position = new Vector3(temp.x, temp.y + 5f, drawnBody.transform.position.z);

                Destroy(lastInstantiatedCollider.gameObject);
            }
        }

        lineRenderer.useWorldSpace = false;


        if (line)
        {
            Destroy(line);
        }

        currentIndex = 0;
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

            Time.timeScale = 0.4f;
            Vector3 distance = mousePos - Input.mousePosition;
            float distanceSqrMagnitude = distance.sqrMagnitude;

            if (distanceSqrMagnitude > distanceBetweenMeshParts)
            {
                lineRenderer.SetPosition(currentIndex, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 10f)));

                if (lastInstantiatedCollider != null)
                {
                    Vector3 currentLinePos = lineRenderer.GetPosition(currentIndex);
                    lastInstantiatedCollider.gameObject.SetActive(false);
                    drawnBody.BodyParts.Add(lastInstantiatedCollider);

                    lastInstantiatedCollider.LookAt(currentLinePos);

                    if (lastInstantiatedCollider.rotation.y == 0)
                    {
                        //  lastInstantiatedCollider.eulerAngles = new Vector3(lastInstantiatedCollider.rotation.eulerAngles.x, 90, lastInstantiatedCollider.rotation.eulerAngles.z);
                    }

                    // lastInstantiatedCollider.localScale = new Vector3(lastInstantiatedCollider.localScale.x, lastInstantiatedCollider.localScale.y, lastInstantiatedCollider.localScale.z);

                }

                lastInstantiatedCollider = Instantiate(meshPartPrefab, lineRenderer.GetPosition(currentIndex), Quaternion.identity, body.transform);

                lastInstantiatedCollider.gameObject.SetActive(false);

                mousePos = Input.mousePosition;

                currentIndex++;

                lineRenderer.positionCount = currentIndex + 1;

                lineRenderer.SetPosition(currentIndex, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z + 10f)));

            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


}
