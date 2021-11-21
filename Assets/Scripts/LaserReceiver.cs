using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public LaserColorList laserColorList;
    public MeshRenderer meshRenderer1, meshRenderer2;

    public LaserColorList.LaserType laserType;
    public int power;
    public string sceneTag, levelName;
    public bool isGoal = false;
    public GameObject linkedSceneObject;

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
        laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), extraSettings[0]);
        power = int.Parse(extraSettings[1]);
        sceneTag = extraSettings[2];
        isGoal = bool.Parse(extraSettings[3]);
        levelName = extraSettings[4];
        ChangeMaterial();
        yield return new WaitForEndOfFrame();
        if (sceneTag != "")
        {
            linkedSceneObject = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>().FindGameObjectWithSceneTag(sceneTag);
        }
        LaserInteraction.hasSceneUpdate = true;
    }

    public void ChangeMaterial()
    {
        Material material = LaserColorList.FindLaserMaterial(laserType, in laserColorList.laserColorList);
        Material[] originalMaterials = meshRenderer1.materials;
        originalMaterials[1] = material;
        meshRenderer1.materials = originalMaterials;

        originalMaterials = meshRenderer2.materials;
        originalMaterials[1] = material;
        meshRenderer2.materials = originalMaterials;
    }
}
