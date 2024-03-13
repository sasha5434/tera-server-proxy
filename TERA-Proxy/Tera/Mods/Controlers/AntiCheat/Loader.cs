using static Tera.Connection.Dispatcher.PacketStruct;
using Tera.Connection.Dispatcher;
using Tera.Mods.Controlers.AntiCheat.AutoLocks;
using Tera.Connection;
using System;
namespace Tera.Mods.Controlers.AntiCheat;


public class Loader : ILoader
{
    public AutoLock autoLockInstance = new AutoLock();
    public Loader()
    {

    }
    public void Load()
    {
        if (!AutoLock.Stopwatch.IsRunning) AutoLock.Stopwatch.Start();
    }
    public void Route(Dispatch handler, ref Packet packet)
    {
        switch(packet.name)
        {
            case "C_CAN_LOCKON_TARGET":
                C_CAN_LOCKON_TARGET c_CAN_LOCKON_TARGET = new C_CAN_LOCKON_TARGET(ref packet);
                autoLockInstance.cCanLockonTarget_handler(c_CAN_LOCKON_TARGET, ref packet);
                break;
            case "S_ACTION_STAGE":

                break;
            case "S_ACTION_END":

                break;
            default:
                break;
        }
    }

    public static void AntiCheatLogger(string msg, UserData userData)
    {
        Console.WriteLine($"AntiCheat - triggered : {msg} - { userData.User.Character.PlayerId } - { userData.User.Character.Name }");
    }
}