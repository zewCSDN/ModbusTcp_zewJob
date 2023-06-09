using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

using _20230509;
namespace _20230509
{
    #region IT6722A命令清单
    //\r\n
    //SYS:REM
    //APPL 13.5,18
    //OUTP 1
    //OUTP 0
    //MEAS:CURR?
    //MEAS:VOLT?
    //MEAS:POW?
    //*CLS清除错误
    #endregion


    public class CITPWR
    {
        private string meascurr = "MEAS:CURR?\r\n";
        private string measvolt = "MEAS:VOLT?\r\n";
        private string measpow = "MEAS:POW?\r\n";
        private string outpon = "OUTP 1\r\n";
        private string outpoff = "OUTP 0\r\n";
        private string cls = "*CLS\r\n";
        private string systrem = "SYST:REM\r\n";


        /// </summary>
        private SerialPort sp = new SerialPort();
        private bool isOpen = false;

        public bool IsOpen
        {
            get { return isOpen; }
        }

        public CITPWR()
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
            //byte[] buffer = new byte[sp.BytesToRead];
            //this.sp.Read(buffer, 0, buffer.Length);

        }

        private void SystRem()
        {
            if (this.sp.IsOpen)
                this.sp.Write(systrem);
        }
        private void CLS()
        {
            if (this.sp.IsOpen)
                this.sp.Write(cls);
        }
        public void Reset()
        {
            this.SystRem();
            System.Threading.Thread.Sleep(20);
            this.SystRem();
            System.Threading.Thread.Sleep(20);
            this.CLS();
            System.Threading.Thread.Sleep(20);
        }
        public void OutPut(bool on)
        {
            if (this.sp.IsOpen)
            {
                if (on)
                    this.sp.Write(outpon);
                else
                    this.sp.Write(outpoff);
            }
        }
        public returnCode ApplVolt(double voltT, double currT)
        {
            if (this.sp.IsOpen)
            {
                string strvol = string.Format("APPL {0:f3},{1:f3}\r\n", voltT, currT);
                this.sp.Write(strvol);
                return returnCode.success;
            }
            else
                return returnCode.errOpenError;
        }
        public bool GetCurr(out double curr)
        {
            if (sp.IsOpen)
            {
                //连续读取5次，只要成功读取那么就返回true
                for (int time = 0; time < 5; time++)
                {
                    byte[] buffer = new byte[sp.BytesToRead];
                    bool bFlag = false;
                    this.sp.Read(buffer, 0, buffer.Length);
                    this.sp.Write(meascurr);
                    for (int i = 0; i < 30; i++)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (sp.BytesToRead >= 2)
                        {
                            bFlag = true;
                            break;
                        }
                    }
                    if (bFlag)
                    {
                        System.Threading.Thread.Sleep(20);
                        buffer = null;
                        buffer = new byte[sp.BytesToRead];
                        sp.Read(buffer, 0, buffer.Length);
                        string strRecData = System.Text.Encoding.Default.GetString(buffer);
                        curr = Convert.ToDouble(strRecData);
                        return true;
                    }
                }
            }
            curr = -20.00;
            return false;
        }
        public bool GetVolt(out double volt)
        {
            if (sp.IsOpen)
            {
                for (int time = 0; time < 5; time++)
                {
                    byte[] buffer = new byte[sp.BytesToRead];
                    bool bFlag = false;
                    this.sp.Read(buffer, 0, buffer.Length);
                    this.sp.Write(measvolt);
                    for (int i = 0; i < 30; i++)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (sp.BytesToRead >= 6)
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
                        volt = Convert.ToDouble(buffer[buffer.Length - 1]);
                        return true;
                    }
                }
            }
            volt = -20.00;
            return false;
        }
        public bool GetPow(out double pow)
        {
            if (sp.IsOpen)
            {
                for (int time = 0; time < 5; time++)
                {
                    byte[] buffer = new byte[sp.BytesToRead];
                    bool bFlag = false;
                    this.sp.Read(buffer, 0, buffer.Length);
                    this.sp.Write(measpow);
                    for (int i = 0; i < 30; i++)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (sp.BytesToRead >= 6)
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
            }
            pow = -20.00;
            return false;
        }

    }

}
