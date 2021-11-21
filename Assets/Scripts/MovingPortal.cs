using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPortal : MonoBehaviour
{
    public string linkedSceneTag;
    public GameObject linkedPortal;

    public Vector3 startPos, endPos;
    public int passedLaserNum;
    private bool toEndPos, canUpdate;
    private float elapsedTime, duration, updateWaitTime;

    private void Start()
    {
        if (GetComponent<SubscribeExtraInformation>().extraSettings.Count > 0)
        {           
            StartCoroutine(FillData());
        }
    }

    IEnumerator FillData()
    {
        List<string> extraSettings = GetComponent<SubscribeExtraInformation>().extraSettings;
        GetComponent<SceneObjectTag>().sceneTag = extraSettings[0];
        linkedSceneTag = extraSettings[1];
        startPos = new Vector3(float.Parse(extraSettings[2]), float.Parse(extraSettings[3]), float.Parse(extraSettings[4]));
        endPos = new Vector3(float.Parse(extraSettings[5]), float.Parse(extraSettings[6]), float.Parse(extraSettings[7]));
        duration = SerializedClasses.GetDistance(startPos, endPos);
        yield return new WaitForEndOfFrame();
        if (linkedSceneTag != "")
        {
            linkedPortal = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>().FindGameObjectWithSceneTag(linkedSceneTag);
        }
        LaserInteraction.hasSceneUpdate = true;
        canUpdate = true;
        enabled = false;
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
            if (other.GetComponent<Laser>().sender != gameObject)
                LaserInteraction.hasSceneUpdate = true;
        }
    }

    private void FixedUpdate()
    {
        if (canUpdate)
        {
            float percentageComplete = elapsedTime / duration;
            if (passedLaserNum > 0)
            {
                if (Time.timeSinceLevelLoad - updateWaitTime > 0.1f)
                {
                    updateWaitTime = Time.timeSinceLevelLoad;
                    LaserInteraction.hasSceneUpdate = true;
                }
            }
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
}
