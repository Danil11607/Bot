using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using PollStruct;


namespace holiday
{
    //enum type {Anon=1,HalfAnon,Public }

    interface IAdd
    {
        void AddAnonPoll();
        void AddHalfPoll();
        void AddPublicPoll();
    }

    class PushData
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public List<string> Push { get; set; }
    }

    class PublicData
    {
        public List<string> push { get; set; }
        public string userID { get; }
    }

    class aDataContext : DbContext
    {
        public aDataContext()
        : base("DbConnection")
        { }

        public DbSet<PushData> Users { get; set; }
    }

    class pDataContext : DbContext
    {
        public pDataContext()
        : base("DbConnection")
        { }

        public DbSet<PublicData> Users { get; set; }
    }

    class Add:IAdd
    {
        Poll ourPoll;
        public Add(Poll ourPoll)
        {
            this.ourPoll = ourPoll;
        }
        public void AddAnonPoll()
        {
            
        }
        public void AddHalfPoll()
        {
            throw new NotImplementedException();
        }

        public void AddPublicPoll()
        {
            throw new NotImplementedException();
        }

        public void AddToDB(List<string> bottle, int userID, string token)
        {
            //DbSet stat;
            using (aDataContext db = new aDataContext())
            {
                for (int i = 0; i < ourPoll.Questions.Count; i++)   //нам передаются варинаты ответа в виде "1 2 c 4"  в листе.(по словам Жени К)
                                                                    //по вопросам
                {
                    int[] arrayToPush = new int[ourPoll.Questions[i].Answers.Count];
                    List<string> listToPush = new List<string>();
                    if ((int)ourPoll.Type == 0)
                    {
                        if ((int)ourPoll.Questions[i].Type == 0)
                        {
                            arrayToPush[Convert.ToInt32(bottle[i]) - 1]++;
                            foreach (var item in arrayToPush)
                                listToPush.Add(item.ToString());

                            if (db.Users == null)
                            {
                                var pushData = new PushData { Id = userID, Push = listToPush, Token = token };
                                db.Users.Add(pushData);
                            }
                            else
                            {
                                IEnumerable<PushData> user = db.Users
                                    .Where(c => c.Push[Convert.ToInt32(bottle[i]) - 1] != null)
                                    .AsEnumerable()
                                    .Select(c => { c.Push[Convert.ToInt32(bottle[i]) - 1] = (Convert.ToInt32(c.Push[Convert.ToInt32(bottle[i]) - 1]) + 1).ToString(); return c; });
                                foreach (var item in user)
                                    db.Entry(item).State = EntityState.Modified;
                            }
                        }
                        else if ((int)ourPoll.Questions[i].Type == 3)
                        {
                            if (db.Users == null)
                            {
                                var pushData = new PushData { Id = userID, Push = listToPush, Token = token };
                                db.Users.Add(pushData);
                            }
                            else
                            {
                                IEnumerable<PushData> user = db.Users
                                    .Where(c => c.Push[Convert.ToInt32(bottle[i]) - 1] != null)
                                    .AsEnumerable()
                                    .Select(c => { c.Push[Convert.ToInt32(bottle[i]) - 1] = c.Push[Convert.ToInt32(bottle[i]) - 1] + ";" + bottle[i]; return c; });
                                foreach (var item in user)
                                    db.Entry(item).State = EntityState.Modified;
                            }
                        }

                    }
                    else if ((int)ourPoll.Type == 1 || (int)ourPoll.Type == 2)
                    {
                        var pushData = new PushData { Id = userID, Push = bottle, Token = token };
                        db.Users.Add(pushData);
                    }
                }

                db.SaveChanges();
                
            }
        }
    }
}