using System;
using System.IO;
using Shouldly;

namespace Tarro.Test
{
    public class AppWatcherTests : IDisposable
    {
        private readonly AppWatcher watcher;
        private readonly string subdirectory;
        public AppWatcherTests()
        {
            subdirectory = Guid.NewGuid().ToString();
            Directory.CreateDirectory(FullPath);
            watcher = new AppWatcher(FullPath, 0.01);

        }

        private string FullPath => Path.Combine(Environment.CurrentDirectory, subdirectory);

        public void WatcherRaisesWhenFileIsCreated()
        {
            var called = false;
            watcher.AppChanged += (sender, args) => called = true;
            File.Create(Path.Combine(FullPath, "created.txt")).Close();
            Should.CompleteIn(() =>
            {
                while (called == false)
                {
                }
            }, TimeSpan.FromMilliseconds(40));
            called.ShouldBe(true);
        }

        public void WatcherRaisesWhenFileIsDeleted()
        {
            var called = false;
            File.Create(Path.Combine(FullPath, "deleteme.txt")).Close();
            watcher.AppChanged += (sender, args) => called = true;
            File.Delete(Path.Combine(FullPath, "deleteme.txt"));
            Should.CompleteIn(() =>
            {
                while (called == false)
                {
                }
            }, TimeSpan.FromMilliseconds(40));

        }

        public void WatcherRaisesWhenFileIsRenamed()
        {
            var called = false;
            File.Create(Path.Combine(FullPath, "before.txt")).Close();
            watcher.AppChanged += (sender, args) => called = true;
            File.Move(Path.Combine(FullPath, "before.txt"), Path.Combine(FullPath, "after.txt"));
            Should.CompleteIn(() =>
            {
                while (called == false)
                {
                }
            }, TimeSpan.FromMilliseconds(40));
        }
      

        public void WatcherRaisesTimeout()
        {
            var called = false;
            watcher.AfterQuietPeriod += (o, args) => called = true;
            File.Create(Path.Combine(FullPath, "created.txt")).Close();
            Should.CompleteIn(() =>
            {
                while (called == false)
                {
                }
            }, TimeSpan.FromMilliseconds(40));
            called.ShouldBe(true);
        }

        public void AdditionalChangesPushesTimeout()
        {
            var called = false;
            watcher.AfterQuietPeriod += (o, args) => called = true;
            File.Create(Path.Combine(FullPath, "1.txt")).Close();
            File.Create(Path.Combine(FullPath, "2.txt")).Close();
            File.Create(Path.Combine(FullPath, "3.txt")).Close();
            File.Create(Path.Combine(FullPath, "4.txt")).Close();
            File.Create(Path.Combine(FullPath, "5.txt")).Close();
            File.Create(Path.Combine(FullPath, "6.txt")).Close();
            called.ShouldBe(false);
            Should.CompleteIn(() =>
            {
                while (called == false)
                {
                }
            }, TimeSpan.FromMilliseconds(10));
            called.ShouldBe(true);
        }


        public void Dispose()
        {
            Directory.Delete(FullPath, true);

        }
    }
}
