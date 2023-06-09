using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20230509.MySRHMethod
{
    public class PLC_Variable
    {
        public PLC_Variable() { }
        public ushort PLC_Heart { get; set; }
        public ushort PropSelection { get; set; }//产品选择
        public bool ConState { get; set; }
        public ushort Station1_StartSinal { get; set; }
        public ushort Station2_StartSinal { get; set; }
        public ushort Station3_StartSinal { get; set; }
        public ushort Station4_StartSinal { get; set; }

        public string[] SN =new string[4];
        public byte[] Station1_EndState { get; set; }
        public byte[] Station2_EndState { get; set; }
        public byte[] Station3_EndState { get; set; }
        public byte[] Station4_EndState { get; set; }
     //   public byte[] PLC_Heart { get; set; }
    }
}
