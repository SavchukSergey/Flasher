using System;

namespace MicroFlasher {
    public interface IProgrammer : IDisposable {

        ProgrammingSession Start();

        void Stop();

        void ReadPage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength);

        void WritePage(int address, AvrMemoryType memType, byte[] data, int dataStart, int dataLength);

        void EraseDevice();

    }
}
