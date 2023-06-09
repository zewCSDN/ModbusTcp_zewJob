

using _20230509.MySRHMethod;
using MySRHMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20230509
{
    public class Device
    {

      

        public NI_USB6002[] NI_usb6002 = new NI_USB6002[4];
    
        public MasterRtu[] master_C=new MasterRtu[4];
        public MasterRtu[] master_SW = new MasterRtu[4];
        public  SwichPCB_COMPLEX[] SwichPCB = new SwichPCB_COMPLEX[4];
        public SwichPCB_COMPLEX[] CPCB = new SwichPCB_COMPLEX[4];
        public CPWR401L[] pwr401l = new CPWR401L[4];//精密电源4个
        public CPLZ50FLoad[] plz50fLoad = new CPLZ50FLoad[4];//电子负载实例化4个
        public ModbusTCPClient modbusTCPClient = new ModbusTCPClient();//PLC实例化1个
      public  UTA0402_lin []uta0402_Lin = new UTA0402_lin[4];
        public UartTest[]  uartTests = new  UartTest[4];
        //public CRbinPCB[] rbinPCB = new CRbinPCB[2];

        //public UTA0503_1[] UTA0503 = new UTA0503_1[2];

        //public CLH_04 CLH_04;
    }



    public class ProductPara
    {

        public List<productInfo> pInfo = new List<productInfo>();
        public ProductPara()
        {
            for (int i = 0; i < this.pInfo.Count; i++)
            {
                this.pInfo[i] = new productInfo();
            }
        }
       
    }

    public class productInfo
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
        public double currStaticMin_18V = 0;
        public double currStaticMax_18V = 0;




        //【2】****CC1恒流输出
        public double CC1LED1Min_9V = 0;
        public double CC1LED1Max_9V = 0;
        public double CC1LED2Min_9V = 0;
        public double CC1LED2Max_9V = 0;
        public double CC1LED3Min_9V = 0;
        public double CC1LED3Max_9V = 0;
        public double CC1LED4Min_9V = 0;
        public double CC1LED4Max_9V = 0;


        public double CC1LED1Min_14V = 0;
        public double CC1LED1Max_14V = 0;
        public double CC1LED2Min_14V = 0;
        public double CC1LED2Max_14V = 0;
        public double CC1LED3Min_14V = 0;
        public double CC1LED3Max_14V = 0;
        public double CC1LED4Min_14V = 0;
        public double CC1LED4Max_14V = 0;

        public double CC1LED1Min_16V = 0;
        public double CC1LED1Max_16V = 0;
        public double CC1LED2Min_16V = 0;
        public double CC1LED2Max_16V = 0;
        public double CC1LED3Min_16V = 0;
        public double CC1LED3Max_16V = 0;
        public double CC1LED4Min_16V = 0;
        public double CC1LED4Max_16V = 0;

        //CC2恒流输出
        public double CC2LED1Min_9V = 0;
        public double CC2LED1Max_9V = 0;
        public double CC2LED2Min_9V = 0;
        public double CC2LED2Max_9V = 0;
        public double CC2LED3Min_9V = 0;
        public double CC2LED3Max_9V = 0;
        public double CC2LED4Min_9V = 0;
        public double CC2LED4Max_9V = 0;

        public double CC2LED1Min_14V = 0;
        public double CC2LED1Max_14V = 0;
        public double CC2LED2Min_14V = 0;
        public double CC2LED2Max_14V = 0;
        public double CC2LED3Min_14V = 0;
        public double CC2LED3Max_14V = 0;
        public double CC2LED4Min_14V = 0;
        public double CC2LED4Max_14V = 0;

        public double CC2LED1Min_16V = 0;
        public double CC2LED1Max_16V = 0;
        public double CC2LED2Min_16V = 0;
        public double CC2LED2Max_16V = 0;
        public double CC2LED3Min_16V = 0;
        public double CC2LED3Max_16V = 0;
        public double CC2LED4Min_16V = 0;
        public double CC2LED4Max_16V = 0;

        //【3】5V输出精度
        public double out5VMin = 0;
        public double out5VMax = 0;
        //【4】单通道逻辑输入电流
        public double Logic_1Min = 0;
        public double Logic_1Max = 0;
        public double Logic_2Min = 0;
        public double Logic_2Max = 0;
        public double Logic_3Min = 0;
        public double Logic_3Max = 0;
        public double Logic_4Min = 0;
        public double Logic_4Max = 0;
        public double Logic_5Min = 0;
        public double Logic_5Max = 0;
        public double Logic_6Min = 0;
        public double Logic_6Max = 0;

        //【5】占空比
        public double Logic_2DutyMin = 0;
        public double Logic_2DutyMax = 0;
        public double Logic_3DutyMin = 0;
        public double Logic_3DutyMax = 0;
        public double Logic_4DutyMin = 0;
        public double Logic_4DutyMax = 0;
        public double Logic_5DutyMin = 0;
        public double Logic_5DutyMax = 0;
        public double Logic_6DutyMin = 0;
        public double Logic_6DutyMax = 0;

        //【6】报警信号参数
        public double outage_riseTimeMin = 0;
        public double outage_riseTimeMax = 0;
        public double outage_fallTimeMin = 0;
        public double outage_fallTimeMax = 0;
        //报警信号低电平
        public double outageLvMIin = 0;
        public double outageLvMax = 0;

        //【7】 HSSS 参数设置
        public double HSSS1_riseLVMin = 0;//上升沿电压
        public double HSSS1_riseLVMax = 0;
        public double HSSS1_fallLVMin = 0;//下降延电压
        public double HSSS1_fallLVMax = 0;

        public double HSSS1_riseTimeMin = 0;
        public double HSSS1_riseTimeMax = 0;

        public double HSSS1_fallTimeMin = 0;
        public double HSSS1_fallTimeMax = 0;

        //【7】 LSSS 参数设置
        public double LSSS1_riseLvMin = 0;
        public double LSSS1_riseLvMax = 0;
        public double LSSS1_fallLvMin = 0;
        public double LSSS1_fallLvMax = 0;
        public double LSSS1_riseTimeMin = 0;
        public double LSSS1_riseTimeMax = 0;
        public double LSSS1_fallTimeMin = 0;
        public double LSSS1_fallTimeMax = 0;

        public double LSSS2_riseLvMin = 0;
        public double LSSS2_riseLvMax = 0;
        public double LSSS2_fallLvMin = 0;
        public double LSSS2_fallLvMax = 0;
        public double LSSS2_riseTimeMin = 0;
        public double LSSS2_riseTimeMax = 0;
        public double LSSS2_fallTimeMin = 0;
        public double LSSS2_fallTimeMax = 0;

        public double LSSS3_riseLvMin = 0;
        public double LSSS3_riseLvMax = 0;
        public double LSSS3_fallLvMin = 0;
        public double LSSS3_fallLvMax = 0;
        public double LSSS3_riseTimeMin = 0;
        public double LSSS3_riseTimeMax = 0;
        public double LSSS3_fallTimeMin = 0;
        public double LSSS3_fallTimeMax = 0;

        public double LSSS4_riseLvMin = 0;
        public double LSSS4_riseLvMax = 0;
        public double LSSS4_fallLvMin = 0;
        public double LSSS4_fallLvMax = 0;
        public double LSSS4_riseTimeMin = 0;
        public double LSSS4_riseTimeMax = 0;
        public double LSSS4_fallTimeMin = 0;
        public double LSSS4_fallTimeMax = 0;

        //【8】RBin电阻
        public double RBinS1Min = 0;
        public double RBinS1Max = 0;
        public double RBinS2Min = 0;
        public double RBinS2Max = 0;
        public double RBinS3Min = 0;
        public double RBinS3Max = 0;
        public double RBinS4Min = 0;
        public double RBinS4Max = 0;

        public double NTC1Min = 0;
        public double NTC1Max = 0;
        public double NTC2Min = 0;
        public double NTC2Max = 0;

        public bool UART_Test = false;//功能测试
        //启动延时设置
        public double startSleepMin = 0;
        public double startSleepMax = 0;


        #endregion






    }

    public class TestData
    {
        public double curStatic = 0;

        public double out5V = 0;

        //版本号
        public string AppPN = "";
        public string AppVer = "";
        public string CaliPN = "";
        public string CaliVer = "";
        public string FBLPN = "";
        public string FBLVer = "";

        public string Year = "";
        public string DayofYear = "";

        public double CC1LED1 = 0;
        public double CC1LED2 = 0;
        public double CC1LED3 = 0;
        public double CC1LED4 = 0;
        public double CC1LED5 = 0;
        public double CC1LED6 = 0;
        public double CC1LED7 = 0;
        public double CC1LED8 = 0;
        public double CC1LED9 = 0;
        public double CC1LED10 = 0;
        public double CC1LED11 = 0;
        public double CC1LED12 = 0;

        public double CC2LED1 = 0;
        public double CC2LED2 = 0;
        public double CC2LED3 = 0;
        public double CC2LED4 = 0;
        public double CC2LED5 = 0;
        public double CC2LED6 = 0;
        public double CC2LED7 = 0;
        public double CC2LED8 = 0;
        public double CC2LED9 = 0;
        public double CC2LED10 = 0;
        public double CC2LED11 = 0;
        public double CC2LED12 = 0;

        public double LSD1HV = 0;
        public double LSD1LV = 0;
        public double LSD2HV = 0;
        public double LSD2LV = 0;
        public double LSD3HV = 0;
        public double LSD3LV = 0;
        public double LSD4HV = 0;
        public double LSD4LV = 0;
        public double LSD5HV = 0;
        public double LSD5LV = 0;
        public double LSD6HV = 0;
        public double LSD6LV = 0;

        public double HSD1HV = 0;
        public double HSD1LV = 0;
        public double HSD2HV = 0;
        public double HSD2LV = 0;
        public double HSD3HV = 0;
        public double HSD3LV = 0;
        public double HSD4HV = 0;
        public double HSD4LV = 0;
        public double HSD5HV = 0;
        public double HSD5LV = 0;
        public double HSD6HV = 0;
        public double HSD6LV = 0;



    }


    public class Para
    {
        public string ip = "192.168.1.200";
        public string port = "502";
        public string p1 = "A014C009";      //高配S06
        public string p2 = "A015C010";      //高配S07
        public string p3 = "A014C009";      //低配S06
        public string p4 = "A015C010";      //低配S07
        public string p5 = "A016C010";      //高配S08
        public string p6 = "A016C010";      //低配S08
        public string p7 = "A016C010";      //EP33AH
        public string p8 = "A016C010";      //EP33AL
        public string p9 = "A016C010";      //EP33BL
    }


}
