using static Tera.Connection.Dispatcher.PacketStruct;
using Tera.Connection.Dispatcher;

namespace Tera.Mods.Controlers
{
    public interface ILoader
    {
        public void Load();
        public void Route(Dispatch handler, ref Packet packet);
    }
}