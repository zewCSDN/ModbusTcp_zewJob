using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;
using System.Threading;


using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MySRHMethod;
using NationalInstruments.UI;
using NationalInstruments.DAQmx;
using USB2XXX;
using System.IO.Ports;
using _20230509.MySRHMethod;
using NationalInstruments.DataInfrastructure;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using System.Data.SqlTypes;

namespace _20230509
{
    public partial class FrmMain : Form
    {
        System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
        public ProductPara _pp = new ProductPara();//MLD产品参数实例化
        private DataGridView[] dgv = new DataGridView[4];//控件实例化
        public Product_MLD[] pro_MLD = new Product_MLD[4];
        public TempVariable tempVariable = new TempVariable();//临时变量
        private System.Windows.Forms.TextBox[] tbInfo = new System.Windows.Forms.TextBox[4];//显示仪器状态
        private System.Windows.Forms.Button[] btnResult = new System.Windows.Forms.Button[4];//计算时间
        private Int32[] UTA0402_ID_DevHandles = new Int32[20];//设备USB_LIN
        private Device device = new Device();
        private int _nCurrIndex = 0;
        private Para para = new Para();//实例化产品
        string path = "";

        private byte[] GetPlcData = new byte[1024];
        private System.Collections.Concurrent.ConcurrentQueue<bool> taskConQueue = new System.Collections.Concurrent.ConcurrentQueue<bool>();
        private System.Threading.Timer sttSchedule = null;              //任务调度 时间定时器 

        private System.Timers.Timer timerPro_1 = new System.Timers.Timer();
        private System.Timers.Timer timerPro_2 = new System.Timers.Timer();

        private System.Timers.Timer timerPro_3 = new System.Timers.Timer();
        private System.Timers.Timer timerPro_4 = new System.Timers.Timer();
        //文件解析类
        private HexFlash_3 Hexflash_3 = new HexFlash_3();
        private HexFlash_3_1 Hexflash_3_1 = new HexFlash_3_1();

        private HexFlash_MLD Hexflash_MLD = new HexFlash_MLD();
        private HexFlash_MLD_1 Hexflash_MLD_1 = new HexFlash_MLD_1();
        //private System.Collections.Concurrent.ConcurrentQueue<Relay> taskRelayQueue = new System.Collections.Concurrent.ConcurrentQueue<Relay>();
        //private System.Threading.Timer RelaySchedule = null;
        private System.Threading.Timer timercon = null;
     //   private System.Threading.Timer timerCycleManager = null;
        private System.Threading.Timer timerCycleRead = null;
        private System.Threading.Timer timerCycleHeartBeat = null;




        //主线程定义
        private bool bReadFlag = true;
       
        private bool startFlag1 = false;
        private bool startFlag2 = false;
        private int sumNumber = 0;
        private int OKNumber = 0;

        private Thread threadMain1;
        private Thread threadMain2;
        private Thread threadMain3;
        private Thread threadMain4;
        private object proMLD;

        public FrmMain()
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
            {
                if (control is System.Windows.Forms.Button)
                {

                    control.Click += Control_Click; ;

                }


            }
            InitPara(ref para);//文件加载
            LoadProductPara();//加载产品

        }



