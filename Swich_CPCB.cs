using MySRHMethod;
using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace _20230509.MySRHMethod
{
    public class SwichPCB_COMPLEX
    {
        public SwichPCB_COMPLEX() { }


        /// <summary>
        /// 电压休眠切换
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <returns></returns>
        public bool Voltage_sleep_switching(MasterRtu masterRtu)
        {
            if (masterRtu != null)
            {

                masterRtu.WriteKeepSingleReg(1, 10, 1);
                return true;
            }
            else
            {
                return false;
            }




        }




        /// <summary>
        /// PWM1通道 频率HZ 10-500Hz
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <returns></returns>
        public bool PWMchannel_HZ(MasterRtu masterRtu, int pwmChannelID, ushort freqSet)
        {
            if (masterRtu != null)
            {

                int address = 9 + pwmChannelID * 2;//地址偏移
                masterRtu.WriteKeepSingleReg(1, address, freqSet);//}

                return true;



            }
            else
            {
                return false;
            }





        }

        public bool RBinRest(MasterRtu masterRtu,int length)
        {
            if (masterRtu!=null)
            {
                for (int i = 0; i < length; i++)
                {
                    masterRtu.WriteKeepSingleReg(1, 51+i, 0);//}
                }
              return true;
            }
        else { return false; }
        
        
        }

        /// <summary>
        /// 频率占空比清空
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <returns></returns>
        public bool Hz_dutyRest(MasterRtu masterRtu)
        {
            if (masterRtu != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    masterRtu.WriteKeepSingleReg(1, 11 + i, 0);//}
                }
                return true;
            }
            else { return false; }


        }
        /// <summary>
        /// 占空比设置 0-10000对应 1-100%
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="dutyChannelID"></param>
        /// <param name="freqSet"></param>
        /// <returns></returns>
        public bool DutyCycle_Set(MasterRtu masterRtu, int dutyChannelID, ushort freqSet)
        {

            if (masterRtu != null)
            {
               
                int address = 10 + dutyChannelID * 2;//地址偏移
                return  masterRtu.WriteKeepSingleReg(1, address, freqSet);//}

              


            }
            else
            {
                return false;
            }


        }
        /// <summary>
        /// 读取 battery 实时电压原始数据 ADC-PC0mv
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="channelD"></param>
        /// <returns></returns>
        public ushort BatteryRealvoltage(MasterRtu masterRtu)
        {
            if (masterRtu != null)
            {

                byte[] result = masterRtu.ReadKeepReg(1, 31, 1);
                if (result != null)
                {
                    Array.Reverse(result);
                    ushort data = BitConverter.ToUInt16(result, 0);
                    return data;
                }
             else return 0;
              
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 获取PWM电流 PWM1-6 通道
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="dutyChannelID"></param>
        /// <param name="freqSet"></param>
        /// <returns></returns>
        public ushort GetPWMCurrent(MasterRtu masterRtu, int pwmChannelID)
        {
            if (masterRtu != null)
            {

                int address = 31 + pwmChannelID;//地址偏移
                byte[] result = masterRtu.ReadKeepReg(1, address, 1);//}
                if (result != null)
                {
                    Array.Reverse(result);
                    ushort data = BitConverter.ToUInt16(result, 0); 
                    return data;
                }
               else { return 0; }
            }

            else
            {
                return 0;
            }

        }





        /// <summary>
        /// 获取通道电流 current1-4 通道
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="currentChannelID"></param>
        /// <returns></returns>
        public ushort GetChannelCurrent(MasterRtu masterRtu, int currentChannelID)
        {
            if (masterRtu != null)
            {

                int address = 37 + currentChannelID;//地址偏移
                byte[] result = masterRtu.ReadKeepReg(1, address, 1);//}
                if (result!=null)
                {
                    Array.Reverse(result);
                    ushort data = BitConverter.ToUInt16(result, 0);
                return data;
                }
               else { return 0; }
            }

            else
            {
                return 0;
            }

        }

       
        /// <summary>
        /// RBIN1Swich 电阻切换 
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="rbinChanelID">CHANNEL 1-6</param>
        /// <param name="value">1 2 4 8  对应电阻2k 1.6k 1k 820Ω</param>
        /// <returns></returns>
        public bool RBIN1Swich( MasterRtu masterRtu, int rbinChanelID,ushort value)
        {

            if (masterRtu != null)
            {
                 int address = 50 + rbinChanelID;//地址偏移
          //bool ret=    masterRtu.WriteKeepSingleReg(1, 0, value);
                bool ret = masterRtu.WriteKeepSingleReg(1, address, value);

                return ret;

            }
            else { return false; }
        }


        /// <summary>
        /// RBIN1Swich 电阻切换 
        /// </summary>
        /// <param name="masterRtu"></param>
        /// <param name="rbinChanelID">CHANNEL 1-6</param>
        /// <param name="value">1 2 4 8  对应电阻10k 2k 1k 820Ω</param>
        /// <returns></returns>
        public bool NTC_Swich(MasterRtu masterRtu, int ntcChanelID, ushort value)
        {

            if (masterRtu != null)
            {
                int address = 56 + ntcChanelID;//地址偏移
                masterRtu.WriteKeepSingleReg(1, address, value);

                return true;

            }
            else { return false; }
        }
        public bool RBIN_NTCRest(MasterRtu masterRtu)
        {

            if (masterRtu != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    int temp = 51 + i;
                    masterRtu.WriteKeepSingleReg(1,temp, 0);
                }
            
              

                return true;

            }
            else { return false; }

        }

    }

}





