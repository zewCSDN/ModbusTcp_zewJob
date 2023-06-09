using _20230509.MySRHMethod;
using MySRHMethod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _20230509
{
    public partial class FrmManual : Form
    {
        private Device device = null;
        public FrmManual(Device device)
        {
            InitializeComponent();
            this.device = device;

            //{
            //    if (groupBox is GroupBox)
            //    {
            //        if (groupBox is Button)
            //        {
            //            foreach (Button btn in groupBox.Controls)
            //            {
            //                btn.Click += Control_Click;
            //            }
            //        }



            //    }
            //    //}


        }

        private void btnPwrVol1_Click(object sender, EventArgs e)
        {
            this.SetVolAndCur(this.tbVol1, this.tbCur1, this.device.pwr401l[0]);
        }

        private void btnPwrVol2_Click(object sender, EventArgs e)
        {

            this.SetVolAndCur(this.tbVol2, this.tbCur2, this.device.pwr401l[1]);
        }



        private void btnPwrVol3_Click(object sender, EventArgs e)
        {
            this.SetVolAndCur(this.tbVol3, this.tbCur3, this.device.pwr401l[2]);
        }
        private void btnPwrVol4_Click(object sender, EventArgs e)
        {
            this.SetVolAndCur(this.tbVol4, this.tbCur4, this.device.pwr401l[3]);
        }



        private void btnCC1_Click(object sender, EventArgs e)
        {
            this.SetCVValue(this.tbLoad1CH1, this.tbLoad1CH2, this.tbLoad1CH3, this.tbLoad1CH4, this.tbLoad1CH5, this.device.plz50fLoad[0]);
        }


        private void btnCC2_Click(object sender, EventArgs e)
        {
            this.SetCVValue(this.tbLoad2CH1, this.tbLoad2CH2, this.tbLoad2CH3, this.tbLoad2CH4, this.tbLoad2CH5, this.device.plz50fLoad[1]);
        }

        private void btnCC3_Click(object sender, EventArgs e)
        {
            this.SetCVValue(this.tbLoad3CH1, this.tbLoad3CH2, this.tbLoad3CH3, this.tbLoad3CH4, this.tbLoad3CH5, this.device.plz50fLoad[2]);
        }

        private void btnCC4_Click(object sender, EventArgs e)
        {
            this.SetCVValue(this.tbLoad4CH1, this.tbLoad4CH2, this.tbLoad4CH3, this.tbLoad4CH4, this.tbLoad4CH5, this.device.plz50fLoad[3]);
        }


        private void SetVolAndCur(TextBox tbvol, TextBox tbCur, CPWR401L pwr)
        {
            double dbVol = 0.00;
            double dbCur = 0.00;
            try
            {
                dbVol = Convert.ToDouble(tbvol.Text);
            }
            catch
            {
                dbVol = 0.00;
                tbvol.Text = "0.00";
                MessageBox.Show("请输入正确的浮点型电压数值！");
                return;
            }
            try
            {
                dbCur = Convert.ToDouble(tbCur.Text);
            }
            catch
            {
                dbCur = 0.00;
                tbCur.Text = "0.00";
                MessageBox.Show("请输入正确的浮点型电流数值！");
                return;
            }
            if (returnCode.success != pwr.ApplVolt(dbVol, dbCur))
            {
                MessageBox.Show("电源未打开，请查找原因！");
                return;
            }
            pwr.OutPut(true);
        }



        private void SetCVValue(TextBox ch1, TextBox ch2, TextBox ch3, TextBox ch4, TextBox ch5, CPLZ50FLoad load)
        {
            double dbCV1 = 0.00;
            double dbCV2 = 0.00;
            double dbCV3 = 0.00;
            double dbCV4 = 0.00;
            double dbCV5 = 0.00;
            try
            {
                dbCV1 = Convert.ToDouble(ch1.Text);
            }
            catch
            {
                dbCV1 = 0.00;
                ch1.Text = "0.00";
                MessageBox.Show("CH1请输入正确的浮点型电压数值！");
                return;
            }
            try
            {
                dbCV2 = Convert.ToDouble(ch2.Text);
            }
            catch
            {
                dbCV2 = 0.00;
                ch2.Text = "0.00";
                MessageBox.Show("CH2请输入正确的浮点型电流数值！");
                return;
            }
            try
            {
                dbCV3 = Convert.ToDouble(ch3.Text);
            }
            catch
            {
                dbCV3 = 0.00;
                ch3.Text = "0.00";
                MessageBox.Show("CH3请输入正确的浮点型电流数值！");
                return;
            }
            try
            {
                dbCV4 = Convert.ToDouble(ch4.Text);
            }
            catch
            {
                dbCV4 = 0.00;
                ch4.Text = "0.00";
                MessageBox.Show("CH4请输入正确的浮点型电流数值！");
                return;
            }
            try
            {
                dbCV5 = Convert.ToDouble(ch5.Text);
            }
            catch
            {
                dbCV5 = 0.00;
                ch5.Text = "0.00";
                MessageBox.Show("CH5请输入正确的浮点型电流数值！");
                return;
            }

            load.VoltFunction(1, dbCV1);
            load.VoltFunction(2, dbCV1);
            load.VoltFunction(3, dbCV1);
            load.VoltFunction(4, dbCV1);
            load.VoltFunction(5, dbCV1);

            load.OutputAll(true);
            //load.OutputAll(false);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hz"></param>
        /// <param name="Duty"></param>
        /// <param name="">wuitch板</param>
        /// <param name="masterRtu"></param>
        /// <param name="pwmChannelID"频率通道ID></param>
        /// <param name="dutyChannelID">占空比ID</param>





        /// <summary>
        /// 频率占空比设置
        /// </summary>
        /// <param name="Hz">频率设置</param>
        /// <param name="Duty">占空比设置</param>
        /// <param name="device"></param>
        /// <param name="stationID">设备工位 从0开始</param>
        /// <param name="pwmChannelID">pwm通道从1开始</param>
        /// <param name="dutyChannelID">占空比ID 从1开始</param>
        /// <returns></returns>

        private bool SetHzDuty(TextBox Hz, TextBox Duty, Device device, int stationID, int pwmChannelID, int dutyChannelID)
        {
            ushort Uhz = 0;
            ushort Uduty = 0;

            try
            {
                Uhz = Convert.ToUInt16(Hz.Text.Trim());//设置频率
                Uduty = Convert.ToUInt16(Duty.Text.Trim());//设置频率
                device.SwichPCB[stationID].PWMchannel_HZ(device.master_SW[stationID], pwmChannelID, Uduty);
                return device.SwichPCB[stationID].DutyCycle_Set(device.master_SW[stationID], dutyChannelID, Uduty);
              
            }
            catch
            {

                return false;
            }


        }

        private void checkedListBoxDO_SelectedIndexChanged(object sender, EventArgs e)
        {


            int nIndex = checked1ListBoxDO.SelectedIndex;
            switch (nIndex)
            {
                case 0: //DO_0 红灯
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_00_staticCurrent(true);
                    else
                        this.device.NI_usb6002[0].DO_00_staticCurrent(false);
                    break;
                case 1: //DO_LOGIC_IO1
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_01_linOutage(true);
                    else
                        this.device.NI_usb6002[0].DO_01_linOutage(false);
                    break;
                case 2: //DO_LOGIC_IO2
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_02_Res(true);
                    else
                        this.device.NI_usb6002[0].DO_02_Res(false);
                    break;
                case 3: //DO_LOGIC_IN1
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_03_K4_FZ(true);
                    else
                        this.device.NI_usb6002[0].DO_03_K4_FZ(false);
                    break;
                case 4: //DO_LOGIC_IN2
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_04_14V_LSS(true);
                    else
                        this.device.NI_usb6002[0].DO_04_14V_LSS(false);
                    break;
                case 5: //DO_LOGIC_IN3
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_05(true);
                    else
                        this.device.NI_usb6002[0].DO_05(false);
                    break;
                case 6: //DO_LOGIC_IN4
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_06(true);
                    else
                        this.device.NI_usb6002[0].DO_06(false);
                    break;
                case 7: //DO_LOGIC_IN5
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_07(true);
                    else
                        this.device.NI_usb6002[0].DO_07(false);
                    break;

                case 8: //DO_LOGIC_IN6
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].DO_08(true);
                    else
                        this.device.NI_usb6002[0].DO_08(false);
                    break;
                case 9: //DO_LOGIC_IN7
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].D0_09_Swich_AI5_0_1_2(true);
                    else
                        this.device.NI_usb6002[0].D0_09_Swich_AI5_0_1_2(false);
                    break;
                case 10: //DO_LOGIC_IN8
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].START_DI_00(true);
                    else
                        this.device.NI_usb6002[0].START_DI_00(false);
                    break;
                case 11: //DO_LOGIC_IN9
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].STOP_DI_01(true);
                    else
                        this.device.NI_usb6002[0].STOP_DI_01(false);
                    break;
                case 12: //DO_LOGIC_IN10
                    if (checkedListBox2.GetItemChecked(nIndex) == true)
                        this.device.NI_usb6002[0].Rest_DI_02(true);
                    else
                        this.device.NI_usb6002[0].Rest_DI_02(false);
                    break;

                default: break;
            }
        }



        private void Display(ListBox listBox, string Data)
        {
           

            listBox.Items.Add("["+Data+"]" + DateTime.Now.ToString("mm:ss")+"]" +"\r\n");








        }

        private void checkedListBoxAO_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIndex = checked1ListBoxAO.SelectedIndex;
            double Logic_out_AI0 = 0;
            double AI_RES_AI4 = 0;
            double Logic_AI1 = 0;
            double LSS1_AD = 0;
            double AI_Start_AI2 = 0;
            double PCH1_IN_AD_AI6 = 0;
            double AI_CC_AI03 = 0;
            double AI07 = 0;
            switch (nIndex)
            {
                case 0:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].Logic_out_AI00(ref Logic_out_AI0);
                        Display(listBox1, Logic_out_AI0.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 1:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].AI_RES_AI4(ref AI_RES_AI4);
                        Display(listBox1, AI_RES_AI4.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 2:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].LSS1_AD(ref Logic_AI1);
                        Display(listBox1, Logic_AI1.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 3:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].Logic_AI01(ref LSS1_AD);
                        Display(listBox1, LSS1_AD.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 4:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].AI_Start_AI02(ref AI_Start_AI2);
                        Display(listBox1, AI_Start_AI2.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 5:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].PCH1_IN_AD_AI06(ref PCH1_IN_AD_AI6);
                        Display(listBox1, PCH1_IN_AD_AI6.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 6:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].AI_CC_AI03(ref AI_CC_AI03);
                        Display(listBox1, AI_CC_AI03.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;
                case 7:
                    if (checked1ListBoxAO.GetItemChecked(nIndex) == true)
                    {
                        this.device.NI_usb6002[0].AI07_out_5V(ref AI07);
                        Display(listBox1, AI07.ToString("0.00"));
                    }
                    else
                        Display(listBox1, "NULL");
                    break;


                default: break;
            }
        }

        private void chk1_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIndex = chk1_ListBox.SelectedIndex;
            bool ret = false;
            if (nIndex != -1)
            {

                switch (nIndex)
                {

                    case 0:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    #region
                    case 1:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    case 2:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    case 3:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    case 4:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;

                    case 5:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;
                    case 6:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;
                    case 7:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;
                    ///通道3
                    case 8:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 0);//写入1K

                        }
                        break;

                    case 9:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 0);//写入1K

                        }
                        break;
                    case 10:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 0);//写入1K

                        }
                        break;
                    case 11:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 3, 0);//写入1K

                        }
                        break;
                    ///通道4
                    case 12:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 0);//写入1K

                        }
                        break;

                    case 13:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 0);//写入1K

                        }
                        break;
                    case 14:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 0);//写入1K

                        }
                        break;
                    case 15:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 4, 0);//写入1K

                        }
                        break;

                    ///通道5
                    case 16:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 0);//写入1K

                        }
                        break;

                    case 17:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 0);//写入1K

                        }
                        break;
                    case 18:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 0);//写入1K

                        }
                        break;
                    case 19:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 5, 0);//写入1K

                        }
                        break;

                    //通道6
                    case 20:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 0);//写入1K

                        }
                        break;

                    case 21:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 0);//写入1K

                        }
                        break;
                    case 22:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 0);//写入1K

                        }
                        break;
                    case 23:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 0);//写入1K

                        }
                        break;
                    //NTC电阻1

                    case 24:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;

                    case 25:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    case 26:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 1, 0);//写入1K

                        }
                        break;
                    case 27:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 6, 0);//写入1K

                        }
                        break;

                    //NTC电阻2


                    case 28:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 1);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;

                    case 29:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 2);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;
                    case 30:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 4);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;
                    case 31:
                        if (chk1_ListBox.GetItemChecked(nIndex) == true)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 8);//写入1K
                            if (ret == false)
                            {
                                MessageBox.Show("写入失败");
                            }
                        }
                        if (chk1_ListBox.GetItemChecked(nIndex) == false)
                        {
                            ret = device.CPCB[0].RBIN1Swich(device.master_C[0], 2, 0);//写入1K

                        }
                        break;








                    #endregion




                    default: break;


                }






            }
        }

        private void btn_swich_Click(object sender, EventArgs e)
        {
            SetHzDuty(txtPWM_CH1_HZ, txt_ch1_duty, device, 0, 1, 1);
            SetHzDuty(txtPWM_CH2_HZ, txt_ch2_duty, device, 0, 1, 1);
            SetHzDuty(txtPWM_CH3_HZ, txt_ch3_duty, device, 0, 1, 1);
            SetHzDuty(txtPWM_CH4_HZ, txt_ch4_duty, device, 0, 1, 1);
            SetHzDuty(txtPWM_CH5_HZ, txt_ch5_duty, device, 0, 1, 1);
            SetHzDuty(txtPWM_CH6_HZ, txt_ch6_duty, device, 0, 1, 1);

        }

        private void btn1GetPWM_DUTY_Click(object sender, EventArgs e)
        {   //PMW 实时电流
            ushort BatteryRealvoltage = device.CPCB[0].BatteryRealvoltage(device.master_SW[0]);
            ushort GetPWMCurrent1 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 1);
            ushort GetPWMCurrent2 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 2);
            ushort GetPWMCurrent3 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 3);
            ushort GetPWMCurrent4 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 4);
            ushort GetPWMCurrent5 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 5);
            ushort GetPWMCurrent6 = device.CPCB[0].GetPWMCurrent(device.master_SW[0], 6);
            //CH 实时电流
            ushort GetChannelCurrent1 = device.CPCB[0].GetChannelCurrent(device.master_SW[0], 1);
            ushort GetChannelCurrent2 = device.CPCB[0].GetChannelCurrent(device.master_SW[0], 2);
            ushort GetChannelCurrent3 = device.CPCB[0].GetChannelCurrent(device.master_SW[0], 3);
            ushort GetChannelCurrent4 = device.CPCB[0].GetChannelCurrent(device.master_SW[0], 4);

            Display(lisbox_swich1, "GetPWM1-" + GetPWMCurrent1);
            Display(lisbox_swich1, "GetPWM2-" + GetPWMCurrent2);
            Display(lisbox_swich1, "GetPWM3-" + GetPWMCurrent3);
            Display(lisbox_swich1, "GetPWM4-" + GetPWMCurrent4);
            Display(lisbox_swich1, "GetPWM5-" + GetPWMCurrent5);
            Display(lisbox_swich1, "GetPWM6-" + GetPWMCurrent6);
            Display(lisbox_swich1, "GetChannel1-" + GetChannelCurrent1);
            Display(lisbox_swich1, "GetChannel2-" + GetChannelCurrent2);
            Display(lisbox_swich1, "GetChannel3-" + GetChannelCurrent3);
            Display(lisbox_swich1, "GetChannel4-" + GetChannelCurrent4);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            device.plz50fLoad[0].Reset();
            device.plz50fLoad[1].Reset();
            device.plz50fLoad[2].Reset();
            device.plz50fLoad[3].Reset();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            device.SwichPCB[0].Hz_dutyRest(device.master_SW[0]);
           device.SwichPCB[0].RBinRest(device.master_C[0], 5);
        }
    }
}
