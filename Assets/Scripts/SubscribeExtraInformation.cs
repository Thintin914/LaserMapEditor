using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubscribeExtraInformation : MonoBehaviour
{
    private BuildModeManagement buildModeManagement;
    private Button extraButton, closeUIButton, defaultButton;
    private GameObject extraInformationUI, testPositionUI;
    private TMPro.TextMeshProUGUI description, testPositionText;
    private Vector3 originalPosition;
    [HideInInspector]public List<GameObject> allInputFields;
    public List<string> extraSettings = new List<string>();

    public const float INPUTFIELD_WIDTH = 160;

    private void Awake()
    {
        enabled = false;
        buildModeManagement = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>();
        buildModeManagement.ShowInformationButtons += ShowExtraInformation;
        buildModeManagement.HideInformationButtons += HideExtraInformation;
    }

    private void OnDestroy()
    {
        buildModeManagement.ShowInformationButtons -= ShowExtraInformation;
        buildModeManagement.HideInformationButtons -= HideExtraInformation;
    }

    public void ShowExtraInformation()
    {
        extraButton = Instantiate(buildModeManagement.extraInformationButtonPrefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.Euler(0, 0, 0)).GetComponent<Button>();
        extraButton.transform.SetParent(GameObject.FindGameObjectWithTag("ExtraInformation").transform);
        extraButton.onClick.AddListener(ExtraButtonOnClick);

        extraInformationUI = null;
        enabled = true;
    }

    public void ToDefaultOnClick()
    {
        List<BuildObjectList.BuildObject.DefaultInformation> defaultList = BuildObjectList.FindBuildObject(name, in buildModeManagement.buildObjectList.buildObjectList).defaultInformation;
        for(int i = 0; i < allInputFields.Count; i++)
        {
            allInputFields[i].GetComponent<TMPro.TMP_InputField>().text = defaultList[i].value;
        }
    }

    private void FixedUpdate()
    {
        if (extraButton != null)
            extraButton.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void HideExtraInformation()
    {
        if (extraButton != null) {
            Destroy(extraButton.gameObject);
            extraButton = null;
        }
        enabled = false;
    }

    private void DestroyExtraButtons()
    {
        Transform extraButtonParent = GameObject.FindGameObjectWithTag("ExtraInformation").transform;
        for(int i = 0; i < extraButtonParent.childCount; i++)
        {
            Destroy(extraButtonParent.GetChild(i).gameObject);
        }
        extraButton = null;
    }

    private void CloseUI()
    {
        Destroy(extraInformationUI);
        extraInformationUI = null;
        BuildModeManagement.isExtraInformationUIShowing = false;
        buildModeManagement.enabled = true;
        description = null;
        buildModeManagement.buildModeUI.SetActive(true);
        closeUIButton = null;
        defaultButton = null;

        if (testPositionUI != null)
        {
            Destroy(testPositionUI);
            testPositionUI = null;
            transform.position = originalPosition;
            testPositionText = null;
        }

        buildModeManagement.CallEvent("ShowInformationButtons");

        SaveSpecialInformationData(name);
    }

    private void ExtraButtonOnClick()
    {
        DestroyExtraButtons();
        BuildModeManagement.isExtraInformationUIShowing = true;
        buildModeManagement.enabled = false;
        buildModeManagement.buildModeUI.SetActive(false);

        extraInformationUI = Instantiate(buildModeManagement.extraInformationUIPrefab, new Vector3(Screen.width * 0.3f, Screen.height * 0.5f), Quaternion.Euler(0,0,0));
        extraInformationUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);

        closeUIButton = extraInformationUI.transform.GetChild(0).GetComponent<Button>();
        closeUIButton.onClick.AddListener(CloseUI);

        defaultButton = extraInformationUI.transform.GetChild(2).GetComponent<Button>();
        defaultButton.onClick.AddListener(ToDefaultOnClick);

        description = extraInformationUI.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();

        originalPosition = transform.position;
        SetSpecialInformationText(name, ref description);
    }

    private Vector3 GetTextContentPosition(ref TMPro.TextMeshProUGUI text, float width, int line)
    {
        float heightEachLine = text.preferredHeight / (text.text.Split('\n').Length - 1);
        Vector3 position = new Vector3(text.transform.position.x + text.preferredWidth  * 0.7f + width * 0.5f + 10, text.transform.position.y - line * heightEachLine, 0);
        return position;
    }

    public void TestPositionLeftOnClick()
    {
        transform.position += Vector3.left * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionRightOnClick()
    {
        transform.position += Vector3.right * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionUpOnClick()
    {
        transform.position += Vector3.up * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionDownOnClick()
    {
        transform.position += Vector3.down * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionFrontOnClick()
    {
        transform.position += Vector3.forward * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionBackOnClick()
    {
        transform.position += Vector3.back * 0.1f;
        TestPositionTextReport();
    }
    public void TestPositionResetOnClick()
    {
        transform.position = originalPosition;
        TestPositionTextReport();
    }
    private void TestPositionTextReport()
    {
        testPositionText.text = "X: " + transform.position.x + "\nY: " + transform.position.y + "\nZ: " + transform.position.z;
    }

    private void CreateTestPositionUI()
    {
        testPositionUI = Instantiate(buildModeManagement.testPositionUIPrefab, new Vector3(510, -230, 0), Quaternion.Euler(0, 0, 0));
        testPositionUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);

        testPositionUI.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(TestPositionLeftOnClick);
        testPositionUI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(TestPositionRightOnClick);
        testPositionUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(TestPositionUpOnClick);
        testPositionUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(TestPositionDownOnClick);
        testPositionUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(TestPositionFrontOnClick);
        testPositionUI.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(TestPositionBackOnClick);
        testPositionUI.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(TestPositionResetOnClick);
        testPositionText = testPositionUI.transform.GetChild(8).GetComponent<TMPro.TextMeshProUGUI>();
        TestPositionTextReport();
    }

    private void SetSpecialInformationText(string name, ref TMPro.TextMeshProUGUI description) // Show Data Description
    {
        string text = name + "\n\n";

        switch (name)
        {
            case "LaserSender":
                text += "Laser Type:\n\nLaser Power:\n\n";
                break;
            case "LaserReceiver":
                text += "Received Laser Type:\n\nReceived Laser Power:\n\nTrigger Tag:\n\nIs Goal:\nLevel Name:\n(Only pick between Trigger Tag and Is Goal)\n\n";
                break;
            case "ColorChanger":
                text += "Color\n\n";
                break;
            case "Door1":
                text += "Scene Tag:\n\n Open Rate:\n\n";
                break;
            case "Glass":
                text += "Color:\n\n";
                break;
            case "Elevator":
                text += "Scene Tag:\n\nStart Position:\nX:\nY:\nZ:\n\nEnd Position:\nX:\nY:\nZ:\n\n";
                break;
            case "Portal":
                text += "Scene Tag:\n\nLinked Portal:\n\n";
                break;
            case "MovingPortal":
                text += "Scene Tag:\n\nLinked Portal:\n\nStart Position:\nX:\nY:\nZ:\n\nEnd Position:\nX:\nY:\nZ:\n\n";
                break;
        }
        description.text = text;
        SetSpecialInformationObject(name, ref description);
    }

    private void SetSpecialInformationObject(string name, ref TMPro.TextMeshProUGUI description) // Set Data
    {
        switch (name)
        {
            case "LaserSender":
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 4), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserSender>().laserType.ToString();
                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserSender>().power.ToString();
                break;
            case "LaserReceiver":
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 4), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 6), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 8), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 9), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserReceiver>().laserType.ToString();
                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserReceiver>().power.ToString();
                allInputFields[2].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserReceiver>().sceneTag;
                allInputFields[3].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserReceiver>().isGoal.ToString();
                allInputFields[4].GetComponent<TMPro.TMP_InputField>().text = GetComponent<LaserReceiver>().levelName;
                break;
            case "ColorChanger":
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<ColorChanger>().laserType.ToString();
                break;
            case "Door1":

                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 4), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<SceneObjectTag>().sceneTag;

                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = GetComponent<Door1>().openRate.ToString();
                break;
            case "Glass":
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<Glass>().laserType.ToString();
                break;
            case "Elevator":
                CreateTestPositionUI();

                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 5), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 6), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 7), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 10), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 11), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 12), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<SceneObjectTag>().sceneTag;
                Vector3 tempStartPos = GetComponent<Elevator>().startPos;
                Vector3 tempEndPos = GetComponent<Elevator>().endPos;
                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.x.ToString();
                allInputFields[2].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.y.ToString();
                allInputFields[3].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.z.ToString();

                allInputFields[4].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.x.ToString();
                allInputFields[5].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.y.ToString();
                allInputFields[6].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.z.ToString();
                break;
            case "Portal":
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 4), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<SceneObjectTag>().sceneTag;
                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = GetComponent<Portal>().linkedSceneTag;
                break;
            case "MovingPortal":
                CreateTestPositionUI();

                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 2), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 4), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 7), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 8), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 9), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 12), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 13), Quaternion.identity));
                allInputFields.Add(Instantiate(buildModeManagement.inputField, GetTextContentPosition(ref description, INPUTFIELD_WIDTH, 14), Quaternion.identity));

                allInputFields[0].GetComponent<TMPro.TMP_InputField>().text = GetComponent<SceneObjectTag>().sceneTag;
                allInputFields[1].GetComponent<TMPro.TMP_InputField>().text = GetComponent<MovingPortal>().linkedSceneTag;

                tempStartPos = GetComponent<MovingPortal>().startPos;
                tempEndPos = GetComponent<MovingPortal>().endPos;
                allInputFields[2].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.x.ToString();
                allInputFields[3].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.y.ToString();
                allInputFields[4].GetComponent<TMPro.TMP_InputField>().text = tempStartPos.z.ToString();

                allInputFields[5].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.x.ToString();
                allInputFields[6].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.y.ToString();
                allInputFields[7].GetComponent<TMPro.TMP_InputField>().text = tempEndPos.z.ToString();
                break;
        }

        for (int i = 0; i < allInputFields.Count; i++)
        {
            allInputFields[i].transform.SetParent(extraInformationUI.transform);
        }
    }

    private void SaveSpecialInformationData(string name) // Save Data
    {
        BuildModeManagement buildModeManagement = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuildModeManagement>();
        int builtObjectIndex = SerializedClasses.SerializedBuildObject.FindIndexByInstanceID(gameObject, ref buildModeManagement.sceneBuildObjects);
        List<string> answers = new List<string>();
        switch (name)
        {
            case "LaserSender":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);

                LaserSender laserSender = GetComponent<LaserSender>();
                laserSender.laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), answers[0]);
                laserSender.power = int.Parse(answers[1]);
                LaserInteraction laserInteraction = GameObject.FindGameObjectWithTag("LaserInteractionManager").GetComponent<LaserInteraction>();
                int index = laserInteraction.mainLaserData.IndexOf(laserSender.laserData);
                laserSender.laserData = new Laser.MainLaserData(laserSender.laserType, laserSender.power, transform, transform.up);
                laserInteraction.mainLaserData[index] = laserSender.laserData;
                laserSender.ChangeMaterial();
                break;
            case "LaserReceiver":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[2].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[3].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[4].GetComponent<TMPro.TMP_InputField>().text);

                LaserReceiver laserReceiver = GetComponent<LaserReceiver>();
                laserReceiver.laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), answers[0]);
                laserReceiver.power = int.Parse(answers[1]);
                laserReceiver.sceneTag = answers[2];
                laserReceiver.isGoal = bool.Parse(answers[3]);
                laserReceiver.linkedSceneObject = buildModeManagement.FindGameObjectWithSceneTag(answers[2]);
                laserReceiver.levelName = answers[4];
                laserReceiver.ChangeMaterial();
                break;
            case "ColorChanger":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);

                ColorChanger colorChanger = GetComponent<ColorChanger>();
                colorChanger.laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), answers[0]);

                colorChanger.meshRenderer.material = LaserColorList.FindLaserMaterial(colorChanger.laserType, in colorChanger.laserColorList.laserColorList);
                break;
            case "Door1":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);

                GetComponent<SceneObjectTag>().sceneTag = answers[0];
                GetComponent<Door1>().openRate = int.Parse(answers[1]);
                break;
            case "Glass":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);

                Glass glass = GetComponent<Glass>();
                glass.laserType = (LaserColorList.LaserType)System.Enum.Parse(typeof(LaserColorList.LaserType), answers[0]);
                glass.meshRenderer.material = LaserColorList.FindLaserMaterial(glass.laserType, in glass.laserColorList.laserColorList);
                break;
            case "Elevator":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[2].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[3].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[4].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[5].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[6].GetComponent<TMPro.TMP_InputField>().text);

                Elevator elevator = GetComponent<Elevator>();
                GetComponent<SceneObjectTag>().sceneTag = answers[0];
                elevator.startPos = new Vector3(float.Parse(answers[1]), float.Parse(answers[2]), float.Parse(answers[3]));
                elevator.endPos = new Vector3(float.Parse(answers[4]), float.Parse(answers[5]), float.Parse(answers[6]));
                break;
            case "Portal":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);

                Portal portal = GetComponent<Portal>();
                GetComponent<SceneObjectTag>().sceneTag = answers[0];
                portal.linkedSceneTag = answers[1];
                portal.linkedPortal = buildModeManagement.FindGameObjectWithSceneTag(answers[1]);
                break;
            case "MovingPortal":
                answers.Add(allInputFields[0].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[1].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[2].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[3].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[4].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[5].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[6].GetComponent<TMPro.TMP_InputField>().text);
                answers.Add(allInputFields[7].GetComponent<TMPro.TMP_InputField>().text);

                MovingPortal movingPortal = GetComponent<MovingPortal>();
                GetComponent<SceneObjectTag>().sceneTag = answers[0];
                movingPortal.linkedSceneTag = answers[1];
                movingPortal.linkedPortal = buildModeManagement.FindGameObjectWithSceneTag(answers[1]);
                movingPortal.startPos = new Vector3(float.Parse(answers[2]), float.Parse(answers[3]), float.Parse(answers[4]));
                movingPortal.endPos = new Vector3(float.Parse(answers[5]), float.Parse(answers[6]), float.Parse(answers[7]));
                break;
        }

        buildModeManagement.builtObjects[builtObjectIndex].stringList = answers;
        if (GetComponent<SceneObjectTag>() != null)
        {
            buildModeManagement.builtObjects[builtObjectIndex].tag = GetComponent<SceneObjectTag>().sceneTag;
        }
        extraSettings = answers;
        allInputFields.Clear();
    }
}
