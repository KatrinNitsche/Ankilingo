using Assets.Scripts.Objects;
using UnityEngine;
using static GameManager;

public class LessonButton : MonoBehaviour
{
    [SerializeField] private LearningMode learningMode;
    private UnitObject currentLesson;
    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameObject.Find("Helpers").GetComponent<GameManager>();
    }

    public void SetCurrentLesson(UnitObject lesson)
    {
        currentLesson = lesson;
    }

    public void StartLesson()
    {
        gameManager.StartLearning(currentLesson, learningMode);
    }
}
