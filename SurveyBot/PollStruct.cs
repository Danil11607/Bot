using BD_For_Test;
using System.Collections.Generic;

namespace PollStruct
{
	class Poll
	{
		public enum PollType { anon, halfanon, nonanon };
		public PollType type;
		private int count;
		public string token;
		public string password;
		public List<Question> Questions = new List<Question>();
		
		public Poll(string token, int count, string password, PollType type)
		{
			this.count = count;
			this.token = token;
			this.type = type;
		}
	}

	class Answer
	{
		public int Id { get; set; }
		public string Content;
		public Question skipTo;// убрать все пропуски
		
        
		public Answer(string answer, Question skipTo = null)
		{
			this.Content = answer;
			this.skipTo = skipTo;// убрать все пропуски
		}
	}

	class Question
	{
		public int Id { get; set; }
		public Node nd;
		public enum QuestionType { test, testMulti, testSkip, question };
		public QuestionType type;
		public string content;
		public int count;
		public List<Answer> Answers=new List<Answer>();

		public Question(string content, QuestionType type, int count)
		{
			this.content = content;
			this.type = type;
			this.count = count;
		}
	}
}