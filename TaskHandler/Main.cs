using GTA;
using GTA.Native;
using System;
using TaskHandler;

public class Main : Script
{
    public Main()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;

        Interval = 10;
    }



    void OnTick(object sender, EventArgs e)
    {
        foreach (TaskHandlerTwo t in TaskHandlerTwo.handlers)
        {
            t.PerformTick();
        }
    }


    void OnKeyDown(object sender, EventArgs e)
    {
        KeyEventArgs pe = (KeyEventArgs)e;

        if (pe.KeyCode == Keys.F6)
        {
            Ped p = World.CreateRandomPed(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 5);
            TaskHandlerTwo t = TaskHandlerTwo.AddTaskHandler(p);
            p.AlwaysKeepTask = true;
            p.IsPersistent = false;
            p.BlockPermanentEvents = true;
            t.AddTask(Hash.SET_PED_COMBAT_ATTRIBUTES, 1, new object[] { p, 5, true });
            t.AddTask(Hash.SET_PED_COMBAT_ATTRIBUTES, 1, new object[] { p, 46, true });
            t.AddTask(Hash.TASK_COMBAT_PED, 10000, new object[] { p, Game.Player.Character });
            t.AddTask(Hash.SET_PED_COMBAT_ATTRIBUTES, 1, new object[] { p, 5, false });
            t.AddTask(Hash.SET_PED_COMBAT_ATTRIBUTES, 1, new object[] { p, 46, false });
            t.AddTask(Hash.TASK_SMART_FLEE_PED, 10000, new object[] { p, Game.Player.Character, 80f, -1, false, false });
        }
    }

}
