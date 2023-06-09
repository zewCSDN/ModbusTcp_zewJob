using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySRHMethod
{
    public class MasterRtu
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
        public bool Connect(MasterRtu commSet)
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
             //   serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                return true;


            }
            catch (Exception)
            {

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
        /// 写单个06保持性寄存器
        /// </summary>
        /// <param name="salveID">站地址</param>
        /// <param name="address">从站地址</param>
        /// <param name="length">长度</param>
        public bool WriteKeepSingleReg(int salveID, int startAddress, ushort value)
        {

            List<byte> SendCommand = new List<byte>();
            int byteLength = 8;//字节长度
            //拼接第一个字节
            SendCommand.Add((byte)salveID);
            //拼接第二个字节
            SendCommand.Add(6);
            //拼接第三四个字节
            SendCommand.Add((byte)(startAddress / 256));
            SendCommand.Add((byte)(startAddress % 256));
            //拼接第五六个字节 凭借属猪
            
            SendCommand.Add((byte)(value / 256));
            SendCommand.Add((byte)(value % 256));
            //crc校验
          //  byte[] crc = CalculateCRC(SendCommand.ToArray(), 6);
            byte[] crc = ToModbus(SendCommand.ToArray());
            //加入CRC
            SendCommand.AddRange(crc);
            //发送报文
            this.serialPort.Write(SendCommand.ToArray(), 0, SendCommand.Count); //01 06 00 33 00 02 f8 04

            System.Threading.Thread.Sleep(100);
            //接受报文
            byte[] ReceiveBytes = new byte[serialPort.BytesToRead];
            this.serialPort.Read(ReceiveBytes, 0, ReceiveBytes.Length);
          //  解析报文


            if (ReceiveBytes.Length == byteLength && ReceiveBytes[1] == 0X06)
            {
                //byte[] result = new byte[2];//去除地址连个字节
                //Array.Copy(ReceiveBytes, 4, result, 0, 2);
                return true;

            }
            return false; 


        }





        /// <summary>
        /// 读取03保持性寄存器
        /// </summary>
        /// <param name="salveID">站地址</param>
        /// <param name="address">从站地址</param>
        /// <param name="length">长度</param>
        public byte[] ReadKeepReg(int salveID, int startAddress, int length)
        {

            List<byte> SendCommand = new List<byte>();
            int byteLength = length * 2;
            //拼接第一个字节
            SendCommand.Add(1);
            //拼接第二个字节
            SendCommand.Add(3);
            //拼接第三四个字节
            SendCommand.Add((byte)(startAddress / 256));
            SendCommand.Add((byte)(startAddress % 256));
            //拼接第五六个字节
            SendCommand.Add((byte)(length / 256));
            SendCommand.Add((byte)(length % 256));
            //crc校验
          //  byte[] crc = CalculateCRC(SendCommand.ToArray(), 6);
            byte[] crc = ToModbus(SendCommand.ToArray());
            //加入CRC
            SendCommand.AddRange(crc);
            //发送报文
            this.serialPort.Write(SendCommand.ToArray(), 0, SendCommand.Count);//01 03 00 22 00 01 24 00

            System.Threading.Thread.Sleep(120);
            //接受报文
            byte[] ReceiveBytes = new byte[serialPort.BytesToRead];//01 03 00 1f 00 01 b5 cc
            this.serialPort.Read(ReceiveBytes, 0, ReceiveBytes.Length);
            //解析报文
      
            if (ReceiveBytes.Length == length * 2 + 5 && ReceiveBytes[0] == 0X01)
            {
                byte[] result = new byte[length * 2];
                Array.Copy(ReceiveBytes, 3, result, 0, byteLength);
                return result;

            }
            else { return null; }


        }


        #region 校验
        private static readonly byte[] aucCRCHi = {
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
             0x00, 0xC1, 0x81, 0x40
         };
        private static readonly byte[] aucCRCLo = {
             0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
             0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
             0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
             0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
             0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
             0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
             0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
             0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
             0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
             0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
             0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
             0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
             0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
             0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
             0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
             0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
             0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
             0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
             0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
             0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
             0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
             0x41, 0x81, 0x80, 0x40
         };

        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="pucFrame">字节数组</param>
        /// <param name="usLen">验证长度</param>
        /// <returns>2个字节</returns>
        private static byte[] CalculateCRC(byte[] pucFrame, int usLen)
        {
            int i = 0;
            byte[] res = new byte[2] { 0xFF, 0xFF };
            ushort iIndex;
            while (usLen-- > 0)
            {
                iIndex = (ushort)(res[0] ^ pucFrame[i++]);
                res[0] = (byte)(res[1] ^ aucCRCHi[iIndex]);
                res[1] = aucCRCLo[iIndex];
            }
            return res;
        }

        #endregion


        public static byte[] ToModbus(byte[] byteData)
        {
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < byteData.Length; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;//异或多项式
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);//高位在后
            CRC[0] = (byte)(wCrc & 0x00FF);       //低位在前
            return CRC;

        }










    }
}

/////******************************************END ModbusRTU*******************************


