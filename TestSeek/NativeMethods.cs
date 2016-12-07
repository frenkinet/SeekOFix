using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace winusbdotnet
{

    internal class EnumeratedDevice
    {
        public string DevicePath { get; set; }
        public Guid InterfaceGuid { get; set; }
        public string DeviceDescription { get; set; }
        public string Manufacturer { get; set; }
        public string FriendlyName { get; set; }
    }

    internal class NativeMethods
    {

        private struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid classGuid;
            public UInt32 devInst;
            public IntPtr reserved;
        }

        private struct SP_DEVICE_INTERFACE_DATA
        {
            public UInt32 cbSize;
            public Guid interfaceClassGuid;
            public UInt32 flags;
            public IntPtr reserved;
        }

        private struct DEVPROPKEY
        {
            public Guid fmtId;
            public UInt32 pId;


            //DEFINE_DEVPROPKEY(DEVPKEY_Device_DeviceDesc,             0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 2);     // DEVPROP_TYPE_STRING
            //DEFINE_DEVPROPKEY(DEVPKEY_Device_Manufacturer,           0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 13);    // DEVPROP_TYPE_STRING
            //DEFINE_DEVPROPKEY(DEVPKEY_Device_FriendlyName,           0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);    // DEVPROP_TYPE_STRING

            public static DEVPROPKEY Device_DeviceDesc { get { return new DEVPROPKEY() { fmtId = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pId = 2 }; } }
            public static DEVPROPKEY Device_Manufacturer { get { return new DEVPROPKEY() { fmtId = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pId = 13 }; } }
            public static DEVPROPKEY Device_FriendlyName { get { return new DEVPROPKEY() { fmtId = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), pId = 14 }; } }
        }

        private const UInt32 DIGCF_PRESENT = 2;
        private const UInt32 DIGCF_ALLCLASSES = 4;
        private const UInt32 DIGCF_DEVICEINTERFACE = 0x10;

        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private const int ERROR_NOT_FOUND = 1168;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int ERROR_MORE_DATA = 234;

        public const int ERROR_SEM_TIMEOUT = 121;
        public const int ERROR_IO_PENDING = 997;

        private const int DICS_FLAG_GLOBAL = 1;
        private const int DICS_FLAG_CONFIGSPECIFIC = 2;

        private const int DIREG_DEV = 1;
        private const int DIREG_DRV = 2;

        private const int KEY_READ = 0x20019; // Registry SAM value.

        private const int RRF_RT_REG_SZ = 2;
        private const int RRF_RT_REG_MULTI_SZ = 0x20;


        /// <summary>
        /// Retrieve device paths that can be opened from a specific device interface guid.
        /// todo: Make this friendlier & query some more data about the devices being returned.
        /// </summary>
        /// <param name="deviceInterface">Guid uniquely identifying the interface to search for</param>
        /// <returns>List of device paths that can be opened with CreateFile</returns>
        public static EnumeratedDevice[] EnumerateDevicesByInterface(Guid deviceInterface)
        {
            // Horribe horrible things have to be done with SetupDI here. These travesties must never leave this class.
            List<EnumeratedDevice> outputPaths = new List<EnumeratedDevice>();

            IntPtr devInfo = SetupDiGetClassDevs(ref deviceInterface, null, IntPtr.Zero, DIGCF_DEVICEINTERFACE | DIGCF_PRESENT);
            if(devInfo == INVALID_HANDLE_VALUE)
            {
                throw new Exception("SetupDiGetClassDevs failed. " + (new Win32Exception()).ToString());
            }

            try
            {
                uint deviceIndex = 0;
                SP_DEVICE_INTERFACE_DATA interfaceData = new SP_DEVICE_INTERFACE_DATA();

                bool success = true;
                for (deviceIndex = 0; ; deviceIndex++)
                {
                    interfaceData.cbSize = (uint)Marshal.SizeOf(interfaceData);
                    success = SetupDiEnumDeviceInterfaces(devInfo, IntPtr.Zero, ref deviceInterface, deviceIndex, ref interfaceData);
                    if (!success)
                    {
                        if (Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS)
                        {
                            throw new Exception("SetupDiEnumDeviceInterfaces failed " + (new Win32Exception()).ToString());
                        }
                        // We have reached the end of the list of devices.
                        break;
                    }

                    // This is a valid interface, retrieve its path
                    EnumeratedDevice dev = new EnumeratedDevice() { DevicePath = RetrieveDeviceInstancePath(devInfo, interfaceData), InterfaceGuid = deviceInterface };


                    // Todo: debug. Not working correctly.
                    /*
                    dev.DeviceDescription = RetrieveDeviceInstancePropertyString(devInfo, interfaceData, DEVPROPKEY.Device_DeviceDesc);
                    dev.Manufacturer = RetrieveDeviceInstancePropertyString(devInfo, interfaceData, DEVPROPKEY.Device_Manufacturer);
                    dev.FriendlyName = RetrieveDeviceInstancePropertyString(devInfo, interfaceData, DEVPROPKEY.Device_FriendlyName);
                    */

                    outputPaths.Add(dev);
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(devInfo);
            }

            return outputPaths.ToArray();
        }

        public static EnumeratedDevice[] EnumerateAllWinUsbDevices()
        {
            List<EnumeratedDevice> outputDevices = new List<EnumeratedDevice>();
            string[] guids = EnumerateAllWinUsbGuids();
            foreach (string guid in guids)
            {
                try
                {
                    Guid g = new Guid(guid);
                    outputDevices.AddRange(EnumerateDevicesByInterface(g));
                }
                catch
                {
                    // Ignore failing guid conversions.
                }
            }
            return outputDevices.ToArray();
        }

        public static string[] EnumerateAllWinUsbGuids()
        {
            // Horribe horrible things have to be done with SetupDI here. These travesties must never leave this class.
            List<string> outputGuids = new List<string>();

            IntPtr devInfo = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
            if (devInfo == INVALID_HANDLE_VALUE)
            {
                throw new Exception("SetupDiGetClassDevs failed. " + (new Win32Exception()).ToString());
            }

            try
            {
                uint deviceIndex = 0;
                SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();

                bool success = true;
                for (deviceIndex = 0; ; deviceIndex++)
                {
                    devInfoData.cbSize = (uint)Marshal.SizeOf(devInfoData);
                    success = SetupDiEnumDeviceInfo(devInfo, deviceIndex, ref devInfoData);
                    if (!success)
                    {
                        if (Marshal.GetLastWin32Error() != ERROR_NO_MORE_ITEMS)
                        {
                            throw new Exception("SetupDiEnumDeviceInfo failed " + (new Win32Exception()).ToString());
                        }
                        // We have reached the end of the list of devices.
                        break;
                    }

                    // Enumerate the WinUSB Interface Guids (if present) by looking at the registry.
                    //DebugEnumRegistryValues(devInfo, devInfoData);
                    string guid = RetrieveDeviceProperty(devInfo, devInfoData, "DeviceInterfaceGUIDs");
                    if(guid == null)
                    {
                        guid = RetrieveDeviceProperty(devInfo, devInfoData, "DeviceInterfaceGUID");
                    }

                    if (guid != null)
                    {
                        outputGuids.Add(guid);
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(devInfo);
            }

            return outputGuids.Distinct().ToArray();
        }

        static void DebugEnumRegistryValues(IntPtr devInfo, SP_DEVINFO_DATA devInfoData)
        {
            System.Console.WriteLine("DebugEnumRegistryValues");
            IntPtr hKey = SetupDiOpenDevRegKey(devInfo, ref devInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            if (hKey == INVALID_HANDLE_VALUE)
            {
                System.Console.WriteLine("Failed");
                return;
                throw new Exception("SetupDiGetClassDevs failed. " + (new Win32Exception()).ToString());
            }

            try
            {

                IntPtr memValue = Marshal.AllocHGlobal(16384);
                try
                {
                    IntPtr memData = Marshal.AllocHGlobal(65536);
                    try
                    {

                        for (int i = 0; ; i++)
                        {
                            UInt32 outValueLen = 16384;
                            UInt32 outDataLen = 65536;
                            UInt32 outType;
                            long output = RegEnumValue(hKey, (uint)i, memValue, ref outValueLen, IntPtr.Zero, out outType, memData, ref outDataLen);
                            if((int)output == ERROR_NO_MORE_ITEMS)
                            {
                                break;
                            }
                            if (output != 0)
                            {
                                throw new Exception("RegEnumValue failed " + (new Win32Exception((int)output)).ToString());
                            }

                            string value = ReadAsciiString(memValue, (int)outValueLen);
                            string data = ReadAsciiString(memData, (int)outDataLen);

                            Console.WriteLine("Enum: '{0}' -  {2} '{1}'", value, data, outType);

                        }

                    }
                    finally
                    {
                        Marshal.FreeHGlobal(memData);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(memValue);
                }

            }
            finally
            {
                RegCloseKey(hKey);
            }
        }




        static string RetrieveDeviceProperty(IntPtr devInfo, SP_DEVINFO_DATA devInfoData, string deviceProperty)
        {
            IntPtr hKey = SetupDiOpenDevRegKey(devInfo, ref devInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            if (hKey == INVALID_HANDLE_VALUE)
            {
                return null; // Ignore failures, probably the key doesn't exist.
            }

            try
            {
                UInt32 outType;
                UInt32 outLength = 0;
                long output = RegGetValue(hKey, null, deviceProperty, RRF_RT_REG_SZ | RRF_RT_REG_MULTI_SZ, out outType, IntPtr.Zero, ref outLength);
                if(output == ERROR_FILE_NOT_FOUND)
                {
                    return null; // Key not present, don't continue.
                }
                if (output != 0)
                {
                    throw new Exception("RegGetValue failed (determining length) " + (new Win32Exception((int)output)).ToString());
                }

                IntPtr mem = Marshal.AllocHGlobal((int)outLength);
                try
                {

                    UInt32 actualLength = outLength;
                    output = RegGetValue(hKey, null, deviceProperty, RRF_RT_REG_SZ | RRF_RT_REG_MULTI_SZ, out outType, mem, ref actualLength);
                    if (output != 0)
                    {
                        throw new Exception("RegGetValue failed (retrieving data) " + (new Win32Exception((int)output)).ToString());
                    }

                    // Convert TCHAR string into chars.
                    if (actualLength > outLength)
                    {
                        throw new Exception("Consistency issue: Actual length should not be larger than buffer size.");
                    }

                    return ReadAsciiString(mem, (int)((actualLength)));
                }
                finally
                {
                    Marshal.FreeHGlobal(mem);
                }

            }
            finally
            {
                RegCloseKey(hKey);
            }
        }

        static string RetrieveDeviceInstancePath(IntPtr devInfo, SP_DEVICE_INTERFACE_DATA interfaceData)
        {
            // This is a valid interface, retrieve its path
            UInt32 requiredLength = 0;

            if (!SetupDiGetDeviceInterfaceDetail(devInfo, ref interfaceData, IntPtr.Zero, 0, ref requiredLength, IntPtr.Zero))
            {
                int err = Marshal.GetLastWin32Error();

                if (err != ERROR_INSUFFICIENT_BUFFER)
                {
                    throw new Exception("SetupDiGetDeviceInterfaceDetail failed (determining length) " + (new Win32Exception()).ToString());
                }
            }

            UInt32 actualLength = requiredLength;
            Int32 structLen = 6;
            if (IntPtr.Size == 8) structLen = 8; // Compensate for 64bit struct packing.

            if (requiredLength < structLen)
            {
                throw new Exception("Consistency issue: Required memory size should be larger");
            }

            IntPtr mem = Marshal.AllocHGlobal((int)requiredLength);
            
            try
            {
                Marshal.WriteInt32(mem, structLen); // set fake size in fake structure

                if (!SetupDiGetDeviceInterfaceDetail(devInfo, ref interfaceData, mem, requiredLength, ref actualLength, IntPtr.Zero))
                {
                    throw new Exception("SetupDiGetDeviceInterfaceDetail failed (retrieving data) " + (new Win32Exception()).ToString());
                }

                // Convert TCHAR string into chars.
                if (actualLength > requiredLength)
                {
                    throw new Exception("Consistency issue: Actual length should not be larger than buffer size.");
                }

                return ReadString(mem, (int)((actualLength - 4) / 2), 4);
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }
        }

        static string RetrieveDeviceInstancePropertyString(IntPtr devInfo, SP_DEVICE_INTERFACE_DATA interfaceData, DEVPROPKEY property)
        {
            // This is a valid interface, retrieve its path
            UInt32 requiredLength = 0;
            UInt32 propertyType;

            if (!SetupDiGetDeviceInterfaceProperty(devInfo, ref interfaceData, ref property, out propertyType, IntPtr.Zero, 0, out requiredLength, 0))
            {
                int err = Marshal.GetLastWin32Error();
                if (err == ERROR_NOT_FOUND)
                {
                    return null;
                }

                if (err != ERROR_INSUFFICIENT_BUFFER)
                {
                    throw new Exception("SetupDiGetDeviceInterfaceProperty failed (determining length) " + (new Win32Exception()).ToString());
                }

            }

            UInt32 actualLength = requiredLength;


            IntPtr mem = Marshal.AllocHGlobal((int)requiredLength);
            try
            {
                Marshal.WriteInt32(mem, 6); // set fake size in fake structure

                if (!SetupDiGetDeviceInterfaceProperty(devInfo, ref interfaceData, ref property, out propertyType, mem, requiredLength, out actualLength, 0))
                {
                    throw new Exception("SetupDiGetDeviceInterfaceProperty failed (retrieving data) " + (new Win32Exception()).ToString());
                }

                // Convert TCHAR string into chars.
                if (actualLength > requiredLength)
                {
                    throw new Exception("Consistency issue: Actual length should not be larger than buffer size.");
                }

                return ReadString(mem, (int)((actualLength) / 2));
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }
        }


        static string ReadString(IntPtr source, int length, int offset = 0)
        {
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = (char)Marshal.ReadInt16(source, i * 2 + offset);
                if (stringChars[i] == 0) { length = i; break; }
            }
            return new string(stringChars, 0, length);
        }


        static string ReadAsciiString(IntPtr source, int length, int offset = 0)
        {
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = (char)Marshal.ReadByte(source, i + offset);
                if (stringChars[i] == 0) { length = i; break; }
            }
            return new string(stringChars, 0, length);
        }


        /*
        HDEVINFO SetupDiGetClassDevs(
          _In_opt_  const GUID *ClassGuid,
          _In_opt_  PCTSTR Enumerator,
          _In_opt_  HWND hwndParent,
          _In_      DWORD Flags
        );
         */
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, IntPtr hwndParent, UInt32 flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static IntPtr SetupDiGetClassDevs(IntPtr classGuid, string enumerator, IntPtr hwndParent, UInt32 flags);

        /*
         BOOL SetupDiEnumDeviceInterfaces(
          _In_      HDEVINFO DeviceInfoSet,
          _In_opt_  PSP_DEVINFO_DATA DeviceInfoData,
          _In_      const GUID *InterfaceClassGuid,
          _In_      DWORD MemberIndex,
          _Out_     PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData
        );
         */
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr optDeviceInfoData, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr optDeviceInfoData, IntPtr interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, IntPtr interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);


        /*
        BOOL SetupDiEnumDeviceInfo(
          _In_   HDEVINFO DeviceInfoSet,
          _In_   DWORD MemberIndex,
          _Out_  PSP_DEVINFO_DATA DeviceInfoData
        );
         */
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, UInt32 memberIndex, ref SP_DEVINFO_DATA deviceInfoData);

        /*
        HKEY SetupDiOpenDevRegKey(
          _In_  HDEVINFO DeviceInfoSet,
          _In_  PSP_DEVINFO_DATA DeviceInfoData,
          _In_  DWORD Scope,
          _In_  DWORD HwProfile,
          _In_  DWORD KeyType,
          _In_  REGSAM samDesired
        );
        */
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static IntPtr SetupDiOpenDevRegKey(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, UInt32 scope, UInt32 hwProfile, UInt32 keyType, UInt32 samDesired);

        /*
        BOOL SetupDiGetDeviceInterfaceProperty(
          _In_       HDEVINFO DeviceInfoSet,
          _In_       PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
          _In_       const DEVPROPKEY *PropertyKey,
          _Out_      DEVPROPTYPE *PropertyType,
          _Out_      PBYTE PropertyBuffer,
          _In_       DWORD PropertyBufferSize,
          _Out_opt_  PDWORD RequiredSize,
          _In_       DWORD Flags
        );
        */
        [DllImport("setupapi.dll", SetLastError = true, CharSet=CharSet.Unicode, EntryPoint="SetupDiGetDeviceInterfacePropertyW")]
        private extern static bool SetupDiGetDeviceInterfaceProperty(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceDataa, ref DEVPROPKEY propertyKey, out UInt32 propertyType, IntPtr outPropertyData, UInt32 dataBufferLength, out UInt32 requredBufferLength, UInt32 flags);


        /*
        BOOL SetupDiGetDevicePropertyKeys(
          _In_       HDEVINFO DeviceInfoSet,
          _In_       PSP_DEVINFO_DATA DeviceInfoData,
          _Out_opt_  DEVPROPKEY *PropertyKeyArray,
          _In_       DWORD PropertyKeyCount,
          _Out_opt_  PDWORD RequiredPropertyKeyCount,
          _In_       DWORD Flags
        );
        */

        /*
        LONG WINAPI RegCloseKey(
        _In_  HKEY hKey
        );
        */
        [DllImport("advapi32.dll", SetLastError = false)]
        private extern static int RegCloseKey(IntPtr hKey);


        /*
        LONG WINAPI RegGetValue(
          _In_         HKEY hkey,
          _In_opt_     LPCTSTR lpSubKey,
          _In_opt_     LPCTSTR lpValue,
          _In_opt_     DWORD dwFlags,
          _Out_opt_    LPDWORD pdwType,
          _Out_opt_    PVOID pvData,
          _Inout_opt_  LPDWORD pcbData
        );
        */
        [DllImport("advapi32.dll", SetLastError = false)]
        private extern static int RegGetValue(IntPtr hKey, string lpSubKey, string lpValue, UInt32 flags, out UInt32 outType, IntPtr outData, ref UInt32 dataLength);

        /*
        LONG WINAPI RegEnumValue(
          _In_         HKEY hKey,
          _In_         DWORD dwIndex,
          _Out_        LPTSTR lpValueName,
          _Inout_      LPDWORD lpcchValueName,
          _Reserved_   LPDWORD lpReserved,
          _Out_opt_    LPDWORD lpType,
          _Out_opt_    LPBYTE lpData,
          _Inout_opt_  LPDWORD lpcbData
        );
        */
        [DllImport("advapi32.dll", SetLastError = false)]
        private extern static int RegEnumValue(IntPtr hKey, UInt32 index, IntPtr outValue, ref UInt32 valueLen, IntPtr reserved, out UInt32 outType, IntPtr outData, ref UInt32 dataLength);

        /*                 
        BOOL SetupDiDestroyDeviceInfoList(
          _In_  HDEVINFO DeviceInfoSet
        );
          */
        [DllImport("setupapi.dll", SetLastError = true)]
        private extern static bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);


        /* 
        BOOL SetupDiGetDeviceInterfaceDetail(
          _In_       HDEVINFO DeviceInfoSet,
          _In_       PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
          _Out_opt_  PSP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData,
          _In_       DWORD DeviceInterfaceDetailDataSize,
          _Out_opt_  PDWORD RequiredSize,
          _Out_opt_  PSP_DEVINFO_DATA DeviceInfoData
        );
          */
        [DllImport("setupapi.dll", SetLastError = true, CharSet=CharSet.Unicode)]
        private extern static bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, [In] ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, 
            IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailSize, ref UInt32 requiredSize, IntPtr deviceInfoData );


        /* 
        HANDLE WINAPI CreateFile(
          _In_      LPCTSTR lpFileName,
          _In_      DWORD dwDesiredAccess,
          _In_      DWORD dwShareMode,
          _In_opt_  LPSECURITY_ATTRIBUTES lpSecurityAttributes,
          _In_      DWORD dwCreationDisposition,
          _In_      DWORD dwFlagsAndAttributes,
          _In_opt_  HANDLE hTemplateFile
        );
          */
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public extern static SafeFileHandle CreateFile(string lpFileName, UInt32 dwDesiredAccess, 
            UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_SHARE_READ = 1;
        public const uint FILE_SHARE_WRITE = 2;




        public struct WINUSB_SETUP_PACKET
        {
            public byte RequestType;
            public byte Request;
            public UInt16 Value;
            public UInt16 Index;
            public UInt16 Length;
        }


        /* 
        BOOL __stdcall WinUsb_Initialize(
          _In_   HANDLE DeviceHandle,
          _Out_  PWINUSB_INTERFACE_HANDLE InterfaceHandle
        );
          */
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_Initialize(SafeFileHandle deviceHandle, out IntPtr interfaceHandle);

        /* 
        BOOL __stdcall WinUsb_Free(
          _In_  WINUSB_INTERFACE_HANDLE InterfaceHandle
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_Free(IntPtr interfaceHandle);

        /*
         BOOL __stdcall WinUsb_ControlTransfer(
          _In_       WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_       WINUSB_SETUP_PACKET SetupPacket,
          _Out_      PUCHAR Buffer,
          _In_       ULONG BufferLength,
          _Out_opt_  PULONG LengthTransferred,
          _In_opt_   LPOVERLAPPED Overlapped
        );
        */
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_ControlTransfer(IntPtr interfaceHandle, WINUSB_SETUP_PACKET setupPacket, byte[] buffer, uint bufferLength, out UInt32 lengthTransferred, IntPtr overlapped);




        /* 
        BOOL __stdcall WinUsb_ReadPipe(
          _In_       WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_       UCHAR PipeID,
          _Out_      PUCHAR Buffer,
          _In_       ULONG BufferLength,
          _Out_opt_  PULONG LengthTransferred,
          _In_opt_   LPOVERLAPPED Overlapped
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_ReadPipe(IntPtr interfaceHandle, byte pipeId, IntPtr buffer, uint bufferLength, IntPtr lengthTransferred, IntPtr overlapped);
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_ReadPipe(IntPtr interfaceHandle, byte pipeId, [Out] byte[] buffer, uint bufferLength, ref UInt32 lengthTransferred, IntPtr overlapped);

        /* 
        BOOL __stdcall WinUsb_WritePipe(
          _In_       WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_       UCHAR PipeID,
          _In_       PUCHAR Buffer,
          _In_       ULONG BufferLength,
          _Out_opt_  PULONG LengthTransferred,
          _In_opt_   LPOVERLAPPED Overlapped
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_WritePipe(IntPtr interfaceHandle, byte pipeId, [In] byte[] buffer, uint bufferLength, IntPtr lengthTransferred, ref NativeOverlapped overlapped);
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_WritePipe(IntPtr interfaceHandle, byte pipeId, [In] byte[] buffer, uint bufferLength, ref UInt32 lengthTransferred, IntPtr overlapped);


        /* 
        BOOL __stdcall WinUsb_GetOverlappedResult(
          _In_   WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_   LPOVERLAPPED lpOverlapped,
          _Out_  LPDWORD lpNumberOfBytesTransferred,
          _In_   BOOL bWait
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_GetOverlappedResult(IntPtr interfaceHandle, IntPtr overlapped, out UInt32 numberOfBytesTransferred, bool wait);


        /* 
        BOOL __stdcall WinUsb_SetPipePolicy(
          _In_  WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_  UCHAR PipeID,
          _In_  ULONG PolicyType,
          _In_  ULONG ValueLength,
          _In_  PVOID Value
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_SetPipePolicy(IntPtr interfaceHandle, byte pipeId, UInt32 policyType, UInt32 valueLength, UInt32[] value);

        /* 
        BOOL __stdcall WinUsb_GetPipePolicy(
          _In_     WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_     UCHAR PipeID,
          _In_     ULONG PolicyType,
          _Inout_  PULONG ValueLength,
          _Out_    PVOID Value
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_GetPipePolicy(IntPtr interfaceHandle, byte pipeId, UInt32 policyType, ref UInt32 valueLength, UInt32[] value);


        /* 
        BOOL __stdcall WinUsb_FlushPipe(
          _In_  WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_  UCHAR PipeID
        );
        */

        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_FlushPipe(IntPtr interfaceHandle, byte pipeId);


        /*
        BOOL __stdcall WinUsb_GetDescriptor(
          _In_   WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_   UCHAR DescriptorType,
          _In_   UCHAR Index,
          _In_   USHORT LanguageID,
          _Out_  PUCHAR Buffer,
          _In_   ULONG BufferLength,
          _Out_  PULONG LengthTransferred
        );
        */


        /*
        BOOL __stdcall WinUsb_QueryInterfaceSettings(
          _In_   WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_   UCHAR AlternateSettingNumber,
          _Out_  PUSB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor
        );
        */

        /*
        BOOL __stdcall WinUsb_GetCurrentAlternateSetting(
          _In_   WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _Out_  PUCHAR AlternateSetting
        );
         */
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_GetCurrentAlternateSetting(IntPtr interfaceHandle, out byte alternateSetting);


        /*
        BOOL __stdcall WinUsb_SetCurrentAlternateSetting(
          _In_  WINUSB_INTERFACE_HANDLE InterfaceHandle,
          _In_  UCHAR AlternateSetting
        );
        */
        [DllImport("winusb.dll", SetLastError = true)]
        public extern static bool WinUsb_SetCurrentAlternateSetting(IntPtr interfaceHandle, byte alternateSetting);






    }

    public struct NativeOverlapped
    {
        public IntPtr Internal;
        public IntPtr InternalHigh;
        public long Pointer; // On 32bit systems this is 32bit, but it's merged with an "Offset" field which is 64bit.
        public IntPtr Event;
    }

    public class Overlapped : IDisposable
    { 
        public Overlapped()
        {
            WaitEvent = new ManualResetEvent(false);
            OverlappedStructShadow = new NativeOverlapped();
            OverlappedStructShadow.Event = WaitEvent.SafeWaitHandle.DangerousGetHandle();

            OverlappedStruct = Marshal.AllocHGlobal(Marshal.SizeOf(OverlappedStructShadow));
            Marshal.StructureToPtr(OverlappedStructShadow, OverlappedStruct, false);
        }
        public void Dispose()
        {
            Marshal.FreeCoTaskMem(OverlappedStruct);
            WaitEvent.Dispose();
            GC.SuppressFinalize(this);
        }

        public ManualResetEvent WaitEvent;
        public NativeOverlapped OverlappedStructShadow;
        public IntPtr OverlappedStruct;
    }

    public enum WinUsbPipePolicy
    {
        SHORT_PACKET_TERMINATE = 1,
        AUTO_CLEAR_STALL = 2,
        PIPE_TRANSFER_TIMEOUT = 3,
        IGNORE_SHORT_PACKETS = 4,
        ALLOW_PARTIAL_READS = 5,
        AUTO_FLUSH = 6,
        RAW_IO = 7,
        MAXIMUM_TRANSFER_SIZE = 8,
        RESET_PIPE_ON_RESUME = 9
    }



}
