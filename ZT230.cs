using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace MySRHMethod
{
    public class ZT230
    {
        private SerialPort sp = new SerialPort();

        public ZT230(string com, int baud)
        {
            sp.PortName = com;
            sp.BaudRate = baud;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.ReadTimeout = 1000;
            sp.WriteTimeout = 1000;
            sp.ReadBufferSize = 1024;
            sp.WriteBufferSize = 1024;

            try
            {
                sp.Close();
                sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口打开失败，异常信息为：" + ex.Message);
            }
        }


        public ZT230()
        {
            // TODO: Complete member initialization
        }

        ~ZT230()
        {
            if (sp.IsOpen)
                sp.Close();
        }

        public bool Init(string com, int baud)
        {
            sp.PortName = string.Format("{0}", com);
            sp.BaudRate = baud;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.ReadTimeout = 1000;
            sp.WriteTimeout = 1000;
            sp.ReadBufferSize = 1024;
            sp.WriteBufferSize = 1024;

            try
            {
                sp.Close();
                sp.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口打开失败，异常信息为：" + ex.Message);
            }
            return false;
        }

        public bool IsOpen()
        {
            return sp.IsOpen;
        }

        public bool Close()
        {
            if (sp.IsOpen)
                sp.Close();
            return true;
        }
       
        public bool PrintBarCode(string strSeq)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("^XZ\r\n");
            //sb.Append("^DFR:SAMPLE.GRF^FS\r\n");
            //sb.Append("^FO20,30^GB750,1100,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,200,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,400,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,700,4^FS\r\n");
            //sb.Append("^FO20,226^GB325,204,4^FS\r\n");
            //sb.Append("^FO30,40^ADN,36,20^FDShip to:^FS\r\n");
            //sb.Append("^FO30,260^ADN,18,10^FDPart number #^FS\r\n");
            //sb.Append("^FO360,260^ADN,18,10^FDDescription:^FS\r\n");
            //sb.Append("^FO30,750^ADN,36,20^FDFrom:^FS\r\n");
            //sb.Append("^FO150,125^ADN,36,20^FN1^FS (ship to)\r\n");
            //sb.Append("^FO60,330^ADN,36,20^FN2^FS(part num)\r\n");
            //sb.Append("^FO400,330^ADN,36,20^FN3^FS(description)\r\n");
            //sb.Append("^FO70,480^BY4^B3N,,200^FN4^FS(barcode)\r\n");
            //sb.Append("^FO150,800^ADN,36,20^FN5^FS (from)\r\n");
            //sb.Append("^XZ\r\n");
            //sb.Append("^XA\r\n");
            //sb.Append("^FO150,50\r\n");
            //sb.Append("^B8N,100,Y,N\r\n");
            //sb.Append(string.Format("^FD{0}^FS\r\n", strSeq));
            //sb.Append("^FO150,250\r\n");
            //sb.Append("^ADN,36,20\r\n");
            //sb.Append(string.Format("^FD{0}^FS\r\n", strSeq));
            //sb.Append("^XZ\r\n");
            string sb = "^XA\r\n" + "^FO150,50\r\n" +
                "^B8N,100,Y,N\r\n" +
                string.Format("^FD{0}^FS\r\n", strSeq) +
                "^FO150,250\r\n" +
                "^ADN,36,20\r\n" +
                string.Format("^FD{0}^FS\r\n", strSeq) +
                "^XZ\r\n";
            if (sp.IsOpen)
            {
                char[] cmd = sb.ToCharArray();
                sp.Write(cmd, 0, cmd.Length);
            }
            return true;
        }

        public bool PrintTSL(string code, string partnum, string version, string supply, string date, string seq)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("^XZ\r\n");
            //sb.Append("^DFR:SAMPLE.GRF^FS\r\n");
            //sb.Append("^FO20,30^GB750,1100,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,200,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,400,4^FS\r\n");
            //sb.Append("^FO20,30^GB750,700,4^FS\r\n");
            //sb.Append("^FO20,226^GB325,204,4^FS\r\n");
            //sb.Append("^FO30,40^ADN,36,20^FDShip to:^FS\r\n");
            //sb.Append("^FO30,260^ADN,18,10^FDPart number #^FS\r\n");
            //sb.Append("^FO360,260^ADN,18,10^FDDescription:^FS\r\n");
            //sb.Append("^FO30,750^ADN,36,20^FDFrom:^FS\r\n");
            //sb.Append("^FO150,125^ADN,36,20^FN1^FS (ship to)\r\n");
            //sb.Append("^FO60,330^ADN,36,20^FN2^FS(part num)\r\n");
            //sb.Append("^FO400,330^ADN,36,20^FN3^FS(description)\r\n");
            //sb.Append("^FO70,480^BY4^B3N,,200^FN4^FS(barcode)\r\n");
            //sb.Append("^FO150,800^ADN,36,20^FN5^FS (from)\r\n");
            //sb.Append("^XZ\r\n");
            //sb.Append("^XA\r\n");
            //sb.Append("^FO150,50\r\n");
            //sb.Append("^B8N,100,Y,N\r\n");
            //sb.Append(string.Format("^FD{0}^FS\r\n", strSeq));
            //sb.Append("^FO150,250\r\n");
            //sb.Append("^ADN,36,20\r\n");
            //sb.Append(string.Format("^FD{0}^FS\r\n", strSeq));
            //sb.Append("^XZ\r\n");
            string sb = "^XA\r\n" + "^FO150,50\r\n" +
                "^B8N,100,Y,N\r\n" +
                string.Format("^FD{0}^FS\r\n", code) +
                "^FO150,250\r\n" +
                "^ADN,36,20\r\n" +
                string.Format("^FD{0}^FS\r\n", code) +
                "^XZ\r\n";
            //if (sp.IsOpen)
            //{
            //    char[] cmd = sb.ToCharArray();
            //    sp.Write(cmd, 0, cmd.Length);
            //}


//^XA
//^FO100,100^BY3
//^BCN,100,Y,N,N            //此处的Y如果改为N，那么可能标签下面就不显示标签字符了
//^FD123456^FS
//^XZ
            sb = "^XA\r\n" +
                 "^FO150,250^ADN,36,20^FD" + string.Format("{0}^FS\r\n", partnum) +
                 "^FO150,250^ADN,36,20^FD" + string.Format("{0}^FS\r\n", version) +
                 "^FO150,250^ADN,36,20^FD" + string.Format("{0}^FS\r\n", supply) +
                 "^FO150,250^ADN,36,20^FD" + string.Format("{0}^FS\r\n", date) +
                 "^FO150,250^ADN,36,20^FD" + string.Format("{0}^FS\r\n", seq) +
                 "^FO150,250^BY3\r\n^BCN,100,N,N,N\r\n^FD" + string.Format("{0}^FS\r\n", code) +
                 "^XZ\r\n";
            if (sp.IsOpen)
            {
                char[] cmd = sb.ToCharArray();
                sp.Write(cmd, 0, cmd.Length);
            }

            return true;
        }
        public bool PrintTSL(string cmdCode)
        {
            if (sp.IsOpen)
            {
                char[] cmd = cmdCode.ToCharArray();
                sp.Write(cmd, 0, cmd.Length);
            }

            return true;
        }

    }
}
