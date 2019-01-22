using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalTestsWebsite.Infrastructure
{
    public class Signaler
    {
        private TaskCompletionSource<object> _tcs;

        public Signaler()
        {
            Reset();
        }

        public void Reset()
        {
            _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public void Set()
        {
            _tcs.TrySetResult(null);
        }

        public Task Task => _tcs.Task;
    }
}