        #region dgv 加载
        private void InitDatagridViewMLD(ref DataGridView dgv)
        {
            int nIndex = 0;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "18V休眠电流";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].currStaticMin_18V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].currStaticMax_18V;
            #region 测试1

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC1测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED1Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED1Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC1测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED2Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED2Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC1测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED3Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED3Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC1测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED4Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED4Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC1测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED1Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED1Max_14V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC1测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED2Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED2Max_14V;
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC1测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED3Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED3Max_14V;
            nIndex = dgv.Rows.Add();

            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC1测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED4Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED4Max_14V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC1测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED1Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED1Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC1测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED2Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED2Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC1测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED3Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED3Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC1测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC1LED4Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC1LED4Max_16V;

          



            #endregion


            #region 测试2  



            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC2测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED1Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED1Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC2测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED2Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED2Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC2测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED3Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED3Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "9V CC2测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED4Min_9V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED4Max_9V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC2测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED1Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED1Max_14V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC2测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED2Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED2Max_14V;
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC2测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED3Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED3Max_14V;
            nIndex = dgv.Rows.Add();

            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "14V CC2测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED4Min_14V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED4Max_14V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC2测试：LED1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED1Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED1Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC2测试：LED2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED2Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED2Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC2测试：LED3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED3Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED3Max_16V;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "16V CC2测试：LED4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CC2LED4Min_16V;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].CC2LED4Max_16V;

            #endregion





            //

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "输出5V精度测试";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].out5VMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].out5VMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic1";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_1Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_1Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_2Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_2Max;
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_3Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_3Max;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_4Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_4Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic5";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_5Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_5Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "单通道逻辑输入电流 Logic6";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_6Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_6Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "占空比 Logic2";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_2DutyMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_2DutyMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "占空比 Logic3";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_3DutyMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_3DutyMax;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "占空比 Logic4";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_4DutyMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_4DutyMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "占空比 Logic5";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_5DutyMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_5DutyMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "占空比 Logic6";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].Logic_6DutyMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].Logic_6DutyMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "报警信号 上升沿ms";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].outage_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].outage_riseTimeMax;
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "报警信号 下降沿ms";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].outage_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].outage_fallTimeMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "报警信号 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].outageLvMIin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].outageLvMax;



            #region  7 HSSS&LSSS
            //

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "HSSS1 下降沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].HSSS1_fallLVMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].HSSS1_fallLVMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "HSSS1 上升沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].HSSS1_riseLVMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].HSSS1_riseLVMax;





            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "HSSS1 下降沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "HSSS1 上升沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMax;


            //LSSS
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS1 下降沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS1_fallLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS1_fallLvMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS1 上升沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS1_riseLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS1_riseLvMax;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS1 下降沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMax;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS1 上升沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMax;


            //lsss2

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS2 下降沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS2_fallLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS2_fallLvMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS2 上升沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS2_riseLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS2_riseLvMax;



            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS2 下降沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS2 上升沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMax;




            //lLSSS3
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS3 下降沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS3_fallLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS3_fallLvMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS3 上升沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS3_riseLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS3_riseLvMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS3 下降沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS3 上升沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMax;


            //lSSS4
            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS4 下降沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS4_fallLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS4_fallLvMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS4 上升沿电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS4_riseLvMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS4_riseLvMax;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS4 下降沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMax;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LSSS4 上升沿时间";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMax;



            #endregion

            #region 测试8 分并电阻 &NTC 电压测试

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "RBin S1 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].RBinS1Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].RBinS1Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "RBin S2 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].RBinS2Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].RBinS2Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "RBin S3 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].RBinS3Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].RBinS3Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "RBin S4 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].RBinS4Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].RBinS4Max;


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "NTC-1 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].NTC1Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].NTC1Max;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "NTC-2 电压";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].NTC1Min;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].NTC1Max;




            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "Uart功能测试";
            dgv.Rows[nIndex].Cells[3].Value = "通讯测试OK";
            dgv.Rows[nIndex].Cells[4].Value = "通讯测试NG";


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "启动延时";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].startSleepMin;
            dgv.Rows[nIndex].Cells[4].Value = _pp.pInfo[_nCurrIndex].startSleepMax;

            #endregion

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LIN读取FBL版本号";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].FBLPN + " " + _pp.pInfo[_nCurrIndex].FBLVer;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LIN读取APP版本号";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].AppPN + " " + _pp.pInfo[_nCurrIndex].AppVer;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LIN读取CALI版本号";
            dgv.Rows[nIndex].Cells[3].Value = _pp.pInfo[_nCurrIndex].CaliPN + " " + _pp.pInfo[_nCurrIndex].CaliVer;

            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "LIN刷写文件";


            nIndex = dgv.Rows.Add();
            dgv.Rows[nIndex].Cells[0].Value = string.Format("{0}", nIndex);
            dgv.Rows[nIndex].Cells[1].Value = "刷写是否成功";


            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;   //自动调整列宽


        }
        #endregion

        #region 程序载入配置载入等
        private void LoadProductPara()
        {
            string strCurrentPath = System.AppDomain.CurrentDomain.BaseDirectory + "sys.xml";
            if (!File.Exists(strCurrentPath))
            {
                MessageBox.Show("sys.xml不存在，请查找原因");
                return;
            }

            this._pp = ObjectIO.importFrom(strCurrentPath, typeof(ProductPara)) as ProductPara;


            this.comboBoxProduct.Items.Clear();
            for (int i = 0; i < this._pp.pInfo.Count; i++)
            {
                if ((this._pp.pInfo[i].strProductName == null) || (this._pp.pInfo[i].strProductName == ""))
                    break;
                this.comboBoxProduct.Items.Add(this._pp.pInfo[i].strProductName);
            }
            if (this.comboBoxProduct.Items.Count > 0)
            {
                this.comboBoxProduct.SelectedIndex = 0;
            }

        }

        #endregion
        #region 文件加载

        private void InitPara(ref Para para)
        {
            path = Application.StartupPath + "\\Para.xml";
            try
            {
                para = (Para)ObjectIO.importFrom(path, typeof(Para));
                if (para == null)
                {
                    para = new Para();
                    ObjectIO.exportTo(path, para, typeof(Para));
                }
            }
            catch
            {
                para = new Para();
                ObjectIO.exportTo(path, para, typeof(Para));
            }
        }
        #endregion
        private void Control_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button button = sender as System.Windows.Forms.Button;

            if (button.Tag.ToString() == "产品参数设置")
            {
                SetupPage setupPage = new SetupPage();
                setupPage.Show();
                return;
            }
            if (button.Tag.ToString() == "手动功能")//手动功能
            {

                FrmManual frmManual = new FrmManual(this.device);
                frmManual.Show();
                return;
            }

            button1.Tag = (button1.Tag.ToString() == "1") ? "Start" : "1";


        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            this.dgv[0] = this.dgvData1;
            this.dgv[1] = this.dgvData2;
            this.dgv[2] = this.dgvData3;
            this.dgv[3] = this.dgvData4;
            //this.InitDatagridViewMLD(ref this.dgv[0]);
            //this.InitDatagridViewMLD(ref this.dgv[1]);
            //this.InitDatagridViewMLD(ref this.dgv[2]);
            //this.InitDatagridViewMLD(ref this.dgv[3]);
            this.tbInfo[0] = this.tbLog1;
            this.tbInfo[1] = this.tbLog2;
            this.tbInfo[2] = this.tbLog1;
            this.tbInfo[3] = this.tbLog2;

            #region 初始化产品参数 MLD四个工位
            for (int i = 0; i < 4; i++)//初始化4个工位
            {
                pro_MLD[i] = new Product_MLD();
            }
            
            
            #endregion

            #region 精密电源连接

            //菊水电源连接1
            this.device.pwr401l[0] = new CPWR401L();
            this.tsslPwr1.BackColor = this.device.pwr401l[0].Init("COM42", 19200)? this.tsslPwr1.BackColor = Color.Green: this.tsslPwr1.BackColor = Color.Red;
            
            //菊水电源连接2
            this.device.pwr401l[1] = new CPWR401L();
            this.tsslPwr2.BackColor = this.device.pwr401l[1].Init("COM43", 19200) ? this.tsslPwr2.BackColor = Color.Green : this.tsslPwr2.BackColor = Color.Red;

            //菊水电源连接3
            this.device.pwr401l[2] = new CPWR401L();
            this.tsslPwr3.BackColor = this.device.pwr401l[2].Init("COM44", 19200) ? this.tsslPwr3.BackColor = Color.Green : this.tsslPwr3.BackColor = Color.Red;


            //菊水电源连接4
            this.device.pwr401l[3] = new CPWR401L();
            this.tsslPwr4.BackColor = this.device.pwr401l[3].Init("COM45", 19200) ? this.tsslPwr4.BackColor = Color.Green : this.tsslPwr4.BackColor = Color.Red;
           
            this.device.pwr401l[0].OutPut(false);
            this.device.pwr401l[1].OutPut(false);
            this.device.pwr401l[2].OutPut(false);
            this.device.pwr401l[3].OutPut(false);
            #endregion

            #region 电子负载连接
            //电子负载连接1
            this.device.plz50fLoad[0] = new CPLZ50FLoad();
            this.tsslLoad1.BackColor = this.device.plz50fLoad[0].Init("COM34", 19200) ?this.tsslLoad1.BackColor=Color.Green:this.tsslLoad1.BackColor= Color.Red;
          
            //电子负载连接2
            this.device.plz50fLoad[1] = new CPLZ50FLoad();
            this.tsslLoad2.BackColor = this.device.plz50fLoad[1].Init("COM35", 19200) ? this.tsslLoad2.BackColor = Color.Green:this.tsslLoad2.BackColor = Color.Red;
            //电子负载连接3
            this.device.plz50fLoad[2] = new CPLZ50FLoad();
            this.tsslLoad3.BackColor = this.device.plz50fLoad[2].Init("COM36", 19200) ? this.tsslLoad3.BackColor = Color.Green : this.tsslLoad3.BackColor = Color.Red;
            //电子负载连接4
            this.device.plz50fLoad[3] = new CPLZ50FLoad();
            this.tsslLoad4.BackColor = this.device.plz50fLoad[3].Init("COM37", 19200) ? this.tsslLoad4.BackColor = Color.Green : this.tsslLoad4.BackColor = Color.Red;

            #endregion


            #region 图莫斯 USB_lin CAN 连接
            Int32 DevNum;
            int USB_ScanDevice = 0;
            //扫描查找设备
            do
            {
                Thread.Sleep(100);
                USB_ScanDevice += 1;
                DevNum = USB_DEVICE.USB_ScanDevice(UTA0402_ID_DevHandles);
                Console.WriteLine("扫描图莫斯设备扫描次数{0}识别到{1} ", USB_ScanDevice, DevNum);
            }
            while (DevNum != 4);


            if (DevNum < 1)
            {
                //System.Console.WriteLine(string.Format("设备需要2张CAN卡，当前识别到的CAN卡数量为{0}，请查找原因!"), DevNum);
                MessageBox.Show("当前设备上只有2个CAN卡，索引为0和1，请确认。");
            }
            else
            {
                System.Console.WriteLine("Have {0} {1}/{2}/{3}/{4}device connected!", DevNum, UTA0402_ID_DevHandles[0].ToString("X2"), UTA0402_ID_DevHandles[1].ToString("X2"), UTA0402_ID_DevHandles[2].ToString("X2"), UTA0402_ID_DevHandles[3].ToString("X2"));
            }
            //DevHandle = DevHandles[index];

            this.device.uta0402_Lin[0] = new UTA0402_lin();
            this.tsslUTA0402_1.BackColor = this.device.uta0402_Lin[0].UTA0402_linOPen(UTA0402_ID_DevHandles[0]) ? this.tsslUTA0402_1.BackColor = Color.Green : this.tsslUTA0402_1.BackColor = Color.Red;



            this.device.uta0402_Lin[1] = new UTA0402_lin();
            this.tsslUTA0402_2.BackColor = this.device.uta0402_Lin[1].UTA0402_linOPen(UTA0402_ID_DevHandles[1]) ? this.tsslUTA0402_2.BackColor = Color.Green : this.tsslUTA0402_2.BackColor = Color.Red;

            this.device.uta0402_Lin[2] = new UTA0402_lin();
            this.tsslUTA0402_3.BackColor = this.device.uta0402_Lin[2].UTA0402_linOPen(UTA0402_ID_DevHandles[2]) ? this.tsslUTA0402_3.BackColor = Color.Green : this.tsslUTA0402_3.BackColor = Color.Red;

            this.device.uta0402_Lin[3] = new UTA0402_lin();
            this.tsslUTA0402_4.BackColor = this.device.uta0402_Lin[3].UTA0402_linOPen(UTA0402_ID_DevHandles[3]) ? this.tsslUTA0402_4.BackColor = Color.Green : this.tsslUTA0402_4.BackColor = Color.Red;



            #endregion







            #region NI_USB_6002 采集卡连接 1-4

            bool NI_usb6002Con = false;
        for (int i = 0; i < 4; i++)
            {
                device.NI_usb6002[i] = new NI_USB6002();
                    NI_usb6002Con = device.NI_usb6002[i].Init("Dev"+(i+1).ToString());//初始化采集卡
                    int index = i;
                    if (!NI_usb6002Con)
                    {
                        
                        switch (index)
                        {
                            case 0:
                                this.NI_usb6002_1.BackColor = Color.Red;
                                break;
                            case 1:
                                this.NI_usb6002_2.BackColor = Color.Red;
                                break;
                            case 2:
                                this.NI_usb6002_3.BackColor = Color.Red;
                                break;
                            case 3:
                                this.NI_usb6002_3.BackColor = Color.Red;
                                break;
                            default:
                                break;
                        }
                    }
                    else {
                        switch (index)
                        {
                            case 0:
                                this.NI_usb6002_1.BackColor = Color.Green;
                                break;
                            case 1:
                                this.NI_usb6002_2.BackColor = Color.Green;
                                break;
                            case 2:
                                this.NI_usb6002_3.BackColor = Color.Green;
                                break;
                            case 3:
                                this.NI_usb6002_4.BackColor = Color.Green;
                                break;
                            default:
                                break;
                        }

                    }
                    

           
            }
            //if (NI_usb6002Con1)
            //    this.tssl4711_1.BackColor = Color.Green;
            //else
            //{
            //    this.tssl4711_1.BackColor = Color.Red;
            //    this.bReadFlag = false;
            //}


            #endregion


            #region  //初始化PLC
            device.modbusTCPClient = new ModbusTCPClient();
            device.modbusTCPClient.Connet("192.168.1.240", 502);
            device.modbusTCPClient.EventConnection += ModbusTCPClient_EventConnection;
            device.modbusTCPClient.EventReceive += ModbusTCPClient_EventReceive;
            #endregion
            #region    ModbusRtu switch 板连接
            for (int i = 0; i < 4; i++)
            {
                device.SwichPCB[i] = new SwichPCB_COMPLEX();
                device.CPCB[i] = new SwichPCB_COMPLEX();
            }
          


            //1
            device.master_SW[0] = new MasterRtu()
            {
                PortName = "COM30",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslSW1.BackColor= device.master_SW[0].Connect(device.master_SW[0]) ? tsslSW1.BackColor = Color.Green : tsslSW1.BackColor = Color.Red;
         
            //2
            device.master_SW[1] = new MasterRtu()
            {
                PortName = "COM31",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslSW2.BackColor = device.master_SW[1].Connect(device.master_SW[1]) ? tsslSW2.BackColor = Color.Green : tsslSW2.BackColor = Color.Red;

            //3
            device.master_SW[2] = new MasterRtu()
            {
                PortName = "COM32",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslSW3.BackColor = device.master_SW[2].Connect(device.master_SW[2]) ? tsslSW3.BackColor = Color.Green : tsslSW3.BackColor = Color.Red;


            //4
            device.master_SW[3] = new MasterRtu()
            {
                PortName = "COM33",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslSW4.BackColor = device.master_SW[3].Connect(device.master_SW[3]) ? tsslSW4.BackColor = Color.Green : tsslSW4.BackColor = Color.Red;


            // ModbusRtu complex 板连接 其电阻
            device.master_C[0] = new MasterRtu()
            {
                PortName = "COM38",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            tsslCM1.BackColor = device.master_C[0].Connect(device.master_C[0]) ? tsslCM1.BackColor = Color.Green : tsslCM1.BackColor = Color.Red;
            device.master_C[1] = new MasterRtu()
            {
                PortName = "COM39",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            tsslCM2.BackColor = device.master_C[1].Connect(device.master_C[1]) ? tsslCM2.BackColor = Color.Green : tsslCM2.BackColor = Color.Red;
            //2
            device.master_C[2] = new MasterRtu()
            {
                PortName = "COM40",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslCM3.BackColor = device.master_C[2].Connect(device.master_C[2]) ? tsslCM3.BackColor = Color.Green : tsslCM3.BackColor = Color.Red;

            device.master_C[3] = new MasterRtu()
            {
                PortName = "COM41",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };
            tsslCM4.BackColor = device.master_C[3].Connect(device.master_C[3]) ? tsslCM4.BackColor = Color.Green : tsslCM4.BackColor = Color.Red;

            #endregion
            #region Uart 串口功能测试   
            //1
            device.uartTests[0] = new UartTest()
            {

                PortName = "COM6",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One


            };
            tsl_uart1.BackColor = device.uartTests[0].Connect(device.uartTests[0]) ? tsl_uart1.BackColor = Color.Green : tsl_uart1.BackColor = Color.Red;
            //2
            device.uartTests[1] = new UartTest()
            {

                PortName = "COM7",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One


            };
            tsl_uart2.BackColor = device.uartTests[1].Connect(device.uartTests[1]) ? tsl_uart2.BackColor = Color.Green : tsl_uart2.BackColor = Color.Red;


            //3
            device.uartTests[2] = new UartTest()
            {

                PortName = "COM8",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One


            };
                tsl_uart3.BackColor = device.uartTests[2].Connect(device.uartTests[2]) ? tsl_uart3.BackColor = Color.Green : tsl_uart3.BackColor = Color.Red;



            //4
            device.uartTests[3] = new UartTest()
            {

                PortName = "COM9",
                BaudRate = 19200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One


            };
            tsl_uart4.BackColor = device.uartTests[3].Connect(device.uartTests[3]) ? tsl_uart4.BackColor = Color.Green : tsl_uart4.BackColor = Color.Red;


            #endregion

            #region 定时器开启
            this.timerCycleHeartBeat = new System.Threading.Timer(timerCycleHeartBeatCallback, null, 2000, 1000);  //PLC心跳 间隔 10S读一次
            this.sttSchedule = new System.Threading.Timer(this.sttScheduleCallback, null, System.Threading.Timeout.Infinite, 300);





            #endregion
            //    this.timercon = new System.Threading.Timer(timerCallbackConnect, null, System.Threading.Timeout.Infinite, 2000);
            //   this.timerCycleRead = new System.Threading.Timer(timerCycleReadCalback, null, 5000, 1000);
            //  this.timerCycleManager = new System.Threading.Timer(timerCycleManagerCallback, null, System.Threading.Timeout.Infinite, 1000);



            //private System.Threading.Timer sttSchedule = null;              //任务调度



            #region
            //启动线程
            this.threadMain1 = new Thread(MainPro1);//开启线程
            this.threadMain1.Start();
            this.threadMain2 = new Thread(MainPro2);
            this.threadMain2.Start();
            this.sttSchedule.Change(50, 30);
            //this.threadMain3 = new Thread(MainPro3);
            //this.threadMain3.Start();
            //this.threadMain4 = new Thread(MainPro3);
            //  this.threadMain4.Start();

            #endregion



            timerPro_1.AutoReset = true;
            timerPro_1.Enabled = true;
            timerPro_1.Interval = 200;
            timerPro_1.Elapsed += new System.Timers.ElapsedEventHandler(timerPro_1_Tick);
            timerPro_1.Stop();

            timerPro_2.AutoReset = true;
            timerPro_2.Enabled = true;
            timerPro_2.Interval = 200;
            timerPro_2.Elapsed += new System.Timers.ElapsedEventHandler(timerPro_2_Tick);
            timerPro_2.Stop();

            timerPro_3.AutoReset = true;
            timerPro_3.Enabled = true;
            timerPro_3.Interval = 200;
            timerPro_3.Elapsed += new System.Timers.ElapsedEventHandler(timerPro_3_Tick);
            timerPro_3.Stop();

            timerPro_4.AutoReset = true;
            timerPro_4.Enabled = true;
            timerPro_4.Interval = 200;
            timerPro_4.Elapsed += new System.Timers.ElapsedEventHandler(timerPro_4_Tick);
            timerPro_4.Stop();
        }



        #region 测试时间


        long startTicks_1 = 0;
        long startTicks_2 = 0;
        long startTicks_3 = 0;
        long startTicks_4 = 0;
        private void timerPro_1_Tick(object sender, EventArgs e)
        {
            this.Invoke(new System.Action(() =>
            {
                this.btnTestTime1.Text = ((DateTime.Now.Ticks - startTicks_1) / 10000000.00).ToString("f2");
            }));

        }

        private void timerPro_2_Tick(object sender, EventArgs e)
        {
            this.Invoke(new System.Action(() =>
            {
                this.btnTestTime2.Text = ((DateTime.Now.Ticks - startTicks_2) / 10000000.00).ToString("f2");
            }));

        }

    
        private void timerPro_3_Tick(object sender, EventArgs e)
        {
            this.Invoke(new System.Action(() =>
            {
                this.btnTestTime3.Text = ((DateTime.Now.Ticks - startTicks_3) / 10000000.00).ToString("f2");
            }));

        }

        private void timerPro_4_Tick(object sender, EventArgs e)
        {
            this.Invoke(new System.Action(() =>
            {
                this.btnTestTime4.Text = ((DateTime.Now.Ticks - startTicks_4) / 10000000.00).ToString("f2");
            }));

        }
        #endregion


       




        /// <summary>
        /// MLD检测流程
        /// </summary>
        /// <param name="indexPro"></param>
        /// <param name="indexPP"></param>
        /// <returns></returns>
        private bool DataProcessMLD(int indexPro)
        {
            #region 初始化 电源 SWICH 板和 complex板 开始计时
            device.NI_usb6002[indexPro].DO_03_K4_FZ(false);
            device.NI_usb6002[indexPro].DO_00_staticCurrent(false);
            device.pwr401l[indexPro].OutPut(false);
            device.SwichPCB[indexPro].RBIN_NTCRest(device.master_SW[indexPro]);
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
                                                                         

            startTicks_1 = DateTime.Now.Ticks;//开始计时
            timerPro_1.Start();


            #endregion
            //刷写前复位设备
            //  device.uta0402_Lin[indexPro].ResetUTA0503();








            ////测试显示数据
            //testSimulate_Data(pro_MLD[indexPro]);
            //judgeResult_MLD(indexPro, pro_MLD[indexPro]);//比较数据
            //   DisplayMLD(indexPro, pro_MLD[indexPro]);

            // 【1】休眠电流检测
            TestProStateDisplay(indexPro, "静态电流测试...", Color.Yellow);
            currStatic_18V(indexPro);//标准40微安


            #region【2】输出恒流检测9V 14 16V
            TestProStateDisplay(indexPro, "输出恒流检测9V/14/16V...", Color.Yellow);
            outCC_Test(indexPro);
            #endregion




            #region 【3】输出精度5V
            TestProStateDisplay(indexPro, "输出精度5V检测...", Color.Yellow);
            Out_5V(indexPro);

            #endregion
            #region 【4】单通道逻辑输入电流
            TestProStateDisplay(indexPro, "单通道逻辑输入电流检测...", Color.Yellow);
            Logic_InputCurrent(indexPro);


            #endregion

            #region【5】占空比 检测
            TestProStateDisplay(indexPro, "占空比检测...", Color.Yellow);
            Duty_test(indexPro);


            #endregion

            #region【6】报警信号跟踪  HSSS1 LSSS1
            //   TestProStateDisplay(indexPro, "报警信号跟踪检测...", Color.Yellow);

            //   LogicDuty(indexPro); //OK
            #endregion

            #region【7】MLD HSSS1 LSS    S1-4 
            //device.NI_usb6002[indexPro].DO_03_K4_FZ(false);
            //TestProStateDisplay(indexPro, "HSSS1检测...", Color.Yellow);
            //     MLD_HSSS1(indexPro);

            //  TestProStateDisplay(indexPro, "LSSS1-4检测...", Color.Yellow);
            //    MLD_LSSS1(indexPro);


            #endregion

            #region【8】Lin读取RBin 电压
            //  TestProStateDisplay(indexPro, "PIN13/14/15/20 电压检测...", Color.Yellow);
            //   LINRead_RBin_V(indexPro);



            #endregion


            #region【9】Lin读取RBin 电压
            //  TestProStateDisplay(indexPro, "PIN21/P22电压检测...", Color.Yellow);
            // NTC_Test(indexPro);

            #endregion

            #region【10】Uart 功能测试
            //  TestProStateDisplay(indexPro, "Uart 功能测试...", Color.Yellow);
            //    Uart_Test(indexPro);
            //   this.taskConQueue.Enqueue(device.modbusTCPClient.ReadKeepReg(1, 0, 6));//设备1
            #endregion
            #region【11】启动延时测试

            //       TestProStateDisplay(indexPro, "启动延时 功能测试...", Color.Yellow);

            //MLDStartSleep(indexPro);

            #endregion





            #region【12】刷写文件
            //if (judgeResult_MLD_NO1(indexPro, pro_MLD[indexPro]))
            //{
            //   // DownHex_MLD(indexPro);
            //}

            //else
            //{
            //    pro_MLD[indexPro].isDownLoad_ok = false;
            //}

            #region 测试
            bool result1 = judgeResult_MLD_NO1(indexPro, pro_MLD[indexPro]);
           DisplayMLD(indexPro, pro_MLD[indexPro]);
            judgeResult_MLD_NO2(indexPro, pro_MLD[indexPro]);
            return result1;
            #endregion
            #endregion

            #region 读取版本号
            //    Get_MlD_Info(indexPro);
            #endregion

            #region 结果判断
            //bool bOKflag = false;
            //bOKflag = judgeResult_MLD_NO2(indexPro, pro_MLD[indexPro]);
            //DisplayMLD(indexPro, pro_MLD[indexPro]);
            #endregion

            //if (bOKflag)
            //{

            //}

            //return bOKflag;


            // SaveDataIntoDataBase_3(pro3[indexPro]);




        }
        /// <summary>
        /// 休眠启动时间计算
        /// </summary>
        /// <param name="indexPro"></param>
        private void MLDStartSleep(int indexPro)
        {
           


            //【1】接入电阻                                                                       
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 1, 1);//【03】分别接入2K电阻  pin13
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 1);//接入2K电阻pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 3, 1);//接入2K电阻pin15
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 4, 1);//接入2K电阻pin20

            device.pwr401l[indexPro].ApplVolt(14, 5);//给电压
            device.pwr401l[indexPro].OutPut(true);//电源打开

            //【2】激活MLD
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空100% 

            //【4】逻辑输出 SWICH 
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 2, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 3, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 4, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 5, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 6, 500);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 2, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 3, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 4, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 5, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 6, 10000);



            //【3】接入负载
            device.plz50fLoad[indexPro].VoltFunction(1, 3.0); //接入负载pin 12 A_RL1
            device.plz50fLoad[indexPro].VoltFunction(2, 3.0); //接入负载pin 19  A_RL2
            device.plz50fLoad[indexPro].VoltFunction(3, 3.0); //接入负载pin 5  A_RL3
            device.plz50fLoad[indexPro].VoltFunction(4, 3.0); //接入负载pin 26  A_RL4
            device.plz50fLoad[indexPro].OutputAll(true);//负载打开

            //AI1 逻辑上升到9V 开始 和AI6
            System.Threading.Tasks.Task.Run(() => {

          //   device.NI_usb6002[indexPro].GetDelayTime(out pro_MLD[indexPro].startSleep);




            });


       



            //【4】清空
            device.pwr401l[indexPro].OutPut(false);//电源打开
            device.plz50fLoad[indexPro].OutputAll(false);//负载打开
            device.SwichPCB[indexPro].RBIN_NTCRest(device.master_SW[indexPro]);
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
        }

        /// <summary>
        /// 烧写配置文件
        /// </summary>
        /// <param name="indexPro"></param>
        private void DownHex_MLD(int indexPro)
        {
         
           
                if (indexPro == 0)
                {
                    //  LogicInput_MLD(indexPro, 1, 1, 1, 1, 1);
                    this.device.pwr401l[indexPro].ApplVolt(14, 10);
                    this.device.pwr401l[indexPro].OutPut(true);

                    Hexflash_MLD.Init();

                    TestProStateDisplay(indexPro, "解析HEX文件...", Color.Yellow);

                    TestProStateDisplay(indexPro, "计算HEX文件校验值...", Color.Yellow);
                    //校验文件
                    if (_pp.pInfo[_nCurrIndex].AppFilePath != "")//APP 下载
                    {
                        byte[] app_data = new byte[0];
                        for (int i = 0; i < Hexflash_MLD.hexFiles_APP.Count; i++)
                        {
                            app_data = app_data.Concat(Hexflash_MLD.hexFiles_APP[i].data.Take(Hexflash_MLD.hexFiles_APP[i].length)).ToArray();
                        }
                        Hexflash_MLD.CRC_App = Hexflash_MLD.FuncCalCrc16(app_data, app_data.Length);

                        Hexflash_MLD.isApp = true;
                    }


                    byte[] cali_data = new byte[0];
                    for (int i = 0; i < Hexflash_MLD.hexFiles_Cali.Count; i++)// 配置文件下载
                    {
                        cali_data = cali_data.Concat(Hexflash_MLD.hexFiles_Cali[i].data.Take(Hexflash_MLD.hexFiles_Cali[i].length)).ToArray();
                    }
                    Hexflash_MLD.CRC_Cali = Hexflash_MLD.FuncCalCrc16(cali_data, cali_data.Length);

                    Hexflash_MLD.isCali = true;

                    if (_pp.pInfo[_nCurrIndex].AnimeFilePath != "")//Anime 文件下载
                    {
                        byte[] anim_data = new byte[0];
                        for (int i = 0; i < Hexflash_MLD.hexFiles_Anim.Count; i++)
                        {
                            anim_data = anim_data.Concat(Hexflash_MLD.hexFiles_Anim[i].data.Take(Hexflash_MLD.hexFiles_Anim[i].length)).ToArray();
                        }
                        Hexflash_MLD.CRC_Anim = Hexflash_MLD.FuncCalCrc16(anim_data, anim_data.Length);

                        Hexflash_MLD.isAnim = true;
                    }


                    Hexflash_MLD.LGDownloadMech = HexFlash_MLD.DownLoadMachine.SID_2701_LIN;


                    //device.UTA0503[indexPro].InitUTA0503(DevHandles[0]);
                    Hexflash_MLD.UTA0503 = device.uta0402_Lin[indexPro];

                    TestProStateDisplay(indexPro, "开始刷写HEX文件...", Color.Yellow);

                    Hexflash_MLD.isRunning = true;
                    System.Threading.Thread.Sleep(200);


                    while (Hexflash_MLD.isRunning)
                    {
                        Hexflash_MLD.ThreadTick();
                        if ((DateTime.Now.Ticks - startTicks_1) / 10000000.00 >= 45)
                        {
                            Hexflash_MLD.isRunning = false;
                            Hexflash_MLD.isDownload = false;
                        }
                    }

                    pro_MLD[indexPro].isDownLoad_ok = Hexflash_MLD.isDownload;
                }
                else
                {
                    //  LogicInput_MLD(indexPro, 1, 1, 1, 1, 1);
                    this.device.pwr401l[indexPro].ApplVolt(14, 10);
                    this.device.pwr401l[indexPro].OutPut(true);

                    Hexflash_MLD_1.Init();

                    TestProStateDisplay(indexPro, "解析HEX文件...", Color.Yellow);

                    TestProStateDisplay(indexPro, "计算HEX文件校验值...", Color.Yellow);
                    //校验文件
                    if (_pp.pInfo[_nCurrIndex].AppFilePath != "")
                    {
                        byte[] app_data = new byte[0];
                        for (int i = 0; i < Hexflash_MLD_1.hexFiles_APP.Count; i++)
                        {
                            app_data = app_data.Concat(Hexflash_MLD_1.hexFiles_APP[i].data.Take(Hexflash_MLD_1.hexFiles_APP[i].length)).ToArray();
                        }
                        Hexflash_MLD_1.CRC_App = Hexflash_MLD_1.FuncCalCrc16(app_data, app_data.Length);

                        Hexflash_MLD_1.isApp = true;
                    }


                    byte[] cali_data = new byte[0];
                    for (int i = 0; i < Hexflash_MLD_1.hexFiles_Cali.Count; i++)
                    {
                        cali_data = cali_data.Concat(Hexflash_MLD_1.hexFiles_Cali[i].data.Take(Hexflash_MLD_1.hexFiles_Cali[i].length)).ToArray();
                    }
                    Hexflash_MLD_1.CRC_Cali = Hexflash_MLD_1.FuncCalCrc16(cali_data, cali_data.Length);

                    Hexflash_MLD_1.isCali = true;

                    if (_pp.pInfo[_nCurrIndex].AnimeFilePath != "")
                    {
                        byte[] anim_data = new byte[0];
                        for (int i = 0; i < Hexflash_MLD_1.hexFiles_Anim.Count; i++)
                        {
                            anim_data = anim_data.Concat(Hexflash_MLD_1.hexFiles_Anim[i].data.Take(Hexflash_MLD_1.hexFiles_Anim[i].length)).ToArray();
                        }
                        Hexflash_MLD_1.CRC_Anim = Hexflash_MLD_1.FuncCalCrc16(anim_data, anim_data.Length);

                        Hexflash_MLD_1.isAnim = true;
                    }


                    Hexflash_MLD_1.LGDownloadMech = HexFlash_MLD_1.DownLoadMachine.SID_2701_LIN;


                    //device.UTA0503[indexPro].InitUTA0503(DevHandles[1]);
                    Hexflash_MLD_1.UTA0503 = device.uta0402_Lin[indexPro];

                    TestProStateDisplay(indexPro, "开始刷写HEX文件...", Color.Yellow);

                    Hexflash_MLD_1.isRunning = true;
                    System.Threading.Thread.Sleep(200);


                    while (Hexflash_MLD_1.isRunning)
                    {
                        Hexflash_MLD_1.ThreadTick();
                        if ((DateTime.Now.Ticks - startTicks_2) / 10000000.00 >= 45)
                        {
                            Hexflash_MLD_1.isRunning = false;
                            Hexflash_MLD_1.isDownload = false;
                        }
                    }

                    pro_MLD[indexPro].isDownLoad_ok = Hexflash_MLD_1.isDownload;
                }
            
           
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="indexPro"></param>
        private void Get_MlD_Info(int indexPro)
        {
            //LIN烧录功能测试，Logic信号，读取软件版本信息
            //读取版本号
            this.device.pwr401l[indexPro].ApplVolt(14, 10);
            this.device.pwr401l[indexPro].OutPut(true);

            System.Threading.Thread.Sleep(200);
            bool flag = this.device.uta0402_Lin[indexPro].SecurityAccess_3();
            //System.Console.WriteLine("安全解锁 " + flag);
            TestProStateDisplay(indexPro, "读取FBL版本号...", Color.Yellow);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x05, ref pro_MLD[indexPro]);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x06, ref pro_MLD[indexPro]);
            TestProStateDisplay(indexPro, "读取APP版本号...", Color.Yellow);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x07, ref pro_MLD[indexPro]);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x08, ref pro_MLD[indexPro]);
            TestProStateDisplay(indexPro, "读取Cali版本号...", Color.Yellow);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x09, ref pro_MLD[indexPro]);
            this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x0A, ref pro_MLD[indexPro]);
            if (_pp.pInfo[_nCurrIndex].AnimePN != "")
            {
                TestProStateDisplay(indexPro, "读取Anim版本号...", Color.Yellow);
                this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x0B, ref pro_MLD[indexPro]);
                this.device.uta0402_Lin[indexPro].ReadPara_MLD(0x0C, ref pro_MLD[indexPro]);
            }
            this.device.pwr401l[indexPro].ApplVolt(14, 10);
            this.device.pwr401l[indexPro].OutPut(false);
        }

        /// <summary>
        /// 低边测试
        /// </summary>
        /// <param name="indexPro"></param>
        private void MLD_LSSS1(int indexPro)
        {    // 电源供电 激活模块
            device.NI_usb6002[indexPro].DO_04_14V_LSS(false);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
         




            device.NI_usb6002[indexPro].DO_04_14V_LSS(true);//接入pin10 28 27 29 接入10欧姆电阻
            device.pwr401l[indexPro].ApplVolt(14, 5);//
            device.pwr401l[indexPro].OutPut(true);

            Thread.Sleep(200);
            //【2】安全解锁
            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            //发送命令

            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, new byte[] { 0x31 ,0x01, 0xF1, 0x5C, 0x5C, 0x5C, 0x5C ,0x00 });//PWM输出
            byte[] temp1 = new byte[8];
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref temp1);
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, new byte[] { 0x31, 0x01, 0xF2, 0x80,0x80,0x80, 0x80 ,0x00 });//占空比  输出占空比 E= (N * 100 / 255 )%

            //采集数据
            device.NI_usb6002[indexPro].D0_09_Swich_AI5_0_1_2(true);//接入pin10 28 27 29 接入10欧姆电阻
            device.NI_usb6002[indexPro].Logic_riseFall_AI5_0_1_2(out pro_MLD[indexPro].LSSS1_fallTime,
                                                           out pro_MLD[indexPro].LSSS1_riseTime,
                                                           out pro_MLD[indexPro].LSSS1_fallLv,
                                                           out pro_MLD[indexPro].LSSS1_riseLv,
                                                           out pro_MLD[indexPro].LSSS2_fallTime,
                                                           out pro_MLD[indexPro].LSSS2_riseTime,
                                                           out pro_MLD[indexPro].LSSS2_fallLv,
                                                           out pro_MLD[indexPro].LSSS2_riseLv,
                                                           out pro_MLD[indexPro].LSSS3_fallTime,
                                                           out pro_MLD[indexPro].LSSS3_riseTime,
                                                           out pro_MLD[indexPro].LSSS3_fallLv,
                                                           out pro_MLD[indexPro].LSSS3_riseLv,
                                                           out pro_MLD[indexPro].LSSS4_fallTime,
                                                           out pro_MLD[indexPro].LSSS4_riseTime,
                                                           out pro_MLD[indexPro].LSSS4_fallLv,
                                                           out pro_MLD[indexPro].LSSS4_riseLv);

            //继电器关闭 电源关闭 清空占空比

            device.NI_usb6002[indexPro].D0_09_Swich_AI5_0_1_2(false);//接入pin10 28 27 29 接入10欧姆电阻
            device.NI_usb6002[indexPro].DO_04_14V_LSS(false);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 0);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 0);//设置占空比【02】
            device.pwr401l[indexPro].OutPut(false);
        }

        private void MLD_HSSS1(int indexPro)
        {
            /// 激活MLD
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
                                                                                          //RBIN电阻切换
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 1, 1);//【03】分别接入2K电阻  pin13
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 1);//【03】分别接入2K电阻  pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 3, 1);//【03】分别接入2K电阻  pin15
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 4, 1);//【03】分别接入2K电阻  pin20
                                                                              //14V供电
           // device.NI_usb6002[indexPro].DO_04_14V_LSS(true);
            device.pwr401l[indexPro].ApplVolt(14, 5);//
            device.pwr401l[indexPro].OutPut(true);




            //负载仪器打开
            device.plz50fLoad[indexPro].VoltFunction(1, 20);
            device.plz50fLoad[indexPro].VoltFunction(2, 30);
            device.NI_usb6002[indexPro].DO_03_K4_FZ(true);
            device.plz50fLoad[indexPro].OutPutOnALL();//负载打开
          

            Thread.Sleep(200);

            //【2】安全解锁
            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            //
            byte[] temp1 = new byte[8];

         //   device.NI_usb6002[indexPro].D0_09_Swich_AI5_0_1_2(true);//AI5接入模拟量
            //device.NI_usb6002[indexPro].Logic_HSSS_AI5_AI6(out pro_MLD[indexPro].HSSS1_fallTime, out pro_MLD[indexPro].HSSS1_riseTime, 
            //    out pro_MLD[indexPro].HSSS1_fallLV, out pro_MLD[indexPro].HSSS1_riseLV);//开始记录数据

            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, new byte[] { 0x31, 0x01, 0xF1, 0x00, 0x00, 0x00, 0x00, 0x01 });//HSSS 没有占空比，只需要控制其开或者关，因此状态位 Byte5 写非零即为接通，建议直接填
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref temp1);
            Thread.Sleep(50);
            //开启任务，开始记录AI5和AI6数据，并数据处理和分析            
            var task1 = System.Threading.Tasks.Task.Run(new System.Action(() => {
                device.NI_usb6002[indexPro].Logic_HSSS_AI5_AI6(out pro_MLD[indexPro].HSSS1_fallTime, out pro_MLD[indexPro].HSSS1_riseTime,
                   out pro_MLD[indexPro].HSSS1_fallLV, out pro_MLD[indexPro].HSSS1_riseLV);//开始记录数据
            }));
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, new byte[] { 0x31, 0x01, 0xF1, 0x00, 0x00, 0x00, 0x00, 0x00 });//HSSS 没有占空比，只需要控制其开或者关，因此状态位 Byte5 写非零即为接通，建议直接填

      
             



            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref temp1);
            device.CPCB[indexPro].RBIN_NTCRest(device.master_C[indexPro]);
            device.pwr401l[indexPro].OutPut(false);
            device.NI_usb6002[indexPro].DO_04_14V_LSS(false);
            device.plz50fLoad[indexPro].OutPutOffALL();//负载打开
         
            device.NI_usb6002[indexPro].DO_03_K4_FZ(false);






        }

        private void LogicDuty(int indexPro)
        {
            device.NI_usb6002[indexPro].DO_01_linOutage(false);
            device.NI_usb6002[indexPro].DO_02_Res(false);


            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 10);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro],1, 5000);//设置占空比【02】


            device.pwr401l[indexPro].ApplVolt(16, 5);//
            device.pwr401l[indexPro].OutPut(true);

            device.plz50fLoad[indexPro].VoltFunction(1, 9.0); //接入负载pin 12 A_RL1
            device.plz50fLoad[indexPro].CurrFunction(1, 1); //接入负载pin 19  A_RL2
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 2);//接入1.6K电阻pin14
          
            Thread.Sleep(200);
            //【2】安全解锁
            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            //【3】LIN   强控输出
            Thread.Sleep(200);
            byte[] temp1 = new byte[8];
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, new byte[] { 0x31, 0x01, 0xF5, 00, 00, 00, 00, 00 });
            Thread.Sleep(20);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref temp1);
            Console.WriteLine(device.uta0402_Lin[indexPro].ToHexStrFromByte(temp1));
            //【4】切换Lin到Outage
            device.NI_usb6002[indexPro].DO_01_linOutage(true);
           // device.NI_usb6002[indexPro].DO_02_Res(true);//暂时不用切
            
            device.NI_usb6002[indexPro].Logic_riseFall(out pro_MLD[indexPro].outage_riseTime, out pro_MLD[indexPro].outage_fallTime,out pro_MLD[indexPro].outageLv);


            device.NI_usb6002[indexPro].DO_01_linOutage(false);
            device.NI_usb6002[indexPro].DO_02_Res(false);
            device.pwr401l[indexPro].OutPut(false);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 0);//设置频率00【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 0);//设置占空比00【02】
        }

        private void Uart_Test(int indexPro)
        {
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空100% 

           

            device.pwr401l[indexPro].ApplVolt(14, 5);//给电压
            device.pwr401l[indexPro].OutPut(true);//电源打开
            Thread.Sleep(1000);
            //【2】安全解锁
            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF);
            ////【3】 获取数据解析
            byte[] tempUart1 = new byte[8];
            
            byte[] tempUart2 = new byte[8];
            byte[] tempUart3 = new byte[8];
            //device.uartTests[indexPro].Put(tempVariable.SendUartTest1);
            // device.uartTests[indexPro].Put(tempVariable.SendUartTest2);
            // tempUart= device.uartTests[indexPro].Put(tempVariable.SendUartTest3);


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10,tempVariable.SendUartTest1);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref tempUart2);

            tempUart3 = device.uartTests[indexPro].Put(tempVariable.SendUartTest2);
            if ( tempUart3 [0]!=0x00)
            {
                pro_MLD[indexPro].Uart_Test = true;
            }
            Thread.Sleep(200);
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.SendUartTest3);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A,ref tempUart3);
       



            device.pwr401l[indexPro].OutPut(false);//电源打开
            device.SwichPCB[indexPro].RBIN_NTCRest(device.master_SW[indexPro]);
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
        }






     




      

        private void NTC_Test(int indexPro)
        {
            byte[] NTC1_V = new byte[8];
            byte[] NTC2_V = new byte[8];

            //【1】激活MLD
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空100% 

            device.pwr401l[indexPro].ApplVolt(14, 5);//给电压
            device.pwr401l[indexPro].OutPut(true);//电源打开

            device.CPCB[indexPro].NTC_Swich(device.master_C[indexPro], 1, 1);//接入10K电阻  pin21
            device.CPCB[indexPro].NTC_Swich(device.master_C[indexPro], 2, 2);//接入2K电阻pin22


            //发送 读取LIN  NTC读
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin2_NTC1);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref NTC1_V);
            byte[] temp1 = new byte[2] { NTC1_V[3], NTC1_V[2] };//字节拼接
            pro_MLD[indexPro].NTC1 = (BitConverter.ToUInt16(temp1, 0) * 30) / 1023.0;


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin4_NTC2);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref NTC2_V);
            byte[] temp2 = new byte[2] { NTC2_V[3], NTC2_V[2] };//字节拼接
            pro_MLD[indexPro].NTC2 = (BitConverter.ToUInt16(temp2, 0) * 30) / 1023.0;



            device.pwr401l[indexPro].OutPut(false);//电源打开
            device.SwichPCB[indexPro].RBIN_NTCRest(device.master_SW[indexPro]);
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
        }

        private void Logic_InputCurrent(int indexPro)
        {
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 2, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 3, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 4, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 5, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 6, 500);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 2, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 3, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 4, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 5, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 6, 10000);
            device.pwr401l[indexPro].ApplVolt(16, 5);//
            device.pwr401l[indexPro].OutPut(true);
            Thread.Sleep(500);
            pro_MLD[indexPro].Logic_1 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 1));
            pro_MLD[indexPro].Logic_2 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 2));
            pro_MLD[indexPro].Logic_3 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 3));
            pro_MLD[indexPro].Logic_4 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 4));
            pro_MLD[indexPro].Logic_5 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 5));
            pro_MLD[indexPro].Logic_6 = Convert.ToDouble(device.SwichPCB[indexPro].GetPWMCurrent(device.master_SW[indexPro], 6));

            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
            device.pwr401l[indexPro].OutPut(false);//电源关闭
        }

        private void Out_5V(int indexPro)
        {
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
            device.pwr401l[indexPro].ApplVolt(14, 2);//9V 2a
            device.pwr401l[indexPro].OutPut(true);//菊水电源打开     
            Thread.Sleep(200);
            device.NI_usb6002[indexPro].AI07_out_5V(ref pro_MLD[indexPro].out5V);//输出5V精度
            pro_MLD[indexPro].out5V =  pro_MLD[indexPro].out5V * 1.05;//调整优化系数
            device.pwr401l[indexPro].OutPut(false);//电源关闭
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
        }

        private void LINRead_RBin_V(int indexPro)
        {
            byte[] BBIN1_V = new byte[8];
            byte[] BBIN2_V = new byte[8];
            byte[] BBIN3_V = new byte[8];
            byte[] BBIN4_V = new byte[8];
            //【1】激活MLD
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空100% 

            device.pwr401l[indexPro].ApplVolt(14, 5);//给电压
            device.pwr401l[indexPro].OutPut(true);//电源打开

            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 1, 1);//【03】分别接入2K电阻  pin13
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 2);//接入1.5K电阻pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 3, 4);//接入1K电阻pin15
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 4, 8);//接入820电阻pin20

           //发送 读取LIN  BBIN1
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin1);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref BBIN1_V);
             byte []temp1=  new byte[2] { BBIN1_V[3], BBIN1_V[2] };//字节拼接
            pro_MLD[indexPro].RBinS1=  (BitConverter.ToUInt16(temp1,0)*5)/1023.0;


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin2_NTC1);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref BBIN2_V);
            byte[] temp2 = new byte[2] { BBIN2_V[3], BBIN2_V[2] };//字节拼接
            pro_MLD[indexPro].RBinS2 = (BitConverter.ToUInt16(temp2, 0) * 5) / 1023.0;


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin3_NTC3);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref BBIN3_V);
            byte[] temp3 = new byte[2] { BBIN3_V[3], BBIN3_V[2] };//字节拼接
            pro_MLD[indexPro].RBinS3 = (BitConverter.ToUInt16(temp3, 0) * 5) / 1023.0;


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendRBin4_NTC2);
            Thread.Sleep(50);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref BBIN4_V);
            byte[] temp4 = new byte[2] { BBIN4_V[3], BBIN4_V[2] };//字节拼接
            pro_MLD[indexPro].RBinS4 = (BitConverter.ToUInt16(temp4, 0) * 5) / 1023.0;


            device.pwr401l[indexPro].OutPut(false);//电源打开
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
        }

        /// <summary>
        /// 【5】占空比 检测
        /// </summary>
        /// <param name="indexPro"></param>
        private void Duty_test(int indexPro)
        {
            device.NI_usb6002[indexPro].DO_01_linOutage(false);
            device.NI_usb6002[indexPro].DO_02_Res(false);

            device.pwr401l[indexPro].ApplVolt(10, 2);//9V 2a
            device.pwr401l[indexPro].OutPut(true);//9V 2a
            //【1】激活MLD
         device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 462);//设置频率【01】控制1-4通道
       device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空100% 
            //【2】设置频率 &占空比
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 2, 462);//设置频率【01】//控制5-6 通道频率
            //device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 3, 462);//设置频率【01】
            //device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 4, 462);//设置频率【01】
            //device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 5, 462);//设置频率【01】
            //device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 6, 462);//设置频率【01】

            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 2, 1000);//设置占空比8%
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 3, 1000);//设置占空比8%
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 4, 800);//设置占空比8%
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 5, 800);//设置占空比8%
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 6, 800);//设置占空比8%
            //【3】lin读取


            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendBuffer);
            Thread.Sleep(200);
            byte[] LinBuffer1 = new byte[8];
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref LinBuffer1);//读取占空比信息

            pro_MLD[0].Logic_2Duty =Convert.ToByte( LinBuffer1[3]);//测试数据
            pro_MLD[0].Logic_3Duty = Convert.ToByte( LinBuffer1[4]);
            pro_MLD[0].Logic_4Duty = Convert.ToByte( LinBuffer1[5]);
            pro_MLD[0].Logic_5Duty = Convert.ToByte(LinBuffer1[6]);
            pro_MLD[0].Logic_6Duty = Convert.ToByte( LinBuffer1[7]);



            byte[] LinBuffer2 = new byte[8];
          
            device.uta0402_Lin[indexPro].SendMsg_Lin(0x10, tempVariable.LinSendLogic1_GPIO); 
            Thread.Sleep(200);
            device.uta0402_Lin[indexPro].ReadMsg_Lin(0x1A, ref LinBuffer2);//逻辑高电平有效


            //pro_MLD[0].Logic_2Duty=1;//测试数据
            //pro_MLD[0].Logic_3Duty=1;
            //pro_MLD[0].Logic_4Duty = 1;
            //pro_MLD[0].Logic_5Duty = 1;
            //pro_MLD[0].Logic_6Duty = 1;
            device.NI_usb6002[indexPro].DO_01_linOutage(false);
            device.NI_usb6002[indexPro].DO_02_Res(false);
            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);//频率和占空比清空
            device.pwr401l[indexPro].OutPut(false);//电源关闭

        }

        /// <summary>
        /// 【2】测试PIN 12 19 5 26 恒流精度
        /// </summary>
        /// <param name="indexPro"></param>
    
        
        public void outCC_Test(int indexPro)
        {
            device.pwr401l[indexPro].ApplVolt(10, 10);//菊水电源电流设置
            device.pwr401l[indexPro].OutPut(true);//菊水电源打开



            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 1, 2);//接入1.6K电阻pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 2);//接入1.6K电阻pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 3, 2);//接入1.6K电阻pin15
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 4, 2);//接入1.6K电阻pin20

            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 1, 500);//设置频率【01】
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 2, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 3, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 4, 500);
            device.SwichPCB[indexPro].PWMchannel_HZ(device.master_SW[indexPro], 5, 500);

            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 1, 10000);//设置占空比【02】
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 2, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 3, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 4, 10000);
            device.SwichPCB[indexPro].DutyCycle_Set(device.master_SW[indexPro], 5, 10000);

            //切换3V负载

            device.plz50fLoad[indexPro].VoltFunction(1, 3.0); //接入负载pin 12 A_RL1
            device.plz50fLoad[indexPro].VoltFunction(2, 3.0); //接入负载pin 19  A_RL2
            device.plz50fLoad[indexPro].VoltFunction(3, 3.0); //接入负载pin 5  A_RL3
            device.plz50fLoad[indexPro].VoltFunction(4, 3.0); //接入负载pin 26  A_RL4
            device.plz50fLoad[indexPro].OutputAll(true);//负载打开
            Thread.Sleep(3000);
            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF);

            // device.pwr401l[indexPro].ApplVolt(9, 10);//菊水电源电流设置

              Thread.Sleep(200);


            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC1LED1_9V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC1LED2_9V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC1LED3_9V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC1LED4_9V);
            //14V
            device.pwr401l[indexPro].ApplVolt(14, 5);//菊水电源电流设置
            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC1LED1_14V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC1LED2_14V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC1LED3_14V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC1LED4_14V);

            //16V
            device.pwr401l[indexPro].ApplVolt(16, 5);//菊水电源电流设置
            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC1LED1_16V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC1LED2_16V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC1LED3_16V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC1LED4_16V);





            #region [2]第二种情况切换55V负载
            device.pwr401l[indexPro].OutPut(false);//菊水电源打开
            device.plz50fLoad[indexPro].VoltFunction(1, 45); //接入负载pin 12 A_RL1
            device.plz50fLoad[indexPro].VoltFunction(2, 45); //接入负载pin 19  A_RL2
            device.plz50fLoad[indexPro].VoltFunction(3, 45); //接入负载pin 5  A_RL3
            device.plz50fLoad[indexPro].VoltFunction(4, 45); //接入负载pin 26  A_RL4
            Thread.Sleep(500);
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 1, 1);//【03】分别接入2K电阻  pin13
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 2, 1);//接入2K电阻pin14
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 3, 1);//接入2K电阻pin15
            device.CPCB[indexPro].RBIN1Swich(device.master_C[indexPro], 4, 1);//接入2K电阻pin20
            device.pwr401l[indexPro].OutPut(true);//菊水电源打开
            device.plz50fLoad[indexPro].OutputAll(true);//负载打开

            if (this.device.uta0402_Lin[indexPro].SecurityAccess_3())
            {
                Console.WriteLine(string.Format("安全解锁{0}OK", indexPro));
            }
            else
            {
                Console.WriteLine(string.Format("安全解锁{0}NG", indexPro));
            }
            if (this.device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF))
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            else
            {
                //Console.WriteLine(string.Format("输出{0}OK", indexPro));
            }
            device.uta0402_Lin[indexPro].CC_CH_OUTPUT_3(1, 0xFF, 0xFF, 0xFF, 0xFF);
         

            



            device.pwr401l[indexPro].ApplVolt(9, 10);//菊水电源电流设置
            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC2LED1_9V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC2LED2_9V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC2LED3_9V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC2LED4_9V);
            //14V
            device.pwr401l[indexPro].ApplVolt(14, 5);//菊水电源电流设置
            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC2LED1_14V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC2LED2_14V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC2LED3_14V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC2LED4_14V);

            //16V
            device.pwr401l[indexPro].ApplVolt(16, 5);//菊水电源电流设置
            device.plz50fLoad[indexPro].GetCurr(0x01, out pro_MLD[indexPro].CC2LED1_16V);
            device.plz50fLoad[indexPro].GetCurr(0x02, out pro_MLD[indexPro].CC2LED2_16V);
            device.plz50fLoad[indexPro].GetCurr(0x03, out pro_MLD[indexPro].CC2LED3_16V);
            device.plz50fLoad[indexPro].GetCurr(0x04, out pro_MLD[indexPro].CC2LED4_16V);



            device.SwichPCB[indexPro].Hz_dutyRest(device.master_SW[indexPro]);
            device.SwichPCB[indexPro].RBinRest(device.master_C[indexPro], 5);//设置清空
            device.plz50fLoad[indexPro].OutPutOffALL();
            device.pwr401l[indexPro].OutPut(false);//菊水电源打开

            #endregion

            //device.plz50fLoad[indexPro].OutputAll(false);
            //device.NI_usb6002[indexPro].DO_05_9V(false);
            //device.pwr401l[indexPro].OutPut(false);
        }

        /// <summary>
        /// 【1休眠电流检测】
        /// </summary>
        /// <param name="indexPro"></param>
        public void currStatic_18V(int indexPro)
        {
           
            device.NI_usb6002[indexPro].DO_00_staticCurrent(true);//NI板卡DO输出
          
      //      Thread.Sleep(1000);
            device.pwr401l[indexPro].ApplVolt(18, 5);
            device.pwr401l[indexPro].OutPut(true);

            Thread.Sleep(3000);

           

            device.NI_usb6002[indexPro].AI_Start_AI02(ref pro_MLD[indexPro].currStatic_18V);//获取休眠电流 
            pro_MLD[indexPro].currStatic_18V = pro_MLD[indexPro].currStatic_18V / 250 / 100 * 1000000;

           


            device.pwr401l[indexPro].OutPut(false);//电源关闭
            device.NI_usb6002[indexPro].DO_00_staticCurrent(false);//NI板卡DO输出
            device.NI_usb6002[indexPro].DO_00_staticCurrent(false);//NI板卡DO输出
       
        }

        private void TestProStateDisplay(int index, string str, Color color)
        {
            this.Invoke(new System.Action(() =>
            {
                this.tbInfo[index].Text = str;
                this.tbInfo[index].BackColor = color;
            }));
        }

    









        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerPro_1.Dispose();
            timerPro_2.Dispose();
            timerPro_3.Dispose();
            timerPro_4.Dispose();
            this.threadMain1.Abort();
            //this.threadMain2.Abort();
            //this.threadMain3.Abort();
            //this.threadMain4.Abort();
            this.timerCycleHeartBeat.Dispose();
          //  this.sttSchedule.Dispose();
        }




        private bool judgeResult_MLD_NO1(int indexPro, Product_MLD testData)
        {
            bool bFlag = true;
            // this.Invoke(new System.Action(() =>
            {
                //18V静态电流

                if (testData.currStatic_18V < _pp.pInfo[_nCurrIndex].currStaticMin_18V || testData.currStatic_18V > _pp.pInfo[_nCurrIndex].currStaticMax_18V)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[0].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[0].Cells[5].Style.BackColor = Color.Green;
                }
                #region 9V CC1 LED1

                if (testData.CC1LED1_9V < _pp.pInfo[_nCurrIndex].CC1LED1Min_9V || testData.CC1LED1_9V > _pp.pInfo[_nCurrIndex].CC1LED1Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[1].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[1].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED2
                if (testData.CC1LED2_9V < _pp.pInfo[_nCurrIndex].CC1LED2Min_9V || testData.CC1LED2_9V > _pp.pInfo[_nCurrIndex].CC1LED2Max_9V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[2].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[2].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED3
                if (testData.CC1LED3_9V < _pp.pInfo[_nCurrIndex].CC1LED3Min_9V || testData.CC1LED3_9V > _pp.pInfo[_nCurrIndex].CC1LED3Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[3].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[3].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED4
                if (testData.CC1LED4_9V < _pp.pInfo[_nCurrIndex].CC1LED4Min_9V || testData.CC1LED4_9V > _pp.pInfo[_nCurrIndex].CC1LED4Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[4].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[4].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region  //14V CC1 LED1

                if (testData.CC1LED1_14V < _pp.pInfo[_nCurrIndex].CC1LED1Min_14V || testData.CC1LED1_14V > _pp.pInfo[_nCurrIndex].CC1LED1Max_14V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[5].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[5].Cells[5].Style.BackColor = Color.Green;
                }


                //14V CC1 LED2
                if (testData.CC1LED2_14V < _pp.pInfo[_nCurrIndex].CC1LED2Min_14V || testData.CC1LED2_14V > _pp.pInfo[_nCurrIndex].CC1LED2Max_14V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[6].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[6].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC1LED3_14V < _pp.pInfo[_nCurrIndex].CC1LED3Min_14V || testData.CC1LED3_14V > _pp.pInfo[_nCurrIndex].CC1LED3Max_14V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[7].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[7].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC1LED4_14V < _pp.pInfo[_nCurrIndex].CC1LED4Min_14V || testData.CC1LED4_14V > _pp.pInfo[_nCurrIndex].CC1LED4Max_14V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[8].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[8].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion


                #region 16V CC1 LED1
                if (testData.CC1LED1_16V < _pp.pInfo[_nCurrIndex].CC1LED1Min_16V || testData.CC1LED1_16V > _pp.pInfo[_nCurrIndex].CC1LED1Max_16V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[9].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[9].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED2_16V < _pp.pInfo[_nCurrIndex].CC1LED2Min_16V || testData.CC1LED2_16V > _pp.pInfo[_nCurrIndex].CC1LED2Max_16V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[10].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[10].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED3_16V < _pp.pInfo[_nCurrIndex].CC1LED3Min_16V || testData.CC1LED3_16V > _pp.pInfo[_nCurrIndex].CC1LED3Max_16V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[11].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[11].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED4_16V < _pp.pInfo[_nCurrIndex].CC1LED4Min_16V || testData.CC1LED4_16V > _pp.pInfo[_nCurrIndex].CC1LED4Max_16V)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[12].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //dgv[indexPro].Rows[12].Cells[5].Style.BackColor = Color.Green;
                }


                #endregion
                ///CCV2 LED
                #region 9V CC2 LED1

                if (testData.CC2LED1_9V < _pp.pInfo[_nCurrIndex].CC1LED1Min_9V || testData.CC2LED1_9V > _pp.pInfo[_nCurrIndex].CC2LED1Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[13].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[13].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED2
                if (testData.CC2LED2_9V < _pp.pInfo[_nCurrIndex].CC2LED2Min_9V || testData.CC2LED2_9V > _pp.pInfo[_nCurrIndex].CC2LED2Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[14].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[14].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED3
                if (testData.CC2LED3_9V < _pp.pInfo[_nCurrIndex].CC2LED3Min_9V || testData.CC2LED3_9V > _pp.pInfo[_nCurrIndex].CC2LED3Max_9V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[15].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[15].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED4
                if (testData.CC2LED4_9V < _pp.pInfo[_nCurrIndex].CC2LED4Min_9V || testData.CC2LED4_9V > _pp.pInfo[_nCurrIndex].CC2LED4Max_9V)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[16].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[16].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion

                #region  //14V CC2 LED1

                if (testData.CC2LED1_14V < _pp.pInfo[_nCurrIndex].CC2LED1Min_14V || testData.CC2LED1_14V > _pp.pInfo[_nCurrIndex].CC2LED1Max_14V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[17].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[17].Cells[5].Style.BackColor = Color.Green;
                }


                //14V CC1 LED2
                if (testData.CC2LED2_14V < _pp.pInfo[_nCurrIndex].CC2LED2Min_14V || testData.CC2LED2_14V > _pp.pInfo[_nCurrIndex].CC2LED2Max_14V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[18].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[18].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC2LED3_14V < _pp.pInfo[_nCurrIndex].CC2LED3Min_14V || testData.CC2LED3_14V > _pp.pInfo[_nCurrIndex].CC2LED3Max_14V)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[19].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[19].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC2LED4_14V < _pp.pInfo[_nCurrIndex].CC2LED4Min_14V || testData.CC2LED4_14V > _pp.pInfo[_nCurrIndex].CC2LED4Max_14V)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[20].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[20].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region 16V CC2 LED1
                if (testData.CC2LED1_16V < _pp.pInfo[_nCurrIndex].CC2LED1Min_16V || testData.CC2LED1_16V > _pp.pInfo[_nCurrIndex].CC2LED1Max_16V)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[21].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[21].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED2_16V < _pp.pInfo[_nCurrIndex].CC2LED2Min_16V || testData.CC2LED2_16V > _pp.pInfo[_nCurrIndex].CC2LED2Max_16V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[22].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //dgv[indexPro].Rows[22].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED3_16V < _pp.pInfo[_nCurrIndex].CC2LED3Min_16V || testData.CC2LED3_16V > _pp.pInfo[_nCurrIndex].CC2LED3Max_16V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[23].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[23].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED4_16V < _pp.pInfo[_nCurrIndex].CC2LED4Min_16V || testData.CC2LED4_16V > _pp.pInfo[_nCurrIndex].CC2LED4Max_16V)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[24].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[24].Cells[5].Style.BackColor = Color.Green;
                }


                #endregion


                if (testData.out5V < _pp.pInfo[_nCurrIndex].out5VMin || testData.currStatic_18V > _pp.pInfo[_nCurrIndex].out5VMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[25].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[25].Cells[5].Style.BackColor = Color.Green;
                }


                #region 单通道逻辑输入电流

                if (testData.Logic_1 < _pp.pInfo[_nCurrIndex].Logic_1Min || testData.Logic_1 > _pp.pInfo[_nCurrIndex].Logic_1Max)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[26].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[26].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_2 < _pp.pInfo[_nCurrIndex].Logic_2Min || testData.Logic_2 > _pp.pInfo[_nCurrIndex].Logic_2Max)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[27].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[27].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_3 < _pp.pInfo[_nCurrIndex].Logic_3Min || testData.Logic_3 > _pp.pInfo[_nCurrIndex].Logic_3Max)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[28].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[28].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_4 < _pp.pInfo[_nCurrIndex].Logic_4Min || testData.Logic_4 > _pp.pInfo[_nCurrIndex].Logic_4Max)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[29].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[29].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_5 < _pp.pInfo[_nCurrIndex].Logic_5Min || testData.Logic_5 > _pp.pInfo[_nCurrIndex].Logic_5Max)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[30].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[30].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_6 < _pp.pInfo[_nCurrIndex].Logic_6Min || testData.Logic_6 > _pp.pInfo[_nCurrIndex].Logic_6Max)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[31].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[31].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion


                #region 【5】占空比

                if (testData.Logic_2Duty < _pp.pInfo[_nCurrIndex].Logic_2DutyMin || testData.Logic_2Duty > _pp.pInfo[_nCurrIndex].Logic_2DutyMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[32].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[32].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_3Duty < _pp.pInfo[_nCurrIndex].Logic_3DutyMin || testData.Logic_3Duty > _pp.pInfo[_nCurrIndex].Logic_3DutyMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[33].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[33].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_4Duty < _pp.pInfo[_nCurrIndex].Logic_4DutyMin || testData.Logic_4Duty > _pp.pInfo[_nCurrIndex].Logic_4DutyMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[34].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[34].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_5Duty < _pp.pInfo[_nCurrIndex].Logic_5DutyMin || testData.Logic_5Duty > _pp.pInfo[_nCurrIndex].Logic_5DutyMax)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[35].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[35].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_6Duty < _pp.pInfo[_nCurrIndex].Logic_6DutyMin || testData.Logic_6Duty > _pp.pInfo[_nCurrIndex].Logic_6DutyMax)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[36].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[36].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion


                #region 【6】报警信号参数

                if (testData.outage_riseTime < _pp.pInfo[_nCurrIndex].outage_riseTimeMin || testData.outage_riseTime > _pp.pInfo[_nCurrIndex].outage_riseTimeMax)
                {
                    bFlag = false;
                    //dgv[indexPro].Rows[37].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //      dgv[indexPro].Rows[37].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.outage_fallTime < _pp.pInfo[_nCurrIndex].outage_fallTimeMin || testData.outage_fallTime > _pp.pInfo[_nCurrIndex].outage_fallTimeMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[38].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[38].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.outageLv < _pp.pInfo[_nCurrIndex].outageLvMIin || testData.outage_riseTime > _pp.pInfo[_nCurrIndex].outageLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[39].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[39].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion

                #region 【7】HSSS 参数设置

                if (testData.HSSS1_riseLV < _pp.pInfo[_nCurrIndex].HSSS1_riseLVMin || testData.HSSS1_riseLV > _pp.pInfo[_nCurrIndex].HSSS1_riseLVMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[40].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[40].Cells[5].Style.BackColor = Color.Green;
                }


                if (testData.HSSS1_fallLV < _pp.pInfo[_nCurrIndex].HSSS1_fallLVMin || testData.HSSS1_fallLV > _pp.pInfo[_nCurrIndex].HSSS1_riseLVMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[41].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[41].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.HSSS1_riseTime < _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMin || testData.HSSS1_riseTime > _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMax)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[42].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[42].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.HSSS1_fallTime < _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMin || testData.HSSS1_fallTime > _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[43].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[43].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region 【8】 LSSS1 -3参数设置
                if (testData.LSSS1_riseLv < _pp.pInfo[_nCurrIndex].LSSS1_riseLvMin || testData.LSSS1_riseLv > _pp.pInfo[_nCurrIndex].LSSS1_riseLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[44].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[44].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS1_fallLv < _pp.pInfo[_nCurrIndex].LSSS1_riseLvMin || testData.LSSS1_fallLv > _pp.pInfo[_nCurrIndex].LSSS1_fallLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[45].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[45].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS1_riseTime < _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMin || testData.LSSS1_riseTime > _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMax)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[46].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[46].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS1_fallTime < _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMin || testData.LSSS1_fallTime > _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[47].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[47].Cells[5].Style.BackColor = Color.Green;
                }



                //LSSS2
                if (testData.LSSS2_riseLv < _pp.pInfo[_nCurrIndex].LSSS2_riseLvMin || testData.LSSS2_riseLv > _pp.pInfo[_nCurrIndex].LSSS2_riseLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[48].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[48].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS2_fallLv < _pp.pInfo[_nCurrIndex].LSSS2_riseLvMin || testData.LSSS2_fallLv > _pp.pInfo[_nCurrIndex].LSSS2_fallLvMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[49].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[49].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS2_riseTime < _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMin || testData.LSSS2_riseTime > _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[50].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[50].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS2_fallTime < _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMin || testData.LSSS2_fallTime > _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[51].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[51].Cells[5].Style.BackColor = Color.Green;
                }

                //LSS3
                if (testData.LSSS3_riseLv < _pp.pInfo[_nCurrIndex].LSSS3_riseLvMin || testData.LSSS3_riseLv > _pp.pInfo[_nCurrIndex].LSSS3_riseLvMax)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[52].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[52].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS3_fallLv < _pp.pInfo[_nCurrIndex].LSSS3_fallLvMin || testData.LSSS3_fallLv > _pp.pInfo[_nCurrIndex].LSSS3_fallLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[53].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[53].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS3_riseTime < _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMin || testData.LSSS3_riseTime > _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[54].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[54].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS3_fallTime < _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMin || testData.LSSS3_fallTime > _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[55].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[55].Cells[5].Style.BackColor = Color.Green;
                }
                //LSS4

                if (testData.LSSS4_riseLv < _pp.pInfo[_nCurrIndex].LSSS4_riseLvMin || testData.LSSS4_riseLv > _pp.pInfo[_nCurrIndex].LSSS4_riseLvMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[56].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[56].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS4_fallLv < _pp.pInfo[_nCurrIndex].LSSS4_fallLvMin || testData.LSSS4_fallLv > _pp.pInfo[_nCurrIndex].LSSS4_fallLvMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[57].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[57].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS4_riseTime < _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMin || testData.LSSS4_riseTime > _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[58].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[58].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS4_fallTime < _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMin || testData.LSSS4_fallTime > _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMax)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[59].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[59].Cells[5].Style.BackColor = Color.Green;
                }




                #endregion

                #region RBIN 电阻

                if (testData.RBinS1 < _pp.pInfo[_nCurrIndex].RBinS1Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS1Max)
                {
                    bFlag = false;
                    //     dgv[indexPro].Rows[60].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[60].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.RBinS2 < _pp.pInfo[_nCurrIndex].RBinS2Min || testData.RBinS2 > _pp.pInfo[_nCurrIndex].RBinS2Max)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[61].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[61].Cells[5].Style.BackColor = Color.Green;
                }


                if (testData.RBinS3 < _pp.pInfo[_nCurrIndex].RBinS3Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS3Max)
                {
                    bFlag = false;
                    //   dgv[indexPro].Rows[62].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[62].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.RBinS4 < _pp.pInfo[_nCurrIndex].RBinS4Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS4Max)
                {
                    bFlag = false;
                    // dgv[indexPro].Rows[63].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[63].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                //NTC
                if (testData.NTC1 < _pp.pInfo[_nCurrIndex].NTC1Min || testData.NTC1 > _pp.pInfo[_nCurrIndex].NTC2Max)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[64].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[64].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.NTC2 < _pp.pInfo[_nCurrIndex].NTC2Min || testData.NTC2 > _pp.pInfo[_nCurrIndex].NTC2Max)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[65].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //   dgv[indexPro].Rows[65].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.Uart_Test == false)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[66].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    //  dgv[indexPro].Rows[66].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.startSleep < _pp.pInfo[_nCurrIndex].startSleepMin || testData.startSleep > _pp.pInfo[_nCurrIndex].startSleepMax)
                {
                    bFlag = false;
                    //  dgv[indexPro].Rows[67].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    // dgv[indexPro].Rows[67].Cells[5].Style.BackColor = Color.Green;
                }









                return bFlag;
            }
        }




        private bool judgeResult_MLD_NO2(int indexPro, Product_MLD testData)
        {
            bool bFlag = true;
            this.Invoke(new System.Action(() =>
            {
                //18V静态电流

                if (testData.currStatic_18V < _pp.pInfo[_nCurrIndex].currStaticMin_18V || testData.currStatic_18V > _pp.pInfo[_nCurrIndex].currStaticMax_18V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[0].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[0].Cells[5].Style.BackColor = Color.Green;
                }
                #region 9V CC1 LED1

                if (testData.CC1LED1_9V < _pp.pInfo[_nCurrIndex].CC1LED1Min_9V || testData.CC1LED1_9V > _pp.pInfo[_nCurrIndex].CC1LED1Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[1].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[1].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED2
                if (testData.CC1LED2_9V < _pp.pInfo[_nCurrIndex].CC1LED2Min_9V || testData.CC1LED2_9V > _pp.pInfo[_nCurrIndex].CC1LED2Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[2].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[2].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED3
                if (testData.CC1LED3_9V < _pp.pInfo[_nCurrIndex].CC1LED3Min_9V || testData.CC1LED3_9V > _pp.pInfo[_nCurrIndex].CC1LED3Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[3].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[3].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED4
                if (testData.CC1LED4_9V < _pp.pInfo[_nCurrIndex].CC1LED4Min_9V || testData.CC1LED4_9V > _pp.pInfo[_nCurrIndex].CC1LED4Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[4].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[4].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region  //14V CC1 LED1

                if (testData.CC1LED1_14V < _pp.pInfo[_nCurrIndex].CC1LED1Min_14V || testData.CC1LED1_14V > _pp.pInfo[_nCurrIndex].CC1LED1Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[5].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[5].Cells[5].Style.BackColor = Color.Green;
                }


                //14V CC1 LED2
                if (testData.CC1LED2_14V < _pp.pInfo[_nCurrIndex].CC1LED2Min_14V || testData.CC1LED2_14V > _pp.pInfo[_nCurrIndex].CC1LED2Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[6].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[6].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC1LED3_14V < _pp.pInfo[_nCurrIndex].CC1LED3Min_14V || testData.CC1LED3_14V > _pp.pInfo[_nCurrIndex].CC1LED3Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[7].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[7].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC1LED4_14V < _pp.pInfo[_nCurrIndex].CC1LED4Min_14V || testData.CC1LED4_14V > _pp.pInfo[_nCurrIndex].CC1LED4Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[8].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[8].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion


                #region 16V CC1 LED1
                if (testData.CC1LED1_16V < _pp.pInfo[_nCurrIndex].CC1LED1Min_16V || testData.CC1LED1_16V > _pp.pInfo[_nCurrIndex].CC1LED1Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[9].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[9].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED2_16V < _pp.pInfo[_nCurrIndex].CC1LED2Min_16V || testData.CC1LED2_16V > _pp.pInfo[_nCurrIndex].CC1LED2Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[10].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[10].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED3_16V < _pp.pInfo[_nCurrIndex].CC1LED3Min_16V || testData.CC1LED3_16V > _pp.pInfo[_nCurrIndex].CC1LED3Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[11].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[11].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC1LED4_16V < _pp.pInfo[_nCurrIndex].CC1LED4Min_16V || testData.CC1LED4_16V > _pp.pInfo[_nCurrIndex].CC1LED4Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[12].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[12].Cells[5].Style.BackColor = Color.Green;
                }


                #endregion
                ///CCV2 LED
                #region 9V CC2 LED1

                if (testData.CC2LED1_9V < _pp.pInfo[_nCurrIndex].CC1LED1Min_9V || testData.CC2LED1_9V > _pp.pInfo[_nCurrIndex].CC2LED1Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[13].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[13].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED2
                if (testData.CC2LED2_9V < _pp.pInfo[_nCurrIndex].CC2LED2Min_9V || testData.CC2LED2_9V > _pp.pInfo[_nCurrIndex].CC2LED2Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[14].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[14].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED3
                if (testData.CC2LED3_9V < _pp.pInfo[_nCurrIndex].CC2LED3Min_9V || testData.CC2LED3_9V > _pp.pInfo[_nCurrIndex].CC2LED3Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[15].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[15].Cells[5].Style.BackColor = Color.Green;
                }

                //9V CC1 LED4
                if (testData.CC2LED4_9V < _pp.pInfo[_nCurrIndex].CC2LED4Min_9V || testData.CC2LED4_9V > _pp.pInfo[_nCurrIndex].CC2LED4Max_9V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[16].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[16].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion

                #region  //14V CC2 LED1

                if (testData.CC2LED1_14V < _pp.pInfo[_nCurrIndex].CC2LED1Min_14V || testData.CC2LED1_14V > _pp.pInfo[_nCurrIndex].CC2LED1Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[17].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[17].Cells[5].Style.BackColor = Color.Green;
                }


                //14V CC1 LED2
                if (testData.CC2LED2_14V < _pp.pInfo[_nCurrIndex].CC2LED2Min_14V || testData.CC2LED2_14V > _pp.pInfo[_nCurrIndex].CC2LED2Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[18].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[18].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC2LED3_14V < _pp.pInfo[_nCurrIndex].CC2LED3Min_14V || testData.CC2LED3_14V > _pp.pInfo[_nCurrIndex].CC2LED3Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[19].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[19].Cells[5].Style.BackColor = Color.Green;
                }

                //14 CC1 LED4
                if (testData.CC2LED4_14V < _pp.pInfo[_nCurrIndex].CC2LED4Min_14V || testData.CC2LED4_14V > _pp.pInfo[_nCurrIndex].CC2LED4Max_14V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[20].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[20].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region 16V CC2 LED1
                if (testData.CC2LED1_16V < _pp.pInfo[_nCurrIndex].CC2LED1Min_16V || testData.CC2LED1_16V > _pp.pInfo[_nCurrIndex].CC2LED1Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[21].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[21].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED2_16V < _pp.pInfo[_nCurrIndex].CC2LED2Min_16V || testData.CC2LED2_16V > _pp.pInfo[_nCurrIndex].CC2LED2Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[22].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[22].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED3_16V < _pp.pInfo[_nCurrIndex].CC2LED3Min_16V || testData.CC2LED3_16V > _pp.pInfo[_nCurrIndex].CC2LED3Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[23].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[23].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.CC2LED4_16V < _pp.pInfo[_nCurrIndex].CC2LED4Min_16V || testData.CC2LED4_16V > _pp.pInfo[_nCurrIndex].CC2LED4Max_16V)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[24].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[24].Cells[5].Style.BackColor = Color.Green;
                }


                #endregion


                if (testData.out5V < _pp.pInfo[_nCurrIndex].out5VMin || testData.currStatic_18V > _pp.pInfo[_nCurrIndex].out5VMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[25].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[25].Cells[5].Style.BackColor = Color.Green;
                }


                #region 单通道逻辑输入电流

                if (testData.Logic_1 < _pp.pInfo[_nCurrIndex].Logic_1Min || testData.Logic_1 > _pp.pInfo[_nCurrIndex].Logic_1Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[26].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[26].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_2 < _pp.pInfo[_nCurrIndex].Logic_2Min || testData.Logic_2 > _pp.pInfo[_nCurrIndex].Logic_2Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[27].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[27].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_3 < _pp.pInfo[_nCurrIndex].Logic_3Min || testData.Logic_3 > _pp.pInfo[_nCurrIndex].Logic_3Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[28].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[28].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_4 < _pp.pInfo[_nCurrIndex].Logic_4Min || testData.Logic_4 > _pp.pInfo[_nCurrIndex].Logic_4Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[29].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[29].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_5 < _pp.pInfo[_nCurrIndex].Logic_5Min || testData.Logic_5 > _pp.pInfo[_nCurrIndex].Logic_5Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[30].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[30].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_6 < _pp.pInfo[_nCurrIndex].Logic_6Min || testData.Logic_6 > _pp.pInfo[_nCurrIndex].Logic_6Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[31].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[31].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion


                #region 【5】占空比

                if (testData.Logic_2Duty < _pp.pInfo[_nCurrIndex].Logic_2DutyMin || testData.Logic_2Duty > _pp.pInfo[_nCurrIndex].Logic_2DutyMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[32].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[32].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_3Duty < _pp.pInfo[_nCurrIndex].Logic_3DutyMin || testData.Logic_3Duty > _pp.pInfo[_nCurrIndex].Logic_3DutyMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[33].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[33].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_4Duty < _pp.pInfo[_nCurrIndex].Logic_4DutyMin || testData.Logic_4Duty > _pp.pInfo[_nCurrIndex].Logic_4DutyMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[34].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[34].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_5Duty < _pp.pInfo[_nCurrIndex].Logic_5DutyMin || testData.Logic_5Duty > _pp.pInfo[_nCurrIndex].Logic_5DutyMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[35].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[35].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.Logic_6Duty < _pp.pInfo[_nCurrIndex].Logic_6DutyMin || testData.Logic_6Duty > _pp.pInfo[_nCurrIndex].Logic_6DutyMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[36].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[36].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion


                #region 【6】报警信号参数

                if (testData.outage_riseTime < _pp.pInfo[_nCurrIndex].outage_riseTimeMin || testData.outage_riseTime > _pp.pInfo[_nCurrIndex].outage_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[37].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[37].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.outage_fallTime < _pp.pInfo[_nCurrIndex].outage_fallTimeMin || testData.outage_fallTime > _pp.pInfo[_nCurrIndex].outage_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[38].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[38].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.outageLv < _pp.pInfo[_nCurrIndex].outageLvMIin || testData.outage_riseTime > _pp.pInfo[_nCurrIndex].outageLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[39].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[39].Cells[5].Style.BackColor = Color.Green;
                }

                #endregion

                #region 【7】HSSS 参数设置

                if (testData.HSSS1_riseLV < _pp.pInfo[_nCurrIndex].HSSS1_riseLVMin || testData.HSSS1_riseLV > _pp.pInfo[_nCurrIndex].HSSS1_riseLVMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[40].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[40].Cells[5].Style.BackColor = Color.Green;
                }


                if (testData.HSSS1_fallLV < _pp.pInfo[_nCurrIndex].HSSS1_fallLVMin || testData.HSSS1_fallLV > _pp.pInfo[_nCurrIndex].HSSS1_riseLVMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[41].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[41].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.HSSS1_riseTime < _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMin || testData.HSSS1_riseTime > _pp.pInfo[_nCurrIndex].HSSS1_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[42].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[42].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.HSSS1_fallTime < _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMin || testData.HSSS1_fallTime > _pp.pInfo[_nCurrIndex].HSSS1_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[43].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[43].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                #region 【8】 LSSS1 -3参数设置
                if (testData.LSSS1_riseLv < _pp.pInfo[_nCurrIndex].LSSS1_riseLvMin || testData.LSSS1_riseLv > _pp.pInfo[_nCurrIndex].LSSS1_riseLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[44].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[44].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS1_fallLv < _pp.pInfo[_nCurrIndex].LSSS1_riseLvMin || testData.LSSS1_fallLv > _pp.pInfo[_nCurrIndex].LSSS1_fallLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[45].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[45].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS1_riseTime < _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMin || testData.LSSS1_riseTime > _pp.pInfo[_nCurrIndex].LSSS1_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[46].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[46].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS1_fallTime < _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMin || testData.LSSS1_fallTime > _pp.pInfo[_nCurrIndex].LSSS1_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[47].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[47].Cells[5].Style.BackColor = Color.Green;
                }



                //LSSS2
                if (testData.LSSS2_riseLv < _pp.pInfo[_nCurrIndex].LSSS2_riseLvMin || testData.LSSS2_riseLv > _pp.pInfo[_nCurrIndex].LSSS2_riseLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[48].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[48].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS2_fallLv < _pp.pInfo[_nCurrIndex].LSSS2_riseLvMin || testData.LSSS2_fallLv > _pp.pInfo[_nCurrIndex].LSSS2_fallLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[49].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[49].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS2_riseTime < _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMin || testData.LSSS2_riseTime > _pp.pInfo[_nCurrIndex].LSSS2_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[50].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[50].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS2_fallTime < _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMin || testData.LSSS2_fallTime > _pp.pInfo[_nCurrIndex].LSSS2_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[51].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[51].Cells[5].Style.BackColor = Color.Green;
                }

                //LSS3
                if (testData.LSSS3_riseLv < _pp.pInfo[_nCurrIndex].LSSS3_riseLvMin || testData.LSSS3_riseLv > _pp.pInfo[_nCurrIndex].LSSS3_riseLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[52].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[52].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS3_fallLv < _pp.pInfo[_nCurrIndex].LSSS3_fallLvMin || testData.LSSS3_fallLv > _pp.pInfo[_nCurrIndex].LSSS3_fallLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[53].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[53].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS3_riseTime < _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMin || testData.LSSS3_riseTime > _pp.pInfo[_nCurrIndex].LSSS3_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[54].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[54].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS3_fallTime < _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMin || testData.LSSS3_fallTime > _pp.pInfo[_nCurrIndex].LSSS3_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[55].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[55].Cells[5].Style.BackColor = Color.Green;
                }
                //LSS4

                if (testData.LSSS4_riseLv < _pp.pInfo[_nCurrIndex].LSSS4_riseLvMin || testData.LSSS4_riseLv > _pp.pInfo[_nCurrIndex].LSSS4_riseLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[56].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[56].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS4_fallLv < _pp.pInfo[_nCurrIndex].LSSS4_fallLvMin || testData.LSSS4_fallLv > _pp.pInfo[_nCurrIndex].LSSS4_fallLvMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[57].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[57].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.LSSS4_riseTime < _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMin || testData.LSSS4_riseTime > _pp.pInfo[_nCurrIndex].LSSS4_riseTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[58].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[58].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.LSSS4_fallTime < _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMin || testData.LSSS4_fallTime > _pp.pInfo[_nCurrIndex].LSSS4_fallTimeMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[59].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[59].Cells[5].Style.BackColor = Color.Green;
                }




                #endregion

                #region RBIN 电阻

                if (testData.RBinS1 < _pp.pInfo[_nCurrIndex].RBinS1Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS1Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[60].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[60].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.RBinS2 < _pp.pInfo[_nCurrIndex].RBinS2Min || testData.RBinS2 > _pp.pInfo[_nCurrIndex].RBinS2Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[61].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[61].Cells[5].Style.BackColor = Color.Green;
                }


                if (testData.RBinS3 < _pp.pInfo[_nCurrIndex].RBinS3Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS3Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[62].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[62].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.RBinS4 < _pp.pInfo[_nCurrIndex].RBinS4Min || testData.RBinS1 > _pp.pInfo[_nCurrIndex].RBinS4Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[63].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[63].Cells[5].Style.BackColor = Color.Green;
                }
                #endregion
                //NTC
                if (testData.NTC1 < _pp.pInfo[_nCurrIndex].NTC1Min || testData.NTC1 > _pp.pInfo[_nCurrIndex].NTC2Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[64].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[64].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.NTC2 < _pp.pInfo[_nCurrIndex].NTC2Min || testData.NTC2 > _pp.pInfo[_nCurrIndex].NTC2Max)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[65].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[65].Cells[5].Style.BackColor = Color.Green;
                }

                if (testData.Uart_Test == false)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[66].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[66].Cells[5].Style.BackColor = Color.Green;
                }
                if (testData.startSleep < _pp.pInfo[_nCurrIndex].startSleepMin || testData.startSleep > _pp.pInfo[_nCurrIndex].startSleepMax)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[67].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[67].Cells[5].Style.BackColor = Color.Green;
                }




                //FBL版本号
                if (testData.FBLPN != "")
                {
                    if (testData.FBLPN != _pp.pInfo[_nCurrIndex].FBLPN && testData.FBLVer != _pp.pInfo[_nCurrIndex].FBLVer)
                    {
                        bFlag = false;
                        dgv[indexPro].Rows[68].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgv[indexPro].Rows[68].Cells[5].Style.BackColor = Color.Green;
                    }
                }


                //APP版本号
                if (_pp.pInfo[_nCurrIndex].AppPN != "")
                {
                    if (testData.AppPN != _pp.pInfo[_nCurrIndex].AppPN && testData.AppVer != _pp.pInfo[_nCurrIndex].AppVer)
                    {
                        bFlag = false;
                        dgv[indexPro].Rows[69].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgv[indexPro].Rows[69].Cells[5].Style.BackColor = Color.Green;
                    }
                }


                //Cali版本号
                if (_pp.pInfo[_nCurrIndex].CaliPN != "")
                {
                    if (testData.CaliPN != _pp.pInfo[_nCurrIndex].CaliPN && testData.CaliVer != _pp.pInfo[_nCurrIndex].CaliVer)
                    {
                        bFlag = false;
                        dgv[indexPro].Rows[70].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgv[indexPro].Rows[70].Cells[5].Style.BackColor = Color.Green;
                    }
                }


                //Anime版本号
                if (_pp.pInfo[_nCurrIndex].AnimePN != "")
                {
                    if (testData.AnimePN != _pp.pInfo[_nCurrIndex].AnimePN && testData.AnimeVer != _pp.pInfo[_nCurrIndex].AnimeVer)
                    {
                        bFlag = false;
                        dgv[indexPro].Rows[71].Cells[5].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        dgv[indexPro].Rows[71].Cells[5].Style.BackColor = Color.Green;
                    }
                }

                //刷写
                if (!testData.isDownLoad_ok)
                {
                    bFlag = false;
                    dgv[indexPro].Rows[72].Cells[5].Style.BackColor = Color.Red;
                }
                else
                {
                    dgv[indexPro].Rows[72].Cells[5].Style.BackColor = Color.Green;
                }

            }));



            return bFlag;
        }
        /// <summary>
        /// 测试数据显示
        /// </summary>
        private void DisplayMLD(int indexPro, Product_MLD product_MLD)
        {
            #region 显示数据
            this.Invoke(new System.Action(() =>
            {
                dgv[indexPro].Rows[0].Cells[2].Value = product_MLD.currStatic_18V.ToString("f3") + "uA";


                dgv[indexPro].Rows[1].Cells[2].Value = product_MLD.CC1LED1_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[2].Cells[2].Value = product_MLD.CC1LED2_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[3].Cells[2].Value = product_MLD.CC1LED3_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[4].Cells[2].Value = product_MLD.CC1LED4_9V.ToString("f3") + "A";

                dgv[indexPro].Rows[5].Cells[2].Value = product_MLD.CC1LED1_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[6].Cells[2].Value = product_MLD.CC1LED2_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[7].Cells[2].Value = product_MLD.CC1LED3_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[8].Cells[2].Value = product_MLD.CC1LED4_14V.ToString("f3") + "A";

                dgv[indexPro].Rows[9].Cells[2].Value = product_MLD.CC1LED1_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[10].Cells[2].Value = product_MLD.CC1LED2_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[11].Cells[2].Value = product_MLD.CC1LED3_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[12].Cells[2].Value = product_MLD.CC1LED4_16V.ToString("f3") + "A";


                dgv[indexPro].Rows[13].Cells[2].Value = product_MLD.CC2LED1_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[14].Cells[2].Value = product_MLD.CC2LED2_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[15].Cells[2].Value = product_MLD.CC2LED3_9V.ToString("f3") + "A";
                dgv[indexPro].Rows[16].Cells[2].Value = product_MLD.CC2LED4_9V.ToString("f3") + "A";

                dgv[indexPro].Rows[17].Cells[2].Value = product_MLD.CC2LED1_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[18].Cells[2].Value = product_MLD.CC2LED2_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[19].Cells[2].Value = product_MLD.CC2LED3_14V.ToString("f3") + "A";
                dgv[indexPro].Rows[20].Cells[2].Value = product_MLD.CC2LED4_14V.ToString("f3") + "A";

                dgv[indexPro].Rows[21].Cells[2].Value = product_MLD.CC2LED1_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[22].Cells[2].Value = product_MLD.CC2LED2_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[23].Cells[2].Value = product_MLD.CC2LED3_16V.ToString("f3") + "A";
                dgv[indexPro].Rows[24].Cells[2].Value = product_MLD.CC2LED4_16V.ToString("f3") + "A";


                dgv[indexPro].Rows[25].Cells[2].Value = product_MLD.out5V.ToString("f3") + "V";


                //【4】单通道逻辑输入电流

                dgv[indexPro].Rows[26].Cells[2].Value = product_MLD.Logic_1.ToString("f3") + "mA";
                dgv[indexPro].Rows[27].Cells[2].Value = product_MLD.Logic_2.ToString("f3") + "mA";
                dgv[indexPro].Rows[28].Cells[2].Value = product_MLD.Logic_3.ToString("f3") + "mA";
                dgv[indexPro].Rows[29].Cells[2].Value = product_MLD.Logic_4.ToString("f3") + "mA";
                dgv[indexPro].Rows[30].Cells[2].Value = product_MLD.Logic_5.ToString("f3") + "mA";
                dgv[indexPro].Rows[31].Cells[2].Value = product_MLD.Logic_6.ToString("f3") + "mA";

                //【5】占空比

                dgv[indexPro].Rows[32].Cells[2].Value = product_MLD.Logic_2Duty.ToString("f3") + "%";
                dgv[indexPro].Rows[33].Cells[2].Value = product_MLD.Logic_3Duty.ToString("f3") + "%";
                dgv[indexPro].Rows[34].Cells[2].Value = product_MLD.Logic_4Duty.ToString("f3") + "%";
                dgv[indexPro].Rows[35].Cells[2].Value = product_MLD.Logic_5Duty.ToString("f3") + "%";
                dgv[indexPro].Rows[36].Cells[2].Value = product_MLD.Logic_6Duty.ToString("f3") + "%";

                //【6】报警信号参数
                dgv[indexPro].Rows[37].Cells[2].Value = product_MLD.outage_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[38].Cells[2].Value = product_MLD.outage_fallTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[39].Cells[2].Value = product_MLD.outageLv.ToString("f3") + "V";


                //【7】 HSSS 参数设置
                dgv[indexPro].Rows[40].Cells[2].Value = product_MLD.HSSS1_riseLV.ToString("f3") + "V";
                dgv[indexPro].Rows[41].Cells[2].Value = product_MLD.HSSS1_fallLV.ToString("f3") + "V";
                dgv[indexPro].Rows[42].Cells[2].Value = product_MLD.HSSS1_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[43].Cells[2].Value = product_MLD.HSSS1_fallTime.ToString("f3") + "ms";

                //【7】 LSSS 参数设置
                dgv[indexPro].Rows[44].Cells[2].Value = product_MLD.LSSS1_riseLv.ToString("f3") + "V";
                dgv[indexPro].Rows[45].Cells[2].Value = product_MLD.LSSS1_fallLv.ToString("f3") + "V";
                dgv[indexPro].Rows[46].Cells[2].Value = product_MLD.LSSS1_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[47].Cells[2].Value = product_MLD.LSSS1_fallTime.ToString("f3") + "ms";

                dgv[indexPro].Rows[48].Cells[2].Value = product_MLD.LSSS2_riseLv.ToString("f3") + "V";
                dgv[indexPro].Rows[49].Cells[2].Value = product_MLD.LSSS2_fallLv.ToString("f3") + "V";
                dgv[indexPro].Rows[50].Cells[2].Value = product_MLD.LSSS2_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[51].Cells[2].Value = product_MLD.LSSS2_fallTime.ToString("f3") + "ms";


                dgv[indexPro].Rows[52].Cells[2].Value = product_MLD.LSSS3_riseLv.ToString("f3") + "V";
                dgv[indexPro].Rows[53].Cells[2].Value = product_MLD.LSSS3_fallLv.ToString("f3") + "V";
                dgv[indexPro].Rows[54].Cells[2].Value = product_MLD.LSSS3_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[55].Cells[2].Value = product_MLD.LSSS3_fallTime.ToString("f3") + "ms";

                dgv[indexPro].Rows[56].Cells[2].Value = product_MLD.LSSS4_riseLv.ToString("f3") + "V";
                dgv[indexPro].Rows[57].Cells[2].Value = product_MLD.LSSS4_fallLv.ToString("f3") + "V";
                dgv[indexPro].Rows[58].Cells[2].Value = product_MLD.LSSS4_riseTime.ToString("f3") + "ms";
                dgv[indexPro].Rows[59].Cells[2].Value = product_MLD.LSSS4_fallTime.ToString("f3") + "ms";


                //【8】RBin电阻
                dgv[indexPro].Rows[60].Cells[2].Value = product_MLD.RBinS1.ToString("f3") + "V";
                dgv[indexPro].Rows[61].Cells[2].Value = product_MLD.RBinS2.ToString("f3") + "V";
                dgv[indexPro].Rows[62].Cells[2].Value = product_MLD.RBinS3.ToString("f3") + "V";
                dgv[indexPro].Rows[63].Cells[2].Value = product_MLD.RBinS4.ToString("f3") + "V";


                //【9】RBin电阻
                dgv[indexPro].Rows[64].Cells[2].Value = product_MLD.NTC1.ToString("f3") + "V";
                dgv[indexPro].Rows[65].Cells[2].Value = product_MLD.NTC2.ToString("f3") + "V";

                ////【10】UART can
                dgv[indexPro].Rows[66].Cells[2].Value = product_MLD.Uart_Test?"Uart功能测试OK" : "Uart功能测试NG";
                //【11】启动延时
                dgv[indexPro].Rows[67].Cells[2].Value = product_MLD.startSleep.ToString("f3") + "ms";

                dgv[indexPro].Rows[68].Cells[2].Value = product_MLD.FBLPN.ToString() + " " + product_MLD.FBLVer.ToString();
                dgv[indexPro].Rows[69].Cells[2].Value = product_MLD.AppPN.ToString() + " " + product_MLD.AnimeVer.ToString();
                dgv[indexPro].Rows[70].Cells[2].Value = product_MLD.CaliPN.ToString() + " " + product_MLD.CaliVer.ToString();
                dgv[indexPro].Rows[71].Cells[2].Value = product_MLD.AnimePN.ToString() + " " + product_MLD.AnimeVer.ToString();

                dgv[indexPro].Rows[72].Cells[2].Value = product_MLD.isDownLoad_ok ? "刷写成功" : "刷写失败";
            }));

            #endregion



        }






        private void testSimulate_Data(Product_MLD product_MLD)
        {
            #region  设置检测参数
            //【1】25°18V输出精度
            product_MLD.currStatic_18V = 0;


            //版本号
            product_MLD. AppPN = "1";
            product_MLD. AppVer = "1";
            product_MLD .CaliPN = "1";
            product_MLD .CaliVer = "1";
            product_MLD. AnimePN = "1";
            product_MLD .AnimeVer = "1";
            product_MLD. FBLPN = "1";
            product_MLD. FBLVer = "1";


        //【2】****CC1恒流输出
        product_MLD.CC1LED1_9V = 2;

            product_MLD.CC1LED2_9V = 1;

            product_MLD.CC1LED3_9V = 1;

            product_MLD.CC1LED4_9V = 4;



            product_MLD.CC1LED1_14V = 1;

            product_MLD.CC1LED2_14V = 1;

            product_MLD.CC1LED3_14V = 1;

            product_MLD.CC1LED4_14V =5;


            product_MLD.CC1LED1_16V = 1;

            product_MLD.CC1LED2_16V = 1;

            product_MLD.CC1LED3_16V = 1;

            product_MLD.CC1LED4_16V = 6;


            //CC2恒流输出
            product_MLD.CC2LED1_9V = 1;

            product_MLD.CC2LED2_9V = 1;

            product_MLD.CC2LED3_9V = 1;

            product_MLD.CC2LED4_9V = 1;


            product_MLD.CC2LED1_14V = 8;

            product_MLD.CC2LED2_14V = 1;

            product_MLD.CC2LED3_14V = 1;

            product_MLD.CC2LED4_14V = 1;

            product_MLD.CC2LED1_16V = 1;

            product_MLD.CC2LED2_16V = 1;

            product_MLD.CC2LED3_16V = 1;

            product_MLD.CC2LED4_16V = 7;


            //【3】5V输出精度
            product_MLD.out5V = 1;

            //【4】单通道逻辑输入电流
            product_MLD.Logic_1 = 1;

            product_MLD.Logic_2 = 1;

            product_MLD.Logic_3 = 1;

            product_MLD.Logic_4 = 1;

            product_MLD.Logic_5 = 1;

            product_MLD.Logic_6 = 1;


            //【5】占空比
            product_MLD.Logic_2Duty = 4;

            product_MLD.Logic_3Duty = 1;

            product_MLD.Logic_4Duty = 1;

            product_MLD.Logic_5Duty = 1;

            product_MLD.Logic_6Duty = 1;


            //【6】报警信号参数
            product_MLD.outage_riseTime = 1;

            product_MLD.outage_fallTime = 1;

            //报警信号低电平
            product_MLD.outageLv = 1;


            //【7】 HSSS 参数设置
            product_MLD.HSSS1_riseLV = 1;//上升沿电压

            product_MLD.HSSS1_fallLV = 1;//下降延电压


            product_MLD.HSSS1_riseTime = 1;


            product_MLD.HSSS1_fallTime = 1;


            //【7】 LSSS 参数设置
            product_MLD.LSSS1_riseLv = 1;

            product_MLD.LSSS1_fallLv = 1;

            product_MLD.LSSS1_riseTime = 1;

            product_MLD.LSSS1_fallTime = 1;


            product_MLD.LSSS2_riseLv = 1;

            product_MLD.LSSS2_fallLv = 1;

            product_MLD.LSSS2_riseTime = 1;

            product_MLD.LSSS2_fallTime = 1;


            product_MLD.LSSS3_riseLv = 1;

            product_MLD.LSSS3_fallLv = 1;

            product_MLD.LSSS3_riseTime = 1;

            product_MLD.LSSS3_fallTime = 1;


            product_MLD.LSSS4_riseLv = 1;

            product_MLD.LSSS4_fallLv = 1;

            product_MLD.LSSS4_riseTime = 1;

            product_MLD.LSSS4_fallTime = 1;

            //【8】RBin电阻
            product_MLD.RBinS1 = 1;

            product_MLD.RBinS2 = 1;

            product_MLD.RBinS3 = 1;

            product_MLD.RBinS4 = 1;


            product_MLD.NTC1 = 1;

            product_MLD.NTC2 = 1;

            //启动延时设置
            product_MLD.startSleep = 1;

 bool isDownLoad_ok = true;
        #endregion
        


    }




















        private bool isBusy = false;
        private bool isRelayBusy = false;
        private PLC_Variable pLC_Variable = new PLC_Variable();
        private void sttScheduleCallback(object state)
        {
            if (this.isBusy)
            {
                return;
            }
            this.isBusy = true;

            if (this.taskConQueue.IsEmpty)
            {
                this.isBusy = false;
                return;
            }

            bool task;
          
            try
            {
              //  this.taskConQueue.Enqueue(device.modbusTCPClient.ReadKeepReg(1, 150, 9));
                this.taskConQueue.TryDequeue(out task);

                if (!task)
                {
                    ////System.Console.WriteLine("重新连接------------------------------;{0}", DateTime.Now.ToString(".fff"));
                    this.timerCycleHeartBeat.Change(System.Threading.Timeout.Infinite, 1000);//间隔2S 捕捉一次
                    this.sttSchedule.Change(50, 200);
                    this.isBusy = false;
                    return;
                }
                else
                {
                }
            }
            catch
            {
                this.isBusy = false;
                return;
            }
            this.isBusy = false;
        }



        /// <summary>
        /// PLC心跳 3s心跳一次
        /// </summary>
        /// <param name="state"></param>
        private void timerCycleHeartBeatCallback(object state)
        {


            this.taskConQueue.Enqueue(device.modbusTCPClient.ReadKeepReg(1, 150, 10));//读启动信号
            Thread.Sleep(20);
            device.modbusTCPClient.ReadKeepReg(1, 178, 1);
            this.taskConQueue.Enqueue(device.modbusTCPClient.ReadKeepReg(1, 178, 2));//心跳信号
            Thread.Sleep(20);
            this.taskConQueue.Enqueue(this.device.modbusTCPClient.ReadKeepReg(1, 7102, 6));//设备1    //外壳1二维码
            Thread.Sleep(20);
            this.taskConQueue.Enqueue(this.device.modbusTCPClient.ReadKeepReg(1, 7152, 6));//设备1    //外壳2二维码
            Thread.Sleep(20);
            this.taskConQueue.Enqueue(this.device.modbusTCPClient.ReadKeepReg(1, 7302, 6));//设备1    //外壳3二维码
            Thread.Sleep(20);
            this.taskConQueue.Enqueue(this.device.modbusTCPClient.ReadKeepReg(1, 7352, 6));//设备1    //外壳4二维码
            Thread.Sleep(20);
            if (pLC_Variable.ConState)//读取10个寄存器从PLC D150开始
            {
                if (pLC_Variable.PLC_Heart == 0x01)
                {
                    byte[] HeartBeat = new byte[2] { 0x00, 0x07 };
                    device.modbusTCPClient.PreSetMultiByteArray(1, 178, HeartBeat);//地址178 写心跳
                    Thread.Sleep(20);
                    this.Invoke(new System.Action(() =>
                    {
                        this.tsslState.Text = "与PLC连接状态：在线/";

                        tsslState.BackColor = Color.Green;

                        //if (GetPlcData[3] == 1)//产品品种选择
                        //{
                        //    comboBoxProduct.SelectedIndex = 0x00;
                        //}

                    }));

                }
                else
                {
                    byte[] HeartBeat = new byte[2] { 0x00, 0x01 };
                    device.modbusTCPClient.PreSetMultiByteArray(1, 178, HeartBeat);
                }

            }
            else
            {

                this.Invoke(new System.Action(() =>
                {
                    this.tsslState.Text = "与PLC连接状态：离线;";
                    tsslState.BackColor = Color.Red;
                    
                }));
            }




        }



        private void ModbusTCPClient_EventConnection(object sender, bool e)
        {
            var temp = (ModbusTCPClient)sender;
            pLC_Variable.ConState = e;
            Console.WriteLine($"tcp1连接状态:{e}");
        }

       

        private void ModbusTCPClient_EventReceive(object sender, ModbusTCPClient.ReceiveEventArgs e)
        {
            var temp = (ModbusTCPClient)sender;
            //       Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||数据:{Encoding.Default.GetString(e.ReceiveData.ToArray())}||RX;{e.ReceivedMegTCP}");
            //   Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接受数据:{BitConverter.ToString(e.ReceiveData)}");



            if (e.ReceiveData != null)
            {
                pLC_Variable.Station1_StartSinal = BitConverter.ToUInt16(e.ReceiveData, 1);//测试数据
                pLC_Variable.Station2_StartSinal = BitConverter.ToUInt16(e.ReceiveData, 7);
                pLC_Variable.Station3_StartSinal = BitConverter.ToUInt16(e.ReceiveData, 13);
                pLC_Variable.Station4_StartSinal = BitConverter.ToUInt16(e.ReceiveData, 15);
                pLC_Variable.PropSelection = BitConverter.ToUInt16(e.ReceiveData, 17);
                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收;启动信号-数据:{BitConverter.ToString(e.ReceiveData)}");
            }
            if (e.PLC_Heart != null)
            {
                pLC_Variable.PLC_Heart = BitConverter.ToUInt16(e.PLC_Heart, 1);
                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收:心跳-数据:{BitConverter.ToString(e.PLC_Heart)}");
            }

          
            if (e.RceeiveSN1!=null)
            {
                pLC_Variable.SN[0] =  asciiEncoding.GetString(e.RceeiveSN1, 0, 12) ;
            
               
                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收-扫码1数据:{BitConverter.ToString(e.RceeiveSN1)}");
             
            }
            if (e.RceeiveSN2 != null)
            {
                pLC_Variable.SN[1] = asciiEncoding.GetString(e.RceeiveSN2, 0, 12);


                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收-扫码2数据:{BitConverter.ToString(e.RceeiveSN2)}");

            }
            if (e.RceeiveSN3!= null)
            {
                pLC_Variable.SN[2] = asciiEncoding.GetString(e.RceeiveSN3, 0, 12);


                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收-扫码3数据:{BitConverter.ToString(e.RceeiveSN3)}");

            }
            if (e.RceeiveSN4 != null)
            {
                pLC_Variable.SN[3] = asciiEncoding.GetString(e.RceeiveSN4, 0, 12);


                Console.WriteLine($"tcp1耗时:{e.ReceivedTime}秒||接收-扫码4数据:{BitConverter.ToString(e.RceeiveSN4)}");

            }



        }

     

        /// <summary>
        /// 1开始测试
        /// </summary>
        private void MainPro1()
        {
            bool bOKFlag = false;
            int TxtBoxIndex = 0;
            bool RunState = false;
            while (true)
            {

                System.Threading.Thread.Sleep(300);
                TestProStateDisplay(TxtBoxIndex, "准备开始", Color.Yellow);

                int proIndex = 0;

                string SN1 = pLC_Variable.SN[proIndex];

                //测试
                string Manual = button1.Tag.ToString();
                if (button1.Tag.ToString()== "Start") // 获取PLC 启动信号
                {
                  
                    bOKFlag = DataProcessMLD(proIndex);


                    if (bOKFlag)
                    {

                      
                     
                        this.Invoke(new System.Action(() =>
                        {
                            this.TestProStateDisplay(TxtBoxIndex, "测试OK", Color.Green);
                            OKNumber += 1;
                            sumNumber += 1;
                            
                            //  tbOK.Text = string.Format("合格{0:d4}", OKNumber);
                            //OK信号
                        }));
                        device.modbusTCPClient.MLDTestComple_Result_1Station(bOKFlag);
                        Thread.Sleep(1000);
                    }

                    else
                    {

                        device.modbusTCPClient.MLDTestComple_Result_1Station(bOKFlag);
                        Thread.Sleep(1000);
                        //NG
                        this.Invoke(new System.Action(() =>
                        {
                            this.TestProStateDisplay(TxtBoxIndex, "测试NG", Color.Red);
                            //   sumNumber += 1;

                        }));


                    }
                    button1.Tag = "1";






                }



                //////test
                ///




                //if (pLC_Variable.Station1_StartSinal ==0x01) // 获取PLC 启动信号
                //  {
                //         RunState = true;
                //         bOKFlag = DataProcessMLD(proIndex);


                //      if (bOKFlag)
                //      {

                //          device.modbusTCPClient.MLDTestComple_Result_1Station(bOKFlag);

                //          this.Invoke(new System.Action(() =>
                //          {
                //              this.TestProStateDisplay(TxtBoxIndex, "测试OK", Color.Green);
                //              OKNumber += 1;
                //              sumNumber += 1;

                //              //  tbOK.Text = string.Format("合格{0:d4}", OKNumber);
                //              //OK信号
                //          }));
                //      }

                //      else
                //      {
                //          device.modbusTCPClient.MLDTestComple_Result_1Station(bOKFlag);
                //          Thread.Sleep(1000);
                //          //NG
                //          this.Invoke(new System.Action(() =>
                //          {
                //              this.TestProStateDisplay(TxtBoxIndex, "测试NG", Color.Red);
                //              //   sumNumber += 1;

                //          }));


                //      }



                //  } 



                timerPro_1.Stop();
                device.modbusTCPClient.PreSetMultiByteArray(1, proIndex + (int)(ModbusTCPClient.MyStationTestComplex.Eol1_Done_), new byte[] { 0x00, 0x00 });//测试结果NG 清零
                Thread.Sleep(20);
                device.modbusTCPClient.PreSetMultiByteArray(1, proIndex + (int)(ModbusTCPClient.MyStationTestResult.Eol1_result_), new byte[] { 0x00, 0x00 });//测试结果NG 清零
              
            }
        }



        /// <summary>
        /// 2开始测试
        /// </summary>
        private void MainPro2()
        {
            while (true)
            {

                System.Threading.Thread.Sleep(300);


                //   if (GetPlcData[5] == 1)//判断PLC数据流程开始
                //  {
                //int index = 0;
                //DataProcessMLD(index);
                // }
                //else
                //{
                //    return;
                //}


            }
        }



   
        private bool bFlagCMBProOnce = false;
        private bool bFlagCMBProSecond = false;
        private void comboBoxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {

            int nIndex = -1;
            nIndex = this.comboBoxProduct.SelectedIndex;
            if (nIndex == -1)
                return;

            if (this.bFlagCMBProOnce)
            {
                if (!this.bFlagCMBProSecond)
                {
                    
                    this._nCurrIndex = nIndex;

                    switch (_nCurrIndex)
                    {
                      

                        case 0://MLD
                            this.dgv[0].Rows.Clear();
                            this.dgv[1].Rows.Clear();
                            this.InitDatagridViewMLD(ref this.dgv[0]);
                            this.InitDatagridViewMLD(ref this.dgv[1]);
                            this.InitDatagridViewMLD(ref this.dgv[2]);
                            this.InitDatagridViewMLD(ref this.dgv[3]);
                            this.label_ProductPara.Text = "当前测试产品为：MLD光源控制器";

                            if (_pp.pInfo[_nCurrIndex].AppFilePath != "")
                            {
                                //Hexflash_MLD.ReadHexFile(_pp.pInfo[_nCurrIndex].AppFilePath, ref Hexflash_MLD.hexFiles_APP);
                                //Hexflash_MLD_1.ReadHexFile(_pp.pInfo[_nCurrIndex].AppFilePath, ref Hexflash_MLD_1.hexFiles_APP);
                            }

                            //Hexflash_MLD.ReadHexFile(_pp.pInfo[_nCurrIndex].CaliFilePath, ref Hexflash_MLD.hexFiles_Cali);
                            //Hexflash_MLD_1.ReadHexFile(_pp.pInfo[_nCurrIndex].CaliFilePath, ref Hexflash_MLD_1.hexFiles_Cali);

                            if (_pp.pInfo[_nCurrIndex].AnimeFilePath != "")
                            {
                           //     Hexflash_MLD.ReadHexFile(_pp.pInfo[_nCurrIndex].AnimeFilePath, ref Hexflash_MLD.hexFiles_Anim);
                             //   Hexflash_MLD_1.ReadHexFile(_pp.pInfo[_nCurrIndex].AnimeFilePath, ref Hexflash_MLD_1.hexFiles_Anim);
                            }

                            break;
                    }
                }
                else
                {
                    this.bFlagCMBProSecond = false;
                    return;
                }
            }
            this.bFlagCMBProOnce = true;



        }





        private void SaveDataIntoDataBase_MLD(Product_MLD testData)
        {
            //string strResult = (testData.TestResult == true) ? "OK" : "NG";

            //string sqlcmd = string.Format("Insert Into {0} (result,curtime,staticCurrent_18V,CC1LED1_9V, CC1LED2_9V, CC1LED3_9V,CC1LED4_9V, CC1LED1_14V, CC1LED2_14V, CC1LED3_14V,CC1LED4_14V, CC1LED1_16V,CC1LED2_16V,CC1LED3_16V,CC1LED4_16V,CC2LED1_9V,CC2LED2_9V,CC2LED3_9V,CC2LED4_9V,CC2LED1_14V,CC2LED2_14V,CC2LED3_14V,CC2LED4_14V,CC2LED1_16V,CC2LED2_16V,CC2LED3_16V,CC2LED4_16V,OUT_5V,FBL_PN_Ver,APP_PN_Ver,Cali_PN_Ver,Anime_PN_Ver,product,download,SN)" +
            //    "values('{1}','{2}', '{3:f5}', '{4:f5}', '{5:f5}', '{6:f5}', '{7:f5}', '{8:f5}', '{9:f5}', '{10:f5}', '{11:f5}', '{12:f5}', '{13:f5}', '{14:f5}', '{15:f5}', '{16:f5}', '{17:f5}', '{18:f5}', '{19:f5}', '{20:f5}', '{21:f5}', '{22:f5}', '{23:f5}', '{24:f5}', '{25:f5}', '{26:f5}', '{27:f5}', '{28:f5}', '{29}', '{30}','{31}','{32}','{33}','{34}','{35}')",
            //    "ecudata", strResult, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), testData.staticCurrent18, testData.CC1_9V[0], testData.CC1_9V[1], testData.CC1_9V[2], testData.CC1_9V[3], testData.CC1_14V[0], testData.CC1_14V[1], testData.CC1_14V[2], testData.CC1_14V[3], testData.CC1_16V[0], testData.CC1_16V[1], testData.CC1_16V[2], testData.CC1_16V[3], testData.CC2_9V[0], testData.CC2_9V[1], testData.CC2_9V[2], testData.CC2_9V[3], testData.CC2_14V[0], testData.CC2_14V[1], testData.CC2_14V[2], testData.CC2_14V[3], testData.CC2_16V[0], testData.CC2_16V[1], testData.CC2_16V[2], testData.CC2_16V[3],
            //    testData.OUTPUT_5V,
            //    testData.strFBLPN + " " + testData.strFBLVersion, testData.strAppPN + " " + testData.strAppVersion, testData.strCaliPN + " " + testData.strCaliVersion, testData.strAnimePN + " " + testData.strAnimeVersion,
            //    testData.product, testData.isDownLoad_ok, testData.SN);

         //   MySqlSRH.ExecuteNonQuery(MySqlSRH.Conn, CommandType.Text, sqlcmd, null);
        }


    }
}
