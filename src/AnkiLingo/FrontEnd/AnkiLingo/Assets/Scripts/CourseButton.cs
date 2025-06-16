using Assets.Scripts;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourseButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CourseName;
    [SerializeField] private Image CourseIcon;
    [SerializeField] private Sprite[] iconList;
    [HideInInspector] public CourseObject course;

    public CourseObject Initialize()
    {
        CourseName.text = course.name;
        CourseIcon.sprite = iconList.FirstOrDefault(x => x.name == course.icon);

        return this.course;
    }
}
