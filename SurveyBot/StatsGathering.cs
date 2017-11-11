using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructBase;

namespace StatsGathering
{
    partial class Analyze
    {
        string Token;
        int UserId;
        public Analyze(string token, int userId)
        {
            Token = token;
            UserId = userId;
        }

        public Poll GetPoll()
        {
            using (Context db = new Context())
            {
                Poll poll = db.Polls.Where(x=>x.Token==Token).First();
                return poll;
            }
        }

        PushData[] TakePushDataForIdAndToken(string token, int userId)
        {
            using (Context db = new Context())
            {
                var data = db.PushDatas.Where(x => x.Token == Token && x.UserId == UserId).ToArray();
                return data;
            }
        }

        public void AnalyzePublic()
        {
            var data = TakePushDataForIdAndToken(Token, UserId);

        }

    }
}
