using NationalInstruments;
//using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using NationalInstruments.Restricted;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MySRHMethod
{
    public class NI_USB6002
    {
        private string devName = "Dev1";

        //public delegate void MyDelegateOutputDIData(int data);
        //public event MyDelegateOutputDIData OutputAIData;
        //启动信号1个
        private Task taskDI_RS1;
        private DigitalSingleChannelReader myDigitalReaderRS1;
        //急停信号1个
        private Task taskDI_EMG;
        private DigitalSingleChannelReader myDigitalReaderEMG;
        //手自动切换信号1个
        private Task taskDI_MA;
        private DigitalSingleChannelReader myDigitalReaderMA;
        //复位信号1个
        private Task taskDI_Reset;
        private DigitalSingleChannelReader myDigitalReaderReset;
        //伺服报警信号2个
        private Task taskDI_AL1;
        private DigitalSingleChannelReader myDigitalReaderAL1;
        private Task taskDI_AL2;
        private DigitalSingleChannelReader myDigitalReaderAL2;
        //光栅1
        private Task taskDI_GS;
        private DigitalSingleChannelReader myDigitalReaderGS;
        //光栅2
        private Task taskDI_GS2;
        private DigitalSingleChannelReader myDigitalReaderGS2;

        //废料箱接近开关1个
        private Task taskDI_LK1;
        private DigitalSingleChannelReader myDigitalReaderLK1;


        ////模拟量输入
        //private task taskai_hall1 = null;
        //private analogmultichannelreader myreaderai_hall1 = null;

        private Task taskAI_Hall_AI5_6 = null;
        private AnalogMultiChannelReader myReaderAI_Hall1_AI5_6 = null;

        private Task taskAI_Hall_AI5_0_1_2 = null;
        private AnalogMultiChannelReader myReaderAI_Hall1_AI5_0_1_2 = null;


        private Task taskAI_Hall_AI1_3 = null;
        private AnalogMultiChannelReader myReaderAI_Hall1_AI1_3 = null;

        private Task taskAI_Cur1 = null;
        private AnalogMultiChannelReader myReaderAI_Cur1 = null;

        private readonly int sampls = 5000;
        private int nCurSamples = 8192;



        public NI_USB6002()
        {

        }

        public bool Init(string devName)
        {
            try
            {
                bool flag = false;
                this.devName = devName;
                flag |= this.DI_Create(this.devName);
                flag |= this.AI_CreateHall(this.devName);//ai1 _ai3
                flag |= this.AI_CreateHall_AI5_6(this.devName);
               flag |= this.AI_CreateHall_AI5_0_1_2(this.devName);
                return flag;
            }
            catch { return false; }
        }


        #region DI功能实现
        private bool DI_Create(string devname)
        {
            bool flag = false;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //Create a task such that it will be disposed after
                //we are done using it.
                taskDI_RS1 = new Task();
                taskDI_EMG = new Task();
                taskDI_MA = new Task();
                taskDI_Reset = new Task();
                taskDI_AL1 = new Task();
                taskDI_AL2 = new Task();
                taskDI_GS = new Task();
                taskDI_LK1 = new Task();
                taskDI_GS2 = new Task();

                //Create channel
                this.taskDI_RS1.DIChannels.CreateChannel(devname + "/port2/line0", "myChanne20", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderRS1 = new DigitalSingleChannelReader(this.taskDI_RS1.Stream);
                this.taskDI_EMG.DIChannels.CreateChannel(devname + "/port2/line1", "myChanne21", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderEMG = new DigitalSingleChannelReader(this.taskDI_EMG.Stream);
                this.taskDI_MA.DIChannels.CreateChannel(devname + "/port2/line2", "myChanne22", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderMA = new DigitalSingleChannelReader(this.taskDI_MA.Stream);
                this.taskDI_Reset.DIChannels.CreateChannel(devname + "/port2/line3", "myChanne23", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderReset = new DigitalSingleChannelReader(this.taskDI_Reset.Stream);
                this.taskDI_AL1.DIChannels.CreateChannel(devname + "/port2/line4", "myChanne24", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderAL1 = new DigitalSingleChannelReader(this.taskDI_AL1.Stream);
                this.taskDI_AL2.DIChannels.CreateChannel(devname + "/port2/line5", "myChanne25", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderAL2 = new DigitalSingleChannelReader(this.taskDI_AL2.Stream);
                this.taskDI_GS.DIChannels.CreateChannel(devname + "/port2/line6", "myChanne26", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderGS = new DigitalSingleChannelReader(this.taskDI_GS.Stream);
                this.taskDI_LK1.DIChannels.CreateChannel(devname + "/port2/line7", "myChanne27", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderLK1 = new DigitalSingleChannelReader(this.taskDI_LK1.Stream);
                this.taskDI_GS.DIChannels.CreateChannel(devname + "/port1/line7", "myChanne28", ChannelLineGrouping.OneChannelForEachLine);
                this.myDigitalReaderGS = new DigitalSingleChannelReader(this.taskDI_GS.Stream);

                flag = true;
            }
            catch (DaqException exception)
            {
                taskDI_RS1.Dispose();
                taskDI_EMG.Dispose();
                taskDI_MA.Dispose();
                taskDI_Reset.Dispose();
                taskDI_AL1.Dispose();
                taskDI_AL2.Dispose();
                taskDI_GS.Dispose();
                taskDI_LK1.Dispose();
                taskDI_GS2.Dispose();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return flag;
        }

        public bool DI_GetBS1()
        {
            try
            {
                bool[] readData = this.myDigitalReaderRS1.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }

        }
        public bool DI_GetEMG()
        {
            try
            {
                bool[] readData = this.myDigitalReaderEMG.ReadSingleSampleMultiLine();
                return !readData[0];
            }
            catch { return false; }

        }
        public bool DI_GetMA()
        {
            try
            {
                bool[] readData = this.myDigitalReaderMA.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }

        }
        public bool DI_GetReset()
        {
            try
            {
                bool[] readData = this.myDigitalReaderReset.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }
        public bool DI_GetAL1()
        {
            try
            {
                bool[] readData = this.myDigitalReaderAL1.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }
        public bool DI_GetAL2()
        {
            try
            {
                bool[] readData = this.myDigitalReaderAL2.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }
        public bool DI_GetGS()
        {
            try
            {
                bool[] readData = this.myDigitalReaderGS.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }
        public bool DI_GetLK1()
        {
            try
            {
                bool[] readData = this.myDigitalReaderLK1.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }
        public bool DI_GetGS2()
        {
            try
            {
                bool[] readData = this.myDigitalReaderGS2.ReadSingleSampleMultiLine();
                return readData[0];
            }
            catch { return false; }
        }

        #endregion

        #region DO功能实现
        /// <summary>
        /// 共有方法
        /// </summary>
        /// <param name="line"></param>
        /// <param name="value"></param>
        private void WriteSingleLine(string line, bool value)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (Task digitalWriteTask = new Task())
                {
                    //digitalWriteTask.DOChannels.CreateChannel("Dev1/Port0/line0", "",
                    digitalWriteTask.DOChannels.CreateChannel(line, "",
                        ChannelLineGrouping.OneChannelForEachLine);
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSampleSingleLine(true, value);
                    System.Threading.Thread.Sleep(30);
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void WriteDigPort(string strport, UInt32 value)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (Task digitalWriteTask = new Task())
                {
                    //  Create an Digital Output channel and name it.
                    //digitalWriteTask.DOChannels.CreateChannel("Dev1/port0", "port0",
                    digitalWriteTask.DOChannels.CreateChannel(strport, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, value);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }



        /// <summary>
        /// 静态电流切换
        /// </summary>
        /// <param name="value"></param>
        public void DO_00_staticCurrent(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line0", value);
        }
        public void DO_01_linOutage(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line1", value);
        }
        /// <summary>
        /// GLOBAL 平台切换
        /// </summary>
        /// <param name="value"></param>
        public void DO_02_Res(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line2", value);
        }
        /// <summary>
        /// 负载仪切换 测试HSSLSS
        /// </summary>
        /// <param name="value"></param>
        public void DO_03_K4_FZ(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line3", value);
        }
        public void DO_04_14V_LSS(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line4", value);
        }
        public void DO_05(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line5", value);
        }
        public void DO_06(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line6", value);
        }
        public void DO_07(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port0/line7", value);
        }

        public void DO_08(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port1/line0", value);
        }
        public void D0_09_Swich_AI5_0_1_2(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port1/line1", value);
        }
        public void START_DI_00(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port1/line2", value);
        }
        public void STOP_DI_01(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port1/line3", value);
        }
        public void Rest_DI_02(bool value = false)
        {
            this.WriteSingleLine(this.devName + "/port2/line0", value);
        }

        //public void DO_K8(bool value = false)
        //{
        //    this.WriteSingleLine(this.devName + "/port1/line7", value);
        //}


        public void ClearDO()
        {
            //this.DO_10();
            //this.DO_11();
            //this.DO_12();
            //this.DO_13();
            //this.DO_14();
            //this.DO_15();
            //this.DO_16();
            ////this.DO_K8();
            //this.DO_01();
            //this.DO_02();
            //this.DO_03();
            //this.DO_04();
            //this.DO_05();
            //this.DO_07();
            //this.DO_10();
            //this.DO_11();
        }

        #endregion

        #region AI功能实现
        private bool GetAIData(string aiport, int nSamples, ref double aidata)
        {
            try
            {
                //Create a new task
                using (Task myTask = new Task())
                {
                    //Create a virtual channel
                    myTask.AIChannels.CreateVoltageChannel(aiport, "",
                        (AITerminalConfiguration.Rse), -10, 10, AIVoltageUnits.Volts);

                    AnalogMultiChannelReader reader = new AnalogMultiChannelReader(myTask.Stream);
                    //Verify the Task
                    myTask.Control(TaskAction.Verify);
                    //Plot Multiple Channels to the table
                    //double[,] datatemp = reader.ReadMultiSample(nSamples);
                    double[] data = new double[50];
                    double dbSum = 0.00;
                    List<double> dataAI = new List<double>();
                    //System.Threading.Thread.Sleep(50);
                    for (int i = 0; i < nSamples; i++)
                    {
                        data = reader.ReadSingleSample();
                        dataAI.Add(data[0]);
                    }
                    //this.WriteFile(dataAI.ToArray(), "AI");
                    dataAI.Sort();
                    for (int i = 2; i < nSamples - 2; i++)
                    {
                        dbSum += dataAI[i];
                    }
                    aidata = dbSum * 1.0 / (nSamples - 4);
                    myTask.Dispose();
                }
                return true;
            }
            catch (DaqException exception)
            {
                //MessageBox.Show(exception.Message);
                return false;
            }
        }



     

        private bool AI_CreateHall_AI5_6(string devname)
        {
            bool flag = false;
            try
            {
                // Create a new task
                this.taskAI_Hall_AI5_6 = new Task();

                // Initialize local variables
                double sampleRate = 5000;//采样率
                double rangeMinimum = 0;//范围最小值
                double rangeMaximum = 10;//范围最大值
                //int samplesPerChannel = 100000;

                // Create a channel AI1 逻辑输出logic AI3 outage AICC 采样; ai hss1  ai6 ;ai5 lss1 ,lss2  ai0 lss3 ai1 lss4
                taskAI_Hall_AI5_6.AIChannels.CreateVoltageChannel( devname + "/ai5" + "," + devname + "/ai6","",
                    (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);

                //taskAI_Hall1.AIChannels.CreateVoltageChannel(devname + "/ai1", "",
                // (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);//添加采样通道

                // Configure timing specs    
                taskAI_Hall_AI5_6.Timing.ConfigureSampleClock("", sampleRate, SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, this.sampls);

                // Verify the task
                taskAI_Hall_AI5_6.Control(TaskAction.Verify);

                // Read the data
                myReaderAI_Hall1_AI5_6 = new AnalogMultiChannelReader(taskAI_Hall_AI5_6.Stream);

                flag = true;
                //AnalogWaveform<double>[] data = myReaderAI_Hall1.ReadWaveform(samplesPerChannel);

                //dataToDataTable(data, ref dataTable);
            }
            catch (DaqException exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //taskAI_Hall1.Dispose();
            }
            return flag;
        }


        private bool AI_CreateHall(string devname)
        {
            bool flag = false;
            try
            {
                // Create a new task
                this.taskAI_Hall_AI1_3 = new Task();

                // Initialize local variables
                double sampleRate = 5000;//采样率
                double rangeMinimum = 0;//范围最小值
                double rangeMaximum = 10;//范围最大值
                //int samplesPerChannel = 100000;

                // Create a channel AI1 逻辑输出logic AI3 outage AICC 采样; ai hss1  ai6 ;ai5 lss1 ,lss2  ai0 lss3 ai1 lss4
                taskAI_Hall_AI1_3.AIChannels.CreateVoltageChannel( devname + "/ai1"+ "," + devname + "/ai3", "",
                    (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);

                //taskAI_Hall1.AIChannels.CreateVoltageChannel(devname + "/ai1", "",
                // (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);//添加采样通道

                // Configure timing specs    
                taskAI_Hall_AI1_3.Timing.ConfigureSampleClock("", sampleRate, SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, this.sampls);

                // Verify the task
                taskAI_Hall_AI1_3.Control(TaskAction.Verify);

                // Read the data
                myReaderAI_Hall1_AI1_3 = new AnalogMultiChannelReader(taskAI_Hall_AI1_3.Stream);

                flag = true;
                //AnalogWaveform<double>[] data = myReaderAI_Hall1.ReadWaveform(samplesPerChannel);

                //dataToDataTable(data, ref dataTable);
            }
            catch (DaqException exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //taskAI_Hall1.Dispose();
            }
            return flag;
        }

        private bool AI_CreateHall_AI5_0_1_2(string devname)
        {
            bool flag = false;
            try
            {
                // Create a new task
                this.taskAI_Hall_AI5_0_1_2 = new Task();

                // Initialize local variables
                double sampleRate = 5000;//采样率
                double rangeMinimum = 0;//范围最小值
                double rangeMaximum = 10;//范围最大值
                //int samplesPerChannel = 100000;

                // Create a channel AI1 逻辑输出logic AI3 outage AICC 采样; ai hss1  ai6 ;ai5 lss1 ,lss2  ai0 lss3 ai1 lss4
                taskAI_Hall_AI5_0_1_2.AIChannels.CreateVoltageChannel(devname + "/ai5" + "," + devname + "/ai0" + "," + devname + "/ai1" + "," + devname + "/ai2" , "",
                    (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);

                //taskAI_Hall1.AIChannels.CreateVoltageChannel(devname + "/ai1", "",
                // (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);//添加采样通道

                // Configure timing specs    
                taskAI_Hall_AI5_0_1_2.Timing.ConfigureSampleClock("", sampleRate, SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, this.sampls);

                // Verify the task
                taskAI_Hall_AI5_0_1_2.Control(TaskAction.Verify);

                // Read the data
                myReaderAI_Hall1_AI5_0_1_2 = new AnalogMultiChannelReader(taskAI_Hall_AI5_0_1_2.Stream);

                flag = true;
                //AnalogWaveform<double>[] data = myReaderAI_Hall1.ReadWaveform(samplesPerChannel);

                //dataToDataTable(data, ref dataTable);
            }
            catch (DaqException exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //taskAI_Hall1.Dispose();
            }
            return flag;
        }

        //public bool AI_GetHall_1(ref double dbUH, ref double dbUL, ref double dbDuty, ref double dbFre, ref List<double> dataHall)
        //{
        //    //try
        //    //{
        //    //    AnalogWaveform<double>[] data = myReaderAI_Hall.ReadWaveform(this.sampls);

        //    //    List<double> dataTemp = new List<double>();
        //    //    if (dataHall == null)
        //    //        dataHall = new List<double>();
        //    //    foreach (AnalogWaveform<double> waveform in data)
        //    //    {
        //    //        for (int sample = 0; sample < waveform.Samples.Count; ++sample)
        //    //        {
        //    //            dataTemp.Add((double)waveform.Samples[sample].Value * 2.98);
        //    //            if ((sample >= 0) && (sample < 2000))
        //    //                dataHall.Add((double)waveform.Samples[sample].Value * 2.98);
        //    //        }
        //    //    }

        //    //    //计算频率
        //    //    double dbmed = dataTemp.Sum() / dataTemp.Count;
        //    //    bool bH = false;
        //    //    bool bL = false;
        //    //    int nFre = 0;
        //    //    for (int index = 0; index < dataTemp.Count; index++)
        //    //    {
        //    //        if ((dataTemp[index] > dbmed) && (bH == false))
        //    //        {
        //    //            bH = true;
        //    //            bL = false;
        //    //            nFre++;
        //    //        }
        //    //        if ((dataTemp[index] < dbmed) && (bL == false))
        //    //        {
        //    //            bL = true;
        //    //            bH = false;
        //    //            nFre++;
        //    //        }
        //    //    }
        //    //    dbFre = nFre * 4 / 2.0;
        //    //    //将数组升序排列后计算占空比
        //    //    dataTemp.Sort();
        //    //    dbUL = dataTemp[200];
        //    //    if (dbUL < 0)
        //    //        dbUL = 0.01;
        //    //    dbUH = dataTemp[dataTemp.Count - 200];
        //    //    //double med = (dataTemp[0] + dataTemp[dataTemp.Count - 1]) / 2;
        //    //    int nIndexMem = 0;
        //    //    for (int index = 0; index < dataTemp.Count; index++)
        //    //    {
        //    //        if (dataTemp[index] > dbmed)
        //    //        {
        //    //            dbDuty = (dataTemp.Count - index) * 100.0 / dataTemp.Count;
        //    //            nIndexMem = index;
        //    //            break;
        //    //        }
        //    //    }

        //    //    //高电平、低电平值处理
        //    //    double dbMemH = 0;
        //    //    double dbMemL = 0;
        //    //    for (int index = 0; index < nIndexMem; index++)
        //    //        dbMemL += dataTemp[index];
        //    //    dbMemL /= nIndexMem;
        //    //    //dbUL = dbMemL;

        //    //    for (int index = nIndexMem; index < dataTemp.Count; index++)
        //    //        dbMemH += dataTemp[index];
        //    //    dbMemH /= (dataTemp.Count - nIndexMem);
        //    //    //dbUH = dbMemH;

        //    //    for (int index = 0; index < dataHall.Count; index++)
        //    //    {
        //    //        if (dataHall[index] > dbmed)
        //    //            dataHall[index] = (dbMemH + (dataHall[index] - dbMemH) * 0.1);
        //    //        else
        //    //            dataHall[index] = (dbMemL + (dataHall[index] - dbMemL) * 0.1);
        //    //    }

        //    //    if ((dbUH - dbUL) < 0.8)
        //    //    {
        //    //        dbDuty = 0;
        //    //        dbFre = 0;
        //    //    }
        //    //    return true;
        //    //}
        //    //catch
        //    //{
        //    //    return false;
        //    //}
        //}



        /// <summary>
        /// HSSS1 测试计算
        /// </summary>
        /// <param name="FallTime1"></param>
        /// <param name="RiseTime1"></param>
        /// <param name="fall_V1"></param>
        /// <param name="rise_V1"></param>
        public void Logic_HSSS_AI5_AI6(out double FallTime1, out double RiseTime1, out double fall_V1, out double rise_V1)
        {
            RiseTime1 = 0; FallTime1 = 0; rise_V1 = 0; fall_V1 = 0;
           
            AnalogWaveform<double>[] data = myReaderAI_Hall1_AI5_6.ReadWaveform(sampls);
  List<double> HSS1_sampleDataAI5 = new List<double>();
            List<double> HSS1_sampleDataAI6 = new List<double>();
          
           

            if (data[0].Samples != null)
            {
                for (int i = 0; i < data[0].SampleCount; i++)
                {
                    HSS1_sampleDataAI5.Add((double)data[0].Samples[i].Value);// ai5 
                    HSS1_sampleDataAI6.Add((double)data[1].Samples[i].Value);// ai6 


                }

            }


            //文件保存
            this.AIDataPrint(HSS1_sampleDataAI5, "HSS1_sampleDataAI5");

            this.AIDataPrint(HSS1_sampleDataAI6, "HSS1_sampleDataAI6");

            //开始解析

            //GetRiseDownTime(LSS1_sampleData, ref FallTime1, ref RiseTime1, out fall_V1, out rise_V1);//lss1 
            //GetRiseDownTime(LSS2_sampleData, ref FallTime2, ref RiseTime2, out fall_V2, out rise_V2);//lss2
            //GetRiseDownTime(LSS3_sampleData, ref FallTime3, ref RiseTime3, out fall_V3, out rise_V3);//lss3

            //GetRiseDownTime(LSS4_sampleData, ref FallTime4, ref RiseTime4, out fall_V4, out rise_V4);//lss3

        }








        /// <summary>
        /// //逻辑上升和下降沿时间间隔
        /// </summary>
        /// <param name="FallTime1">LS1下降时间</param>
        /// <param name="RiseTime1"></param>
        /// <param name="fall_V1"></param>
        /// <param name="rise_V1"></param>
        /// <param name="FallTime2">LS1下降时间</param>
        /// <param name="RiseTime2"></param>
        /// <param name="fall_V2"></param>
        /// <param name="rise_V2"></param>
        /// <param name="FallTime3">LS1下降时间</param>
        /// <param name="RiseTime3"></param>
        /// <param name="fall_V3"></param>
        /// <param name="rise_V3"></param>
        /// <param name="FallTime4">LS1下降时间</param>
        /// <param name="RiseTime4"></param>
        /// <param name="fall_V4"></param>
        /// <param name="rise_V4"></param>
        public void Logic_riseFall_AI5_0_1_2(out double FallTime1 , out double RiseTime1, out double fall_V1, out double rise_V1, out double FallTime2, out double RiseTime2, out double fall_V2, out double rise_V2,

            out double FallTime3, out double RiseTime3, out double fall_V3, out double rise_V3, out double FallTime4, out double RiseTime4, out double fall_V4, out double rise_V4)
        {
            RiseTime1 = 0;FallTime1 = 0; rise_V1 = 0; fall_V1 = 0;
            RiseTime2 = 0; FallTime2 = 0; rise_V2 = 0; fall_V2 = 0;
            RiseTime3 = 0; FallTime3 = 0; rise_V3 = 0; fall_V3 = 0;
            RiseTime4 = 0; FallTime4 = 0; rise_V4 = 0; fall_V4 = 0;
            AnalogWaveform<double>[] data = myReaderAI_Hall1_AI5_0_1_2.ReadWaveform(sampls);

            List<double> LSS1_sampleData = new List<double>();
            List<double> LSS2_sampleData = new List<double>();
            List<double> LSS3_sampleData = new List<double>();
            List<double> LSS4_sampleData = new List<double>();
          
            if (data[0].Samples != null)
            {
                for (int i = 0; i < data[0].SampleCount; i++)
                {
                    LSS1_sampleData.Add((double)data[0].Samples[i].Value);//LSS1-4 ai5  ai1  ai2 ai3
                    LSS2_sampleData.Add((double)data[1].Samples[i].Value);
                    LSS3_sampleData.Add((double)data[2].Samples[i].Value);
                    LSS4_sampleData.Add((double)data[3].Samples[i].Value);
                }

            }


            //文件保存
            this.AIDataPrint(LSS1_sampleData, "LSS1");

            this.AIDataPrint(LSS2_sampleData, "LSS2");

            this.AIDataPrint(LSS3_sampleData, "LSS3");
            this.AIDataPrint(LSS4_sampleData, "LSS4");
            //开始解析
           
            GetRiseDownTime(LSS1_sampleData, ref FallTime1, ref RiseTime1, out fall_V1, out rise_V1);//lss1 
            GetRiseDownTime(LSS2_sampleData, ref FallTime2, ref RiseTime2, out fall_V2, out rise_V2);//lss2
            GetRiseDownTime(LSS3_sampleData, ref FallTime3, ref RiseTime3, out fall_V3, out rise_V3);//lss3

            GetRiseDownTime(LSS4_sampleData, ref FallTime4, ref RiseTime4, out fall_V4, out rise_V4);//lss3

        }


        private static void AIDataGetFromCSV(out List<double> listData, string flag)
        {
            listData = new List<double>();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "log\\" + flag + ".csv";



            try
            {
                List<double> samples = new List<double>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');

                        if (double.TryParse(values[0], out double parsedValue))
                        {
                            samples.Add(parsedValue);

                        }
                        else
                        {
                            Console.WriteLine($"无法解析值 {values[0]} 为 double 类型");
                            samples.Add(0.0); // 如果解析失败，添加默认值
                        }
                    }
                }


                listData = samples;
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取文件时出错: " + ex.Message);
            }







        }
        public bool GetPWM_8(List<double> data, ref double dbUH, ref double dbUL, ref double dbDuty, ref double dbFre)
        {
            List<double> dataTemp = new List<double>();

            int ncnt = 0;
         

            for (int i = 0; i < data.Count; i++)
            {
                dataTemp.Add(data[i] * 3.4);
                ncnt++;
            }
            //求平均值
            double dbmed = dataTemp.Sum() / dataTemp.Count;
            //dbmed -= 0.5;
            bool bH = false;
            bool bL = false;
            int nFre = 0;
            for (int index = 0; index < dataTemp.Count; index++)
            {
                if ((dataTemp[index] > dbmed) && (bH == false))
                {
                    bH = true;
                    bL = false;
                    nFre++;
                }
                if ((dataTemp[index] < dbmed) && (bL == false))
                {
                    bL = true;
                    bH = false;
                    nFre++;
                }
            }
            dbFre = nFre;
            //将数组升序排列后计算占空比
            dataTemp.Sort();
            dbUL = dataTemp[800] * 0.97;
            dbUH = dataTemp[dataTemp.Count - 800] * 0.97;

            int nIndexMem = 0;
            for (int index = 0; index < dataTemp.Count; index++)
            {
                if (dataTemp[index] > dbmed)
                {
                    if (index > dataTemp.Count / 2)
                    {
                        double delt = (2 * index - dataTemp.Count) * 100.0 / (2 * dataTemp.Count);
                        dbDuty = (dataTemp.Count - index) * 100.0 / dataTemp.Count + delt * 0.9652;
                    }
                    else
                    {
                        double delt = (dataTemp.Count - 2 * index) * 100.0 / (2 * dataTemp.Count);
                        dbDuty = (dataTemp.Count - index) * 100.0 / dataTemp.Count - delt * 0.9652;
                    }
                    nIndexMem = index;
                    break;
                }
            }
            //高电平、低电平值处理
            double dbMemH = 0;
            double dbMemL = 0;
            for (int index = 0; index < nIndexMem; index++)
                dbMemL += dataTemp[index];
            dbMemL /= nIndexMem;
            //dbUL = dbMemL;

            for (int index = nIndexMem; index < dataTemp.Count; index++)
                dbMemH += dataTemp[index];
            dbMemH /= (dataTemp.Count - nIndexMem);
            //dbUH = dbMemH;

            dbUL = dbMemL;
            dbUH = dbMemH;
            //if ((dbUH - dbUL) <= 2)
            //    dbDuty = 0;
            return true;
        }




        /// <summary>
        /// AI1 AI3
        /// </summary>
        /// <param name="RiseStartIndex"></param>
        /// <param name="FallStartIndex"></param>
        /// <param name="outageLv"></param>
        //逻辑上升和下降沿时间间隔 zew
        public void Logic_riseFall(out double RiseStartIndex, out double FallStartIndex, out double outageLv)
        {
            FallStartIndex = 0;RiseStartIndex = 0; outageLv=0;
           
                AnalogWaveform<double>[] data = myReaderAI_Hall1_AI1_3.ReadWaveform(sampls);

                List<double> dataTemp1_AI1_logic1 = new List<double>();
            List<double> dataTemp2_AI3_outage = new List<double>();
           

            AnalogWaveform<double> firstWaveform1 = data[0];  // 获取第一个波形数据 AI1
            AnalogWaveform<double> firstWaveform2 = data[1];//AI3
            if (firstWaveform1.Samples != null)
            {
                for (int i = 0; i < firstWaveform1.SampleCount; i++)
                {
                    dataTemp1_AI1_logic1.Add((double)firstWaveform1.Samples[i].Value);
                }
              
            }

            if (firstWaveform2.Samples != null)
            {
                for (int i = 0; i < firstWaveform2.SampleCount; i++)
                {
                    dataTemp2_AI3_outage.Add((double)firstWaveform2.Samples[i].Value);
                }


                //文件保存
                this.AIDataPrint(dataTemp1_AI1_logic1, "dataTemp1_AI1_logic1");
                this.AIDataPrint(dataTemp2_AI3_outage, "dataTemp2_AI3_outage");
                //计算频率
                double calculateAverage1 = dataTemp1_AI1_logic1.ToArray().Sum() / dataTemp1_AI1_logic1.Count;
                double calculateAverage2 = dataTemp2_AI3_outage.ToArray().Sum() / dataTemp2_AI3_outage.Count;
                // double calculateAverage1 = CalculateAverage(dataTemp1_AI1_logic1.ToArray());//计算平局数值

                List<double> AI1_logic1_Fall = new List<double>();
                List<double> AI1_logic1_Rise = new List<double>();
                List<double> AI3_outag_Fall = new List<double>();
                List<double> AI3_outage_Rise = new List<double>();
                bool bH1 = false;
                bool bL1 = false;
                int nFre1 = 0;
                for (int index = 0; index < dataTemp1_AI1_logic1.Count; index++)
                {
                    if ((dataTemp1_AI1_logic1[index] > calculateAverage1) && (bH1 == false))
                    {
                        bH1 = true;
                        bL1 = false;
                        AI1_logic1_Rise.Add(index);

                    }
                    if ((dataTemp1_AI1_logic1[index] < calculateAverage1) && (bL1 == false))
                    {
                        bL1 = true;
                        bH1 = false;
                        AI1_logic1_Fall.Add(index);

                    }
                }
                bool bH2 = false;
                bool bL2 = false;
                int nFre2 = 0;
                for (int index = 0; index < dataTemp2_AI3_outage.Count; index++)
                {
                    if ((dataTemp2_AI3_outage[index] > calculateAverage2) && (bH2 == false))
                    {
                        bH2 = true;
                        bL2 = false;
                        AI3_outage_Rise.Add(index);

                    }
                    if ((dataTemp2_AI3_outage[index] < calculateAverage2) && (bL2 == false))
                    {
                        bL2 = true;
                        bH2 = false;
                        AI3_outag_Fall.Add(index);

                    }
                }



                int temp1 = Convert.ToInt16( AI3_outag_Fall[3]);
                outageLv = dataTemp2_AI3_outage[temp1]* 11.0;//分压电阻×11
                RiseStartIndex = (AI3_outag_Fall[3]-AI1_logic1_Rise[3] ) * (1 / 5000.0)*1000;//逻辑上升时间相当于200微妙1个数据 取第三组数据作为判断
                FallStartIndex = (AI3_outage_Rise[3]-AI1_logic1_Fall[3] ) * (1 / 5000.0)*1000;//逻辑 下降 相当于200微妙1个数据
            }
        }





       ///// <summary>
       ///// zew
       ///// </summary>
       ///// <param name="DelayTime"></param>
       // public void GetDelayTime(out double DelayTime)
       // {
       //     //DelayTime = 0;

       //     //AnalogWaveform<double>[] data = myReaderAI_Hall.ReadWaveform(sampls);

       //     //List<double> GetDelayTime_AI1_logic1 = new List<double>();
       //     //List<double> GetDelayTime_AI6_ChanneL1 = new List<double>();


       //     //AnalogWaveform<double> firstWaveform1 = data[1];  // 获取第一个波形数据 AI1
       //     //AnalogWaveform<double> firstWaveform2 = data[5];//AI5
       //     //if (firstWaveform1.Samples != null)
       //     //{
       //     //    for (int i = 0; i < firstWaveform1.SampleCount; i++)
       //     //    {
       //     //        GetDelayTime_AI1_logic1.Add((double)firstWaveform1.Samples[i].Value);
       //     //        GetDelayTime_AI6_ChanneL1.Add((double)firstWaveform2.Samples[i].Value);
       //     //    }

       //     //}


       //     //    //文件保存
       //     //    this.AIDataPrint(GetDelayTime_AI1_logic1, "GetDelayTime_AI1_logic1");
       //     //    this.AIDataPrint(GetDelayTime_AI6_ChanneL1, "GetDelayTime_AI6_ChanneL1");
            
            
       // }






        /// <summary>
        /// 获取上升沿、下降沿电压和时间
        /// </summary>
        /// <param name="listData"></param>
        /// <param name="riseTime"></param>
        /// <param name="failTime"></param>
        /// <param name="minMemData">上升沿电压</param>
        /// <param name="maxMemData">下降沿电压</param>
        private void GetRiseDownTime(List<double> listData, ref double riseTime, ref double failTime, out double minMemData, out double maxMemData)
        {
            //把原始数据拷贝一份
            List<double> listDataCopy = new List<double>();
            foreach (double data in listData)
                listDataCopy.Add(data);

            //从小到大排序
            listDataCopy.Sort();

            double allMemData = 0;
            minMemData = 0;//计算最小均值
            maxMemData = 0;//计算最大均值

            allMemData = listDataCopy.Sum() * 1.0 / listDataCopy.Count();//平均数

            int index = 0;
            double sumMin = 0;
            double sumMax = 0;
            for (int i = 0; i < listDataCopy.Count; i++)
            {
                if (listDataCopy[i] < allMemData)
                    sumMin += listDataCopy[i];
                else
                    sumMax += listDataCopy[i];

                if ((listDataCopy[i] > (allMemData - 0.1)) && (listDataCopy[i] < (allMemData + 0.1)))
                    index = i;
            }




            minMemData = sumMin * 1.0 / (index * 1.0);
            maxMemData = sumMax * 1.0 / (listDataCopy.Count - index * 1.0);



            int indexMaxRise = 0;
            int indexMinRise = 0;
            int indexMaxFall = 0;
            int indexMinFall = 0;

            //算法步进，类似于10%~90%
            double stepValue = 0.1;
            bool bRiseFlag = false;     //上升沿标志
            bool bFallFlag = false;     //下降沿标志

            int state = 1;
            List<double> listUpDownIndex = new List<double>();

            for (int i = 0; i < listData.Count; i++)
            {

                switch (state)
                {
                    case 1:
                        if (listData[i] < maxMemData - stepValue)
                        {
                            listUpDownIndex.Add(i);
                            state = 2;
                        }
                        break;
                    case 2:
                        if (listData[i] < minMemData + stepValue)
                        {
                            listUpDownIndex.Add(i);
                            state = 3;
                        }
                        break;
                    case 3:
                        if (listData[i] > minMemData + stepValue)
                        {
                            listUpDownIndex.Add(i);
                            state = 4;
                        }
                        break;
                    case 4:
                        if (listData[i] > maxMemData - stepValue)
                        {
                            listUpDownIndex.Add(i);
                            state = 1;
                        }
                        break;
                }



             
            }


            AIDataPrint(listUpDownIndex, "listUpDownIndex");


            List<double> riseTimeList = new List<double>();
            List<double> fallTimeList = new List<double>();

            for (int i = 0; i < listUpDownIndex.Count; i += 4)
            {
                if (riseTimeList.Count>i+1)
                {
riseTimeList.Add(listUpDownIndex[i + 1] - listUpDownIndex[i]);
                }
                
                if (listUpDownIndex.Count > i+4)
                {
                    fallTimeList.Add(listUpDownIndex[i + 3] - listUpDownIndex[i + 2]);
                }
                
            }
            AIDataPrint(riseTimeList, "riseTimeList");
            AIDataPrint(fallTimeList, "fallTimeList");


            riseTime = riseTimeList.Sum() * 1.0 / riseTimeList.Count() * (1 / 5000.0) * 1000.0;        //1000代表1000ms， 10000代表10K采样率
            failTime = fallTimeList.Sum() * 1.0 / fallTimeList.Count() * (1 / 5000.0) * 1000.0;
            minMemData = minMemData * 11.0 / 31.0;//计算最大上升沿电压
            maxMemData = maxMemData *11.0/31.0;//计算最小上升延电压


        }

        public void AIDataPrint(List<double> listData, string flag)
        {
            if (listData == null)
                return;
            //创建日志文件
            string filename = AppDomain.CurrentDomain.BaseDirectory + "log//" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + flag + ".csv";
            if (!File.Exists(filename))
            {
                FileStream stream = File.Create(@filename);//创建日志文件

                stream.Close();
            }

            //写入日志文件
            FileStream f = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(f);

            //sw.WriteLine(str);
            for (int array = 0; array < listData.Count; array++)
                sw.WriteLine(string.Format("{0:f2},", listData[array]));

            sw.Flush();
            sw.Close();
            f.Close();
        }


        private bool AI_CreateCur(string devname, string aich)
        {
            bool flag = false;
            try
            {
                // Create a new task
                this.taskAI_Cur1 = new Task();

                // Initialize local variables
                double sampleRate = 32768;
                double rangeMinimum = -10;
                double rangeMaximum = 10;
                //int samplesPerChannel = 100000;"/ai2"

                // Create a channel
                taskAI_Cur1.AIChannels.CreateVoltageChannel(devname + "/" + aich, "",
                    (AITerminalConfiguration.Rse), rangeMinimum, rangeMaximum, AIVoltageUnits.Volts);

                // Configure timing specs    
                taskAI_Cur1.Timing.ConfigureSampleClock("", sampleRate, SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples, this.nCurSamples);

                // Verify the task
                taskAI_Cur1.Control(TaskAction.Verify);

                // Read the data
                myReaderAI_Cur1 = new AnalogMultiChannelReader(taskAI_Cur1.Stream);

                flag = true;
                //AnalogWaveform<double>[] data = myReaderAI_Hall1.ReadWaveform(samplesPerChannel);

                //dataToDataTable(data, ref dataTable);
            }
            catch (DaqException exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                //taskAI_Hall1.Dispose();
            }
            return flag;
        }
        public bool AI_GetCur_1(ref List<double> dataCur)
        {
            try
            {
                AnalogWaveform<double>[] data = myReaderAI_Cur1.ReadWaveform(this.nCurSamples);

                if (dataCur == null)
                    dataCur = new List<double>();
                foreach (AnalogWaveform<double> waveform in data)
                    for (int sample = 0; sample < waveform.Samples.Count; ++sample)
                        dataCur.Add((double)waveform.Samples[sample].Value * 6);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private int _nSamples = 10;
        /// <summary>
        /// 获取P1电压
        /// </summary>
        /// <returns></returns>
        public bool Logic_out_AI00(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai0", this._nSamples, ref aidata);
            aidata *= 3;
            return bResult;
        }
        /// <summary>
        /// 获取P2电压
        /// </summary>
        /// <returns></returns>
        public bool Logic_AI01(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai1", this._nSamples, ref aidata);
            aidata *= 3;
            return bResult;
        }
        /// <summary>
        /// 获取电流值
        /// </summary>
        /// <returns></returns>
        public bool AI_Start_AI02(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai2", this._nSamples, ref aidata);
            
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool AI_CC_AI03(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai3", this._nSamples, ref aidata);
            aidata *= 1;
            return bResult;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool AI_RES_AI4(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai4", this._nSamples, ref aidata);
            aidata *= 1;
            aidata = Math.Abs(aidata - 2.5) / 2.5 * 200 * 9.8;
            return bResult;
        }
        /// <summary>
        /// 获取位移传感器
        /// </summary>
        /// <returns></returns>
        public bool LSS1_AD(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai5", this._nSamples, ref aidata);
            aidata *= 25;
            return bResult;
        }

        /// <summary>
        /// 获取类比输出1
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool PCH1_IN_AD_AI06(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai6", this._nSamples, ref aidata);
            aidata *= 1;
            return bResult;
        }

        public bool AI07_out_5V(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai7", this._nSamples, ref aidata);
            aidata *= 1;
            return bResult;
        }

        /// <summary>
        /// 伺服电机正转
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool GetVol_Forward(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai8", this._nSamples, ref aidata);
            return (aidata > 3);
        }

        /// <summary>
        /// 伺服电机反转
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool GetVol_Rev(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai9", this._nSamples, ref aidata);
            return (aidata > 3);
        }

        /// <summary>
        /// 工装接近开关
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool GetVol_LK2(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai15", this._nSamples, ref aidata);
            return (aidata > 3);
        }

        /// <summary>
        /// 滑块接近开关
        /// </summary>
        /// <param name="aidata"></param>
        /// <returns></returns>
        public bool GetVol_LK3(ref double aidata)
        {
            bool bResult = this.GetAIData(this.devName + "/ai14", this._nSamples, ref aidata);
            return (aidata > 3);
        }
        #endregion

        #region AO功能实现
        public void AO_WriteCF1(double value)
        {
            this.WriteVol(this.devName + "/ao0", value);
        }
        public void AO_WriteCF2(double value)
        {
            this.WriteVol(this.devName + "/ao1", value);
        }
        private void WriteVol(string aoport, double data)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (Task myTask = new Task())
                {
                    myTask.AOChannels.CreateVoltageChannel(aoport, "aoChannel",
                        -10, 10,
                        AOVoltageUnits.Volts);
                    AnalogSingleChannelWriter writer = new AnalogSingleChannelWriter(myTask.Stream);
                    writer.WriteSingleSample(true, data);
                }
            }
            catch (DaqException ex)
            {
                //MessageBox.Show(ex.Message);
            }
            Cursor.Current = Cursors.Default;
        }

        internal void Rest_DO_02(bool v)
        {
            throw new NotImplementedException();
        }

        #endregion

        //#region PWM功能实现


        //#endregion

        //#region CI功能实现
        //private double measTime = 0.0001;
        //public double speed2;
        //public double speed3;


        //private double PeriodDataProcess(double[] data)
        //{
        //    double mem = 0;
        //    double sum = 0;
        //    List<double> period = new List<double>();
        //    List<double> period2 = new List<double>();
        //    List<double> save = new List<double>();
        //    //this.WriteFile(data);
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        if (data[i] == this.measTime)
        //        {
        //            period.Add(i * this.measTime);
        //            save.Add(i * this.measTime);
        //        }
        //    }
        //    save.Add(period.Count);
        //    save.Add(period2.Count);
        //    for (int i = 1; i < period.Count; i++)
        //    {
        //        period2.Add(period[i] - period[i - 1]);
        //        save.Add(period[i] - period[i - 1]);
        //    }
        //    if (period.Count % 2 == 0)
        //    {
        //        for (int j = 0; j < period2.Count; j++)
        //            sum += period2[j];
        //        save.Add(sum);
        //        if (sum != 0)
        //            mem = period2.Count * 60.0 / sum;
        //    }
        //    else
        //    {
        //        for (int j = 0; j < period2.Count - 1; j++)
        //            sum += period2[j];
        //        save.Add(sum);
        //        if (sum != 0)
        //            mem = (period2.Count - 1) * 60.0 / sum;
        //    }
        //    save.Add(mem);
        //    //this.WriteFile(save.ToArray(), "S");
        //    return mem;
        //}
        //private void WriteFile(double[] data, string header = "")
        //{
        //    //string path = System.Environment.CurrentDirectory + string.Format("\\log\\Data\\{0}.xml", header + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
        //    //ObjectIO.exportTo(path, data, typeof(double[]));
        //}

        //#endregion


      
    }
}
