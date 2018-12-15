using System;
using System.Threading;
using System.Runtime.InteropServices;
using EasyHook;

namespace HookInject
{
    public class Main : EasyHook.IEntryPoint
    {
        eve_probe.HookInterface Interface;
        LocalHook CryptEncryptHook = null;
        LocalHook CryptDecryptHook = null;

        public Main(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // connect to host...
            Interface = RemoteHooking.IpcConnectClient<eve_probe.HookInterface>(InChannelName);

            Interface.Ping();
        }

        public void Run(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // install hook...
            try
            {
                CryptEncryptHook = LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "CryptEncrypt"),
                    new DCryptEncrypt(CryptEncrypt_Hooked),
                    this);
                CryptEncryptHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                CryptDecryptHook = LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "CryptDecrypt"),
                    new DCryptDecrypt(CryptDecrypt_Hooked),
                    this);
                CryptDecryptHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                Interface.ReportException(ExtInfo);

                return;
            }

            Interface.IsInstalled(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();

            // wait for host process termination...
            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                    Interface.Ping();
                }
            }
            catch //(Exception ExtInfo)
            {
                //System.Windows.Forms.MessageBox.Show(ExtInfo.ToString());
                // Ping() will raise an exception if host is unreachable
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool DCryptEncrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen);

        [DllImport("advapi32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CryptEncrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen);

        private bool CryptEncrypt_Hooked(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen)
        {
            bool ret = false;
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                var size = Marshal.ReadInt32(pdwDataLen);
                byte[] rawData = new byte[size];
                bool doCopy = (size != 0 && pbData != IntPtr.Zero);

                if (doCopy)
                    Marshal.Copy(pbData, rawData, 0, size);

                ret = CryptEncrypt(hKey, hHash, Final, dwFlags, pbData, pdwDataLen, dwBufLen);

                if (doCopy)
                {
                    byte[] cryptedData = new byte[size];
                    Marshal.Copy(pbData, cryptedData, 0, size);

                    This.Interface.Enqueue(new Tuple<bool, byte[], byte[]>(true, rawData, cryptedData));
                }
            }
            catch //(Exception ExtInfo)
            {
                //System.Windows.Forms.MessageBox.Show(ExtInfo.ToString());
            }
            return ret;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool DCryptDecrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CryptDecrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen);

        static bool CryptDecrypt_Hooked(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen)
        {
            bool ret = false;
            try
            {
                Main This = (Main)HookRuntimeInfo.Callback;
                var size = Marshal.ReadInt32(pdwDataLen);
                byte[] cryptedData = new byte[size];
                bool doCopy = (size != 0 && pbData != IntPtr.Zero);

                if (doCopy)
                    Marshal.Copy(pbData, cryptedData, 0, size);

                ret = CryptDecrypt(hKey, hHash, Final, dwFlags, pbData, pdwDataLen);

                if (doCopy)
                {
                    byte[] rawData = new byte[size];
                    Marshal.Copy(pbData, rawData, 0, size);

                    This.Interface.Enqueue(new Tuple<bool, byte[], byte[]>(false, rawData, cryptedData));
                }
            }
            catch //(Exception ExtInfo)
            {
                //System.Windows.Forms.MessageBox.Show(ExtInfo.ToString());
            }
            return ret;
        }
    }
}
