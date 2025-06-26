using Assets.Scripts.Objects;
using UnityEngine;

public class SectionUI : MonoBehaviour
{
    [SerializeField] private GameObject UnitPrefab;
    [SerializeField] private Transform UnitContainer;

    private SectionObject section;
    private int sectionNumber;

    public void SetSectionInfo(SectionObject section, int sectionNumber)
    {
        this.section = section;
        this.sectionNumber = sectionNumber;

        for (int i = 0; i < section.units.Count; i++)
        {
            var unitObject = Instantiate(UnitPrefab, UnitContainer);
            unitObject.GetComponent<UnitHeaderUI>().SetUnitInfo(section.units[i], sectionNumber, i + 1, section.description);
        }
    }
}
