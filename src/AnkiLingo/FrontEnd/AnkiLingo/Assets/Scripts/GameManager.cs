using Assets.Scripts.Objects;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Avatar Settings")]
    [SerializeField] private IconSelectionWindowUI avatarSelectionPanel;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Sprite[] iconList;

    [Header("Score Settings")]
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    [Header("Learning Settings")]
    [SerializeField] private GameObject learningPanel;
    [SerializeField] private GameObject readingPanel;
    [SerializeField] private GameObject multipleChoicePanel;
    [SerializeField] private TextMeshProUGUI lessonInfoDisplay;
    [SerializeField] private TextMeshProUGUI lessonEntries;

    private Sprite currentAvatar;
    private int currentScore = 0;
    private UnitObject currentLesson;

    private void Start()
    {
        multipleChoicePanel.SetActive(false);
        avatarSelectionPanel.GetComponent<IconSelectionWindowUI>().onIconClicked += GameManager_onIconClicked;
        LoadCurrentCourse();
        LoadCurrentScore();
    }

    public enum LearningMode { Reading, MultipleChoice }

    public void StartLearning(UnitObject unit, LearningMode learningMode)
    {
        currentLesson = unit;
        lessonInfoDisplay.text = unit.name;

        switch (learningMode)
        {
            case LearningMode.Reading: ReadingMode(unit); break;
            case LearningMode.MultipleChoice: MultipleChoiceMode(unit); break;
        }


        learningPanel.SetActive(true);
    }

    private void MultipleChoiceMode(UnitObject unit)
    {
        multipleChoicePanel.SetActive(true);
        multipleChoicePanel.GetComponent<MultipleChoiceMode>().SetCurrentLesson(unit);
    }

    private void ReadingMode(UnitObject unit)
    {
        lessonEntries.text = "<color=blue><size=24>Entries:</size></color>" + System.Environment.NewLine;
        foreach (EntryObject entry in unit.entries)
        {
            var levelDisplay = $"<color=red>{entry.levelOnKnowledge}</color>";
            if (entry.levelOnKnowledge > 10)
            {
                levelDisplay = $"<color=orange>{entry.levelOnKnowledge}</color>";
            }
            else if (entry.levelOnKnowledge > 20)
            {
                levelDisplay = $"<color=green>{entry.levelOnKnowledge}</color>";
            }

            lessonEntries.text += $"({levelDisplay}) <b>{entry.value1}</b> - {entry.value2} {System.Environment.NewLine}";
        }
        readingPanel.SetActive(true);
    }

    public void StopLearning()
    {
        currentLesson = null;
        lessonInfoDisplay.text = string.Empty;
        lessonEntries.text = string.Empty;
        learningPanel.SetActive(false);
        readingPanel.SetActive(false);
        multipleChoicePanel.SetActive(false);
    }

    private void GameManager_onIconClicked(Sprite avatar)
    {
        avatarImage.sprite = iconList.FirstOrDefault(x => x.name == avatar.name);
        PlayerPrefs.SetString("current-avatar", avatar.name);
        PlayerPrefs.Save();
    }

    private void LoadCurrentCourse()
    {
        if (PlayerPrefs.HasKey("current-avatar"))
        {
            string avatarName = PlayerPrefs.GetString("current-avatar");
            if (!string.IsNullOrEmpty(avatarName))
            {
                avatarImage.sprite = iconList.FirstOrDefault(x => x.name == avatarName);
            }
        }
    }

    private void LoadCurrentScore()
    {
        if (PlayerPrefs.HasKey("current-score"))
        {
            currentScore = PlayerPrefs.GetInt("current-score");
        }

        scoreDisplay.text = $"{currentScore}";
    }

    public void GainPoints(int v)
    {
        currentScore += v;
        PlayerPrefs.SetInt("current-score", currentScore);
        PlayerPrefs.Save();
        scoreDisplay.text = $"{currentScore}";
    }
}
