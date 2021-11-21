using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door1 : MonoBehaviour
{
    public Transform door1Model, door2Model;
    public int openRate;

    private void Start()
    {
        if (GetComponent<SubscribeExtraInformation>().extraSettings.Count > 0)
        {
            List<string> extraSettings = GetComponent<SubscribeExtraInformation>().extraSettings;
            GetComponent<SceneObjectTag>().sceneTag = extraSettings[0];
            openRate = int.Parse(extraSettings[1]);
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(MoveDoor(true));
    }
    public void CloseDoor()
    {
        StartCoroutine(MoveDoor(false));
    }

    IEnumerator MoveDoor(bool isOpen)
    {
        if (isOpen)
        {
            for (int i = 0; i < openRate; i++)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                door1Model.position += transform.right * 0.05f;
                door2Model.position += -transform.right * 0.05f;
            }
        }
        else
        {
            for (int i = 0; i < openRate; i++)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                door1Model.position += -transform.right * 0.05f;
                door2Model.position += transform.right * 0.05f;
            }
        }
        LaserInteraction.hasSceneUpdate = true;
    }
}
