using Assets.Scripts;
using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourseDetailsUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField courseNameDisplay;
    [SerializeField] private TMP_InputField courseDescriptionDisplay;
    [SerializeField] private TextMeshProUGUI courseDatesDisplay;
    [SerializeField] private Image courseIconDisplay;
    [SerializeField] private GameObject iconSelectionPanel;
    [SerializeField] private CourseService CourseService;
    [SerializeField] private Sprite[] iconList;

    [Header("Sections")]
    [SerializeField] private GameObject SectionButonPrefab;
    [SerializeField] private Transform SectionListContentContainer;

    [Header("Buttons")]
    [SerializeField] private Sprite addIcon;
    [SerializeField] private Sprite updateIcon;
    [SerializeField] private GameObject SaveButton;
    [SerializeField] private GameObject DeleteButton;

    private CourseObject course;
    private bool newCourse;

    private void Start()
    {
        iconSelectionPanel.GetComponent<IconSelectionWindowUI>().onIconClicked += CourseDetailsUI_onIconClicked;
        SaveButton.GetComponent<Image>().sprite = updateIcon;
    }

    private void CourseDetailsUI_onIconClicked(Sprite obj)
    {
        SetIcon(obj);
    }

    public void Initialize(CourseObject courseData, bool newCourse)
    {
        this.course = courseData;
        this.newCourse = newCourse;

        courseNameDisplay.text = course.name;
        courseDescriptionDisplay.text = course.description;
        courseIconDisplay.sprite = iconList.FirstOrDefault(x => x.name == course.icon);
        courseDatesDisplay.text = course.created + " / " + course.updated;

        if (courseData.Sections != null && courseData.Sections.Count() > 0)
        {
            foreach (SectionObject section in courseData.Sections)
            {
                var sectionButton = Instantiate(SectionButonPrefab, SectionListContentContainer);
                sectionButton.GetComponentInChildren<TextMeshProUGUI>().text = section.name;
            }
        }

        SaveButton.GetComponent<Image>().sprite = newCourse ? addIcon : updateIcon;
        DeleteButton.GetComponent<Button>().enabled = newCourse ? false : true;
    }

    public void CloseDetailsWindow()
    {
        gameObject.SetActive(false);
        EventsLib.CourseDetailsWindowClosed.CallEvent(EventArgs.Empty);
    }

    public void OpenIconSelectionPanel()
    {
        iconSelectionPanel.SetActive(true);
    }

    public void SetIcon(Sprite icon)
    {
        courseIconDisplay.GetComponent<Image>().sprite = icon;
        course.icon = icon.name;
    }

    public void DeleteCourse()
    {
        if (newCourse) return;

        StartCoroutine(CourseService.RemoveCourse(course.id, result =>
        {
            CloseDetailsWindow();
        }));
    }

    public void SaveCoureDetails()
    {
        if (course.created == null) course.created = DateTime.Now.ToString();
        if (course.updated == null) course.updated = DateTime.Now.ToString();
        course.name = courseNameDisplay.text;
        course.description = courseDescriptionDisplay.text;
        
        if (newCourse)
        {
            StartCoroutine(CourseService.AddCourse(course, result =>
            {
                CloseDetailsWindow();
            }));
        }
        else
        {
            StartCoroutine(CourseService.UpdateCourse(course, result =>
            {
                if (result)
                {
                    CloseDetailsWindow();
                }
            }));
        }
    }   
}
