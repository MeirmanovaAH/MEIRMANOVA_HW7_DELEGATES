using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace MEIRMANOVA_HW7_DELEGATES
{
    class Program
    {
        static void Main()
        {

            string toDirectory = Path.Combine(Directory.GetCurrentDirectory(), Configuration.GetDirectory());
            Console.WriteLine($"toDirectory = {toDirectory}");
            List<string> fileList = new() { "Паспорт.jpg", "Заявление.txt", "Фото.jpg" };
            int waitingInterval = Configuration.GetWaitingInterval();
            Console.WriteLine($"watiniginterval = {waitingInterval}");
            if (!Directory.Exists(toDirectory))
                Directory.CreateDirectory(toDirectory);

            using GetDocs getdocs = new();
            getdocs.DocumentsReady += DocumentsReadyEvent;
            getdocs.TimedOut += TimedOutEvent;

            Console.WriteLine("Для создания документов введите Y");
            var auto = Console.ReadLine();

            bool isStarted = getdocs.Start(toDirectory, waitingInterval, fileList);

            if (!isStarted)
            {
                Console.WriteLine("Прием документов не был запущен!");
                return;
            }

            if (auto == "Y")
            {
                Thread.Sleep(waitingInterval / 2);
                CreateDocs(toDirectory, fileList);
            }
            Console.ReadLine();
        }

        private static void DocumentsReadyEvent()
        {
            Console.WriteLine("Документы загружены, нажмите любую клавишу...");
        }

        private static void TimedOutEvent()
        {
            Console.WriteLine("Превышено время ожидания документов. Попробуйте загрузить документы заново");
        }

        private static void CreateDocs(string path, List<string> fileList)
        {
            foreach (string fileName in fileList)
            {
                File.Delete(Path.Combine(path, fileName));
                File.Create(Path.Combine(path, fileName));
            }
        }


    }
}
