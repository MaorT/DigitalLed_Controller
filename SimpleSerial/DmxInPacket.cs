using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSerial
{
    class DmxInPacket
    {
        public DmxInPacket()
        {
            DmxData = new byte[512];
        }

        public int Universe { get; set; }
        public byte[] DmxData { get; set; }
        public SupportedTypes Protocol { get; set; }
    }

    public enum SupportedTypes
    {
        ArtNet,
        PathPort,
        sAcn,
        EnttecUsbPro
    }
}
