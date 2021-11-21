using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGrab : MonoBehaviour
{
    public GameObject mousePrefab;
    public static int moveStep = 0;
    private GameObject mousePoint, holdingObject;
    private float bound;
    private Vector3 originalPosition, lastPossiblePosition, originalRotation, lastMousePosition;
    public enum ObjectInteractState { PickUp, Rotate};
    private ObjectInteractState objectInteractState;
    private BuildObjectList buildObjectList;
    private LaserInteraction laserInteraction;
    private Laser laser;
    private bool hasMousePressed, detectMousePressLock, hasRB;
    private Rigidbody rb;

    private void Start()
    {
        mousePoint = Instantiate(mousePrefab);
        bound = -1;
        buildObjectList = GetComponent<BuildModeManagement>().buildObjectList;
        laserInteraction = GameObject.FindGameObjectWithTag("LaserInteractionManager").GetComponent<LaserInteraction>();
    }

    private void Update()
    {
        if (detectMousePressLock == false)
        {
            if (hasMousePressed == false && Input.GetMouseButton(0) == true)
            {
                detectMousePressLock = true;
                hasMousePressed = true;
            }
        }
        else if (detectMousePressLock == true)
        {
            if (Input.GetMouseButton(0) == false)
            {
                hasMousePressed = true;
            }
        }

        if (lastMousePosition != Input.mousePosition || hasMousePressed == true)
        {
            hasMousePressed = false;
            lastMousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 30))
            {
                mousePoint.transform.position = ray.GetPoint(hit.distance - 0.1f);
                if (holdingObject == null)
                {
                    if (Input.GetMouseButton(0))
                    {
                        originalPosition = hit.transform.position;
                        originalRotation = hit.transform.eulerAngles;
                        if (IsObjectPickable(hit.transform.name))
                        {
                            objectInteractState = ObjectInteractState.PickUp;
                            holdingObject = hit.transform.gameObject;

                            holdingObject.transform.position = hit.point + Vector3.up;
                            if (holdingObject.GetComponent<Rigidbody>() == true)
                            {
                                hasRB = true;
                                rb = holdingObject.GetComponent<Rigidbody>();
                                rb.isKinematic = true;
                            }
                        }
                        else if (IsObjectRotatable(hit.transform.name))
                        {
                            objectInteractState = ObjectInteractState.Rotate;
                            holdingObject = hit.transform.gameObject;

                            Vector3 direction = (hit.point - holdingObject.transform.position).normalized;
                            direction = new Vector3(direction.x, 0, direction.z);
                            laser = Instantiate(laserInteraction.laserPrefab).GetComponent<Laser>();
                            laser.SetLaser(LaserColorList.LaserType.NA, hit.transform.gameObject, direction, 0.5f, 5, in laserInteraction);
                        }
                    }
                }
                else
                {
                    if (objectInteractState == ObjectInteractState.PickUp)
                    {
                        PickUp(ref ray, ref hit);
                    }
                    else if (objectInteractState == ObjectInteractState.Rotate)
                    {
                        Rotate(ref hit);
                    }
                }
            }
            else if (holdingObject != null)
            {
                holdingObject.transform.position = originalPosition;
                holdingObject.transform.rotation = Quaternion.Euler(originalRotation);
                bound = -1;
                holdingObject = null;
                laser = null;
                detectMousePressLock = false;
                LaserInteraction.hasSceneUpdate = true;
            }
        }
    }
    private void PickUp(ref Ray ray, ref RaycastHit hit)
    {
        if (bound == -1)
        {
            bound = Laser.FindBestBound(holdingObject.transform, ray.direction, holdingObject.GetComponent<BoxCollider>().bounds.extents);
        }

        if (hit.transform.gameObject != null)
        {
            lastPossiblePosition = hit.transform.position;
        }

        if (hit.transform.gameObject == holdingObject)
        {
            Ray newRay = new Ray(hit.point + Vector3.down * bound * 2, Vector3.down);
            if (Physics.Raycast(newRay, out RaycastHit newHit, 30))
            {
                ray = newRay;
                hit = newHit;
                mousePoint.transform.position = ray.GetPoint(hit.distance - 0.1f);
            }
            else
            {
                holdingObject.transform.position = lastPossiblePosition;
            }
        }
        holdingObject.transform.position = ray.GetPoint(hit.distance - bound - 0.05f) + Vector3.up;

        if (Input.GetMouseButton(0) == false)
        {
            float yBound = Laser.FindBestBound(holdingObject.transform, Vector3.down, holdingObject.GetComponent<BoxCollider>().bounds.extents);
            Ray groundRay = new Ray(holdingObject.transform.position + Vector3.down * yBound, Vector3.down);
            if (Physics.Raycast(groundRay, out hit, 30))
            {
                if (hit.transform.tag == "Ground")
                {
                    holdingObject.transform.position = groundRay.GetPoint(hit.distance - yBound);
                }
                else
                {
                    holdingObject.transform.position = originalPosition;
                }
            }
            else
            {
                holdingObject.transform.position = originalPosition;
            }
            detectMousePressLock = false;
            LaserInteraction.hasSceneUpdate = true;
            moveStep++;
            bound = -1;
            holdingObject = null;
            if (hasRB)
            {
                hasRB = false;
                rb.isKinematic = false;
                rb = null;
            }
        }
    }

    private void Rotate(ref RaycastHit hit)
    {
        Vector3 direction = (hit.point - holdingObject.transform.position).normalized;
        direction = new Vector3(direction.x, 0, direction.z);
        holdingObject.transform.rotation = Quaternion.LookRotation(direction);
        if (lastPossiblePosition != Input.mousePosition)
        {
            lastPossiblePosition = Input.mousePosition;
            laser.UpdateLaserDirection(holdingObject.transform.forward);
        }
        if (Input.GetMouseButton(0) == false)
        {
            detectMousePressLock = false;
            LaserInteraction.hasSceneUpdate = true;
            moveStep++;
            holdingObject = null;
            laser = null;
        }
    }

    private bool IsObjectPickable(string currentObjectName)
    {
        for (int i = 0; i < buildObjectList.pickableObjectList.Count; i++)
        {
            if (currentObjectName == buildObjectList.pickableObjectList[i])
            {
                return true;
            }
        }
        return false;
    }

    private bool IsObjectRotatable(string currentObjectName)
    {
        for (int i = 0; i < buildObjectList.rotatableObjectList.Count; i++)
        {
            if (currentObjectName == buildObjectList.rotatableObjectList[i])
            {
                return true;
            }
        }
        return false;
    }
}
