using Assets.Scripts.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHeaderUI : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private Sprite[] headerSprites;
    [SerializeField] private Image headerButtonImage;
    [SerializeField] private TextMeshProUGUI unitNameDisplay;
    [SerializeField] private TextMeshProUGUI unitInfoDisplay;
    [SerializeField] private TextMeshProUGUI unitInfoTextDisplay;

    [Header("Lessons")]
    [SerializeField] private Sprite[] lessonsSprites;
    [SerializeField] private GameObject[] lessons;

    private UnitObject unit;

    public void SetUnitInfo(UnitObject unitData, int sectionNumber, int unitNumber, string sectionDescription)
    {
        this.unit = unitData;
        unitNameDisplay.text = unitData.name;
        unitInfoDisplay.text = $"Section {sectionNumber} - Unit {unitNumber}";
        headerButtonImage.sprite = headerSprites[unitNumber];
        unitInfoTextDisplay.text = sectionDescription;

        int index = 0;
        foreach (GameObject lessonButton in lessons)
        {
            lessonButton.GetComponent<Image>().sprite = lessonsSprites[unitNumber];
            lessonButton.GetComponent<LessonButton>().SetCurrentLesson(unitData);
 
            if (sectionNumber == 1 && unitNumber == 1 || index ==0)
            {
                lessonButton.GetComponent<Button>().interactable = true;
            }
        }       
    }
}
