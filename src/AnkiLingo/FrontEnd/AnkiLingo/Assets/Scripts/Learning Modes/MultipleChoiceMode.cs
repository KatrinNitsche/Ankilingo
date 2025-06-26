using Assets.Scripts;
using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MultipleChoiceMode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionDisplay;
    [SerializeField] private TextMeshProUGUI[] answerButtonTexts;
    [SerializeField] private int pointsForCorrectAnswer = 1;
    [SerializeField] private int pointsMax = 10;
    [SerializeField] private TextMeshProUGUI statisticText;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctAnswer;
    [SerializeField] private AudioClip wrongAnswer;

    private UnitObject currentLesson;
    private List<Question> questions;
    private int currentQuestion;
    private GameManager gameManager;
    private CourseService courseService;
    private bool hasAnswered;
    private int correctAnswers;
    private int wrongAnswers;

    private void Start()
    {
        gameManager = GameObject.Find("Helpers").GetComponent<GameManager>();
        courseService = GameObject.Find("Helpers").GetComponent<CourseService>();
    }

    private struct Question
    {
        public int qustionId { get; set; }
        public string ValueInQuestion { get; set; }
        public List<string> AnswerOptions { get; set; }
        public string CorrectOption { get; set; }
    }

    public void SetCurrentLesson(UnitObject lesson)
    {
        this.currentLesson = lesson;
        this.currentQuestion = 0;
        PrepareTestingData();
        DisplayQuestion();
    }

    public void CheckAnswer(int answerIndex)
    {
        if (hasAnswered) return;

        hasAnswered = true;
        var questionId = questions.ElementAt(currentQuestion).qustionId;
        var givenAnswer = questions.ElementAt(currentQuestion).AnswerOptions.ElementAt(answerIndex);
        var expectedAnswer = questions.ElementAt(currentQuestion).CorrectOption;
        if (givenAnswer == expectedAnswer)
        {
            audioSource.PlayOneShot(correctAnswer);
            gameManager.GainPoints(1);
            correctAnswers++;

            var currentQuestionObject = currentLesson.entries.FirstOrDefault(x => x.id == questionId);
            if (currentQuestionObject != null && currentQuestionObject.levelOnKnowledge < 11)
            {
                currentQuestionObject.levelOnKnowledge += 1;
                StartCoroutine(courseService.UpdateEntry(currentQuestionObject, completed =>
                {
                }));
            }
        }
        else
        {
            audioSource.PlayOneShot(wrongAnswer);
            wrongAnswers++;
        }

        currentQuestion++;
        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (currentQuestion >= questions.Count)
        {
            gameObject.SetActive(false);
        }
        else
        {
            var questionObject = questions.ElementAt(currentQuestion);
            var question = questionObject.ValueInQuestion;
            var options = questionObject.AnswerOptions;
            questionDisplay.text = question;

            int index = 0;
            foreach (string optionText in options)
            {
                answerButtonTexts[index].text = optionText;
                index++;
            }

            hasAnswered = false;

            statisticText.text = $"Question {currentQuestion + 1} of {questions.Count()} \r\n({correctAnswers} correct - {wrongAnswers} wrong)";
        }
    }

    private void PrepareTestingData()
    {
        questions = new List<Question>();
        foreach (EntryObject entry in currentLesson.entries)
        {
            AddQuestionsAboutValue1(entry);
            AddQuestionsAboutValue2(entry);
        }

        questions.Shuffle<Question>();
    }

    private void AddQuestionsAboutValue1(EntryObject entry)
    {
        var answerOptions = new List<string>();
        var randomCorrectAnswerPosition = UnityEngine.Random.Range(0, 5);
        var index = 0;
        foreach (string potentialAnswer in currentLesson.entries.Select(x => x.value2))
        {
            if (index == randomCorrectAnswerPosition)
            {
                answerOptions.Add(entry.value2);
            }
            else
            {
                // get a random entry from currentLesson.entries which is not the correct answer or already in the answersOptions list
                var incorrectOptions = currentLesson.entries
                    .Where(x => x.value2 != entry.value2 && !answerOptions.Contains(x.value2))
                    .Select(x => x.value2)
                    .ToList();

                string answerOption;
                if (incorrectOptions.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, incorrectOptions.Count);
                    answerOption = incorrectOptions[randomIndex];
                }
                else
                {
                    // fallback: just use the correct answer (should not happen if enough entries)
                    answerOption = entry.value2;
                }

                answerOptions.Add(answerOption);
            }

            index++;
            if (index == 5) break;
        }

        questions.Add(new Question()
        {
            qustionId = entry.id,
            ValueInQuestion = entry.value1,
            CorrectOption = entry.value2,
            AnswerOptions = answerOptions
        });
    }

    private void AddQuestionsAboutValue2(EntryObject entry)
    {
        var answerOptions = new List<string>();
        var randomCorrectAnswerPosition = UnityEngine.Random.Range(0, 5);
        var index = 0;
        foreach (string potentialAnswer in currentLesson.entries.Select(x => x.value1))
        {
            if (index == randomCorrectAnswerPosition)
            {
                answerOptions.Add(entry.value1);
            }
            else
            {
                // get a random entry from currentLesson.entries which is not the correct answer or already in the answersOptions list
                var incorrectOptions = currentLesson.entries
                    .Where(x => x.value1 != entry.value1 && !answerOptions.Contains(x.value1))
                    .Select(x => x.value1)
                    .ToList();

                string answerOption;
                if (incorrectOptions.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, incorrectOptions.Count);
                    answerOption = incorrectOptions[randomIndex];
                }
                else
                {
                    // fallback: just use the correct answer (should not happen if enough entries)
                    answerOption = entry.value1;
                }

                answerOptions.Add(answerOption);
            }

            index++;
            if (index == 5) break;
        }

        questions.Add(new Question()
        {
            qustionId = entry.id,
            ValueInQuestion = entry.value2,
            CorrectOption = entry.value1,
            AnswerOptions = answerOptions
        });
    }
}
