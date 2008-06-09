using System;
using Lephone.Data.Definition;

namespace OrmA
{
    public abstract class PlayerGroup : DbObjectModel<PlayerGroup>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string Name { get; set; }

        [Length(100), AllowNull]
        public abstract string Description { get; set; }

        public abstract Time StartTime { get; set; }

        public abstract Time EndTime { get; set; }

        public abstract Guid? NewScheduleGUID { get; set; }

        public abstract bool? NewScheduleIsActive { get; set; }

        public abstract Guid? LastScheduleGUID { get; set; }

        public PlayerGroup Init(string Name, string Description,
            Time StartTime, Time EndTime,
            Guid NewScheduleGUID, bool NewScheduleIsActive,
            Guid LastScheduleGUID)
        {
            this.Name = Name;
            this.Description = Description;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.NewScheduleGUID = NewScheduleGUID;
            this.NewScheduleIsActive = NewScheduleIsActive;
            this.LastScheduleGUID = LastScheduleGUID;
            return this;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PlayerGroup g = PlayerGroup.New().Init("beijing", "", new Time(7, 30, 0),
                new Time(23, 30, 0), Guid.NewGuid(), true, Guid.NewGuid());

            g.Save();

            Console.ReadLine();
        }
    }
}
