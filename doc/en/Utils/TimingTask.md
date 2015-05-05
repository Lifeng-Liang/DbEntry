TimingTask
==========

TimingTask is a sub component of DbEntry. It belongs to util. It provides a simple interface to develop the looped tasks.

It could use for doing task by schedule. Just like task scheduler do.

It must in a service-like program. In web development, it should in the http module to keep running.

It already has some ``Timing`` such as ``TimeSpanTiming, DayTiming, MonthTiming, WeekTiming`` in it. So normally we just need to develop the tasks.

To develop a task is very simple. Just implement the ``ITask`` interface, define what you want to do in the ``Run`` function.

The following shows the details:

````c#
public class SecendTask : ITask
{
    public void Run()
    {
        Console.WriteLine( "SecendTask Run." );
    }
}

public class FiveSecendTask : ITask
{
    public void Run()
    {
        Console.WriteLine( "FiveSecendTask Run." );
    }
}

public class DayTask : ITask
{
    public void Run()
    {
        Console.WriteLine( "DayTask Run." );
    }
}

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        TimingTask tt1 = new TimingTask( new SecendTask(),
            new TimeSpanTiming(new TimeSpan(0,0,1)) );

        TimingTask tt5 = new TimingTask( new FiveSecendTask(),
            new TimeSpanTiming(new TimeSpan(0,0,5)) );

        DateTime dt = DateTime.Now.AddSeconds(8);

        TimingTask ttd = new TimingTask( new DayTask(),
            new DayTiming(new TimeOfDayStructure(dt.Hour, dt.Minute, dt.Second)) );

        TimingTaskCollection ttc = new TimingTaskCollection( tt1, tt5, ttd );
        TimingTaskController Controller = new TimingTaskController( ttc );

        Controller.Start();

        Console.WriteLine("Press [Enter] to exit.");
        Console.ReadLine();
        Controller.Dispose();
    }
}
````

