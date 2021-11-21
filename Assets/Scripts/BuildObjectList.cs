using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Build Object List")]
public class BuildObjectList : ScriptableObject
{
    public List<BuildObject> buildObjectList;
    public List<string> pickableObjectList;
    public List<string> rotatableObjectList;

    [System.Serializable]
    public class BuildObject
    {
        public GameObject model;
        public int ID;
        public string name;
        public List<DefaultInformation> defaultInformation;

        [System.Serializable]
        public class DefaultInformation
        {
            public string name;
            public string value;
        }
    }

    public static BuildObject FindBuildObject(string name, in List<BuildObject> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].name == name)
            {
                return list[i];
            }
        }
        return list[0];
    }
}