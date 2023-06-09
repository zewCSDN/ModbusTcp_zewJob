using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using MySqlX.XDevAPI.Common;
using NationalInstruments.DAQmx;

namespace _20230509
{
    public class ModbusTCPClient
    {
        




        public class ReceiveEventArgs : EventArgs
        {
            public byte[] ReceiveData { get; set; }
            public byte[] PLC_Heart { get; set; }
            public byte[] RceeiveSN1 { get; set; }
            public byte[] RceeiveSN2 { get; set; }
            public byte[] RceeiveSN3 { get; set; }
            public byte[] RceeiveSN4 { get; set; }
            public Single ReceivedTime { get; set; }
            public int ReceivedMegTCP = 0;
        }
        //public class SendEventArgs : EventArgs//用作监控显示
        //{
        //    public byte[] SendData { get; set; }//发送数据
        //    public int SendCount { get; set; }//发送次数
        //    public int SendInterval { get; set; }//发送间隔时间
        //}
        Stopwatch GetStopwatch = new Stopwatch();//开始计算耗时【1】

        public ushort PLC_Address;
        private static int RevCount = 0;//接受数据条数
        private ushort  DataLength{ get; set; }
  
        public string IP { get; set; }
        public int Port { get; set; }
        private enum MyStationSN_Register //工位条码寄存器地址
        {
            EOL1_=7102,
            EOL2_ = 7152,
            EOL3_ =7302,
            EOL4_ =7352
        }

        public enum MyStationTestComplex //工位测试完成地址
        {
            Eol1_Done_ = 162,//1#工位测试完成地址OK 写1
            Eol2_Done_ = 163,
            Eol3_Done_ = 166,
            Eol4_Done_ = 167
        }

        public enum MyStationTestResult //工位测试完成地址
        {
            Eol1_result_ = 172,//1#工位测试结果 OK 写1 NG写6
            Eol2_result_ = 173,
            Eol3_result_ = 175,
            Eol4_result_ = 176
        }
        public event Action<object, bool> EventConnection;
        public event Action<object, ReceiveEventArgs> EventReceive;

        private bool IsConnet = false;

        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public void Connet(string Iptxt, int Port)//接收参数是目标ip地址和目标端口号。客户端无须关心本地端口号
        {
            //   创建一个新的Socket对象
            IP = Iptxt;
            this.Port = Port;

            System.Threading.Tasks.Task taskTcp = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {

                while (!IsConnet)//循环
                {
                    try
                    {
                        client.Connect(IPAddress.Parse(Iptxt), Port);//尝试连接，失败则会跳去catch
                        IsConnet = true;//成功连接后修改bool值为false,这样下一步循环就不再执行。
                        EventConnection?.Invoke(this, true);
                        break;//在此处加上break，成功就跳出循环，避免死循环
                    }
                    catch
                    {
                        EventConnection?.Invoke(this, false);
                        client.Close();//先关闭
                        /*使用新的客户端资源覆盖，上一个已经废弃。如果继续使用以前的资源进行连接，
                        即使参数正确， 服务器全部打开也会无法连接*/
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Thread.Sleep(1000);//等待1s再去重连
                    }
                }

                if (IsConnet)
                { 
                     ClientReceiveData();
                }

                //    Task tskClicentRecive = Task.Run(() => ClientReceiveData());



            }, TaskCreationOptions.LongRunning);


        }


