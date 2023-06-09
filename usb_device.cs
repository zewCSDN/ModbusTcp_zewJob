﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace USB2XXX
{
    class USB_DEVICE
    {
        //定义电压输出值
        public const Byte POWER_LEVEL_NONE = 0;	//不输出
        public const Byte POWER_LEVEL_1V8 = 1;	//输出1.8V
        public const Byte POWER_LEVEL_2V5 = 2;	//输出2.5V
        public const Byte POWER_LEVEL_3V3 = 3;	//输出3.3V
        public const Byte POWER_LEVEL_5V0 = 4;	//输出5.0V
        //设备信息定义
        public struct DEVICE_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] FirmwareName;   //固件名称字符串
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public Byte[] BuildDate;    //固件编译时间字符串
            public UInt32 HardwareVersion;//硬件版本号
            public UInt32 FirmwareVersion;//固件版本号
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public UInt32[] SerialNumber; //适配器序列号
            public UInt32 Functions;      //适配器当前具备的功能
        }
        //方法定义
        /**
          * @brief  初始化USB设备，并扫描设备连接数，必须调用
          * @param  pDevHandle 每个设备的设备号存储地址
          * @retval 扫描到的设备数量
          */
        [DllImport("USB2XXX.dll")]
        public static extern Int32 USB_ScanDevice(Int32[] pDevHandle);
        /**
          * @brief  打开设备，必须调用
          * @param  DevHandle 设备索引号
          * @retval 打开设备的状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool USB_OpenDevice(Int32 DevHandle);
        /**
          * @brief  关闭设备
          * @param  DevHandle 设备索引号
          * @retval 关闭设备的状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool USB_CloseDevice(Int32 DevHandle);
        /**
          * @brief  复位设备程序，复位后需要重新调用USB_ScanDevice，USB_OpenDevice函数
          * @param  DevHandle 设备索引号
          * @retval 复位设备的状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool USB_ResetDevice(Int32 DevHandle);

        /**
          * @brief  检测到USB断开连接后，重新连接设备
          * @param  DevHandle 设备号
          * @retval 重连状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool USB_RetryConnect(Int32 DevHandle);

        /**
          * @brief  获取设备信息，比如设备名称，固件版本号，设备序号，设备功能说明字符串等
          * @param  DevHandle 设备索引号
          * @param  pDevInfo 设备信息存储结构体指针
          * @param  pFunctionStr 设备功能说明字符串
          * @retval 获取设备信息的状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_GetDeviceInfo(Int32 DevHandle, ref DEVICE_INFO pDevInfo, StringBuilder pFunctionStr);
        /**
          * @brief  擦出用户区数据
          * @param  DevHandle 设备索引号
          * @retval 用户区数据擦出状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_EraseUserData(Int32 DevHandle);

        /**
          * @brief  向用户区域写入用户自定义数据，写入数据之前需要调用擦出函数将数据擦出
          * @param  DevHandle 设备索引号
          * @param  OffsetAddr 数据写入偏移地址，起始地址为0x00，用户区总容量为0x10000字节，也就是64KBye
          * @param  pWriteData 用户数据缓冲区首地址
          * @param  DataLen 待写入的数据字节数
          * @retval 写入用户自定义数据状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_WriteUserData(Int32 DevHandle, Int32 OffsetAddr, byte[] pWriteData, Int32 DataLen);

        /**
          * @brief  从用户自定义数据区读出数据
          * @param  DevHandle 设备索引号
          * @param  OffsetAddr 数据写入偏移地址，起始地址为0x00，用户区总容量为0x10000字节，也就是64KBye
          * @param  pReadData 用户数据缓冲区首地址
          * @param  DataLen 待读出的数据字节数
          * @retval 读出用户自定义数据的状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_ReadUserData(Int32 DevHandle, Int32 OffsetAddr, byte[] pReadData, Int32 DataLen);

        /**
          * @brief  设置可变电压输出引脚输出电压值
          * @param  DevHandle 设备索引号
          * @param  PowerLevel 输出电压值
          * @retval 设置输出电压状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_SetPowerLevel(Int32 DevHandle, byte PowerLevel);
        /**
          * @brief  或者CAN或者LIN的时间戳原始值
          * @param  DevHandle 设备索引号
          * @param  pTimestamp 时间戳指针
          * @retval 获取时间戳状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_GetTimestamp(Int32 DevHandle, byte BusType, Int32[] pTimestamp);

        /**
          * @brief  复位CAN/LIN时间戳，需要在初始化CAN/LIN之后调用
          * @param  DevHandle 设备索引号
          * @retval 复位时间戳状态
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_ResetTimestamp(Int32 DevHandle);
        /**
          * @brief  获取dll编译日期
          * @param  pDateTime 输出DLL编译日期字符串
          * @retval 获取dll编译日期字符串
          */
        [DllImport("USB2XXX.dll")]
        public static extern bool DEV_GetDllBuildTime(StringBuilder pDateTime);
    }
}
