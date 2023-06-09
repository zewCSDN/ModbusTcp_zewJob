using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20230509.MySRHMethod
{
    public class Product_MLD
    {

        //产品名称
        public string strProductName;

        //文件路径
        public string FlashDrvPath = "";
        public string AppFilePath = "";
        public string CaliFilePath = "";
        public string AnimeFilePath = "";

        //版本号
        public string AppPN = "";
        public string AppVer = "";
        public string CaliPN = "";
        public string CaliVer = "";
        public string AnimePN = "";
        public string AnimeVer = "";
        public string FBLPN = "";
        public string FBLVer = "";


        #region  设置检测参数
        //【1】25°18V输出精度
        public double currStatic_18V = 0;





        //【2】****CC1恒流输出
        public double CC1LED1_9V = 0;

        public double CC1LED2_9V = 0;

        public double CC1LED3_9V = 0;

        public double CC1LED4_9V = 0;



        public double CC1LED1_14V = 0;

        public double CC1LED2_14V = 0;

        public double CC1LED3_14V = 0;

        public double CC1LED4_14V = 0;


        public double CC1LED1_16V = 0;

        public double CC1LED2_16V = 0;

        public double CC1LED3_16V = 0;

        public double CC1LED4_16V = 0;


        //CC2恒流输出
        public double CC2LED1_9V = 0;

        public double CC2LED2_9V = 0;

        public double CC2LED3_9V = 0;

        public double CC2LED4_9V = 0;


        public double CC2LED1_14V = 0;

        public double CC2LED2_14V = 0;

        public double CC2LED3_14V = 0;

        public double CC2LED4_14V = 0;

        public double CC2LED1_16V = 0;

        public double CC2LED2_16V = 0;

        public double CC2LED3_16V = 0;

        public double CC2LED4_16V = 0;


        //【3】5V输出精度
        public double out5V = 0;

        //【4】单通道逻辑输入电流
        public double Logic_1 = 0;

        public double Logic_2 = 0;

        public double Logic_3 = 0;

        public double Logic_4 = 0;

        public double Logic_5 = 0;

        public double Logic_6 = 0;


        //【5】占空比
        public double Logic_2Duty = 0;

        public double Logic_3Duty = 0;

        public double Logic_4Duty = 0;

        public double Logic_5Duty = 0;

        public double Logic_6Duty = 0;


        //【6】报警信号参数
        public double outage_riseTime = 0;

        public double outage_fallTime = 0;

        //报警信号低电平
        public double outageLv = 0;


        //【7】 HSSS 参数设置
        public double HSSS1_riseLV = 0;//上升沿电压

        public double HSSS1_fallLV = 0;//下降延电压


        public double HSSS1_riseTime = 0;


        public double HSSS1_fallTime = 0;


        //【7】 LSSS 参数设置
        public double LSSS1_riseLv = 0;

        public double LSSS1_fallLv = 0;

        public double LSSS1_riseTime = 0;

        public double LSSS1_fallTime = 0;


        public double LSSS2_riseLv = 0;

        public double LSSS2_fallLv = 0;

        public double LSSS2_riseTime = 0;

        public double LSSS2_fallTime = 0;


        public double LSSS3_riseLv = 0;

        public double LSSS3_fallLv = 0;

        public double LSSS3_riseTime = 0;

        public double LSSS3_fallTime = 0;


        public double LSSS4_riseLv = 0;

        public double LSSS4_fallLv = 0;

        public double LSSS4_riseTime = 0;

        public double LSSS4_fallTime = 0;

        //【8】RBin电阻
        public double RBinS1 = 0;

        public double RBinS2 = 0;

        public double RBinS3 = 0;

        public double RBinS4 = 0;


        public double NTC1 = 0;

        public double NTC2 = 0;
        public bool Uart_Test = false;
        //启动延时设置
        public double startSleep = 0;


        #endregion
        public bool isDownLoad_ok = false;
        public bool TestResult = false;
    }
        
    }
