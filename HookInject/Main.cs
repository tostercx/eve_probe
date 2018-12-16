using System;
using System.Threading;
using System.Runtime.InteropServices;
using EasyHook;
using System.Collections.Generic;
using System.Linq;

namespace HookInject
{
    public class Main : EasyHook.IEntryPoint
    {
        eve_probe.HookInterface Interface;
        IntPtr ccpSock = IntPtr.Zero;
        IntPtr lastHkey = IntPtr.Zero;
        List<LocalHook> antiGC = new List<LocalHook>();

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
                antiGC.Add(LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "CryptEncrypt"),
                    new DCryptEncrypt(CryptEncrypt_Hooked),
                    this));
                antiGC.Last().ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                antiGC.Add(LocalHook.Create(
                    LocalHook.GetProcAddress("advapi32.dll", "CryptDecrypt"),
                    new DCryptDecrypt(CryptDecrypt_Hooked),
                    this));
                antiGC.Last().ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                antiGC.Add(LocalHook.Create(
                    LocalHook.GetProcAddress("ws2_32.dll", "WSASend"),
                    new DWSASend(WSASend_Hooked),
                    this));
                antiGC.Last().ThreadACL.SetExclusiveACL(new Int32[] { 0 });
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

                    try
                    { 
                        var pck = Interface.PollInjectionQueue();
                        if (pck != null && ccpSock != IntPtr.Zero)
                        {
                            var exlen = 256; // Extra space for in place crypt + length header
                            var wsabuf = Marshal.AllocHGlobal(pck.Length + exlen);
                            IntPtr plen = Marshal.AllocHGlobal(4);
                            Marshal.WriteInt32(plen, pck.Length);

                            var wsaData = IntPtr.Add(wsabuf, 4); // Add space for length header
                            Marshal.Copy(pck, 0, wsaData, pck.Length);
                            CryptEncrypt(lastHkey, IntPtr.Zero, 1, 0, wsaData, plen, (uint)(pck.Length + exlen - 4));
                            var len  = Marshal.ReadInt32(plen);
                            Marshal.WriteInt32(wsabuf, Marshal.ReadInt32(plen)); // Wtf MS, no Marshal.Copy for IntPtr 2 IntPtr?

                            send(ccpSock, wsabuf, len + 4, 0);

                            Marshal.FreeHGlobal(plen);
                            Marshal.FreeHGlobal(wsabuf);
                        }
                    }
                    catch (Exception ExtInfo)
                    {
                        Interface.ReportException(ExtInfo);
                    }

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
        delegate int DWSASend(IntPtr s, IntPtr lpBuffers, uint dwBufferCount, IntPtr lpNumberOfBytesSent, uint dwFlags, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);

        [DllImport("ws2_32", SetLastError = true)]
        static extern int WSASend(IntPtr s, IntPtr lpBuffers, uint dwBufferCount, IntPtr lpNumberOfBytesSent, uint dwFlags, IntPtr lpOverlapped, IntPtr lpCompletionRoutine);

        [DllImport("ws2_32", SetLastError = true)]
        static extern int getpeername(IntPtr s, IntPtr sockaddr, IntPtr namelen);

        [DllImport("ws2_32")]
        static extern int WSAGetLastError();

        [DllImport("ws2_32", SetLastError = true)]
        static extern int send(IntPtr s, IntPtr buf, int len, int flags);

        private int WSASend_Hooked(IntPtr s, IntPtr lpBuffers, uint dwBufferCount, IntPtr lpNumberOfBytesSent, uint dwFlags, IntPtr lpOverlapped, IntPtr lpCompletionRoutine)
        {
            Main This = (Main)HookRuntimeInfo.Callback;
            try
            {
                if (ccpSock == IntPtr.Zero)
                {
                    int len = 16;
                    var name = Marshal.AllocHGlobal(len);
                    var plen = Marshal.AllocHGlobal(sizeof(int));
                    Marshal.WriteInt32(plen, len);
                    var ret = getpeername(s, name, plen);

                    if ((uint)Marshal.ReadInt32(name, 4) == 3357732183) // 87.237.34.200 (reversed cause machine order...)
                    {
                        ccpSock = s;
                    }

                    Marshal.FreeHGlobal(name);
                    Marshal.FreeHGlobal(plen);
                }
            }
            catch (Exception ExtInfo)
            {
                This.Interface.ReportException(ExtInfo);
            }
            return WSASend(s, lpBuffers, dwBufferCount, lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool DCryptEncrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen);

        [DllImport("advapi32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CryptEncrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen);

        private bool CryptEncrypt_Hooked(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen, uint dwBufLen)
        {
            bool ret = false;
            Main This = (Main)HookRuntimeInfo.Callback;
            try
            {
                var size = Marshal.ReadInt32(pdwDataLen);
                byte[] rawData = new byte[size];
                bool doCopy = (size != 0 && pbData != IntPtr.Zero);

                if (doCopy)
                {
                    lastHkey = hKey;
                    Marshal.Copy(pbData, rawData, 0, size);
                }

                ret = CryptEncrypt(hKey, hHash, Final, dwFlags, pbData, pdwDataLen, dwBufLen);

                if (doCopy)
                {
                    size = Marshal.ReadInt32(pdwDataLen);
                    byte[] cryptedData = new byte[size];
                    Marshal.Copy(pbData, cryptedData, 0, size);

                    This.Interface.Enqueue(new Tuple<bool, byte[], byte[]>(true, rawData, cryptedData));
                }
            }
            catch (Exception ExtInfo)
            {
                This.Interface.ReportException(ExtInfo);
            }
            return ret;
        }


        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool DCryptDecrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen);

        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CryptDecrypt(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen);

        static bool CryptDecrypt_Hooked(IntPtr hKey, IntPtr hHash, int Final, uint dwFlags, [In] [Out] IntPtr pbData, [In] [Out] IntPtr pdwDataLen)
        {
            bool ret = false;
            Main This = (Main)HookRuntimeInfo.Callback;
            try
            {
                var size = Marshal.ReadInt32(pdwDataLen);
                byte[] cryptedData = new byte[size];
                bool doCopy = (size != 0 && pbData != IntPtr.Zero);

                if (doCopy)
                    Marshal.Copy(pbData, cryptedData, 0, size);

                ret = CryptDecrypt(hKey, hHash, Final, dwFlags, pbData, pdwDataLen);

                if (doCopy)
                {
                    size = Marshal.ReadInt32(pdwDataLen);
                    byte[] rawData = new byte[size];
                    Marshal.Copy(pbData, rawData, 0, size);

                    This.Interface.Enqueue(new Tuple<bool, byte[], byte[]>(false, rawData, cryptedData));
                }
            }
            catch (Exception ExtInfo)
            {
                This.Interface.ReportException(ExtInfo);
            }
            return ret;
        }
    }
}
