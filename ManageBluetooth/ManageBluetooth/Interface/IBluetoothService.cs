using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageBluetooth.Interface
{
    public interface IBluetoothService
    {
        bool IsBluetoothEnabled();
        void ChangeBluetoothState();
        Task<IEnumerable<string>> GetConnectedOrKnowBluetoothDevices();
    }
}
