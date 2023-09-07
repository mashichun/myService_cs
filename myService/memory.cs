using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

class Program
{
    // Import the necessary Win32 APIs
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    public static void Main()
    {
        // Specify the starting physical address and size you want to map
        ulong physicalAddress = 0x10000000; // Example: 0x10000000 (268435456 in decimal)
        int sizeToMap = 4096; // Specify the size to map in bytes (e.g., 4096 bytes)

        // Open a handle to the physical memory
        IntPtr hPhysicalMemory = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, 0x10000, null); // 0x04 = PAGE_READWRITE

        // Map the physical memory into the process
        IntPtr pMemory = MapViewOfFile(hPhysicalMemory, 0x02 | 0x04, (uint)(physicalAddress >> 32), (uint)(physicalAddress & 0xFFFFFFFF), (uint)sizeToMap);

        if (pMemory != IntPtr.Zero)
        {
            try
            {
                // Read and write to the mapped physical memory
                byte valueToWrite = 0xAA;
                Marshal.WriteByte(pMemory, 0, valueToWrite);

                byte readValue = Marshal.ReadByte(pMemory, 0);
                Console.WriteLine("Read value: 0x" + readValue.ToString("X2"));
            }
            finally
            {
                // Unmap and close the handle to the physical memory
                UnmapViewOfFile(pMemory);
                CloseHandle(hPhysicalMemory);
            }
        }
        else
        {
            Console.WriteLine("Failed to map physical memory!");
        }
    }
}