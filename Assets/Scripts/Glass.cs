using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public LaserColorList.LaserType laserType;
    public LaserColorList laserColorList;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        if (GetComponent<SubscribeExtraInformation>().extraSettings.Count > 0)
        {
            List<string> extraSettings = GetComponent<SubscribeExtraInformation>().extraSettings;
            laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), extraSettings[0]);
            meshRenderer.material = LaserColorList.FindLaserMaterial(laserType, in laserColorList.laserColorList);
        }
    }
}
