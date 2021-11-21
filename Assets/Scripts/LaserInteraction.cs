using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInteraction : MonoBehaviour
{
    public GameObject laserPrefab;
    [HideInInspector]public List<Laser> lasers;
    public static bool hasSceneUpdate;
    private string loadLevelName;
    [HideInInspector]public List<Laser.MainLaserData> mainLaserData;
    public List<GameObject> triggeredObjects, possibleTriggerObjects;
    public LaserColorList laserColorList;

    private void LateUpdate()
    {
        if (hasSceneUpdate == true)
        {
            StartCoroutine(UpdateLaser());
            hasSceneUpdate = false;
        }
    }

    IEnumerator UpdateLaser()
    {
        yield return new WaitForFixedUpdate();
        CheckLaserInteractions();
    }

    private void DestroyLasers()
    {
        for (int i = 0; i < lasers.Count; i++)
        {
            if (lasers[i].sender.name == "MovingPortal")
                lasers[i].sender.GetComponent<MovingPortal>().passedLaserNum--;

            Destroy(lasers[i].gameObject);
        }
        lasers.Clear();
    }

    private Laser CreateLaser(LaserColorList.LaserType laserType , Transform startPosition, Vector3 direction, BoxCollider bounds, int power)
    {
        GameObject currentLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        LaserInteraction laserInteraction = this;
        float preferedBound = Laser.FindBestBound(startPosition, direction, bounds.GetComponent<BoxCollider>().bounds.extents);
        Laser laser = currentLaser.GetComponent<Laser>();
        laser.SetLaser(laserType , startPosition.gameObject, direction, preferedBound, power, in laserInteraction);
        return laser;
    }

    private void CheckLaserInteractions()
    {
        possibleTriggerObjects.Clear();
        DestroyLasers();
        loadLevelName = null;
        foreach (Laser.MainLaserData laserData in mainLaserData)
        {
            FireLaser(laserData.laserType , laserData.position.gameObject, laserData.direction, laserData.power);
        }
        if (loadLevelName == null)
        {
            foreach (GameObject g in possibleTriggerObjects)
            {
                if (!triggeredObjects.Contains(g))
                {                                               // ----Why I Have Exception?----
                    g.GetComponent<SceneObjectTag>().Trigger(); // Remember To Match The Names Of Both Caller And Receiver.
                    triggeredObjects.Add(g);                    // And Reclick The Caller's Button After Changing The Receiver Name In Order To Re-register.
                }                                               // Else You Would Encounter Exception.
            }
            for (int i = 0; i < triggeredObjects.Count; i++)
            {
                if (!possibleTriggerObjects.Contains(triggeredObjects[i]))
                {
                    triggeredObjects[i].GetComponent<SceneObjectTag>().Detrigger();
                    triggeredObjects.Remove(triggeredObjects[i]);
                    i--;
                }
            }
            possibleTriggerObjects.Clear();
        }
        else
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>().ToNextLevel(loadLevelName);
            loadLevelName = null;

        }
    }

    public void DetriggerAllLasers()
    {
        for (int i = 0; i < triggeredObjects.Count; i++)
        {
            triggeredObjects[i].GetComponent<SceneObjectTag>().Detrigger();
        }
    }

    private void FireLaser(LaserColorList.LaserType laserType , GameObject fromObject, Vector3 direction, int power)
    {
        if (lasers.Count < 20 && power > 0)
        {
            Laser laser = CreateLaser(laserType , fromObject.transform, direction, fromObject.GetComponent<BoxCollider>(), power);
            if (laser.hitObject != null)
            {
                BoxCollider hitbox = laser.hitObject.GetComponent<BoxCollider>();
                switch (laser.hitObject.name)
                {
                    case "2WayCube":
                        FireLaser(laserType, laser.hitObject, laser.hitObject.transform.forward, power - 1);
                        FireLaser(laserType, laser.hitObject, -laser.hitObject.transform.forward, power - 1);
                        break;
                    case "Mirror":
                        FireLaser(laserType, laser.hitObject, laser.hitObject.transform.forward, power - 1);
                        break;
                    case "LaserReceiver":
                        LaserReceiver laserReceiver = laser.hitObject.GetComponent<LaserReceiver>();
                        if (laserReceiver.laserType == LaserColorList.LaserType.NA || laserReceiver.laserType == laserType)
                        {
                            if (laserReceiver.power == -1 || laserReceiver.power == power)
                            {
                                if (laserReceiver.linkedSceneObject != null)
                                {
                                    if (!possibleTriggerObjects.Contains(laserReceiver.linkedSceneObject)) {
                                        possibleTriggerObjects.Add(laserReceiver.linkedSceneObject);
                                    }
                                }
                                else if (laserReceiver.isGoal == true)
                                {
                                    loadLevelName = laserReceiver.levelName;
                                    return;
                                }
                            }
                        }
                        break;
                    case "ColorChanger":
                        LaserColorList.LaserType colorChangerLaserType = laser.hitObject.GetComponent<ColorChanger>().laserType;
                        FireLaser(colorChangerLaserType, laser.hitObject, laser.ray.direction, power - 1);
                        break;
                    case "Portal":
                        if (laser.hitObject.GetComponent<Portal>().linkedPortal.name == "MovingPortal")
                            laser.hitObject.GetComponent<Portal>().linkedPortal.GetComponent<MovingPortal>().passedLaserNum++;

                        FireLaser(laserType, laser.hitObject.GetComponent<Portal>().linkedPortal, laser.hitObject.GetComponent<Portal>().linkedPortal.transform.forward, power);
                        break;
                    case "MovingPortal":
                        FireLaser(laserType, laser.hitObject.GetComponent<MovingPortal>().linkedPortal, laser.hitObject.GetComponent<MovingPortal>().linkedPortal.transform.forward, power);
                        break;
                }
            }
        }
    }
}
