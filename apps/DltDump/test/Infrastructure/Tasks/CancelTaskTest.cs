namespace RJCP.App.DltDump.Infrastructure.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class CancelTaskTest
    {
        [Test]
        [CancelAfter(5000)]
        public async Task CancelStartedTask()
        {
            ManualResetEvent started = new(false);
            CancelTask task = new((t) => {
                Console.WriteLine("Starting...");
                started.Set();
                bool first = true;
                while (true) {
                    if (first) {
                        Console.WriteLine("Started...");
                        first = false;
                    }
                    if (t.WaitHandle.WaitOne(1000)) return;
                    Console.WriteLine("Wait Loop...");
                }
            });

            _ = task.Run();
            Assert.That(started.WaitOne(2000), Is.True);

            Thread.Sleep(50);
            await task.Cancel();
        }

        [Test]
        [CancelAfter(2000)]
        public async Task CancelNotStartedTask()
        {
            CancelTask task = new((t) => {
                bool first = true;
                while (true) {
                    if (first) {
                        Console.WriteLine("Started...");
                        first = false;
                    }
                    if (t.WaitHandle.WaitOne(100)) return;
                    Console.WriteLine("Wait Loop...");
                }
            });

            await task.Cancel();
        }

        [Test]
        [CancelAfter(2000)]
        public async Task CancelNotStartedTaskRun()
        {
            CancelTask task = new((t) => {
                bool first = true;
                while (true) {
                    if (first) {
                        Console.WriteLine("Started...");
                        first = false;
                    }
                    if (t.WaitHandle.WaitOne(100)) return;
                    Console.WriteLine("Wait Loop...");
                }
            });

            await task.Cancel();
            await task.Run();
        }

        [Test]
        public void NullAction()
        {
            Assert.That(() => {
                _ = new CancelTask(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task RunTwice()
        {
            ManualResetEvent started = new(false);
            CancelTask task = new((t) => {
                Console.WriteLine("Starting...");
                started.Set();
                bool first = true;
                while (true) {
                    if (first) {
                        Console.WriteLine("Started...");
                        first = false;
                    }
                    if (t.WaitHandle.WaitOne(1000)) return;
                    Console.WriteLine("Wait Loop...");
                }
            });

            _ = task.Run();
            Assert.That(started.WaitOne(2000), Is.True);

            Thread.Sleep(50);
            Assert.That(() => {
                _ = task.Run();
            }, Throws.TypeOf<InvalidOperationException>());

            await task.Cancel();
        }
    }
}
