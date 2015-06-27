using System;

namespace MicroFlasher {
    public class ProgrammingSession : IDisposable {

        private readonly IProgrammer _programmer;

        public ProgrammingSession(IProgrammer programmer) {
            _programmer = programmer;
        }

        public void Dispose() {
            _programmer.Stop();
        }
    }
}
