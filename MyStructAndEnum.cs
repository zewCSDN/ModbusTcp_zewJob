using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20230509

{

    public enum returnCode
    {
        success,
        errOpenError,
        errTimeOut,
        errReadError,
        errWriteError,
    }
    public enum enumGWState
    {
        stateReqVol,
        stateGetVol,
        stateReqCur,
        stateGetCur,
        stateReqPow,
        stateGetPow,
        stateIdel,
    }

    public struct stuProductNumber
    {
        /// <summary>
        /// 产品总数
        /// </summary>
        public int nOKNum;
        /// <summary>
        /// NG个数
        /// </summary>
        public int nNGNum;
        /// <summary>
        /// OK百分比
        /// </summary>
        public double dbOKPercent;
    }

    public struct stuTestData
    {

        //产品名称
        public string strProductName;

        public double zcur;
        public double fcur;

        //霍尔范围
        public double zdbDutyH;
        public double zdbDutyL;
        public double zdbDuty;
        public double zdbFre;
        public double fdbDutyH;
        public double fdbDutyL;
        public double fdbDuty;
        public double fdbFre;


    }

    public struct stuWMGameFlag
    {
        public bool bStartFlag;
        public bool bMainThreadFlag;
        public bool bMeasureFlag;
        public Stopwatch stopwach;
    }
    public struct pulse
    {
        public double HiPeriod;
        public double LoPeriod;
        public double Fre;
    }

    public struct testPulseData {
        public pulse pwF;
        public pulse pwR;
        public bool bFlagFR;
    }

}



