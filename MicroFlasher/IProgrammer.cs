using System;

namespace MicroFlasher {
    public interface IProgrammer : IDisposable {

        void Start();

        void Stop();

        void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength);

        void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength);

        void EraseDevice();

    }
}
