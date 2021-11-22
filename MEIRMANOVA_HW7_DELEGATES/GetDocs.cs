using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
namespace MEIRMANOVA_HW7_DELEGATES

{
    public delegate void FilesLoadedEventHandler();
    public delegate void TimedOutEventHandler();

    public class GetDocs : IDisposable
    {
        public event FilesLoadedEventHandler DocumentsReady;
        public event TimedOutEventHandler TimedOut;

        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private Timer _timer = new Timer();
        private Dictionary<string, bool> _filesDictionary = new Dictionary<string, bool>();

        public bool Start(string targetDirectory, int waitingInterval, List<string> fileList)
        {
            if (Directory.Exists(targetDirectory))
                _watcher.Path = targetDirectory;
            else
            {
                Console.WriteLine($"Директория {targetDirectory} не существует!");
                return false;
            }
            if (fileList.Count != 0)
                fileList.ForEach(f => _filesDictionary.Add(f, false));
            else
            {
                Console.WriteLine("Пустой список файлов!");
                return false;
            }
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Error += OnError;
            _watcher.EnableRaisingEvents = true;

            _timer.Interval = waitingInterval;

            _timer.Start();
            _timer.Elapsed += OnTimedEvent;

            return true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            TimedOut?.Invoke();
            UnsetSubscribe();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            AddFilesFromFolder(e.Name);

            if (IsAllFiles())
            {
                DocumentsReady?.Invoke();
                UnsetSubscribe();
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (_filesDictionary.Any(a => a.Key == e.Name))
                _filesDictionary[e.Name] = false;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            PrintException(e.GetException());
        }

        private void AddFilesFromFolder(string fileName)
        {
            if (_filesDictionary.Any(a => a.Key == fileName))
                _filesDictionary[fileName] = true;
        }

        private bool IsAllFiles()
        {
            return _filesDictionary.All(a => a.Value);
        }

        private void PrintException(Exception e)
        {
            Console.WriteLine($"Message: {e.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(e.StackTrace);
            Console.WriteLine();
        }

        private void UnsetSubscribe()
        {
            _timer.Elapsed -= OnTimedEvent;
            _watcher.Created -= OnCreated;
            _watcher.Deleted -= OnDeleted;

            Dispose();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _watcher.Dispose();
        }
    }
}