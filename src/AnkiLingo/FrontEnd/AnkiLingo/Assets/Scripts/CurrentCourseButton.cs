using Assets.Scripts;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentCourseButton : MonoBehaviour
{
    [SerializeField] private Sprite[] iconList;
    [SerializeField] private Image courseIconDisplay;
    [SerializeField] private TextMeshProUGUI courseNameDisplay;
    [SerializeField] private CourseService CourseService;
    [SerializeField] private string username = "AnkiLingoTester";
    [SerializeField] private string password = "S#Tb3B]ww,m1AgU";

    [Header("Course Details")]
    [SerializeField] private Transform sectionPanel;
    [SerializeField] private GameObject sectionPrefab;

    private CourseObject currentCourse;
    private CourseObject[] courseList;

    private void Start()
    {
        StartCoroutine(CourseService.SetToken(username, password, result =>
        {
            LoadCourseList();
        }));
    }

    private void SetCurrentCourse()
    {
        if (PlayerPrefs.HasKey("current-course"))
        {
            string currentCourseName = PlayerPrefs.GetString("current-course");
            if (string.IsNullOrEmpty(currentCourseName))
            {
                currentCourse = courseList.FirstOrDefault();
            }
            else
            {
                currentCourse = courseList.FirstOrDefault(x => x.name == currentCourseName);
            }
        }
        else
        {
            currentCourse = courseList.FirstOrDefault();
        }

        if (currentCourse != null)
        {
            DisplayCurrentCourseDetails();

        }
    }

    private void DisplayCurrentCourseDetails()
    {
        courseNameDisplay.text = currentCourse.name;
        courseIconDisplay.sprite = iconList.FirstOrDefault(x => x.name == currentCourse.icon);
    }

    private void LoadCourseList()
    {
        StartCoroutine(CourseService.GetCourses(courses =>
        {
            if (courses != null)
            {
                courseList = courses;
                SetCurrentCourse();
                LoadCurrentCourseDetails();
            }
        }));
    }

    private void LoadCurrentCourseDetails()
    {
        StartCoroutine(CourseService.GetCourse(currentCourse.id, courseDetails =>
        {
            for (int i = 0; i < courseDetails.sections.Count; i++)
            {
                var section = courseDetails.sections[i];
                var sectionObject = Instantiate(sectionPrefab, sectionPanel);
                sectionObject.GetComponent<SectionUI>().SetSectionInfo(section, i + 1);
            }
        }));
    }
}
