using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Threading;
using Debug;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using StructBase;
using PollStruct;
using StatsGathering;
using System.IO;

namespace SurveyBot
{
    class Program
    {
        static List<Session> Sessions = new List<Session>();
        static List<string> TokenContainer = new List<string>();
        static TelegramBotClient Bot = new TelegramBotClient("343759311:AAGwKYHxEX4kC2DvLlXpKovytJKCDq1Inwk");
        static void Main(string[] args)
        {
            TokenContainer.Add("test");
            Bot.StartReceiving();
            Bot.OnMessage += MessageParse;
            Console.ReadKey(); 
        }

        private async static void MessageParse(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var x = new Session(e.Message.Chat, null, Session.SessionType.Pass, null);
            var Sesion = x.ContatinSession(Sessions);
            if (Sesion!=null)
            {
                switch (Sesion.Type)
                {
                    case Session.SessionType.Pass:
                        Sessions.Remove(Sesion);
                        Sesion.Answers.Add(e.Message.Text);
                        Add add = new Add(Sesion.Poll);
                        add.AddResultsToDb(int.Parse(Sesion.Chat.Id.ToString()), int.Parse(e.Message.Text), null);
                        if (Sesion.Questions.Count == 0)
                        {
                            return;
                        }
                        SendQuestion(Sesion.Questions, Sesion.Chat);
                        Sessions.Add(Sesion);
                        break;
                    case Session.SessionType.Create:
                        var file = await Bot.GetFileAsync(e.Message.Document.FileId);
                        Add AddPoll = new Add(new PollReader().ReadPoll(file));
                        AddPoll.AddPollsToDb();
                        Sessions.Remove(Sesion);

                        break;
                }
                
            }
            if(e.Message.Text!=null)
            { string[] msg = e.Message.Text.Split(' ');
                switch (msg[0])
                {
                    case "/join":
                        if (msg.Count() != 2)
                        {
                            DebugMessage.SendDebug("MainClient", "Wrong Arguments", DebugMessage.MessageLevel.Error);
                            Bot.SendTextMessageAsync(e.Message.Chat.Id, "Wrong Arguments");
                            break;
                        }
                        if (TokenContainer.Contains(msg[1]))
                            InitiateSesion(e.Message.Chat, msg[1], Session.SessionType.Pass);
                        break;
                    case "/create":
                        Bot.SendTextMessageAsync(e.Message.Chat.Id, "Send *.txt file with your poll");
                        InitiateSesion(e.Message.Chat, "test", Session.SessionType.Create);
                        break;
                }
            }
        }
        
        private static void InitiateSesion(Chat User, string token, Session.SessionType Type)
        {
            switch (Type)
            {
                case Session.SessionType.Pass:
                    var ThisPoll = new Analyze(token, 1488).GetPoll();
                    Queue<Question> Questions = new Queue<Question>();
                    foreach (var question in ThisPoll.Questions)
                    {
                        Questions.Enqueue(question);

                    }
                    var ses = new Session(User, Questions, Session.SessionType.Pass, ThisPoll);
                    SendQuestion(ses.Questions, ses.Chat);
                    Sessions.Add(ses);
                    break;
                case Session.SessionType.Create:
                    Sessions.Add(new Session(User, null, Session.SessionType.Create, null));
                    break;
            }
        }
        
        private static int SendQuestion(Queue<Question> Questions, Chat chat)
        {
            List<KeyboardButton> KButtons = new List<KeyboardButton>();
            if (Questions.Count == 0)
                return 0;
            var question = Questions.Dequeue();
            foreach (var answer in question.Answers)
                KButtons.Add(new KeyboardButton(answer.Content));
            Bot.SendTextMessageAsync(chat.Id, question.Content, ParseMode.Default, false, false, 0, new ReplyKeyboardMarkup(KButtons.ToArray(), true, true));
            return 1;
        }

        public string GetToken()
        {
            Guid g;
            g = Guid.NewGuid();
            string token = g.ToString();
            token = token.Substring(0, 13);

            return token;
        }

    }
    class Session
    {
        public List<string> Answers;
        public readonly Chat Chat;
        public Queue<Question> Questions;
        public Poll Poll;
        public enum SessionType { Create, Pass };
        public SessionType Type;

        public Session(Chat user, Queue<Question> questions , SessionType SType, Poll poll)
        {
            Answers = new List<string>();
            Chat = user;
            Type = SType;
            if (questions == null)
            { Questions = new Queue<Question>(); return; }
            Questions = new Queue<Question>(questions);
            
        }

        public Session ContatinSession(List<Session> x)
        {
            foreach (var item in x)
                if (item.Chat.Username == this.Chat.Username)
                {
                    return item;
                }
            return null;
        }
    }
}
