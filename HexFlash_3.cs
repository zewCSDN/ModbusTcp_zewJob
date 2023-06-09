using MySRHMethod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _20230509.MySRHMethod
{
    class HexFlash_3
    {
        [DllImport("MLD_KEY.dll", EntryPoint = "?appSeedKeyCals@@YGKK@Z")]
        public static extern UInt16 appSeedKeyCals(UInt16 Seed);

        UInt16[] CRCTable = new UInt16[16] { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };

        private UInt16 Seed = 0;
        private UInt16 Key = 0;


        public List<HexFile> hexFiles_APP = new List<HexFile>();
        public List<HexFile> hexFiles_Cali = new List<HexFile>();

        int hexFiles_APP_blockNum = 0;
        int hexFiles_Cali_blockNum = 0;
        int hexFiles_APP_dataIndex = 1;
        int hexFiles_Cali_dataIndex = 1;

        byte App_36_index = 0;
        byte Cali_36_index = 0;
        byte App_76_index = 0;
        byte Cali_76_index = 0;

        public UInt16 CRC_App = 0;
        public UInt16 CRC_Cali = 0;

        public bool isDownload = false;
        public bool isRunning = false;

        private bool running = false;
        private Thread thread = null;
        public DownLoadMachine LGDownloadMech = new DownLoadMachine();

        byte[] DataBuffer = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        byte[] ReadBuffer = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        UInt32 Tp_SendTimeout;
        UInt32 Tp_ReadDelay;
        Byte Tp_SendTask = 0;
        int Tp_SendLength = 0;

        int count = 0;

        public UTA0402_lin UTA0503 = new UTA0402_lin();

        public void Init()
        {
            Seed = 0;
            Key = 0;

            hexFiles_APP_blockNum = 0;
            hexFiles_Cali_blockNum = 0;
            hexFiles_APP_dataIndex = 1;
            hexFiles_Cali_dataIndex = 1;

            App_36_index = 0;
            Cali_36_index = 0;
            App_76_index = 0;
            Cali_76_index = 0;

            CRC_App = 0;
            CRC_Cali = 0;

            isDownload = false;
            isRunning = false;
            running = false;
            thread = null;

            DataBuffer = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            ReadBuffer = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            Tp_SendTask = 0;
            Tp_SendLength = 0;
        }


        /// <summary>
        /// 下载流程
        /// </summary>
        public enum DownLoadMachine
        #region 下载流程
        {
            IDLE_0,//空闲

            SID_2701, SID_2701_RUNNING,
            SID_2702, SID_2702_RUNNING,

            SID_1002, SID_1002_RUNNING,
            SID_3101FF01, SID_3101FF01_RUNNING,

            SID_34_App, SID_34_App_RUNNING,
            SID_36_App, SID_36_App_RUNNING,
            SID_37_App, SID_37_App_RUNNING,

            SID_3101F66B4C, SID_3101F66B4C_RUNNING,

            SID_3101FF02, SID_3101FF02_RUNNING,

            SID_34_Cali, SID_34_Cali_RUNNING,
            SID_36_Cali, SID_36_Cali_RUNNING,
            SID_37_Cali, SID_37_Cali_RUNNING,

            SID_3101F64DE1, SID_3101F64DE1_RUNNING,

            SID_1100,

            SID_Complete
        };
        #endregion


        /// <summary>
        /// 解析HEX文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hexFile"></param>
        /// <returns></returns>
        public bool ReadHexFile(string fileName, ref List<HexFile> hexFile)
        {
            if (fileName == null || fileName.Trim() == "")  //文件存在
            {
                return false;
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Open))  //open file
            {
                StreamReader HexReader = new StreamReader(fs);    //读取数据流
                string szLine = "";
                List<string> szAddress = new List<string>();

                bool isHead = false;

                bool isdone = false;
                while (true)
                {
                    szLine = HexReader.ReadLine();      //读取Hex中一行
                    if (szLine == null) { break; }          //读取完毕，退出
                    if (szLine.Substring(0, 1) == ":")    //判断首字符是”:”
                    {
                        string type = szLine.Substring(8, 1);

                        switch (type)
                        {
                            case "0"://直接解析数据类型
                                szAddress.Add(szLine.Substring(3, 4));

                                if (szAddress.Count > 1)
                                {
                                    UInt32 addr1 = UInt32.Parse(szAddress[szAddress.Count - 1], System.Globalization.NumberStyles.HexNumber);
                                    UInt32 addr2 = UInt32.Parse(szAddress[szAddress.Count - 2], System.Globalization.NumberStyles.HexNumber);

                                    if ((addr1 - addr2) > 0x20)
                                    {
                                        HexFile hexdata1 = new HexFile();

                                        hexdata1.address = hexFile[hexFile.Count - 1].address.Substring(0, 4);
                                        hexFile.Add(hexdata1);
                                        isHead = true;
                                    }
                                }

                                if (isHead)
                                {
                                    hexFile[hexFile.Count - 1].address += szLine.Substring(3, 4);
                                    isHead = false;
                                }

                                for (int i = 0; i < szLine.Length - 11; i += 2)
                                {
                                    string data = szLine.Substring(9 + i, 2);
                                    //hexFile[hexFile.Count - 1].data[hexFile[hexFile.Count - 1].length] = Convert.ToByte(data);
                                    hexFile[hexFile.Count - 1].data[hexFile[hexFile.Count - 1].length] = (byte)Int16.Parse(data, System.Globalization.NumberStyles.HexNumber);
                                    hexFile[hexFile.Count - 1].length++;
                                }
                                break;

                            case "1"://文件结束标识
                                if (szLine.Substring(1, 6) == "000000")
                                {
                                    isdone = true;
                                }
                                break;

                            case "4"://新的拓展地址
                                HexFile hexdata = new HexFile();

                                szAddress.Clear();
                                hexdata.address = szLine.Substring(9, 4);
                                hexFile.Add(hexdata);
                                isHead = true;
                                break;

                            default:
                                break;
                        }

                        if (isdone)
                        {
                            break;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 文件校验
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public UInt16 FuncCalCrc16(byte[] data, int length)
        {
            UInt16 crcVal = 0x0000;
            int crcIdx;
            byte tmpData;

            crcVal = 0x0000;
            for (crcIdx = 0; crcIdx < length; crcIdx++)
            {
                tmpData = data[crcIdx];
                crcVal = (ushort)(CRCTable[(tmpData ^ crcVal) & 0x0F] ^ (crcVal >> 4));
                crcVal = (ushort)(CRCTable[((tmpData >> 4) ^ crcVal) & 0x0F] ^ (crcVal >> 4));
            }
            return crcVal;
        }

        public void ThreadTick()//10ms
        {
            FblDiagTask();//1ms call

            switch (LGDownloadMech)
            {
                case DownLoadMachine.SID_2701:
                    #region
                    //System.Console.WriteLine("SID_2701");

                    DataBuffer[0] = 0x27;
                    DataBuffer[1] = 0x01;
                    DataBuffer[2] = 0x00;
                    DataBuffer[3] = 0x00;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_2701_RUNNING;
                    break;
                    #endregion

                case DownLoadMachine.SID_2702:
                    #region
                    //System.Console.WriteLine("SID_2702");

                    DataBuffer[0] = 0x27;
                    DataBuffer[1] = 0x02;
                    DataBuffer[2] = (byte)(Key / 0x100);
                    DataBuffer[3] = (byte)(Key % 0x100);
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_2702_RUNNING;
                    break;
                    #endregion

                case DownLoadMachine.SID_1002:
                    #region
                    //System.Console.WriteLine("SID_1002");

                    DataBuffer[0] = 0x10;
                    DataBuffer[1] = 0x02;
                    DataBuffer[2] = 0x00;
                    DataBuffer[3] = 0x00;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 1000;
                    LGDownloadMech = DownLoadMachine.SID_1002_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_3101FF01:
                    #region
                    //System.Console.WriteLine("SID_3101FF01");

                    DataBuffer[0] = 0x31;
                    DataBuffer[1] = 0x01;
                    DataBuffer[2] = 0xFF;
                    DataBuffer[3] = 0x01;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 1000;
                    LGDownloadMech = DownLoadMachine.SID_3101FF01_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_34_App:
                    #region
                    //System.Console.WriteLine("SID_34_App");

                    DataBuffer[0] = (Byte)(0x34);
                    DataBuffer[1] = byte.Parse(hexFiles_APP[hexFiles_APP_blockNum].address.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[2] = byte.Parse(hexFiles_APP[hexFiles_APP_blockNum].address.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[3] = byte.Parse(hexFiles_APP[hexFiles_APP_blockNum].address.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[4] = byte.Parse(hexFiles_APP[hexFiles_APP_blockNum].address.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[5] = (Byte)(hexFiles_APP[hexFiles_APP_blockNum].length % 65536 / 256);
                    DataBuffer[6] = (Byte)(hexFiles_APP[hexFiles_APP_blockNum].length % 256);
                    DataBuffer[7] = (Byte)(0x01);


                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_34_App_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_36_App:
                    #region
                    //System.Console.WriteLine("SID_36_App");

                    DataBuffer[0] = 0x36;
                    DataBuffer[1] = App_36_index;

                    for (int i = 0; i < 6; i++)
                    {
                        if (hexFiles_APP_dataIndex == hexFiles_APP[hexFiles_APP_blockNum].length)
                        {
                            for (int j = 0; j < 6 - i; j++)
                            {
                                DataBuffer[2 + i + j] = 0xFF;

                            }

                            App_36_index = 1;
                            Tp_SendTask = 0;
                            Tp_SendLength = 8;
                            Tp_SendTimeout = 1;
                            Tp_ReadDelay = 500;
                            LGDownloadMech = DownLoadMachine.SID_36_App_RUNNING;

                            break;
                        }
                        else if (App_36_index == 1 && i == 0)
                        {
                            DataBuffer[2] = hexFiles_APP[hexFiles_APP_blockNum].data[hexFiles_APP_dataIndex];
                            hexFiles_APP_dataIndex++;
                        }
                        else if ((hexFiles_APP_dataIndex % 2048) == 0 && hexFiles_APP_dataIndex != 0)
                        {
                            DataBuffer[4] = 0xFF;
                            DataBuffer[5] = 0xFF;
                            DataBuffer[6] = 0xFF;
                            DataBuffer[7] = 0xFF;


                            Tp_ReadDelay = 100;
                            LGDownloadMech = DownLoadMachine.SID_36_App_RUNNING;

                            break;
                        }
                        else
                        {
                            DataBuffer[2 + i] = hexFiles_APP[hexFiles_APP_blockNum].data[hexFiles_APP_dataIndex];
                            hexFiles_APP_dataIndex++;
                        }
                    }

                    if (hexFiles_APP_dataIndex == hexFiles_APP[hexFiles_APP_blockNum].length)
                    {
                        App_36_index = 1;
                        Tp_SendTask = 0;
                        Tp_SendLength = 8;
                        Tp_SendTimeout = 1;
                        Tp_ReadDelay = 2500;
                        LGDownloadMech = DownLoadMachine.SID_36_App_RUNNING;

                    }
                    else if (App_36_index == 0x57)
                    {
                        //App_36_index = 1;
                        App_36_index++;
                        Tp_SendTask = 0;
                        Tp_SendLength = 8;
                        Tp_SendTimeout = 1;
                        //LGDownloadMech = DownLoadMachine.SID_36_App_RUNNING;
                    }
                    else
                    {

                        if (App_36_index == 0xFF)
                        {
                            App_36_index = 0x01;
                        }
                        else
                        {
                            App_36_index++;
                        }
                        Tp_SendTask = 0;
                        Tp_ReadDelay = 0;
                    }

                    break;
                    #endregion

                case DownLoadMachine.SID_37_App:
                    #region
                    //System.Console.WriteLine("SID_37_App");

                    DataBuffer[0] = 0x37;
                    DataBuffer[1] = 0x00;
                    DataBuffer[2] = 0x00;
                    DataBuffer[3] = 0x00;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_37_App_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_3101F66B4C:
                    #region
                    //System.Console.WriteLine("SID_3101F66B4C");

                    DataBuffer[0] = 0x31;
                    DataBuffer[1] = 0x01;
                    DataBuffer[2] = 0xF6;
                    DataBuffer[3] = (byte)(CRC_App / 0x100);
                    DataBuffer[4] = (byte)(CRC_App % 0x100);
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 500;
                    LGDownloadMech = DownLoadMachine.SID_3101F66B4C_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_3101FF02:
                    #region
                    //System.Console.WriteLine("SID_3101FF02");

                    DataBuffer[0] = 0x31;
                    DataBuffer[1] = 0x01;
                    DataBuffer[2] = 0xFF;
                    DataBuffer[3] = 0x02;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 1000;
                    LGDownloadMech = DownLoadMachine.SID_3101FF02_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_34_Cali:
                    #region
                    //System.Console.WriteLine("SID_34_Cali");

                    DataBuffer[0] = (Byte)(0x34);
                    DataBuffer[1] = byte.Parse(hexFiles_Cali[hexFiles_Cali_blockNum].address.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[2] = byte.Parse(hexFiles_Cali[hexFiles_Cali_blockNum].address.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[3] = byte.Parse(hexFiles_Cali[hexFiles_Cali_blockNum].address.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[4] = byte.Parse(hexFiles_Cali[hexFiles_Cali_blockNum].address.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    DataBuffer[5] = (Byte)(hexFiles_Cali[hexFiles_Cali_blockNum].length % 65536 / 256);
                    DataBuffer[6] = (Byte)(hexFiles_Cali[hexFiles_Cali_blockNum].length % 256);
                    DataBuffer[7] = (Byte)(0x02);


                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_34_Cali_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_36_Cali:
                    #region
                    //System.Console.WriteLine("SID_36_Cali");
                    DataBuffer[0] = 0x36;
                    DataBuffer[1] = Cali_36_index;

                    for (int i = 0; i < 6; i++)
                    {
                        if (hexFiles_Cali_dataIndex == hexFiles_Cali[hexFiles_Cali_blockNum].length)
                        {
                            for (int j = 0; j < 6 - i; j++)
                            {
                                DataBuffer[2 + i + j] = 0xFF;

                            }

                            Cali_36_index = 1;
                            Tp_SendTask = 0;
                            Tp_SendLength = 8;
                            Tp_SendTimeout = 1;
                            Tp_ReadDelay = 2500;
                            LGDownloadMech = DownLoadMachine.SID_36_Cali_RUNNING;

                            break;
                        }
                        else if (Cali_36_index == 1 && i == 0)
                        {
                            DataBuffer[2] = hexFiles_Cali[hexFiles_Cali_blockNum].data[hexFiles_Cali_dataIndex];
                            hexFiles_Cali_dataIndex++;
                            //Tp_ReadDelay = 0;

                        }
                        else if ((hexFiles_Cali_dataIndex % 2048) == 0 && hexFiles_Cali_dataIndex != 0)
                        {
                            DataBuffer[4] = 0xFF;
                            DataBuffer[5] = 0xFF;
                            DataBuffer[6] = 0xFF;
                            DataBuffer[7] = 0xFF;

                            Tp_ReadDelay = 100;
                            LGDownloadMech = DownLoadMachine.SID_36_Cali_RUNNING;

                            break;
                        }
                        else
                        {
                            DataBuffer[2 + i] = hexFiles_Cali[hexFiles_Cali_blockNum].data[hexFiles_Cali_dataIndex];
                            hexFiles_Cali_dataIndex++;

                            //Tp_ReadDelay = 0;
                        }
                    }

                    if (hexFiles_Cali_dataIndex == hexFiles_Cali[hexFiles_Cali_blockNum].length)
                    {
                        Cali_36_index = 1;
                        Tp_SendTask = 0;
                        Tp_SendLength = 8;
                        Tp_SendTimeout = 1;
                        Tp_ReadDelay = 1500;
                        LGDownloadMech = DownLoadMachine.SID_36_Cali_RUNNING;
                    }
                    else if (Cali_36_index == 0x57)
                    {
                        //Cali_36_index = 1;
                        Cali_36_index++;
                        Tp_SendTask = 0;
                        Tp_SendLength = 8;
                        Tp_SendTimeout = 1;

                        //LGDownloadMech = DownLoadMachine.SID_36_Cali_RUNNING;
                    }
                    else
                    {
                        if (Cali_36_index == 0xFF)
                        {
                            Cali_36_index = 0x01;
                        }
                        else
                        {
                            Cali_36_index++;
                        }
                        Tp_SendTask = 0;
                        Tp_ReadDelay = 0;
                    }

                    break;
                    #endregion

                case DownLoadMachine.SID_37_Cali:
                    #region
                    //System.Console.WriteLine("SID_37_Cali");

                    DataBuffer[0] = 0x37;
                    DataBuffer[1] = 0x00;
                    DataBuffer[2] = 0x00;
                    DataBuffer[3] = 0x00;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 500;
                    LGDownloadMech = DownLoadMachine.SID_37_Cali_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_3101F64DE1:
                    #region
                    //System.Console.WriteLine("SID_3101F64DE1");

                    DataBuffer[0] = 0x31;
                    DataBuffer[1] = 0x01;
                    DataBuffer[2] = 0xF6;
                    DataBuffer[3] = (byte)(CRC_Cali / 0x100);
                    DataBuffer[4] = (byte)(CRC_Cali % 0x100);
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 500;
                    LGDownloadMech = DownLoadMachine.SID_3101F64DE1_RUNNING;

                    break;
                    #endregion

                case DownLoadMachine.SID_1100:
                    #region
                    //System.Console.WriteLine("SID_1100");

                    DataBuffer[0] = 0x11;
                    DataBuffer[1] = 0x00;
                    DataBuffer[2] = 0x00;
                    DataBuffer[3] = 0x00;
                    DataBuffer[4] = 0x00;
                    DataBuffer[5] = 0x00;
                    DataBuffer[6] = 0x00;
                    DataBuffer[7] = 0x00;

                    Tp_SendTask = 0;
                    Tp_SendLength = 8;
                    Tp_SendTimeout = 1;
                    Tp_ReadDelay = 100;
                    LGDownloadMech = DownLoadMachine.SID_Complete;

                    break;
                    #endregion

                case DownLoadMachine.SID_Complete:
                    #region
                    System.Console.WriteLine("SID_Complete");

                    this.isDownload = true;
                    this.isRunning = false;

                    LGDownloadMech = DownLoadMachine.IDLE_0;
                    break;
                    #endregion
            };

            //start to send request
            if (Tp_SendTimeout > 0)
            {
                Tp_SendTimeout = Tp_SendTimeout - 1;
                if (Tp_SendTimeout == 0)
                {
                    // need send 
                    if (Tp_SendTask == 0)
                    {

                        bool ret = this.UTA0503.SendMsg_Lin(0x10, DataBuffer);

                        for (int i = 0; i < 8; i++)
                        {
                            Console.Write(DataBuffer[i].ToString("X2") + " ");
                        }
                        Console.WriteLine();

                        System.Threading.Thread.Sleep((int)Tp_ReadDelay);

                        //if (ret != CAN_UDS.CAN_UDS_OK)
                        //{
                        //    return;
                        //}

                        Tp_SendTimeout = 1;
                        Tp_SendTask = 1;//need response
                        if (LGDownloadMech == DownLoadMachine.SID_36_App || LGDownloadMech == DownLoadMachine.SID_36_Cali || LGDownloadMech == DownLoadMachine.SID_1100)
                        {
                            Tp_SendTask = 0;//dont need response
                        }
                    }
                }
            }
        }

        public void FblDiagTask()
        {
            int Receive_flag = 0;
            if (1 == Tp_SendTask && LGDownloadMech != DownLoadMachine.IDLE_0)
            {
                Receive_flag = this.UTA0503.ReadMsg_Lin(0x1A, ref ReadBuffer);


            }
            if (Receive_flag > 0 || Receive_flag == -101)
            {

                for (int i = 0; i < 8; i++)
                {
                    Console.Write("读取： " + ReadBuffer[i].ToString("X2") + " ");
                }
                Console.WriteLine();

                switch (LGDownloadMech)
                {
                    case DownLoadMachine.SID_2701_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_2701_RUNNING");

                        this.UTA0503.ReadMsg_Lin(0x1A, ref ReadBuffer);

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            Seed = (UInt16)(ReadBuffer[2] * 0x100 + ReadBuffer[3]);
                            Key = appSeedKeyCals(Seed);

                            LGDownloadMech = DownLoadMachine.SID_2702;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_2702_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_2702_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {

                            LGDownloadMech = DownLoadMachine.SID_1002;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_1002_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_1002_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            LGDownloadMech = DownLoadMachine.SID_3101FF01;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_3101FF01_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_3101FF01_RUNNING");

                        if (/*ReadBuffer[0] == DataBuffer[0] + 0x40 &&*/ ReadBuffer[1] == DataBuffer[1] && ReadBuffer[2] == DataBuffer[2])
                        {
                            Tp_SendTask = 0;

                            LGDownloadMech = DownLoadMachine.SID_34_App;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_34_App_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_34_App_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            Tp_SendTask = 0;
                            hexFiles_APP_dataIndex = 0;

                            App_36_index = 1;
                            App_76_index = 1;

                            LGDownloadMech = DownLoadMachine.SID_36_App;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_36_App_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_36_App_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == App_76_index)
                        {

                            if (hexFiles_APP_dataIndex >= hexFiles_APP[hexFiles_APP_blockNum].length)
                            {
                                LGDownloadMech = DownLoadMachine.SID_37_App;
                            }
                            else
                            {
                                Tp_SendTask = 0;
                                App_36_index = 1;
                                App_76_index++;

                                LGDownloadMech = DownLoadMachine.SID_36_App;
                            }
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_37_App_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_37_App_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            Tp_SendTask = 0;
                            hexFiles_APP_blockNum++;

                            if (hexFiles_APP_blockNum >= hexFiles_APP.Count)
                            {
                                LGDownloadMech = DownLoadMachine.SID_3101F66B4C;
                            }
                            else
                            {
                                LGDownloadMech = DownLoadMachine.SID_34_App;
                            }
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_3101F66B4C_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_3101F66B4C_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1] && ReadBuffer[2] == DataBuffer[2])
                        {
                            Tp_SendTask = 0;

                            LGDownloadMech = DownLoadMachine.SID_3101FF02;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_3101FF02_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_3101FF01_RUNNING");

                        if (/*ReadBuffer[0] == DataBuffer[0] + 0x40 &&*/ ReadBuffer[1] == DataBuffer[1] && ReadBuffer[2] == DataBuffer[2])
                        {
                            Tp_SendTask = 0;

                            LGDownloadMech = DownLoadMachine.SID_34_Cali;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_34_Cali_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_34_Cali_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            Tp_SendTask = 0;
                            hexFiles_Cali_dataIndex = 0;

                            Cali_36_index = 1;
                            Cali_76_index = 1;

                            LGDownloadMech = DownLoadMachine.SID_36_Cali;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_36_Cali_RUNNING:
                        #region
                        System.Console.WriteLine("SID_36_Cali_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == Cali_76_index)
                        {

                            if (hexFiles_Cali_dataIndex >= hexFiles_Cali[hexFiles_Cali_blockNum].length)
                            {
                                LGDownloadMech = DownLoadMachine.SID_37_Cali;
                            }
                            else
                            {
                                Tp_SendTask = 0;
                                Cali_36_index = 1;
                                Cali_76_index++;

                                LGDownloadMech = DownLoadMachine.SID_36_Cali;
                            }
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_37_Cali_RUNNING:
                        #region
                        System.Console.WriteLine("SID_37_Cali_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1])
                        {
                            Tp_SendTask = 0;
                            hexFiles_Cali_blockNum++;

                            if (hexFiles_Cali_blockNum >= hexFiles_Cali.Count)
                            {
                                LGDownloadMech = DownLoadMachine.SID_3101F64DE1;
                            }
                            else
                            {
                                LGDownloadMech = DownLoadMachine.SID_34_Cali;
                            }
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion

                    case DownLoadMachine.SID_3101F64DE1_RUNNING:
                        #region
                        //System.Console.WriteLine("SID_3101F64DE1_RUNNING");

                        if (ReadBuffer[0] == DataBuffer[0] + 0x40 && ReadBuffer[1] == DataBuffer[1] && ReadBuffer[2] == DataBuffer[2])
                        {
                            Tp_SendTask = 0;

                            LGDownloadMech = DownLoadMachine.SID_1100;
                        }
                        else
                        {
                            LGDownloadMech = DownLoadMachine.IDLE_0;
                            this.isDownload = false;
                            this.isRunning = false;
                        }

                        break;
                        #endregion


                }

            }
            else if (Receive_flag == -101)
            {
                Console.WriteLine("数据接收超时！");
                LGDownloadMech = DownLoadMachine.IDLE_0;
                this.isDownload = false;
                this.isRunning = false;
            }
        }
    }
}
