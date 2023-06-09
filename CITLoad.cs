using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace _20230509
{
    #region IT8700命令清单
    //\r\n
    //SYS:REM
    //*RST
    //*CLS
    //*SRE 0
    //*ESE 0
    //CHAN 1
    //FUNC CURR
    //CURR 0.5
    //INP 1
    //INP 0
    //INP:ALL 1
    //INP:ALL 0
    //CHAN 2
    //FUNC RES
    //RES 100
    //MEAS:VOLT?
    //MEAS:CURR?
    //MEAS:POW?
    #endregion

    public class CPLZ50FLoad
    {
        private string inpon = "INP 1\r\n";
        private string inpoff = "INP 0\r\n";
        private string cls = "*CLS\r\n";
        private string systrem = "SYST:REM\r\n";
        private string rst = "*RST\r\n";
        private string sre = "*SRE 0\r\n";
        private string ese = "*ESE 0\r\n";
        private string measvolt = "MEAS:VOLT?\r\n";
        private string meascurr = "MEAS:CURR?\r\n";
        private string measpow = "MEAS:POW?\r\n";

        //以下指令在PLZ50-F.pdf的109页开始

        //INST CH1      选择CH1通道
        //INST:COUP CH1,CH3     选择CH1和CH3通道
        //
        //MEAS:CURR?        读取电流
        //MEAS:POW?         读取功率
        //MEAS:VOLT？       读取电压
        //MEAS:ETIM         读取测量间隔时间
        //COND  10        COND?         设置恒电阻输出
        //CURR 0.1        CURR?         设置恒流输出
        //VOLT 3          VOLT?         设置恒压输出
        //
        //
        //

        //DISP:MET 0        0:电压和电流; 1:电压和功率; 2:功率和电流；3；4；
        //
        //INP ON   
        //INP OFF
        //
        //
        //


        /// </summary>
        private SerialPort sp = new SerialPort();
        private bool isOpen = false;

        public bool IsOpen
        {
            get { return isOpen; }
        }

        public CPLZ50FLoad()
        {
        }

        public bool Init(string com, int baud)
        {
            try
            {
                if (this.sp.IsOpen)
                    this.sp.Close();
                this.sp.PortName = com;
                this.sp.BaudRate = baud;
                this.sp.Parity = System.IO.Ports.Parity.None;
                this.sp.DataBits = 8;
                this.sp.StopBits = System.IO.Ports.StopBits.One;
                this.sp.DataReceived += sp_DataReceived;
                this.sp.ReadTimeout = 1000;
                this.sp.WriteTimeout = 1000;

                this.sp.Open();
                this.isOpen = true;

                this.Reset();
                return true;
            }
            catch
            {
                MessageBox.Show("精密电源通讯口初始化失败！检查串口是否存在，串口名是否正确！", "错误提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }


        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        public void SystRem()
        {
            if (this.sp.IsOpen)
                this.sp.Write(systrem);
            System.Threading.Thread.Sleep(20);
        }
        private void CLS()
        {
            if (this.sp.IsOpen)
                this.sp.Write(cls);
            System.Threading.Thread.Sleep(20);
        }

        private void RST()
        {
            if (this.sp.IsOpen)
                this.sp.Write(rst);
            System.Threading.Thread.Sleep(20);
        }
        private void SRE()
        {
            if (this.sp.IsOpen)
                this.sp.Write(sre);
            System.Threading.Thread.Sleep(20);
        }
        private void ESE()
        {
            if (this.sp.IsOpen)
                this.sp.Write(ese);
            System.Threading.Thread.Sleep(20);
        }
        public void Reset()
        {
            this.SystRem();
            this.RST();
            this.CLS();
            this.SRE();
            this.ESE();
            selet_ALL();
        }

        private void selet_ALL()
        {
            string chan = string.Format("INST:COUP ALL\r\n");
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(50);
        }

        public void OutPutOn(byte ch)
        {
            string chan = string.Format("INST CH{0}\r\n", ch);
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);
            chan = string.Format("INST:COUP CH{0}\r\n", ch);
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);
            string LOAD = string.Format("INP ON\r\n");
            this.sp.Write(LOAD);
            System.Threading.Thread.Sleep(50);
        }
        public void OutPutOnALL()
        {
            string chan = string.Format("INST:COUP ALL\r\n");
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);

            string LOAD = string.Format("INP ON\r\n");
            this.sp.Write(LOAD);
            System.Threading.Thread.Sleep(50);
        }
        public void OutPutOff(byte ch)
        {
            string chan = string.Format("INST CH{0}\r\n", ch);
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);
            chan = string.Format("INST:COUP CH{0}\r\n", ch);
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);
            string LOAD = string.Format("INP OFF\r\n");
            this.sp.Write(LOAD);
            System.Threading.Thread.Sleep(50);
        }

        public void OutPutOffALL()
        {
            string chan = string.Format("INST:COUP ALL\r\n");
            this.sp.Write(chan);
            System.Threading.Thread.Sleep(100);
            string LOAD = string.Format("INP OFF\r\n");
            this.sp.Write(LOAD);
            System.Threading.Thread.Sleep(50);
        }

        public void CurrFunction(byte ch, double currSet)
        {
            if (this.sp.IsOpen)
            {
                string chan = string.Format("INST CH{0}\r\n", ch);
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(100);
                string func = "FUNC CC\r\n";
                this.sp.Write(func);
                System.Threading.Thread.Sleep(100);
                string curr = string.Format("CURR {0:f3}\r\n", currSet);
                this.sp.Write(curr);
                System.Threading.Thread.Sleep(100);
            }
        }
        public void ResFunction(byte ch, double resSet)
        {
            if (this.sp.IsOpen)
            {
                string chan = string.Format("INST CH{0}\r\n", ch);
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(100);
                string func = "FUNC CR\r\n";
                this.sp.Write(func);
                System.Threading.Thread.Sleep(100);
                string curr = string.Format("COND {0:f3}\r\n", resSet);
                this.sp.Write(curr);
                System.Threading.Thread.Sleep(100);
            }
        }

        public void VoltFunction(byte ch, double volSet)
        {
            if (this.sp.IsOpen)
            {
                string chan1 = string.Format("INST:COUP ALL\r\n");
                this.sp.Write(chan1);
                System.Threading.Thread.Sleep(100);
                string chan = string.Format("INST CH{0}\r\n", ch);
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(100);
                chan = string.Format("INST:COUP CH{0}\r\n", ch);
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(100);
                string func = "FUNC CV\r\n";
                this.sp.Write(func);
                System.Threading.Thread.Sleep(100);
                string curr = string.Format("VOLT {0:f3}\r\n", volSet);
                this.sp.Write(curr);
                System.Threading.Thread.Sleep(50);
            }
        }
        public void OutputAll(bool flag)
        {
            if (this.sp.IsOpen)
            {
                if (flag)
                {
                    OutPutOnALL();
                }
                else
                {
                    OutPutOffALL();
                }
            }
        }

        public void ApplVolt(double voltT, double currT)
        {
            if (this.sp.IsOpen)
            {
                string strvol = string.Format("APPL {0:f3},{1:f3}\r\n", voltT, currT);
                this.sp.Write(systrem);
            }
        }
        public bool GetCurr(byte ch, out double curr)
        {
            if (sp.IsOpen)
            {
                byte[] buffer = new byte[sp.BytesToRead];
                bool bFlag = false;
                this.sp.Read(buffer, 0, buffer.Length);
                //选择通道
                string chan = string.Format("INST:COUP ALL\r\n");
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(50);
                chan = string.Format("INST CH{0}\r\n", ch);
                this.sp.Write(chan);
                System.Threading.Thread.Sleep(50);
                this.sp.Write(string.Format("INST:COUP CH{0}\r\n", ch));
                System.Threading.Thread.Sleep(50);
                //string chan = string.Format("INST:COUP CH{0}\r\n", ch);
                //this.sp.Write(chan);
                //this.sp.Read(buffer, 0, buffer.Length);
                //读取电压
                this.sp.Write(meascurr);
                for (int i = 0; i < 30; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    if (sp.BytesToRead >= 1)
                    {
                        System.Threading.Thread.Sleep(100);
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag)
                {
                    buffer = null;
                    buffer = new byte[sp.BytesToRead];
                    sp.Read(buffer, 0, buffer.Length);
                    string strRecData = System.Text.Encoding.Default.GetString(buffer);
                    try
                    {
                        curr = Convert.ToDouble(strRecData);
                        return true;
                    }
                    catch { }
                }
                else
                {
                    buffer = null;
                    buffer = new byte[sp.BytesToRead];
                    sp.Read(buffer, 0, buffer.Length);
                    string strRec = System.Text.Encoding.ASCII.GetString(buffer);
                }
            }
            curr = -20.00;
            return false;
        }
        public bool GetVolt(byte ch, out double volt)
        {
            if (sp.IsOpen)
            {
                byte[] buffer = new byte[sp.BytesToRead];
                bool bFlag = false;
                this.sp.Read(buffer, 0, buffer.Length);
                //选择通道
                this.sp.Write(string.Format("INST {0}\r\n", ch));
                System.Threading.Thread.Sleep(100);
                //this.sp.Read(buffer, 0, buffer.Length);
                //读取电压
                this.sp.Write(measvolt);
                for (int i = 0; i < 30; i++)
                {
                    System.Threading.Thread.Sleep(1);
                    if (sp.BytesToRead >= 1)
                    {
                        System.Threading.Thread.Sleep(100);
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag)
                {
                    buffer = null;
                    buffer = new byte[sp.BytesToRead];
                    sp.Read(buffer, 0, buffer.Length);
                    string strRecData = System.Text.Encoding.ASCII.GetString(buffer);
                    //volt = -20;
                    //return true;
                    try
                    {
                        volt = Convert.ToDouble(strRecData);
                        return true;
                    }
                    catch { }
                }
                else
                {
                    buffer = null;
                    buffer = new byte[sp.BytesToRead];
                    sp.Read(buffer, 0, buffer.Length);
                    string strRec = System.Text.Encoding.ASCII.GetString(buffer);
                }
            }
            volt = -20.00;
            return false;
        }
        public bool GetPow(out double pow)
        {
            if (sp.IsOpen)
            {
                byte[] buffer = new byte[sp.BytesToRead];
                bool bFlag = false;
                this.sp.Read(buffer, 0, buffer.Length);
                this.sp.Write(measpow);
                for (int i = 0; i < 30; i++)
                {
                    System.Threading.Thread.Sleep(1);
                    if (sp.BytesToRead >= 8)
                    {
                        bFlag = true;
                        break;
                    }
                }
                if (bFlag)
                {
                    buffer = null;
                    buffer = new byte[sp.BytesToRead];
                    sp.Read(buffer, 0, buffer.Length);
                    pow = Convert.ToDouble(buffer[buffer.Length - 1]);
                    return true;
                }
            }
            pow = -20.00;
            return false;
        }

    }
}
