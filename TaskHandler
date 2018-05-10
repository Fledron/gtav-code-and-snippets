using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace TestModForTesting
{
    class TaskHandler
    {
        Ped ped;

        public TaskHandler(Ped p)
        {
            ped = p;
        }

        Hash TaskNow;
        object[] ArgsNow;

        List<TaskNTime> tasks = new List<TaskNTime>();
        public static List<TaskHandler> handlers = new List<TaskHandler>();



        public void PerformTick()
        {
            if (GetPendingTasks() > 0)
            {
                TaskNTime t = tasks[0];
                if (ArgsNow == null || TaskNow != t.Task || ArgsNow != t.Args)
                {
                    TaskNow = t.Task;
                    ArgsNow = t.Args;
                    List<InputArgument> InArgs = new List<InputArgument>();
                    foreach (object o in t.Args)
                    {
                        InArgs.Add(new InputArgument(o));
                    }
                    Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, ped);
                    Function.Call(t.Task, InArgs.ToArray());
                    //UI.Notify("Performing "+t.Task.ToString());
                }
                if (Game.GameTime >= t.EndTime && t.EndTime != -1)
                {
                    tasks.RemoveAt(0);
                    TaskNow = Hash.WAIT;
                    ArgsNow = null;
                }
            }
        }


        public int GetEndTime()
        {
            if (GetPendingTasks() > 0)
            {
                TaskNTime t = tasks[0];
                return t.EndTime;
            }
            else
            {
                return -1;
            }
        }

        public int GetRemainingTime()
        {
            if (GetPendingTasks() > 0)
            {
                TaskNTime t = tasks[0];
                return t.EndTime - Game.GameTime;
            }
            else
            {
                return -1;
            }
        }

        public TaskNTime GetCurrentTask()
        {
            return new TaskNTime(TaskNow, GetRemainingTime(), ArgsNow);
        }

        public Hash GetTask(int pos)
        {
            if (tasks.Count > pos)
            {
                TaskNTime b = tasks[pos];
                return b.Task;
            }
            return tasks[tasks.Count - 1].Task;
        }

        public double GetTimeLeft(int pos)
        {
            if (tasks.Count > pos)
            {
                TaskNTime b = tasks[pos];
                return b.EndTime - Game.GameTime;
            }
            return -1;
        }

        public int GetPendingTasks()
        {
            return tasks.Count;
        }

        public Hash[] GetAllTasksAsArray()
        {
            Hash[] t = { };
            foreach (TaskNTime tnt in tasks)
            {
                t[t.Length - 1] = tnt.Task;
            }
            return t;
        }

        public void ChangeTask(int pos, Hash newTask, object[] args)
        {
            ChangeTask(pos, newTask, -1, args);
        }

        public void ChangeTask(int pos, Hash newTask, int howLong, object[] args)
        {
            tasks.RemoveAt(pos);
            tasks.Insert(pos, new TaskNTime(newTask, Game.GameTime + howLong, args));
        }

        public void AddTask(Hash task, object[] args)
        {
            AddTask(task, -1, args);
        }

        public void AddTask(Hash task, int howLong, object[] args)
        {
            AddTask(task, int.MaxValue, howLong, args);
        }

        public void AddTask(Hash task, int pos, int HowLong, object[] args)
        {
            if (tasks.Count > pos)
            {
                tasks.Insert(pos, new TaskNTime(task, Game.GameTime + HowLong, args));
            }
            else
            {
                tasks.Add(new TaskNTime(task, Game.GameTime + HowLong, args));
            }
        }


    }

    class TaskNTime
    {
        public Hash Task { get; set; }
        public object[] Args { get; set; }
        public int EndTime { get; set; }
        public TaskNTime(Hash Task, int EndTime, object[] Args)
        {
            this.Task = Task;
            this.EndTime = EndTime;
            this.Args = Args;
        }

    }
}
