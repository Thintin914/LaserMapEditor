using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedClasses
{
    [System.Serializable]
    public class SerializedVector3
    {
        public float x, y, z;

        public SerializedVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public SerializedVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public bool Equals(SerializedVector3 value)
        {
            if (x == value.x && y == value.y && z == value.z)
            {
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class SerializedBuildObject
    {
        public SerializedVector3 position;
        public SerializedVector3 rotation;
        public int ID;
        public List<string> stringList = new List<string>();
        public string tag;

        public SerializedBuildObject(SerializedVector3 position, SerializedVector3 rotation, int ID, List<BuildObjectList.BuildObject.DefaultInformation> stringList, string tag = null)
        {
            this.position = position;
            this.rotation = rotation;
            this.ID = ID;
            this.stringList = ToSerializedStringList(stringList);
            this.tag = tag;
        }

        public SerializedBuildObject(SerializedVector3 position, SerializedVector3 rotation, int ID, List<string> stringList, string tag = null)
        {
            this.position = position;
            this.rotation = rotation;
            this.ID = ID;
            this.stringList = stringList;
            this.tag = tag;
        }

        public static int FindIndexByInstanceID(GameObject gameObject, ref List<GameObject> list)
        {
            int instanceID = gameObject.GetInstanceID();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetInstanceID() == instanceID)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetBuildObjectID(int index, ref List<SerializedBuildObject> list)
        {
            return list[index].ID;
        }

        public List<string> ToSerializedStringList (List<BuildObjectList.BuildObject.DefaultInformation> list)
        {
            List<string> stringList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                stringList.Add(list[i].value);
            }
            return stringList;
        }
    }

    public static float GetDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2) + Mathf.Pow(b.z - a.z, 2));
    }
}
