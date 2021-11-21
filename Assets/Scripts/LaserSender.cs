using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSender : MonoBehaviour
{
    public Laser.MainLaserData laserData;
    public LaserColorList laserColorList;
    public MeshRenderer meshRenderer1, meshRenderer2;
    public LaserColorList.LaserType laserType;
    public int power;


    private void Start()
    {
        if (GetComponent<SubscribeExtraInformation>().extraSettings.Count > 0)
        {
            List<string> extraSettings = GetComponent<SubscribeExtraInformation>().extraSettings;
            laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), extraSettings[0]);
            power = int.Parse(extraSettings[1]);
            ChangeMaterial();
        }

        LaserInteraction laserInteraction = GameObject.FindGameObjectWithTag("LaserInteractionManager").GetComponent<LaserInteraction>();
        laserData = new Laser.MainLaserData(laserType, power, transform, transform.up);
        laserInteraction.mainLaserData.Add(laserData);
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
