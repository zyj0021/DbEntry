﻿using System;
//using System.Linq;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Debug
{
    class Program
    {
        //public abstract class User : DbObjectModel<User>
        //{
        //    public abstract string Name { get; set; }
        //    public abstract int Age { get; set; }
        //    public abstract bool Gender { get; set; }
        //    public abstract DateTime Birthday { get; set; }

        //    public abstract User Init(string name, int age, bool gender, DateTime birthday);
        //}

        //static void Main()
        //{
        //    // Create
        //    var u = User.New.Init("tom", 18, true, DateTime.Now);
        //    u.Save();
        //    // Read
        //    var u1 = User.FindById(u.Id);
        //    // Update
        //    u1.Name = "jerry";
        //    u1.Save();
        //    // Delete
        //    u1.Delete();
        //    // Query
        //    var ids = from p in User.Table where p.Age > 15 select new {p.Id};
        //    var l1 = from p in User.Table where p.Age > 15 && p.Gender == true select p;
        //    var l2 = User.Find(p => p.Age > 15 && p.Gender == true); // another style of linq
        //    var l3 = User.FindBySql("Select * From [User] Where [Age] > 15 And [Gender] = true"); // sql
        //}

        public class MyUser : IDbObject
        {
            [DbKey]
            public Guid Id { get; set; }

            public string Name { get; set; }

            public MyUser Init(string name)
            {
                this.Name = name;
                return this;
            }
        }

        static void Main()
        {
            var u = new MyUser();
            u.Init("tom");
            DbEntry.Save(u);

            Console.WriteLine("{0},{1}", u.Id, u.Name);

            var u1 = DbEntry.GetObject<MyUser>(u.Id);
            Console.WriteLine(u1);

            Console.WriteLine("{0},{1}", u.Id, u.Name);
        }
    }
}