using Assets.Scripts;
using Assets.Scripts.Helpers;
using System;
using UnityEngine;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject CourseButtonPrefab;
    [SerializeField] private Transform CourseContainer;
    [SerializeField] private CourseService CourseService;
    [SerializeField] private string username = "AnkiLingoTester";
    [SerializeField] private string password = "S#Tb3B]ww,m1AgU";

    private void Start()
    {
        StartCoroutine(CourseService.SetToken(username, password, result =>
        {

        }));

        EventsLib.CourseDetailsWindowClosed.OnEventCalled += RefreshCourseList;
    }

    private void OnDisable()
    {
        EventsLib.CourseDetailsWindowClosed.OnEventCalled -= RefreshCourseList;
    }

    private void LoadCourses()
    {
        StartCoroutine(CourseService.GetCourses(courses =>
        {
            if (courses != null)
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
        }
    }

    private void RefreshCourseList(object sender, EventArgs e)
    {
        LoadCourses();
    }
}
