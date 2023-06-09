#define LIN_MASTER_TEST
//#define LIN_SLAVE_TEST


using _20230509.MySRHMethod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using USB2XXX;


namespace MySRHMethod
{
    public class UTA0402_lin
    {
        public Int32 DevHandle = 0;    //设备赋值
        public Byte CAN_CH1 = 0;       //CAN通道1
        public Byte CAN_CH2 = 1;       //CAN通道2

        public object locker = new object();
        //读取CAN数据线程标志位
        public bool CANReadMsgFlag = false;
        //读取CAN数据线程
        public Thread CANReadMsgThread = null;

        //LIN通道发送数据线程
        //public Thread LIN1SendMsgThread = null;

        public static bool[] LIN_SendMsgFlag = new bool[2] { false, false };
        public static bool[] LIN_SendMsgFlagContinue = new bool[2] { false, false };
        public static String[] MSGTypeStr = new String[10] { "UN", "MW", "MR", "SW", "SR", "BK", "SY", "ID", "DT", "CK" };
        public static String[] CKTypeStr = new String[5] { "STD", "EXT", "USER", "NONE", "ERROR" };

        System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();

        [DllImport("MLD_KEY.dll", EntryPoint = "?appSeedKeyCals@@YGKK@Z")]
        public static extern UInt16 appSeedKeyCals(UInt16 Seed);


        public UTA0402_lin()
        {

        }


        public bool UTA0402_linOPen(Int32 Handle)
        {
            //if (index > 1)
            //{
            //    MessageBox.Show("当前设备上只有2个CAN卡，索引为0和1，请确认。");
            //    return false;
            //}
            #region 扫描设备，打开设备，获取设备信息

            USB_DEVICE.DEVICE_INFO DevInfo = new USB_DEVICE.DEVICE_INFO();
            //Int32[] DevHandles = new Int32[20];
            bool state;
            Int32 DevNum, ret;

            ////扫描查找设备
            //DevNum = USB_DEVICE.USB_ScanDevice(DevHandles);
            //if (DevNum < 2)
            //{
            //    //System.Console.WriteLine(string.Format("设备需要2张CAN卡，当前识别到的CAN卡数量为{0}，请查找原因!"), DevNum);
            //    MessageBox.Show("当前设备上只有2个CAN卡，索引为0和1，请确认。");
            //    return false;
            //}
            //else
            //{
            //    //System.Console.WriteLine("Have {0} device connected!", DevNum);
            //}
            DevHandle = Handle;
            //打开设备
            state = USB_DEVICE.USB_OpenDevice(DevHandle);
            if (!state)
            {
                //System.Console.WriteLine("Open device error!");
                return false;
            }
            else
            {
                //System.Console.WriteLine("Open device success!");
            }
            //获取固件信息
            StringBuilder FuncStr = new StringBuilder(256);
            state = USB_DEVICE.DEV_GetDeviceInfo(DevHandle, ref DevInfo, FuncStr);
            if (!state)
            {
                //System.Console.WriteLine("Get device infomation error!");
                return false;
            }
            else
            {
                //System.Console.WriteLine("Firmware Info:");
                //System.Console.WriteLine("    Name:" + Encoding.Default.GetString(DevInfo.FirmwareName));
                //System.Console.WriteLine("    Build Date:" + Encoding.Default.GetString(DevInfo.BuildDate));
                //System.Console.WriteLine("    Firmware Version:v{0}.{1}.{2}", (DevInfo.FirmwareVersion >> 24) & 0xFF, (DevInfo.FirmwareVersion >> 16) & 0xFF, DevInfo.FirmwareVersion & 0xFFFF);
                //System.Console.WriteLine("    Hardware Version:v{0}.{1}.{2}", (DevInfo.HardwareVersion >> 24) & 0xFF, (DevInfo.HardwareVersion >> 16) & 0xFF, DevInfo.HardwareVersion & 0xFFFF);
                //System.Console.WriteLine("    Functions:" + DevInfo.Functions.ToString("X8"));
                //System.Console.WriteLine("    Functions String:" + FuncStr);
            }
            #endregion

            #region CAN的两个通道初始化

            //初始化配置CAN
            //USB2CAN.CAN_INIT_CONFIG CANConfig = new USB2CAN.CAN_INIT_CONFIG();
            //CANConfig.CAN_Mode = 0x80;//正常模式
            //CANConfig.CAN_ABOM = 0;//禁止自动离线
            //CANConfig.CAN_NART = 1;//禁止报文重传
            //CANConfig.CAN_RFLM = 0;//FIFO满之后覆盖旧报文
            //CANConfig.CAN_TXFP = 1;//发送请求决定发送顺序
            ////配置波特率,波特率 = 42M/(BRP*(SJW+BS1+BS2))
            //CANConfig.CAN_BRP = 2;
            //CANConfig.CAN_BS1 = 15;
            //CANConfig.CAN_BS2 = 5;
            //CANConfig.CAN_SJW = 1;
            //CAN通道1初始化
            //ret = USB2CAN.CAN_Init(DevHandle, CAN_CH1, ref CANConfig);
            //if (ret != USB2CAN.CAN_SUCCESS)
            //{
            //    //System.Console.WriteLine("Config Send CAN1 failed!");
            //    return false;
            //}
            //else
            //{
            //    //System.Console.WriteLine("Config Send CAN1 Success!");
            //}
            ////CAN通道2初始化
            //ret = USB2CAN.CAN_Init(DevHandle, CAN_CH2, ref CANConfig);
            //if (ret != USB2CAN.CAN_SUCCESS)
            //{
            //    //System.Console.WriteLine("Config Read CAN2 failed!");
            //    return false;
            //}
            //else
            //{
            //    //System.Console.WriteLine("Config Read CAN2 Success!");
            //}

            #endregion

            #region LIN的两个通道初始化

            //LIN两个通道初始化
            //LIN1通道
            ret = USB2LIN_EX.LIN_EX_Init(DevHandle, 0, 19200, 1);
            if (ret != USB2LIN_EX.LIN_EX_SUCCESS)
            {
                //System.Console.WriteLine("Config LIN failed!");
                return false;
            }
            else
            {
                //System.Console.WriteLine("Config LIN Success!");
                return true;
            }


            #endregion


        
        }

