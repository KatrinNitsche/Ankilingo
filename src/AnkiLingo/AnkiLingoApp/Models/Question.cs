namespace AnkiLingoApp.Models
{
    public enum QuestionType
    {
        MultipleChoiceValue1,
        MultipleChoiceValue2,
        InputValue1,
        InputValue2,
        ListModelValue1,
        ListModelValue2,
    }

    public class Question
    {
        #region UpdateEntry Properties     
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        #endregion 

        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public int LevelOfKnowledge { get; set; }
        public DateTime LastReviewed { get; set; }
        public QuestionType QuestionType { get; set; }

        #region Additional Properties
        public List<string> Options { get; set; } = new List<string>();
        public string UserAnswer { get; set; }
        public List<string> LeftValues { get; set; } = new List<string>();
        public List<string> RightValues { get; set; } = new List<string>();
        #endregion
    }
}
