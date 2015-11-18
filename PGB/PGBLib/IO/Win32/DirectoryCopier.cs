﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace PGBLib.IO.Win32
{
    class DirectoryCloner
    {
        // Kernal32 Calls
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CreateDirectoryEx(string lpTemplateDirectory, string lpNewDirectory, IntPtr lpSecurityAttributes);

        internal static void CloneDirectory(string template, string target)
        {
            if (!CreateDirectoryEx(template, target, IntPtr.Zero)) {
                HandleCreateDirectoryExError(template, target, Marshal.GetLastWin32Error());
            }
        }

        private static void HandleCreateDirectoryExError(string template, string target, int errorCode)
        {
            Win32Exception win32Exception = new Win32Exception(errorCode);
            Exception error;
            switch ((Win32Error)errorCode)
            {
                case Win32Error.ERROR_ACCESS_DENIED:
                    error = new UnauthorizedAccessException(
                        string.Format("Access was denied to clone '{0}' to '{1}'.", template, target),
                        win32Exception);
                    break;
                case Win32Error.ERROR_PATH_NOT_FOUND:
                    error = new DirectoryNotFoundException(
                        string.Format("The path '{0}' or '{1}' could not be found.", template, target),
                        win32Exception);
                    break;
                case Win32Error.ERROR_INVALID_DRIVE:
                    error = new DriveNotFoundException(
                        string.Format("The source or destination drive was not found when cloning '{0}' to '{1}'.", template, target),
                        win32Exception);
                    break;
                case Win32Error.ERROR_SHARING_VIOLATION:
                    error = new SharingViolationException(
                        string.Format("The source or destination file was in use when copying '{0}' to '{1}'.", template, target),
                        win32Exception);
                    break;
                case Win32Error.ERROR_ALREADY_EXISTS:
                    error = new DirectoryAlreadyExistsException(
                        string.Format("The directory '{0}' could not be cloned to '{1}' because the target directory already exists.", template, target),
                        win32Exception);
                    break;
                default:
                    error = win32Exception;
                    break;
            }
            throw error;
        }
    }
}