        //public bool SendMsg(Int32 devHandle, UInt32 ID, byte[] data, byte chIndex)
        //{
        //    USB2CAN.CAN_MSG[] CanMsg = new USB2CAN.CAN_MSG[1];
        //    CanMsg[0] = new USB2CAN.CAN_MSG();
        //    CanMsg[0].ExternFlag = 0;
        //    CanMsg[0].RemoteFlag = 0;
        //    CanMsg[0].ID = ID;
        //    CanMsg[0].DataLen = 8;
        //    CanMsg[0].Data = new Byte[data.Length];
        //    for (int j = 0; j < CanMsg[0].DataLen; j++)
        //    {
        //        CanMsg[0].Data[j] = data[j];
        //    }

        //    int SendedNum = USB2CAN.CAN_SendMsg(devHandle, chIndex, CanMsg, (UInt32)CanMsg.Length);
        //    if (SendedNum >= 0)
        //    {
        //        //System.Console.WriteLine("Success send frames:{0}", SendedNum);
        //        return true;
        //    }
        //    else
        //    {
        //        //System.Console.WriteLine("Send CAN data failed!");
        //        return false;
        //    }
        //}

        public bool CloseUTA0402()
        {
           //// this.CANReadMsgFlag = false;
           // //USB2CAN.CAN_StopGetMsg(DevHandle, this.CAN_CH1);
           // //USB2CAN.CAN_StopGetMsg(DevHandle, this.CAN_CH2);

           // //LIN关闭
           // Console.ReadLine();
           // LIN_SendMsgFlag[0] = false;
           // //LIN_SendMsgFlag[1] = false;
           // //LIN1SendMsgThread.Join();
           // //LIN2SendMsgThread.Join();
            USB_DEVICE.USB_CloseDevice(DevHandle);

            return true;
        }

        public bool ResetUTA0503()
        {
            USB_DEVICE.USB_ResetDevice(DevHandle);

            return true;
        }

        public bool SendMsg_Lin(byte ID, byte[] DataBuffer)
        {
            int ret = USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, ID, DataBuffer, (byte)DataBuffer.Length, (byte)1);

            if (ret != USB2LIN_EX.LIN_EX_SUCCESS)
            {
                //System.Console.WriteLine("Send LIN data Failed!");
                return false;
            }
            else
            {
                System.Console.WriteLine("LIN Send data Success  {0} {1}",ID.ToString("X2"), ToHexStrFromByte(DataBuffer));
                return true;
            }
        }

