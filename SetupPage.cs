using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _20230509
{
    public partial class SetupPage : Form
    {
        public SetupPage()
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.Click += Control_Click;

                }


            }

            InitSteupPage();
        }



        public ProductPara pp = new ProductPara();
        private string strCurrentPath = "";
        private int curIndex = 0;
        //初始化参数页面
        public void InitSteupPage()
        {

            strCurrentPath = System.AppDomain.CurrentDomain.BaseDirectory + "sys.xml";

            this.pp = ObjectIO.importFrom(strCurrentPath, typeof(ProductPara)) as ProductPara;
            this.comboBoxProduct.Items.Clear();
            for (int i = 0; i < this.pp.pInfo.Count; i++)
            {
                if ((this.pp.pInfo[i].strProductName == null) || (this.pp.pInfo[i].strProductName == ""))
                    break;
                this.comboBoxProduct.Items.Add(this.pp.pInfo[i].strProductName);
            }
            if (this.comboBoxProduct.Items.Count > 0)
            {
                this.comboBoxProduct.SelectedIndex = 0;
            }

        }
        /// <summary>
        /// UI-类数据
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="pInfo"></param>
        private void ReadDataFromControl(int nIndex, ref List<productInfo> pInfo)
        {
            if (nIndex > pInfo.Count)
                return;

            //版本号验证
            pInfo[nIndex].strProductName = this.comboBoxProduct.Text;

            pInfo[nIndex].FlashDrvPath = this.txt_FlashDrvPath.Text;
            pInfo[nIndex].AppFilePath = this.txt_AppFilePath.Text;

            pInfo[nIndex].CaliFilePath = this.txt_CaliFilePath.Text;

            pInfo[nIndex].AnimeFilePath = this.txt_AnimeFilePath.Text;

            pInfo[nIndex].FBLVer = this.txt_FBLver.Text;
            pInfo[nIndex].FBLPN = this.txt_FBLPN.Text;
            pInfo[nIndex].AppVer = this.txt_Appver.Text;
            pInfo[nIndex].AppPN = this.txt_AppPN.Text;
            pInfo[nIndex].CaliVer = this.txt_Caliver.Text;
            pInfo[nIndex].CaliPN = this.txt_CaliPN.Text;
            pInfo[nIndex].AnimeVer = this.txt_Animever.Text;
            pInfo[nIndex].AnimePN = this.txt_AnimePN.Text;



            //【1】25°18V输出精度
          
            pInfo[nIndex].currStaticMin_18V = double.Parse(this.txt_currStaticMin_18V.Text);
            pInfo[nIndex].currStaticMax_18V = double.Parse(this.txt_currStaticMax_18V.Text);

            #region

            #region  【2】恒流输出
            //2 检测1
            pInfo[nIndex].CC1LED1Min_9V = double.Parse(this.txt_9VCC1LED1Min.Text);
            pInfo[nIndex].CC1LED1Max_9V = double.Parse(this.txt_9VCC1LED1Max.Text);


            pInfo[nIndex].CC1LED2Min_9V = double.Parse(this.txt_9VCC1LED2Min.Text);
            pInfo[nIndex].CC1LED2Max_9V = double.Parse(this.txt_9VCC1LED2Max.Text);

            pInfo[nIndex].CC1LED3Min_9V = double.Parse(this.txt_9VCC1LED3Min.Text);
            pInfo[nIndex].CC1LED3Max_9V = double.Parse(this.txt_9VCC1LED3Max.Text);

            pInfo[nIndex].CC1LED4Min_9V = double.Parse(this.txt_9VCC1LED4Min.Text);
            pInfo[nIndex].CC1LED4Max_9V = double.Parse(this.txt_9VCC1LED4Max.Text);

            //14v
            pInfo[nIndex].CC1LED1Min_14V = double.Parse(this.txt_14VCC1LED1Min.Text);
            pInfo[nIndex].CC1LED1Max_14V = double.Parse(this.txt_14VCC1LED1Max.Text);


            pInfo[nIndex].CC1LED2Min_14V = double.Parse(this.txt_14VCC1LED2Min.Text);
            pInfo[nIndex].CC1LED2Max_14V = double.Parse(this.txt_14VCC1LED2Max.Text);

            pInfo[nIndex].CC1LED3Min_14V = double.Parse(this.txt_14VCC1LED3Min.Text);
            pInfo[nIndex].CC1LED3Max_14V = double.Parse(this.txt_14VCC1LED3Max.Text);

            pInfo[nIndex].CC1LED4Min_14V = double.Parse(this.txt_14VCC1LED4Min.Text);
            pInfo[nIndex].CC1LED4Max_14V = double.Parse(this.txt_14VCC1LED4Max.Text);
            //16v
            pInfo[nIndex].CC1LED1Min_16V = double.Parse(this.txt_16VCC1LED1Min.Text);
            pInfo[nIndex].CC1LED1Max_16V = double.Parse(this.txt_16VCC1LED1Max.Text);


            pInfo[nIndex].CC1LED2Min_16V = double.Parse(this.txt_16VCC1LED2Min.Text);
            pInfo[nIndex].CC1LED2Max_16V = double.Parse(this.txt_16VCC1LED2Max.Text);

            pInfo[nIndex].CC1LED3Min_16V = double.Parse(this.txt_16VCC1LED3Min.Text);
            pInfo[nIndex].CC1LED3Max_16V = double.Parse(this.txt_16VCC1LED3Max.Text);

            pInfo[nIndex].CC1LED4Min_16V = double.Parse(this.txt_16VCC1LED4Min.Text);
            pInfo[nIndex].CC1LED4Max_16V = double.Parse(this.txt_16VCC1LED4Max.Text);
            //2 检测2

            pInfo[nIndex].CC2LED1Min_9V = double.Parse(this.txt_9VCC2LED1Min.Text);
            pInfo[nIndex].CC2LED1Max_9V = double.Parse(this.txt_9VCC2LED1Max.Text);

            pInfo[nIndex].CC2LED2Min_9V = double.Parse(this.txt_9VCC2LED2Min.Text);
            pInfo[nIndex].CC2LED2Max_9V = double.Parse(this.txt_9VCC2LED2Max.Text);

            pInfo[nIndex].CC2LED3Min_9V = double.Parse(this.txt_9VCC2LED3Min.Text);
            pInfo[nIndex].CC2LED3Max_9V = double.Parse(this.txt_9VCC2LED3Max.Text);

            pInfo[nIndex].CC2LED4Min_9V = double.Parse(this.txt_9VCC2LED4Min.Text);
            pInfo[nIndex].CC2LED4Max_9V = double.Parse(this.txt_9VCC2LED4Max.Text);
            // 14V
            pInfo[nIndex].CC2LED1Min_14V = double.Parse(this.txt_14VCC2LED1Min.Text);
            pInfo[nIndex].CC2LED1Max_14V = double.Parse(this.txt_14VCC2LED1Max.Text);

            pInfo[nIndex].CC2LED2Min_14V = double.Parse(this.txt_14VCC2LED2Min.Text);
            pInfo[nIndex].CC2LED2Max_14V = double.Parse(this.txt_14VCC2LED2Max.Text);

            pInfo[nIndex].CC2LED3Min_14V = double.Parse(this.txt_14VCC2LED3Min.Text);
            pInfo[nIndex].CC2LED3Max_14V = double.Parse(this.txt_14VCC2LED3Max.Text);

            pInfo[nIndex].CC2LED4Min_14V = double.Parse(this.txt_14VCC2LED4Min.Text);
            pInfo[nIndex].CC2LED4Max_14V = double.Parse(this.txt_14VCC2LED4Max.Text);

            // 16V
            pInfo[nIndex].CC2LED1Min_16V = double.Parse(this.txt_16VCC2LED1Min.Text);
            pInfo[nIndex].CC2LED1Max_16V = double.Parse(this.txt_16VCC2LED1Max.Text);

            pInfo[nIndex].CC2LED2Min_16V = double.Parse(this.txt_16VCC2LED2Min.Text);
            pInfo[nIndex].CC2LED2Max_16V = double.Parse(this.txt_16VCC2LED2Max.Text);

            pInfo[nIndex].CC2LED3Min_16V = double.Parse(this.txt_16VCC2LED3Min.Text);
            pInfo[nIndex].CC2LED3Max_16V = double.Parse(this.txt_16VCC2LED3Max.Text);

            pInfo[nIndex].CC2LED4Min_16V = double.Parse(this.txt_16VCC2LED4Min.Text);
            pInfo[nIndex].CC2LED4Max_16V = double.Parse(this.txt_16VCC2LED4Max.Text);

            #endregion

            //【3】5V输出精度
            pInfo[nIndex].out5VMin = double.Parse(this.txt_out5VMin.Text);
            pInfo[nIndex].out5VMax = double.Parse(this.txt_out5VMax.Text);

            #region【4】单通道输出精度  逻辑通道电流检测


            pInfo[nIndex].Logic_1Min = double.Parse(this.txtLogic_1Min.Text);
            pInfo[nIndex].Logic_1Max = double.Parse(this.txtLogic_1Max.Text);
            pInfo[nIndex].Logic_2Min = double.Parse(this.txtLogic_2Min.Text);
            pInfo[nIndex].Logic_2Max = double.Parse(this.txtLogic_2Max.Text);
            pInfo[nIndex].Logic_3Min = double.Parse(this.txtLogic_3Min.Text);
            pInfo[nIndex].Logic_3Max = double.Parse(this.txtLogic_3Max.Text);
            pInfo[nIndex].Logic_4Min = double.Parse(this.txtLogic_4Min.Text);
            pInfo[nIndex].Logic_4Max = double.Parse(this.txtLogic_4Max.Text);
            pInfo[nIndex].Logic_5Min = double.Parse(this.txtLogic_5Min.Text);
            pInfo[nIndex].Logic_5Max = double.Parse(this.txtLogic_5Max.Text);
            pInfo[nIndex].Logic_6Min = double.Parse(this.txtLogic_6Min.Text);
            pInfo[nIndex].Logic_6Max = double.Parse(this.txtLogic_6Max.Text);
            #endregion
            //【5】占空比
            pInfo[nIndex].Logic_2DutyMin = double.Parse(this.txtLogic_2DutyMin.Text);
            pInfo[nIndex].Logic_2DutyMax = double.Parse(this.txtLogic_2DutyMax.Text);
            pInfo[nIndex].Logic_3DutyMin = double.Parse(this.txtLogic_3DutyMin.Text);
            pInfo[nIndex].Logic_3DutyMax = double.Parse(this.txtLogic_3DutyMax.Text);
            pInfo[nIndex].Logic_4DutyMin = double.Parse(this.txtLogic_4DutyMin.Text);
            pInfo[nIndex].Logic_4DutyMax = double.Parse(this.txtLogic_4DutyMax.Text);

            pInfo[nIndex].Logic_5DutyMin = double.Parse(this.txtLogic_5DutyMin.Text);
            pInfo[nIndex].Logic_5DutyMax = double.Parse(this.txtLogic_5DutyMax.Text);
            pInfo[nIndex].Logic_6DutyMin = double.Parse(this.txtLogic_6DutyMin.Text);
            pInfo[nIndex].Logic_6DutyMax = double.Parse(this.txtLogic_6DutyMax.Text);

            //【6】报警信号测试

            pInfo[nIndex].outage_riseTimeMin = double.Parse(this.txtoutage_riseTimeMin.Text);
            pInfo[nIndex].outage_riseTimeMax = double.Parse(this.txtoutage_riseTimeMax.Text);

            pInfo[nIndex].outage_fallTimeMin = double.Parse(this.txtoutage_fallTimeMin.Text);
            pInfo[nIndex].outage_fallTimeMax = double.Parse(this.txtoutage_fallTimeMax.Text);

            pInfo[nIndex].outageLvMIin = double.Parse(this.txt_outageLvMIin.Text);//电压
            pInfo[nIndex].outageLvMax = double.Parse(this.txt_outageLvMax.Text);

            #region【7】 HSSS&LSSS 测试
            //HSS
            pInfo[nIndex].HSSS1_riseLVMin = double.Parse(this.txtHSSS1_riseLVMin.Text);//电压
            pInfo[nIndex].HSSS1_riseLVMax = double.Parse(this.txtHSSS1_riseLVMax.Text);//电压
            pInfo[nIndex].HSSS1_fallLVMin = double.Parse(this.txtHSSS1_fallLVMin.Text);
            pInfo[nIndex].HSSS1_fallLVMax = double.Parse(this.txtHSSS1_fallLVMax.Text);

            pInfo[nIndex].HSSS1_riseTimeMin = double.Parse(this.txtHSSS1_riseTimeMin.Text);//电压
            pInfo[nIndex].HSSS1_riseTimeMax = double.Parse(this.txtHSSS1_riseTimeMax.Text);//电压

            pInfo[nIndex].HSSS1_fallTimeMin = double.Parse(this.txtHSSS1_fallTimeMin.Text);
            pInfo[nIndex].HSSS1_fallTimeMax = double.Parse(this.txtHSSS1_fallTimeMax.Text);

            //LSSS

            pInfo[nIndex].LSSS1_riseLvMin = double.Parse(this.txtLSSS1_riseLvMin.Text);//电压
            pInfo[nIndex].LSSS1_riseLvMax = double.Parse(this.txtLSSS1_riseLvMax.Text);//电压
            pInfo[nIndex].LSSS1_fallLvMin = double.Parse(this.txtLSSS1_fallLvMin.Text);
            pInfo[nIndex].LSSS1_fallLvMax = double.Parse(this.txtLSSS1_fallLvMax.Text);
            pInfo[nIndex].LSSS1_riseTimeMin = double.Parse(this.txtLSSS1_riseTimeMin.Text);//电压
            pInfo[nIndex].LSSS1_riseTimeMax = double.Parse(this.txtLSSS1_riseTimeMax.Text);//电压
            pInfo[nIndex].LSSS1_fallLvMin = double.Parse(this.txtLSSS1_fallLvMin.Text);
            pInfo[nIndex].LSSS1_fallTimeMax = double.Parse(this.txtLSSS1_fallTimeMax.Text);


            pInfo[nIndex].LSSS2_riseLvMin = double.Parse(this.txtLSSS2_riseLvMin.Text);//电压
            pInfo[nIndex].LSSS2_riseLvMax = double.Parse(this.txtLSSS2_riseLvMax.Text);//电压
            pInfo[nIndex].LSSS2_fallLvMin = double.Parse(this.txtLSSS2_fallLvMin.Text);
            pInfo[nIndex].LSSS2_fallLvMax = double.Parse(this.txtLSSS2_fallLvMax.Text);
            pInfo[nIndex].LSSS2_riseTimeMin = double.Parse(this.txtLSSS2_riseTimeMin.Text);//电压
            pInfo[nIndex].LSSS2_riseTimeMax = double.Parse(this.txtLSSS2_riseTimeMax.Text);//电压
            pInfo[nIndex].LSSS2_fallLvMin = double.Parse(this.txtLSSS2_fallTimeMin.Text);
            pInfo[nIndex].LSSS2_fallTimeMax = double.Parse(this.txtLSSS2_fallTimeMax.Text);

            pInfo[nIndex].LSSS3_riseLvMin = double.Parse(this.txtLSSS3_riseLvMin.Text);//电压
            pInfo[nIndex].LSSS3_riseLvMax = double.Parse(this.txtLSSS3_riseLvMax.Text);//电压
            pInfo[nIndex].LSSS3_fallLvMin = double.Parse(this.txtLSSS3_fallLvMin.Text);
            pInfo[nIndex].LSSS3_fallLvMax = double.Parse(this.txtLSSS3_fallLvMax.Text);
            pInfo[nIndex].LSSS3_riseTimeMin = double.Parse(this.txtLSSS3_riseTimeMin.Text);//电压
            pInfo[nIndex].LSSS3_riseTimeMax = double.Parse(this.txtLSSS3_riseTimeMax.Text);//电压
            pInfo[nIndex].LSSS3_fallTimeMin = double.Parse(this.txtLSSS3_fallTimeMin.Text);
            pInfo[nIndex].LSSS3_fallTimeMax = double.Parse(this.txtLSSS3_fallTimeMax.Text);

            pInfo[nIndex].LSSS4_riseLvMin = double.Parse(this.txtLSSS4_riseLvMin.Text);//电压
            pInfo[nIndex].LSSS4_riseLvMax = double.Parse(this.txtLSSS4_riseLvMax.Text);//电压
            pInfo[nIndex].LSSS4_fallLvMin = double.Parse(this.txtLSSS4_fallLvMin.Text);
            pInfo[nIndex].LSSS4_fallLvMax = double.Parse(this.txtLSSS4_fallLvMax.Text);
            pInfo[nIndex].LSSS4_riseTimeMin = double.Parse(this.txtLSSS4_riseTimeMin.Text);//电压
            pInfo[nIndex].LSSS4_riseTimeMax = double.Parse(this.txtLSSS4_riseTimeMax.Text);//电压
            pInfo[nIndex].LSSS4_fallTimeMin = double.Parse(this.txtLSSS4_fallTimeMin.Text);
            pInfo[nIndex].LSSS4_fallTimeMax = double.Parse(this.txtLSSS4_fallTimeMax.Text);

            #endregion


            #region 【8】RBin 测试分并电阻电压
            pInfo[nIndex].RBinS1Min = double.Parse(this.txt_RBinS1Min.Text);
            pInfo[nIndex].RBinS1Max = double.Parse(this.txt_RBinS1Max.Text);
            pInfo[nIndex].RBinS2Max = double.Parse(this.txt_RBinS2Max.Text);
            pInfo[nIndex].RBinS2Min = double.Parse(this.txt_RBinS2Min.Text);

            pInfo[nIndex].RBinS3Max = double.Parse(this.txt_RBinS3Max.Text);
            pInfo[nIndex].RBinS3Min = double.Parse(this.txt_RBinS3Min.Text);
            pInfo[nIndex].RBinS4Max = double.Parse(this.txt_RBinS4Max.Text);
            pInfo[nIndex].RBinS4Min = double.Parse(this.txt_RBinS4Min.Text);
            pInfo[nIndex].NTC1Min = double.Parse(this.txt_NTC1Min.Text);
            pInfo[nIndex].NTC1Max = double.Parse(this.txt_NTC1Max.Text);
            pInfo[nIndex].NTC2Min = double.Parse(this.txt_NTC2Min.Text);
            pInfo[nIndex].NTC2Max = double.Parse(this.txt_NTC2Max.Text);
            #endregion
            // 【9】启动延时
            pInfo[nIndex].startSleepMin = double.Parse(this.txt_startSleepMin.Text);
            pInfo[nIndex].startSleepMax = double.Parse(this.txt_startSleepMax.Text);

            #endregion
        }

        /// <summary>
        /// 写数据->UI
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="pInfo"></param>
        private void WriteDataToControl(int nIndex, List<productInfo> pInfo)
        {
            if (nIndex > pInfo.Count)
                return;
            //版本号验证
            this.comboBoxProduct.Text = pInfo[nIndex].strProductName;

            this.txt_FlashDrvPath.Text = pInfo[nIndex].FlashDrvPath;
            this.txt_AppFilePath.Text = pInfo[nIndex].AppFilePath;
            this.txt_CaliFilePath.Text = pInfo[nIndex].CaliFilePath;
            this.txt_AnimeFilePath.Text = pInfo[nIndex].AnimeFilePath;

            this.txt_FBLver.Text = pInfo[nIndex].FBLVer;
            this.txt_FBLPN.Text = pInfo[nIndex].FBLPN;
            this.txt_Appver.Text = pInfo[nIndex].AppVer;
            this.txt_AppPN.Text = pInfo[nIndex].AppPN;
            this.txt_Caliver.Text = pInfo[nIndex].CaliVer;
            this.txt_CaliPN.Text = pInfo[nIndex].CaliPN;
            this.txt_Animever.Text = pInfo[nIndex].AnimeVer;
            this.txt_AnimePN.Text = pInfo[nIndex].AnimePN;



            //【1】25°18V输出精度
        this.txt_currStaticMin_18V.Text = pInfo[nIndex].currStaticMin_18V.ToString();
        this.txt_currStaticMax_18V.Text =pInfo[nIndex].currStaticMax_18V.ToString() ;


            #region

            #region  【2】恒流输出
            //2 检测1
            this.txt_9VCC1LED1Min.Text = pInfo[nIndex].CC1LED1Min_9V.ToString();
            this.txt_9VCC1LED1Max.Text = pInfo[nIndex].CC1LED1Max_9V.ToString();
            this.txt_9VCC1LED2Min.Text = pInfo[nIndex].CC1LED2Min_9V.ToString();
            this.txt_9VCC1LED2Max.Text = pInfo[nIndex].CC1LED2Max_9V.ToString();

            this.txt_9VCC1LED3Min.Text = pInfo[nIndex].CC1LED3Min_9V.ToString();
            this.txt_9VCC1LED3Max.Text = pInfo[nIndex].CC1LED3Max_9V.ToString();

            this.txt_9VCC1LED4Min.Text = pInfo[nIndex].CC1LED4Min_9V.ToString();
            this.txt_9VCC1LED4Max.Text = pInfo[nIndex].CC1LED4Max_9V.ToString();
            //14v
            this.txt_14VCC1LED1Min.Text = pInfo[nIndex].CC1LED1Min_14V.ToString();
            this.txt_14VCC1LED1Max.Text = pInfo[nIndex].CC1LED1Max_14V.ToString();
            this.txt_14VCC1LED2Min.Text = pInfo[nIndex].CC1LED2Min_14V.ToString();
            this.txt_14VCC1LED2Max.Text = pInfo[nIndex].CC1LED2Max_14V.ToString();

            this.txt_14VCC1LED3Min.Text = pInfo[nIndex].CC1LED3Min_14V.ToString();
            this.txt_14VCC1LED3Max.Text = pInfo[nIndex].CC1LED3Max_14V.ToString();

            this.txt_14VCC1LED4Min.Text = pInfo[nIndex].CC1LED4Min_14V.ToString();
            this.txt_14VCC1LED4Max.Text = pInfo[nIndex].CC1LED4Max_14V.ToString();

            //16v
            this.txt_16VCC1LED1Min.Text = pInfo[nIndex].CC1LED1Min_16V.ToString();
            this.txt_16VCC1LED1Max.Text = pInfo[nIndex].CC1LED1Max_16V.ToString();
            this.txt_16VCC1LED2Min.Text = pInfo[nIndex].CC1LED2Min_16V.ToString();
            this.txt_16VCC1LED2Max.Text = pInfo[nIndex].CC1LED2Max_16V.ToString();

            this.txt_16VCC1LED3Min.Text = pInfo[nIndex].CC1LED3Min_16V.ToString();
            this.txt_16VCC1LED3Max.Text = pInfo[nIndex].CC1LED3Max_16V.ToString();

            this.txt_16VCC1LED4Min.Text = pInfo[nIndex].CC1LED4Min_16V.ToString();
            this.txt_16VCC1LED4Max.Text = pInfo[nIndex].CC1LED4Max_16V.ToString();
            //2 检测2

            this.txt_9VCC2LED1Min.Text = pInfo[nIndex].CC2LED1Min_9V.ToString();
            this.txt_9VCC2LED1Max.Text = pInfo[nIndex].CC2LED1Max_9V.ToString();

            this.txt_9VCC2LED2Min.Text = pInfo[nIndex].CC2LED2Min_9V.ToString();
            this.txt_9VCC2LED2Max.Text = pInfo[nIndex].CC2LED2Max_9V.ToString();

            this.txt_9VCC2LED3Min.Text = pInfo[nIndex].CC2LED3Min_9V.ToString();
            this.txt_9VCC2LED3Max.Text = pInfo[nIndex].CC2LED3Max_9V.ToString();

            this.txt_9VCC2LED4Min.Text = pInfo[nIndex].CC2LED4Min_9V.ToString();
            this.txt_9VCC2LED4Max.Text = pInfo[nIndex].CC2LED4Max_9V.ToString();
            //14v
            this.txt_14VCC2LED1Min.Text = pInfo[nIndex].CC2LED1Min_14V.ToString();
            this.txt_14VCC2LED1Max.Text = pInfo[nIndex].CC2LED1Max_14V.ToString();

            this.txt_14VCC2LED2Min.Text = pInfo[nIndex].CC2LED2Min_14V.ToString();
            this.txt_14VCC2LED2Max.Text = pInfo[nIndex].CC2LED2Max_14V.ToString();

            this.txt_14VCC2LED3Min.Text = pInfo[nIndex].CC2LED3Min_14V.ToString();
            this.txt_14VCC2LED3Max.Text = pInfo[nIndex].CC2LED3Max_14V.ToString();

            this.txt_14VCC2LED4Min.Text = pInfo[nIndex].CC2LED4Min_14V.ToString();
            this.txt_14VCC2LED4Max.Text = pInfo[nIndex].CC2LED4Max_14V.ToString();
            //16V
            //14v
            this.txt_16VCC2LED1Min.Text = pInfo[nIndex].CC2LED1Min_16V.ToString();
            this.txt_16VCC2LED1Max.Text = pInfo[nIndex].CC2LED1Max_16V.ToString();

            this.txt_16VCC2LED2Min.Text = pInfo[nIndex].CC2LED2Min_16V.ToString();
            this.txt_16VCC2LED2Max.Text = pInfo[nIndex].CC2LED2Max_16V.ToString();

            this.txt_16VCC2LED3Min.Text = pInfo[nIndex].CC2LED3Min_16V.ToString();
            this.txt_16VCC2LED3Max.Text = pInfo[nIndex].CC2LED3Max_16V.ToString();

            this.txt_16VCC2LED4Min.Text = pInfo[nIndex].CC2LED4Min_16V.ToString();
            this.txt_16VCC2LED4Max.Text = pInfo[nIndex].CC2LED4Max_16V.ToString();
            #endregion




            //【3】5V输出精度
            this.txt_out5VMin.Text = pInfo[nIndex].out5VMin.ToString();
            this.txt_out5VMax.Text = pInfo[nIndex].out5VMax.ToString();

            #region【4】单通道输出精度  逻辑通道电流检测


            this.txtLogic_1Min.Text = pInfo[nIndex].Logic_1Min.ToString();
            this.txtLogic_1Max.Text = pInfo[nIndex].Logic_1Max.ToString();
            this.txtLogic_2Min.Text = pInfo[nIndex].Logic_2Min.ToString();
            this.txtLogic_2Max.Text = pInfo[nIndex].Logic_2Max.ToString();
            this.txtLogic_3Min.Text = pInfo[nIndex].Logic_3Min.ToString();
            this.txtLogic_3Max.Text = pInfo[nIndex].Logic_3Max.ToString();
            this.txtLogic_4Min.Text = pInfo[nIndex].Logic_4Min.ToString();
            this.txtLogic_4Max.Text = pInfo[nIndex].Logic_4Max.ToString();
            this.txtLogic_5Min.Text = pInfo[nIndex].Logic_5Min.ToString();
            this.txtLogic_5Max.Text = pInfo[nIndex].Logic_5Max.ToString();
            this.txtLogic_6Min.Text = pInfo[nIndex].Logic_6Min.ToString();
            this.txtLogic_6Max.Text = pInfo[nIndex].Logic_6Max.ToString();
            #endregion
            //【5】占空比
            this.txtLogic_2DutyMin.Text = pInfo[nIndex].Logic_2DutyMin.ToString();
            this.txtLogic_2DutyMax.Text = pInfo[nIndex].Logic_2DutyMax.ToString();
            this.txtLogic_3DutyMin.Text = pInfo[nIndex].Logic_3DutyMin.ToString();
            this.txtLogic_3DutyMax.Text = pInfo[nIndex].Logic_3DutyMax.ToString();
            this.txtLogic_4DutyMin.Text = pInfo[nIndex].Logic_4DutyMin.ToString();
            this.txtLogic_4DutyMax.Text = pInfo[nIndex].Logic_4DutyMax.ToString();

            this.txtLogic_5DutyMin.Text = pInfo[nIndex].Logic_5DutyMin.ToString();
            this.txtLogic_5DutyMax.Text = pInfo[nIndex].Logic_5DutyMax.ToString();
            this.txtLogic_6DutyMin.Text = pInfo[nIndex].Logic_6DutyMin.ToString();
            this.txtLogic_6DutyMax.Text = pInfo[nIndex].Logic_6DutyMax.ToString();

            //【6】报警信号测试

            this.txtoutage_riseTimeMin.Text = pInfo[nIndex].outage_riseTimeMin.ToString();
            this.txtoutage_riseTimeMax.Text = pInfo[nIndex].outage_riseTimeMax.ToString();

            this.txtoutage_fallTimeMin.Text = pInfo[nIndex].outage_fallTimeMin.ToString(); ;
            this.txtoutage_fallTimeMax.Text = pInfo[nIndex].outage_fallTimeMax.ToString(); ;

            this.txt_outageLvMIin.Text = pInfo[nIndex].outageLvMIin.ToString();//电压
            this.txt_outageLvMax.Text = pInfo[nIndex].outageLvMax.ToString();

            #region【7】 HSSS&LSSS 测试
            //HSS
            this.txtHSSS1_riseLVMin.Text = pInfo[nIndex].HSSS1_riseLVMin.ToString();//电压
            this.txtHSSS1_riseLVMax.Text = pInfo[nIndex].HSSS1_riseLVMax.ToString();//电压
            this.txtHSSS1_fallLVMin.Text = pInfo[nIndex].HSSS1_fallLVMin.ToString();
            this.txtHSSS1_fallLVMax.Text = pInfo[nIndex].HSSS1_fallLVMax.ToString(); ;

            this.txtHSSS1_riseTimeMin.Text = pInfo[nIndex].HSSS1_riseTimeMin.ToString();//电压
            this.txtHSSS1_riseTimeMax.Text = pInfo[nIndex].HSSS1_riseTimeMax.ToString();//电压

            this.txtHSSS1_fallTimeMin.Text = pInfo[nIndex].HSSS1_fallTimeMin.ToString();
            this.txtHSSS1_fallTimeMax.Text = pInfo[nIndex].HSSS1_fallTimeMax.ToString();

            //LSSS

            this.txtLSSS1_riseLvMin.Text = pInfo[nIndex].LSSS1_riseLvMin.ToString();//电压
            this.txtLSSS1_riseLvMax.Text = pInfo[nIndex].LSSS1_riseLvMax.ToString();//电压
            this.txtLSSS1_fallLvMin.Text = pInfo[nIndex].LSSS1_fallLvMin.ToString();
            this.txtLSSS1_fallLvMax.Text = pInfo[nIndex].LSSS1_fallLvMax.ToString();
            this.txtLSSS1_riseTimeMin.Text = pInfo[nIndex].LSSS1_riseTimeMin.ToString();//电压
            this.txtLSSS1_riseTimeMax.Text = pInfo[nIndex].LSSS1_riseTimeMax.ToString();//电压
            this.txtLSSS1_fallTimeMin.Text = pInfo[nIndex].LSSS1_fallTimeMin.ToString();
            this.txtLSSS1_fallTimeMax.Text = pInfo[nIndex].LSSS1_fallTimeMax.ToString();


            this.txtLSSS2_riseLvMin.Text = pInfo[nIndex].LSSS2_riseLvMin.ToString();//电压
            this.txtLSSS2_riseLvMax.Text = pInfo[nIndex].LSSS2_riseLvMax.ToString();//电压
            this.txtLSSS2_fallLvMin.Text = pInfo[nIndex].LSSS2_fallLvMin.ToString();
            this.txtLSSS2_fallLvMax.Text = pInfo[nIndex].LSSS2_fallLvMax.ToString();
            this.txtLSSS2_riseTimeMin.Text = pInfo[nIndex].LSSS2_riseTimeMin.ToString();//电压
            this.txtLSSS2_riseTimeMax.Text = pInfo[nIndex].LSSS2_riseTimeMax.ToString();//电压
            this.txtLSSS2_fallTimeMin.Text = pInfo[nIndex].LSSS2_fallTimeMin.ToString();
            this.txtLSSS2_fallTimeMax.Text = pInfo[nIndex].LSSS2_fallTimeMax.ToString();

            this.txtLSSS3_riseLvMin.Text = pInfo[nIndex].LSSS3_riseLvMin.ToString();//电压
            this.txtLSSS3_riseLvMax.Text = pInfo[nIndex].LSSS3_riseLvMax.ToString();//电压
            this.txtLSSS3_fallLvMin.Text = pInfo[nIndex].LSSS3_fallLvMin.ToString();
            this.txtLSSS3_fallLvMax.Text = pInfo[nIndex].LSSS3_fallLvMax.ToString();
            this.txtLSSS3_riseTimeMin.Text = pInfo[nIndex].LSSS3_riseTimeMin.ToString();//电压
            this.txtLSSS3_riseTimeMax.Text = pInfo[nIndex].LSSS3_riseTimeMax.ToString();//电压
            this.txtLSSS3_fallTimeMin.Text = pInfo[nIndex].LSSS3_fallTimeMin.ToString();
            this.txtLSSS3_fallTimeMax.Text = pInfo[nIndex].LSSS3_fallTimeMax.ToString();

            this.txtLSSS4_riseLvMin.Text = pInfo[nIndex].LSSS4_riseLvMin.ToString();//电压
            this.txtLSSS4_riseLvMax.Text = pInfo[nIndex].LSSS4_riseLvMax.ToString();//电压
            this.txtLSSS4_fallLvMin.Text = pInfo[nIndex].LSSS4_fallLvMin.ToString();
            this.txtLSSS4_fallLvMax.Text = pInfo[nIndex].LSSS4_fallLvMax.ToString();
            this.txtLSSS4_riseTimeMin.Text = pInfo[nIndex].LSSS4_riseTimeMin.ToString();//电压
            this.txtLSSS4_riseTimeMax.Text = pInfo[nIndex].LSSS4_riseTimeMax.ToString();//电压
            this.txtLSSS4_fallTimeMin.Text = pInfo[nIndex].LSSS4_fallTimeMin.ToString();
            this.txtLSSS4_fallTimeMax.Text = pInfo[nIndex].LSSS4_fallTimeMax.ToString();

            #endregion


            #region 【8】RBin 测试分并电阻电压
            this.txt_RBinS1Min.Text = pInfo[nIndex].RBinS1Min.ToString();
            this.txt_RBinS1Max.Text = pInfo[nIndex].RBinS1Max.ToString();
            this.txt_RBinS2Min.Text = pInfo[nIndex].RBinS2Min.ToString();
            this.txt_RBinS2Max.Text = pInfo[nIndex].RBinS2Max.ToString();
            this.txt_RBinS3Min.Text = pInfo[nIndex].RBinS3Min.ToString();
            this.txt_RBinS3Max.Text = pInfo[nIndex].RBinS3Max.ToString();
            this.txt_RBinS4Min.Text = pInfo[nIndex].RBinS4Min.ToString();
            this.txt_RBinS4Max.Text = pInfo[nIndex].RBinS4Max.ToString();
            this.txt_NTC1Min.Text = pInfo[nIndex].NTC1Min.ToString();
            this.txt_NTC1Max.Text = pInfo[nIndex].NTC1Max.ToString();
            this.txt_NTC2Min.Text = pInfo[nIndex].NTC2Min.ToString();
            this.txt_NTC2Max.Text = pInfo[nIndex].NTC2Max.ToString();
            #endregion
            // 【9】启动延时
            this.txt_startSleepMin.Text = pInfo[nIndex].startSleepMin.ToString();
            this.txt_startSleepMax.Text = pInfo[nIndex].startSleepMax.ToString();

            #endregion

        }
        private void Control_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Tag.ToString() == "Save")
            {
                string strName = this.comboBoxProduct.Text;
                if (strName.Trim() == "")
                {
                    MessageBox.Show("请先输入产品名称！");
                    return;
                }
                bool bFlagExist = false;
                foreach (productInfo info in this.pp.pInfo)
                {
                    if (info.strProductName == strName)
                        bFlagExist = true;
                }
                int nIndex = -1;
                if (!bFlagExist)
                {
                    productInfo info = new productInfo();
                    info.strProductName = strName;
                    this.comboBoxProduct.Items.Add(strName);
                    nIndex = this.comboBoxProduct.Items.Count - 1;
                    //info.strTable = "shbData";
                    this.pp.pInfo.Add(info);
                    this.ReadDataFromControl(nIndex, ref pp.pInfo);
                    ObjectIO.exportTo(strCurrentPath, this.pp, typeof(ProductPara));
                    this.comboBoxProduct.SelectedIndex = nIndex;

                    return;
                }
                nIndex = this.comboBoxProduct.SelectedIndex;
                if (nIndex != -1)
                    this.ReadDataFromControl(nIndex, ref pp.pInfo);
                else
                    return;
                ObjectIO.exportTo(strCurrentPath, this.pp, typeof(ProductPara));
                MessageBox.Show("保存成功！");
            }
            if (button.Tag.ToString() == "OpenFliashDrv")
            {
                string filename = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;      //获取文件的路径

                    this.txt_FlashDrvPath.Text = filename;
                }
                return;
            }
            if (button.Tag.ToString() == "OpenApp")
            {
                string filename = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;      //获取文件的路径

                    this.txt_AppFilePath.Text = filename;
                }
                return;
            }
            if (button.Tag.ToString() == "OpenCali")
            {
                string filename = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;      //获取文件的路径

                    this.txt_CaliFilePath.Text = filename;
                }
                return;
            }


            if (button.Tag.ToString() == "OpenAnime")
            {
                string filename = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;      //获取文件的路径

                    this.txt_AnimeFilePath.Text = filename;
                }
                return;
            }

        }

        /// <summary>
        /// 产品选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxProduct_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            this.curIndex = this.comboBoxProduct.SelectedIndex;
            if (this.curIndex == -1)
                return;
            WriteDataToControl(this.curIndex, this.pp.pInfo);
        }
    }
}
