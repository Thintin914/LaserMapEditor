using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string linkedSceneTag;
    public GameObject linkedPortal;

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
        yield return new WaitForEndOfFrame();
        if (linkedSceneTag != "")
        {
            linkedPortal = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>().FindGameObjectWithSceneTag(linkedSceneTag);
        }
        LaserInteraction.hasSceneUpdate = true;
    }
}