        public bool ReadPara_MLD(byte DID, ref Product_MLD readData)
        {
            try
            {
                byte[] DataBuffer = new byte[8] { 0x22, DID, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] ReadBuffer = new byte[8];
                int ral;
                ral = USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                //for (int i = 0; i < 8; i++)
                //{
                //    Console.Write(DataBuffer[i].ToString("X2") + " ");
                //}
                //System.Console.WriteLine();
                System.Threading.Thread.Sleep(50);
                ral = USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);
                //Console.Write("读取： ");
                //for (int i = 0; i < 8; i++)
                //{
                //    Console.Write(ReadBuffer[i].ToString("X2") + " ");
                //}
                //System.Console.WriteLine();
                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {
                    switch (DID)
                    {
                        case 0x05://APP零件号
                            readData.AppPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
                            break;

                        case 0x06://APP版本号
                            readData.AppVer = asciiEncoding.GetString(ReadBuffer, 2, 6);
                            break;

                        case 0x07://引导零件号
                            readData.FBLPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
                            break;

                        case 0x08://引导版本号
                            readData.FBLVer = asciiEncoding.GetString(ReadBuffer, 2, 4);
                            break;

                        case 0x09://配置零件号
                            readData.CaliPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
                            break;

                        case 0x0A://配置版本号
                            readData.CaliVer = asciiEncoding.GetString(ReadBuffer, 2, 4);
                            break;

                        case 0x0B://动画零件号
                            readData.AnimePN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
                            break;

                        case 0x0C://动画版本号
                            readData.AnimeVer = asciiEncoding.GetString(ReadBuffer, 2, 4);
                            break;

                        default:
                            break;
                    }

                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
                //throw;
            }
        }

        public int ReadMsg_Lin(byte ID, ref byte[] DataBuffer)
        {
            
            int ret = USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, ID, DataBuffer);

