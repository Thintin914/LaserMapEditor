using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Vector3 startPos, endPos;
    private bool toEndPos;
    private float elapsedTime, duration;

    private void Awake()
    {
        enabled = false;
    }

    private void Start()
    {
        if (GetComponent<SubscribeExtraInformation>().extraSettings.Count > 0)
        {
            List<string> extraSettings = GetComponent<SubscribeExtraInformation>().extraSettings;
            GetComponent<SceneObjectTag>().sceneTag = extraSettings[0];
            startPos = new Vector3(float.Parse(extraSettings[1]), float.Parse(extraSettings[2]), float.Parse(extraSettings[3]));
            endPos = new Vector3(float.Parse(extraSettings[4]), float.Parse(extraSettings[5]), float.Parse(extraSettings[6]));
            duration = SerializedClasses.GetDistance(startPos, endPos);
        }
    }

    public void ToOppositePos(bool toEndPos)
    {
        this.toEndPos = toEndPos;
        if (enabled == false)
        {
            enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled == true && other.tag == "Laser")
        {
            LaserInteraction.hasSceneUpdate = true;
        }
    }

    private void FixedUpdate()
    {
        float percentageComplete = elapsedTime / duration;
        if (toEndPos)
        {
            if (elapsedTime <= duration)
                elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, percentageComplete);
            if (percentageComplete >= 1)
            {
                elapsedTime = duration;
                LaserInteraction.hasSceneUpdate = true;
                enabled = false;
            }
        }
        else
        {
            if (elapsedTime >= 0)
                elapsedTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, percentageComplete);
            if (percentageComplete <= 0)
            {
                elapsedTime = 0;
                LaserInteraction.hasSceneUpdate = true;
                enabled = false;
            }
        }
    }
}
