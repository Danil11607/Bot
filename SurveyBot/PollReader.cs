using System;
using System.IO;
using StructBase;

namespace PollStruct
{
    class PollReader
    {
        public Poll ReadPoll(Telegram.Bot.Types.File file)
        {
            StreamReader sr = new StreamReader(file.FileStream);
            string text = "";
            while (!text.Contains("Type"))
                text = sr.ReadLine();
            string[] tempText = text.Split('=');
            int type = Convert.ToInt32(tempText[1].Trim());
            Poll.PollType enmType = (Poll.PollType)type;

            while (!text.Contains("Pass"))
                text = sr.ReadLine();
            tempText = text.Split('=');
            string password = tempText[1].Trim();
			text = "";
            while (!text.Contains("QCount"))
                text = sr.ReadLine();
            tempText = text.Split('=');
            int numberOfQuestions = Convert.ToInt32(tempText[1].Trim());

            Poll poll = new Poll("test", "abc", numberOfQuestions);


            int i = 1;
            while (!sr.EndOfStream)
            {
                text = sr.ReadLine();
                if (text.StartsWith($"Q{i}"))
                {
                    string questionContent = text.Substring(text.IndexOf(':') + 1);

                    while (!text.StartsWith("ACount"))
                        text = sr.ReadLine();
                    int aCount = Convert.ToInt32(text.Substring(text.IndexOf('=') + 1));

                    while (!text.StartsWith("QType"))
                        text = sr.ReadLine();
                    string qType1 = text.Substring(text.IndexOf('=') + 1);
                    Question.QuestionType qType = (Question.QuestionType)Enum.Parse(typeof(Question.QuestionType), qType1);

                    Question question = new Question(questionContent, qType, aCount);
                    poll.Questions.Add(question);

                    int j = 1;
                    while (!text.StartsWith("}"))
                    {
                        text = sr.ReadLine();
                        if (text.StartsWith($"A{j}"))
                        {
                            string tempAnswer = text.Substring(text.IndexOf(':') + 1).Trim();
                            Answer answer = new Answer(tempAnswer);
                            poll.Questions[i-1].Answers.Add(answer);
                            j++;
                        }

                    }
                    j = 1;
                    i++;
                }
            }
			foreach (var it in poll.Questions)
				Console.WriteLine(it);

			return poll;
        }
    }
}