            if (ret != DataBuffer.Length)
            {
              System.Console.WriteLine("Read LIN data error! {0}", ToHexStrFromByte(DataBuffer));
                return ret;
            }
            else
            {
                System.Console.WriteLine("LIN Recive data Success  {0} {1}",ID .ToString("X2"), ToHexStrFromByte(DataBuffer) );
                ////System.Console.WriteLine("Read LIN data Success");
                return ret;
            }
        }

        public bool SecurityAccess_3()
        {
            try
            {
                byte[] DataBuffer = new byte[8] { 0x27, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] ReadBuffer = new byte[8];

                int temp = USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                UInt16 Seed = (UInt16)(ReadBuffer[2] * 0x100 + ReadBuffer[3]);
                UInt16 Key = appSeedKeyCals(Seed);

                DataBuffer = new byte[8] { 0x27, 0x02, (byte)(Key / 0x100), (byte)(Key % 0x100), 0x00, 0x00, 0x00, 0x00 };

                USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                string str = "解锁: ";
                for (int i = 0; i < 8; i++)
                    str += string.Format("{0:X2} ", ReadBuffer[i]);
                //System.Console.WriteLine(str);

                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
                //throw;
            }


        }


        private bool WritePara_3(DateTime date, int index)
        {
            try
            {
                byte[] year = System.Text.Encoding.ASCII.GetBytes(date.Year.ToString("yy"));
                byte[] day = System.Text.Encoding.ASCII.GetBytes(date.DayOfYear.ToString());

                byte[] DataBuffer = new byte[8] { 0x2E, 0x02, year[0], year[1], day[0], day[1], day[2], 0x00 };
                byte[] ReadBuffer = new byte[8];

                USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {
                }
                else
                {
                    return false;
                }

                byte[] SN = System.Text.Encoding.ASCII.GetBytes(index.ToString("D6"));

                DataBuffer = new byte[8] { 0x2E, 0x03, SN[0], SN[1], SN[2], SN[3], SN[4], SN[5] };

                USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
                //throw;
            }
        }


        /// <summary>
        /// 字节数组转16进制字符串：空格分隔
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public  string ToHexStrFromByte( byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }






        //public bool ReadPara_3(byte DID, ref Pro_A58_3 readData)
        //{
        //    try
        //    {
        //        byte[] DataBuffer = new byte[8] { 0x22, DID, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //        byte[] ReadBuffer = new byte[8];
        //        int ral;
        //        ral = USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
        //        for (int i = 0; i < 8; i++)
        //        {
        //            Console.Write(DataBuffer[i].ToString("X2") + " ");
        //        }
        //        //System.Console.WriteLine();
        //        System.Threading.Thread.Sleep(50);
        //        ral = USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);
        //        Console.Write("读取： ");
        //        for (int i = 0; i < 8; i++)
        //        {
        //            Console.Write(ReadBuffer[i].ToString("X2") + " ");
        //        }
        //        //System.Console.WriteLine();
        //        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
        //        {
        //            switch (DID)
        //            {
        //                //case 0x02://生产日期
        //                //    readData.str_02 = asciiEncoding.GetString(ReadBuffer, 2, 5);
        //                //    break;

        //                //case 0x03://生产序列号
        //                //    readData.str_03 = asciiEncoding.GetString(ReadBuffer, 2, 6);
        //                //    break;

        //                case 0x05://APP零件号
        //                    readData.strApp_05 = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x06://APP版本号
        //                    readData.strAppVersion_06 = asciiEncoding.GetString(ReadBuffer, 2, 6);
        //                    break;

        //                case 0x07://引导零件号
        //                    readData.strFBL_07 = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x08://引导版本号
        //                    readData.strFBLVersion_08 = asciiEncoding.GetString(ReadBuffer, 2, 4);
        //                    break;

        //                case 0x09://引导零件号
        //                    readData.strCali_09 = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x0A://引导版本号
        //                    readData.strCaliVersion_0A = asciiEncoding.GetString(ReadBuffer, 2, 4);
        //                    break;

        //                default:
        //                    break;
        //            }

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //        //throw;
        //    }
        //}

        //public bool ReadPara_MLD(byte DID, ref Pro_A58_MLD readData)
        //{
        //    try
        //    {
        //        byte[] DataBuffer = new byte[8] { 0x22, DID, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //        byte[] ReadBuffer = new byte[8];
        //        int ral;
        //        ral = USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
        //        //for (int i = 0; i < 8; i++)
        //        //{
        //        //    Console.Write(DataBuffer[i].ToString("X2") + " ");
        //        //}
        //        //System.Console.WriteLine();
        //        System.Threading.Thread.Sleep(50);
        //        ral = USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);
        //        //Console.Write("读取： ");
        //        //for (int i = 0; i < 8; i++)
        //        //{
        //        //    Console.Write(ReadBuffer[i].ToString("X2") + " ");
        //        //}
        //        //System.Console.WriteLine();
        //        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
        //        {
        //            switch (DID)
        //            {
        //                case 0x05://APP零件号
        //                    readData.strAppPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x06://APP版本号
        //                    readData.strAppVersion = asciiEncoding.GetString(ReadBuffer, 2, 6);
        //                    break;

        //                case 0x07://引导零件号
        //                    readData.strFBLPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x08://引导版本号
        //                    readData.strFBLVersion = asciiEncoding.GetString(ReadBuffer, 2, 4);
        //                    break;

        //                case 0x09://配置零件号
        //                    readData.strCaliPN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x0A://配置版本号
        //                    readData.strCaliVersion = asciiEncoding.GetString(ReadBuffer, 2, 4);
        //                    break;

        //                case 0x0B://动画零件号
        //                    readData.strAnimePN = asciiEncoding.GetString(ReadBuffer, 2, 2) + (ReadBuffer[4] * 0x10000 + ReadBuffer[5] * 0x100 + ReadBuffer[6]).ToString("D7");
        //                    break;

        //                case 0x0C://动画版本号
        //                    readData.strAnimeVersion = asciiEncoding.GetString(ReadBuffer, 2, 4);
        //                    break;

        //                default:
        //                    break;
        //            }

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //        //throw;
        //    }
        //}



        //public bool HardwareCheck_3(byte DID, ref Pro_A58_3 readData)
        //{
        //    try
        //    {
        //        byte[] DataBuffer = new byte[8] { 0x1A, DID, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //        byte[] ReadBuffer = new byte[8];

        //        USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
        //        System.Threading.Thread.Sleep(50);
        //        USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

        //        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
        //        {
        //            switch (DID)
        //            {
        //                case 0xC0: //逻辑BOOST输出电压，一般为60V
        //                    readData.boostValue = ReadBuffer[2];
        //                    break;
        //                case 0xC1: //逻辑输入信号
        //                    //readData = string.Format("{0}", ReadBuffer[2]);
        //                    break;
        //                case 0xC2: //NTC信号
        //                    readData.NTC1 = (ReadBuffer[2] * 256 + ReadBuffer[3]) * 5 / 1024.0;
        //                    readData.NTC2 = (ReadBuffer[4] * 256 + ReadBuffer[5]) * 5 / 1024.0;
        //                    break;
        //                case 0xC4: //电源电压
        //                    readData.volPower = (ReadBuffer[2] * 256 + ReadBuffer[3]) * 35 / 1024.0;
        //                    break;
        //                case 0xC5: //4路输出通道(Bin电路)1，2通道
        //                    readData.Rbin1 = (ReadBuffer[2] * 256 + ReadBuffer[3]) * 5 / 1024.0;
        //                    readData.Rbin2 = (ReadBuffer[4] * 256 + ReadBuffer[5]) * 5 / 1024.0;
        //                    break;
        //                case 0xC6: //4路输出通道(Bin电路)1，2通道
        //                    readData.Rbin3 = (ReadBuffer[2] * 256 + ReadBuffer[3]) * 5 / 1024.0;
        //                    readData.Rbin4 = (ReadBuffer[4] * 256 + ReadBuffer[5]) * 5 / 1024.0;
        //                    break;
        //                case 0xCC: //通道故障报警
        //                    readData.BuckChannel_fault = ReadBuffer[4];

        //                    readData.PowerChannel_fault[0] = (ReadBuffer[2] % 2 == 1);
        //                    readData.PowerChannel_fault[1] = (ReadBuffer[2] >> 1 % 2 == 1);
        //                    readData.PowerChannel_fault[2] = (ReadBuffer[2] >> 2 % 2 == 1);
        //                    readData.PowerChannel_fault[3] = (ReadBuffer[2] >> 3 % 2 == 1);

        //                    readData.NtcChannel_fault[0] = (ReadBuffer[3] % 2 == 1);
        //                    readData.NtcChannel_fault[0] = (ReadBuffer[3] >> 1 % 2 == 1);
        //                    readData.NtcChannel_fault[0] = (ReadBuffer[3] >> 2 % 2 == 1);

        //                    break;

        //                default:
        //                    break;
        //            }

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //        //throw;
        //    }
        //}


        /// <summary>
        /// LSSS底边驱动
        /// </summary>
        /// <param name="flag">1：打开；2：关闭</param>
        /// <param name="value1">LSSS 1 PWM值</param>
        /// <param name="value2">LSSS 2 PWM值</param>
        /// <param name="value3">LSSS 3 PWM值</param>
        /// <returns></returns>
        public bool LSSS_OUTPUT_3(byte flag, byte value1, byte value2, byte value3)
        {
            try
            {
                byte[] DataBuffer = new byte[8] { 0x31, flag, 0xF1, value1, value2, value3, 0x00, 0x00 };
                byte[] ReadBuffer = new byte[8];

                USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                //System.Console.WriteLine("LSSS底边驱动发送: {0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2} ", 0x31, flag, 0xF1, value1, value2, value3, 0x00, 0x00);

                string str = "LSSS底边驱动接收: ";
                for (int i = 0; i < 8; i++)
                    str += string.Format("{0:X2} ", ReadBuffer[i]);
                //System.Console.WriteLine(str);

                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
                //throw;
            }
        }

        /// <summary>
        /// 恒流通道输出
        /// </summary>
        /// <param name="flag">1：打开；2：关闭</param>
        /// <param name="value1">通道1的PWM值</param>
        /// <param name="value2">通道2的PWM值</param>
        /// <param name="value3">通道3的PWM值</param>
        /// <returns></returns>
        public bool CC_CH_OUTPUT_3(byte flag, byte value1, byte value2, byte value3, byte value4)
        {
            try
            {
                byte[] DataBuffer = new byte[8] { 0x31, flag, 0xF2, value1, value2, value3, value4, 0x00 };
                byte[] ReadBuffer = new byte[8];

                USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
                System.Threading.Thread.Sleep(50);
                USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

                //System.Console.WriteLine("CC输出发送: {0:X2} {1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2} ", 0x31, flag, 0xF2, value1, value2, value3, 0x00, 0x00);

                string str = "CC输出接收: ";
                for (int i = 0; i < 8; i++)
                    str += string.Format("{0:X2} ", ReadBuffer[i]);
                System.Console.WriteLine(str);

                if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
                //throw;
            }
        }


        public bool Outage_OUTPUT(byte flag)
        {
            byte[] DataBuffer = new byte[8] { 0x31, flag, 0xF5, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] ReadBuffer = new byte[8];

            USB2LIN_EX.LIN_EX_MasterWrite(DevHandle, 0, (byte)0x10, DataBuffer, (byte)DataBuffer.Length, (byte)1);
            System.Threading.Thread.Sleep(50);
            USB2LIN_EX.LIN_EX_MasterRead(DevHandle, 0, (byte)0x1A, ReadBuffer);

            if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }











    }
}




