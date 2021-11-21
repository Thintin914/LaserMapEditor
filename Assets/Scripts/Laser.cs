using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public BoxCollider boxCollider;
    [HideInInspector]public Ray ray;
    [HideInInspector]public RaycastHit hit;
    [HideInInspector] public GameObject sender;
    private Vector3 position, direction;
    private float preferedBound;
    [HideInInspector]public GameObject hitObject;
    private LaserInteraction laserInteraction;
    public int power;
    private float laserWidth;
    public LaserColorList.LaserType laserType;

    [System.Serializable]
    public class MainLaserData
    {
        public Transform position;
        public Vector3 direction;
        public LaserColorList.LaserType laserType;
        public int power;

        public MainLaserData(LaserColorList.LaserType laserType , int power, Transform position, Vector3 direction)
        {
            this.laserType = laserType;
            this.power = power;
            this.position = position;
            this.direction = direction;
        }
    }

    public void SetLaser(LaserColorList.LaserType laserType , GameObject sender, Vector3 direction, float offset, int power, in LaserInteraction laserInteraction)
    {
        this.laserInteraction = laserInteraction;
        this.laserInteraction.lasers.Add(this);

        this.laserType = laserType;
        SetLaserColor(laserType, power);
        this.sender = sender;
        position = sender.transform.position;
        this.direction = direction;
        lineRenderer.positionCount = 2;
        preferedBound = offset;
        this.power = power;
        laserWidth = 0.02f + (power - 1) * 0.025f;
        if (laserWidth < 0.02f)
        {
            laserWidth = 0.02f;
        }
        if (laserWidth > 0.5f)
        {
            laserWidth = 0.5f;
        }
        lineRenderer.SetPosition(0, sender.transform.position);
        ray = new Ray(sender.transform.position + direction * preferedBound, direction);
        if (Physics.Raycast(ray, out hit))
        {
            lineRenderer.SetPosition(1, hit.point);
            hitObject = hit.transform.gameObject;
        }
        else
        {
            lineRenderer.SetPosition(1, ray.GetPoint(12));
            hitObject = null;
        }

        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        if (laserType == LaserColorList.LaserType.NA)
        {
            lineRenderer.endWidth = 0.02f;
        }
        name = laserType + " Power " + power;
        SetColliderBox();
    }

    private void SetColliderBox()
    {
        Vector3 center = (ray.origin + hit.point) * 0.5f;
        boxCollider.center = center + Vector3.down * 0.05f;
        boxCollider.size = new Vector3(hit.distance, 0.3f, 0.2f);
    }

    private void SetLaserColor(LaserColorList.LaserType laserType, int power)
    {
        float preA = 100 + (power - 1) * 25;
        if (preA < 100)
        {
            preA = 100;
        }
        if (preA > 255)
        {
            preA = 255;
        }
        byte a = (byte)preA;

        int index = LaserColorList.FindLaserIndex(laserType, in laserInteraction.laserColorList.laserColorList);
        lineRenderer.startColor = laserInteraction.laserColorList.laserColorList[index].startColor;
        lineRenderer.endColor = laserInteraction.laserColorList.laserColorList[index].endColor;

        lineRenderer.startColor = new Color32((byte)(lineRenderer.startColor.r * 255), (byte)(lineRenderer.startColor.g * 255), (byte)(lineRenderer.startColor.b * 255), a);
        lineRenderer.endColor = new Color32((byte)(lineRenderer.endColor.r * 255), (byte)(lineRenderer.endColor.g * 255), (byte)(lineRenderer.endColor.b * 255), a);
    }

    public void UpdateLaserDirection(Vector3 direction)
    {
        if (lineRenderer == null)
            return;
        lineRenderer.SetPosition(0, position);
        ray = new Ray(position + direction * preferedBound, direction);
        if (Physics.Raycast(ray, out hit, 2))
        {
            lineRenderer.SetPosition(1, hit.point);
            hitObject = hit.transform.gameObject;
        }
        else
        {
            lineRenderer.SetPosition(1, ray.GetPoint(2));
            hitObject = null;
        }
    }

    public static float FindBestBound(Transform position, Vector3 direction, Vector3 bounds)
    {
        List<float> boundValue = new List<float>();
        boundValue.Add(bounds.x);
        boundValue.Add(bounds.y);
        boundValue.Add(bounds.z);

        float[] sortedBoundValue = new float[3];
        int currentSlot = 2;

        for (int j = 0; j < 2; j++)
        {
            int index = 0;
            float holdingValue = boundValue[0];
            for (int i = 0; i < boundValue.Count; i++)
            {
                if (boundValue[i] > holdingValue)
                {
                    holdingValue = boundValue[i];
                    index = i;
                }
            }
            sortedBoundValue[currentSlot] = holdingValue;
            currentSlot--;
            boundValue.RemoveAt(index);
        }
        sortedBoundValue[currentSlot] = boundValue[0];

        bool[] isBoundPossible = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            Ray tempRay = new Ray(position.position + direction * sortedBoundValue[i], direction);
            RaycastHit tempHit;
            if (Physics.Raycast(tempRay, out tempHit, 1))
            {
                if (tempHit.transform.gameObject == position.gameObject)
                {
                    isBoundPossible[i] = false;
                }
                else
                {
                    isBoundPossible[i] = true;
                }
            }
            else
            {
                isBoundPossible[i] = true;
            }
        }

        float minBound = 100;
        for (int i = 0; i < 3; i++)
        {
            if (isBoundPossible[i] == true && sortedBoundValue[i] < minBound)
            {
                minBound = sortedBoundValue[i];
            }
        }
        return minBound;
    }

    private void FixedUpdate()
    {
        lineRenderer.startWidth = Mathf.Sin(Time.timeSinceLevelLoad * 0.5f) * laserWidth;
        lineRenderer.endWidth = Mathf.Sin(Time.timeSinceLevelLoad) * laserWidth;
    }
}
