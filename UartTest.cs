using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySRHMethod
{
    public class UartTest
    {

        private SerialPort serialPort = null;
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public int DataBits { get; set; }

        byte[] bData= new byte[1024];
        byte myReceiveByte;
        int myReceiveBytecount=0;

        /////*********例子************
        ///// ModbusHelper.ModbusRTU modbusRTU;
        //modbusRTU = new ModbusRTU()
        //{
        //    PortName = "COM30", BaudRate = 9600, Parity = Parity.None, DataBits = 8, StopBits = StopBits.One
        //   };
        //modbusRTU.Connect(modbusRTU);
        // byte[] receive = modbusRTU.ReadKeepReg(1, 0, 1);
     

        /////********************************************ModbusRTU*****************************
        /// <summary>
        /// ModubsRTU 连接 打开串口
        /// </summary>
        /// <param name="commSet">通讯参数设置</param>
        public bool Connect(UartTest commSet)
        {
            try
            {

                this.serialPort = new SerialPort()
                {
                    PortName = commSet.PortName,
                    BaudRate = commSet.BaudRate,
                    Parity = commSet.Parity,
                    DataBits = commSet.DataBits,
                    StopBits = commSet.StopBits
                   
                    
            };
                serialPort.ReceivedBytesThreshold = 1;
          
                serialPort.Open();
                return true;


            }
            catch (Exception ex)
            {
            MessageBox.Show(ex.Message);
                return false;
            }

           



        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
        

            //while (this.serialPort.BytesToRead > 0)
            //{
            //    byte[] ReceiveBytes = new byte[serialPort.BytesToRead];
            //    myReceiveByte=(byte)this.serialPort.ReadByte();
            //    myReceiveBytecount += 1;
            //    if (myReceiveBytecount>=1024)
            //    {
            //        myReceiveBytecount = 0;
            //        this.serialPort.DiscardInBuffer();
            //        return;
            //    }

            //}
        }

      
        public void Disconnect()
        {

            if (this.serialPort != null)
            {
                this.serialPort.Close();

            }

        }
        


     /// <summary>
     /// 传入数据
     /// </summary>
     /// <param name="id"></param>
     /// <param name="data"></param>
     /// <returns></returns>
        public byte[] Put( byte[] data)
        {
            List<byte> list = new List<byte>();
         
           list.AddRange(data);
            try
            {
                if (data!=null)
                {
                   this.serialPort.Write(list.ToArray(), 0,list.Count); //01 06 00 33 00 02 f8 04
                    Console.WriteLine("Uart TXT: {0}", ToHexStrFromByte(data));
                    System.Threading.Thread.Sleep(200);
                //接受报文
                byte[] ReceiveBytes = new byte[serialPort.BytesToRead];//01 03 00 1f 00 01 b5 cc
                this.serialPort.Read(ReceiveBytes, 0, ReceiveBytes.Length);
                    Console.WriteLine("Uart Recive: {0}",ToHexStrFromByte(ReceiveBytes));
                    return ReceiveBytes;
                }
             else { return null; }  
            }
            catch (Exception)
            {

                return null;
            }
         

        }



        /// <summary>
        /// 字节数组转16进制字符串：空格分隔
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }







    }
}

/////******************************************END ModbusRTU*******************************


