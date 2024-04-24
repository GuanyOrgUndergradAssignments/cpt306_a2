using System.Diagnostics;
using System.Runtime.InteropServices;

using System;

/// <summary>
/// Contains all utility functions globally available.
/// </summary>
public static class Utility
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-exitprocess
    /// </summary>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern void ExitProcess(UInt32 uExitCode);

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-messagebox
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern void MessageBox(IntPtr hWnd, string text, string title, UInt32 uType);

    /// <summary>
    /// Don't know who the fuck decided that Unity should catch and ignore all assertions and exceptions.
    /// I won't allow that to happen.
    /// </summary>
    public static void MyDebugAssert(bool condition, String msg = "")
    {
        if (!condition)
        {
            Debugger.Break();
            Debugger.Log(0, "Assertion", "Debug Assertion Failed!\n" + msg);
            MessageBox
            (
                IntPtr.Zero, // no parent window
                "Message: " + msg, 
                "Debug Assertion Failed!", 
                0x00000000 | 0x00000010 // MB_OK | MB_ICONERROR
            );
            //ExitProcess(1);
        }
    }
}
