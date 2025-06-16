using Assets.Scripts;
using Assets.Scripts.Helpers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject CourseDetailsPanel;
    [SerializeField] private GameObject ManageCoursesPanel;
    [SerializeField] private GameObject CourseButtonPrefab;
    [SerializeField] private Transform CourseContainer;
    [SerializeField] private CourseService CourseService;
    [SerializeField] private string username = "AnkiLingoTester";
    [SerializeField] private string password = "S#Tb3B]ww,m1AgU";
      
    private void Start()
    {
        CourseDetailsPanel.SetActive(false);
        ManageCoursesPanel.SetActive(false);
        CourseService.SetToken(username, password);
        EventsLib.CourseDetailsWindowClosed.OnEventCalled += RefreshCourseList;
    }

    private void OnDisable()
    {
        EventsLib.CourseDetailsWindowClosed.OnEventCalled -= RefreshCourseList;
    }

    public void ToggleManageCoursesPanel()
    {
        ManageCoursesPanel.SetActive(!ManageCoursesPanel.activeSelf);
        LoadCourses();
    }

    private void LoadCourses()
    {
        StartCoroutine(CourseService.GetCourses(courses =>
        {
            if(courses != null)
            {
                PopulateCourseButtons(courses);
            }
        }));       
    }

    private void PopulateCourseButtons(CourseObject[] courses)
    {
        foreach (Transform child in CourseContainer)
        {
            Destroy(child.gameObject); // Clear old buttons
        }

        foreach (var course in courses)
        {
            GameObject buttonObj = Instantiate(CourseButtonPrefab, CourseContainer);
            CourseButton courseButton = buttonObj.GetComponent<CourseButton>();
            courseButton.course = course;
            var courseObject = courseButton.Initialize();

            buttonObj.GetComponent<Button>().onClick.AddListener(() => DisplayCourseDetails(courseObject));
        }
    }

    private void DisplayCourseDetails(CourseObject courseObject)
    {
        CourseDetailsPanel.SetActive(true);

        StartCoroutine(CourseService.GetSections(courseObject.id, sections =>
        {
            if (sections != null)
            {
                courseObject.Sections = sections.ToList();
                CourseDetailsPanel.GetComponent<CourseDetailsUI>().Initialize(courseObject, false);
            }
        }));       
    }
    public void AddCourse()
    {
        CourseDetailsPanel.SetActive(true);
        CourseDetailsPanel.GetComponent<CourseDetailsUI>().Initialize(new CourseObject(), true);
    }

    private void RefreshCourseList(object sender, EventArgs e)
    {
        LoadCourses();
    }
}