        public void  ClientReceiveData()//TCPClient消息的方法
        {
            byte[] rev = new byte[1024 * 1024];//接收消息的缓冲区
           
            while (!TokenSource.Token.IsCancellationRequested)
            {
                Array.Clear(rev, 0, rev.Length);
                int len = 0;//记录消息长度，以及判断是否连接
               
                try
                {

                   
                    len = client.Receive(rev, 0, rev.Length, SocketFlags.None);

                }
                catch
                {
                    client.Close();
                    Connet(this.IP, this.Port);//重新尝试去连接
                    IsConnet = false;//注意，此处是全局变量，将其设置为false,防止循环
                    return;//让方法结束，终结当前接收服务端数据的异步线程
                }


                if (len > 0)
                {
                   
                  
                    byte[] result = new byte[len];
                    //解析PLC寄存器数据 03功能码
                    Array.Copy(rev,0,result,0,len);
                    GetStopwatch.Start();
                    if (len == 9 + 2 * this.DataLength)
                    {
                    
                        //解析
                        if (result[7] == 0x03 && result[8] == this.DataLength * 2&& this.PLC_Address== 150)//接受PLC启动信号信息
                        {
                           
                            
                            byte[] data = new byte[this.DataLength*2];
                            Array.Copy(result,9,data,0,this.DataLength*2);
                          
                            EventReceive?.Invoke(this, new ReceiveEventArgs() { ReceiveData = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】
                        
                             GetStopwatch.Stop();
                            GetStopwatch.Restart();
                         
                        }
                        //解析
                        if (result[7] == 0x03 && result[8] == this.DataLength * 2 && this.PLC_Address == 178)//接受PLC心跳
                        {


                            byte[] data = new byte[this.DataLength * 2];
                            Array.Copy(result, 9, data, 0, this.DataLength * 2);



                            EventReceive?.Invoke(this, new ReceiveEventArgs() { PLC_Heart = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】

                            GetStopwatch.Stop();
                            GetStopwatch.Restart();
                        }
                        #region 获取条码信息
                        if (result[7] == 0x03 && result[8] == this.DataLength * 2 && this.DataLength == 6)//接受PLC条码信息
                        {
                            if (PLC_Address==(ushort) MyStationSN_Register.EOL1_)
                            {
                                byte[] data = new byte[this.DataLength * 2];
                                Array.Copy(result, 9, data, 0, this.DataLength * 2);


                               
                                EventReceive?.Invoke(this, new ReceiveEventArgs() { RceeiveSN1 = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】
                               GetStopwatch.Stop();
                                GetStopwatch.Restart();    
                            }
                            if (PLC_Address == (ushort)MyStationSN_Register.EOL2_)
                            {
                                byte[] data = new byte[this.DataLength * 2];
                                Array.Copy(result, 9, data, 0, this.DataLength * 2);


                               
                                EventReceive?.Invoke(this, new ReceiveEventArgs() { RceeiveSN2 = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】
                                GetStopwatch.Stop();
                                GetStopwatch.Restart();
                            }

                            if (PLC_Address == (ushort)MyStationSN_Register.EOL3_)
                            {
                                byte[] data = new byte[this.DataLength * 2];
                                Array.Copy(result, 9, data, 0, this.DataLength * 2);


                               
                                EventReceive?.Invoke(this, new ReceiveEventArgs() { RceeiveSN3 = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】
                                GetStopwatch.Stop();
                                GetStopwatch.Restart();
                            }
                            if (PLC_Address == (ushort)MyStationSN_Register.EOL4_)
                            {
                                byte[] data = new byte[this.DataLength * 2];
                                Array.Copy(result, 9, data, 0, this.DataLength * 2);


                               
                                EventReceive?.Invoke(this, new ReceiveEventArgs() { RceeiveSN4 = data, ReceivedTime = Convert.ToSingle(GetStopwatch.ElapsedMilliseconds) / 1000 });//结束计算耗时【1】
                                GetStopwatch.Stop();
                                GetStopwatch.Restart();

                            }
                            #endregion
                          
                        }


                    }

                }

             
            }


                   
                

          
        }
        /// <summary>
        /// 读取保持型寄存器 读字节功能码03 最多123个字 
        /// </summary>
        /// <param name="SlaveID">从站ID</param>
        /// <param name="iAddress">从站地址</param>
        /// <param name="iLength">数据长度 字节数</param>
        /// <returns></returns>
        public bool ReadKeepReg(ushort SlaveID,ushort iAddress, ushort iLength)
        {
          this.PLC_Address = iAddress;

            //第一步：拼接报文
            DataLength = iLength;
            List<byte> SendCommand  = new List<byte>();
             //单元标识符
            SendCommand.AddRange(new byte[] { 00,00});
            SendCommand.AddRange(new byte[] { 00,00});
            //协议标识符
            SendCommand.AddRange(new byte[] { 00,06 });
             
            SendCommand.Add((byte)SlaveID);
            //功能码
            SendCommand.Add(0x03);
            //起始地址
            SendCommand.Add((byte)(iAddress/256));
            SendCommand.Add((byte)(iAddress % 256));
            //长度
            SendCommand.Add((byte)(iLength / 256));
            SendCommand.Add((byte)(iLength % 256));
            //发送

            try
            {
               
             
                client.Send(SendCommand.ToArray());
             
                return true;

            }
            catch (Exception)
            {

                return false;
            }
      
        }


        //预置多个寄存器
        public bool PreSetMultiByteArray(int SlaveID, int iAddress, byte[] SetValue)
        {
            //首先判断一下字节数组是否正确

            if (SetValue == null || SetValue.Length == 0 )
            {
                return false;
            }
            byte[] tmp = null;
            int RegLength = SetValue.Length / 2;
            if (SetValue.Length % 2 == 1)
            {
                int length = SetValue.Length + 1;
                RegLength = length / 2;
                tmp = new byte[length];
                tmp[length -1] = 0x00;
                Array.Copy(SetValue, tmp, SetValue.Length);


            }
            else {

                int length = SetValue.Length;
                RegLength = length / 2;
                tmp = new byte[length];
                Array.Copy(SetValue, tmp, SetValue.Length); }

            //第一步：拼接报文

            List<byte> SendCommand = new List<byte>();

            SendCommand.AddRange(new byte[] { 0, 0, 0, 0 });

            int byteLength = 7 + tmp.Length;

            //字节长度
            SendCommand.Add((byte)(byteLength / 256));

            SendCommand.Add((byte)(byteLength % 256));

            //单元标识符和功能码
            SendCommand.AddRange(new byte[] { (byte)SlaveID, 0x10 });

            //起始寄存器
            SendCommand.Add((byte)(iAddress / 256));

            SendCommand.Add((byte)(iAddress % 256));

            //寄存器数量

            SendCommand.Add((byte)(RegLength / 256));

            SendCommand.Add((byte)(RegLength % 256));

            //字节计数
            SendCommand.Add((byte)(tmp.Length));
            SendCommand.AddRange(tmp);
            //具体字节
            try
            {
                client.Send(SendCommand.ToArray());
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }
        /// <summary>
        /// 测试完成结果数据 1号工位
        /// </summary>
        /// <param name="index"></param>
        /// <param name=""></param>
        public void MLDTestComple_Result_1Station(bool ok_ng)
        {
           
            if (ok_ng)//OK
            {
                
                PreSetMultiByteArray(1,  (int)MyStationTestResult.Eol1_result_, new byte[] { 0x00, 0x01 });//测试结果 地址偏移
                Thread.Sleep(25);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol1_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(25);
            }
            else
            {
               
                PreSetMultiByteArray(1,  (int)MyStationTestResult.Eol1_result_, new byte[] { 0x00, 0x06 });//测试结果 地址偏移 //NG写6
                Thread.Sleep(25);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol1_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(25);
            }
           

        }
        /// <summary>
        /// 2#工位
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ok_ng"></param>
        public void MLDTestComple_Result_2Station( bool ok_ng)
        {

            if (ok_ng)//OK
            {
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol1_result_, new byte[] { 0x00, 0x01 });//测试结果 地址偏移
                Thread.Sleep(20);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol1_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
               
            }
            else
            {
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol1_result_, new byte[] { 0x00, 0x06 });//测试结果 地址偏移 //NG写6
                Thread.Sleep(20);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol1_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
             
            }


        }

        public void MLDTestComple_Result_3Station( bool ok_ng)
        {

            if (ok_ng)//OK
            {
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol3_result_, new byte[] { 0x00, 0x01 });//测试结果 地址偏移
                Thread.Sleep(20);
                PreSetMultiByteArray(1,  (int)MyStationTestComplex.Eol3_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
             
            }
            else
            {
               
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol3_result_, new byte[] { 0x00, 0x06 });//测试结果 地址偏移 //NG写6
                Thread.Sleep(20);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol3_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
            }


        }

        public void MLDTestComple_Result_4Station(bool ok_ng)
        {

            if (ok_ng)//OK
            {
            
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol4_result_, new byte[] { 0x00, 0x01 });//测试结果 地址偏移
                Thread.Sleep(20);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol4_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
            }
            else
            {
             
                PreSetMultiByteArray(1, (int)MyStationTestResult.Eol4_result_, new byte[] { 0x00, 0x06 });//测试结果 地址偏移 //NG写6
                Thread.Sleep(20);
                PreSetMultiByteArray(1, (int)MyStationTestComplex.Eol4_Done_, new byte[] { 0x00, 0x01 });//测试完成 地址偏移
                Thread.Sleep(20);
            }


        }



    }
}