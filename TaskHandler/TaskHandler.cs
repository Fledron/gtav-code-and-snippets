using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace TestModForTesting
{
    class TaskHandlerTwo
    {
        public Ped ped;

        public TaskHandlerTwo(Ped p)
        {
            ped = p;
        }

        private Hash TaskNow;
        private object[] ArgsNow;
        private int SequenceNow;

        public List<TaskNTimeTwo> tasks = new List<TaskNTimeTwo>();
        private static List<TaskHandlerTwo> handlers = new List<TaskHandlerTwo>();

        public static List<TaskHandlerTwo> GetTaskHandlers()
        {
            return handlers;
        }

        public static TaskHandlerTwo GetTaskHandler(Ped p)
        {
            foreach (TaskHandlerTwo t in handlers)
            {
                if (t.ped == p)
                {
                    return t;
                }
            }
            return AddTaskHandler(p);
        }

        public static TaskHandlerTwo AddTaskHandler(Ped p)
        {
            TaskHandlerTwo t = null;
            foreach (TaskHandlerTwo th in handlers)
            {
                if (th.ped == p)
                {
                    t = th;
                }
            }
            if (t == null)
            {
                t = new TaskHandlerTwo(p);
            }
            handlers.Add(t);
            return t;
        }

        public void PerformTick()
        {

            if (GetPendingTasks() > 0)
            {
                TaskNTimeTwo t = tasks[0];
                if (ArgsNow == null || TaskNow != t.Task || ArgsNow != t.Args)
                {
                    TaskNow = t.Task;
                    ArgsNow = t.Args;
                    List<InputArgument> InArgs = new List<InputArgument>();
                    foreach (object o in t.Args)
                    {
                        InArgs.Add(new InputArgument(o));
                    }
                    OutputArgument Sequence = new OutputArgument();
                    Function.Call(Hash.OPEN_SEQUENCE_TASK, Sequence);
                    int SeqID = Sequence.GetResult<int>();
                    Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, 0);
                    Function.Call(t.Task, InArgs.ToArray());
                    Function.Call(Hash.TASK_STAND_STILL, 0, 2000);
                    Function.Call(Hash.CLOSE_SEQUENCE_TASK, SeqID);
                    Function.Call(Hash.TASK_PERFORM_SEQUENCE, ped, SeqID);
                    Function.Call(Hash.CLEAR_SEQUENCE_TASK, Sequence);
                    SequenceNow = SeqID;
                    //UI.Notify("Performing " + t.Task);
                    t.StartTime = Game.GameTime;
                }
                int Prog = Function.Call<int>(Hash.GET_SEQUENCE_PROGRESS, ped);
                int RunningTime = Game.GameTime - t.StartTime;
                if ((GetRemainingTime() != -1 && GetRemainingTime() <= 0)||(t.HowLong == -1 && Prog == 2 && RunningTime>=2000))
                {
                    tasks.RemoveAt(0);
                    TaskNow = Hash.WAIT;
                    ArgsNow = null;
                }

            }
            else
            {
                //UI.ShowSubtitle("Progress: No Task");
                Function.Call(Hash.CLEAR_PED_TASKS, ped);
            }
            int Progress = Function.Call<int>(Hash.GET_SEQUENCE_PROGRESS, ped);
            //UI.ShowSubtitle(tasks.Count + " Progress: " + Progress);
        }


        public int GetEndTime()
        {
            if (GetPendingTasks() > 0)
            {
                TaskNTimeTwo t = tasks[0];
                return t.HowLong + t.StartTime;
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
                return GetTimeLeft(0);
            }
            else
            {
                return -1;
            }
        }

        public Hash GetTask(int pos)
        {
            if (tasks.Count > pos)
            {
                TaskNTimeTwo b = tasks[pos];
                return b.Task;
            }
            return tasks[tasks.Count - 1].Task;
        }

        public int GetTimeLeft(int pos)
        {
            if (tasks.Count > pos)
            {
                TaskNTimeTwo b = tasks[pos];
                if (b.StartTime != -1 && b.HowLong != -1)
                {
                    return b.HowLong - (Game.GameTime - b.StartTime);
                }
                return -1;
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
            foreach (TaskNTimeTwo tnt in tasks)
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
            tasks.Insert(pos, new TaskNTimeTwo(newTask, howLong, -1, args));
        }

        public void AddTask(Hash task, object[] args)
        {
            AddTask(task, -1, args);
        }

        public void AddTask(Hash task, int HowLong, object[] args)
        {
            AddTask(task, int.MaxValue, HowLong, args);
        }

        public void AddTask(Hash task, int pos, int HowLong, object[] args)
        {
            if (tasks.Count > pos)
            {
                tasks.Insert(pos, new TaskNTimeTwo(task, HowLong, -1, args));
            }
            else
            {
                tasks.Add(new TaskNTimeTwo(task, HowLong, -1, args));
            }
        }

        public void ClearTasks()
        {
            tasks.Clear();
        }

    }

    internal class TaskNTimeTwo
    {
        public Hash Task { get; set; }
        public object[] Args { get; set; }
        public int StartTime { get; set; }
        public int HowLong { get; set; }
        public TaskNTimeTwo(Hash Task, int HowLong, int StartTime, object[] Args)
        {
            this.Task = Task;
            this.HowLong = HowLong;
            this.StartTime = StartTime;
            this.Args = Args;

        }

    }
}
