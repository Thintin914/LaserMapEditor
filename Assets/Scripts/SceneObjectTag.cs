using UnityEngine;

public class SceneObjectTag : MonoBehaviour
{
    public string sceneTag;

    public void Trigger()
    {
        switch (name)
        {
            case "Door1":
                GetComponent<Door1>().OpenDoor();
                break;
            case "Elevator":
                GetComponent<Elevator>().ToOppositePos(true);
                break;
            case "MovingPortal":
                GetComponent<MovingPortal>().ToOppositePos(true);
                break;
        }
    }

    public void Detrigger()
    {
        switch (name)
        {
            case "Door1":
                GetComponent<Door1>().CloseDoor();
                break;
            case "Elevator":
                GetComponent<Elevator>().ToOppositePos(false);
                break;
            case "MovingPortal":
                GetComponent<MovingPortal>().ToOppositePos(false);
                break;
        }
    }
}
