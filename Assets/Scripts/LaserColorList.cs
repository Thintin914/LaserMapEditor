using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Laser Color List")]
public class LaserColorList : ScriptableObject
{
    public List<LaserColor> laserColorList;
    public enum LaserType { NA, WhiteLaser, RedLaser, OrangeLaser, YellowLaser, GreenLaser, TintLaser, BlueLaser, PurpleLaser, BlackLaser};

    [System.Serializable]
    public class LaserColor
    {
        public LaserType laserType;
        public Color startColor, endColor;
        public Material material;
    }

    public static Material FindLaserMaterial(LaserType laserType, in List<LaserColor> laserColorList)
    {
        for (int i = 0; i < laserColorList.Count; i++)
        {
            if (laserColorList[i].laserType == laserType)
            {
                return laserColorList[i].material;
            }
        }
        return laserColorList[0].material;
    }

    public static int FindLaserIndex(LaserType laserType, in List<LaserColor> laserColorList)
    {
        for (int i = 0; i < laserColorList.Count; i++)
        {
            if (laserColorList[i].laserType == laserType)
            {
                return i;
            }
        }
        return 0;
    }
}
