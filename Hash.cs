using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Hash {
    
    internal class Program {
        
        public static void Main(string[] args) {
            
            Console.WriteLine("Путь к папке:");
            var path = Console.ReadLine();

            if (!Directory.Exists(path)) { // Проверка существования.
            
                Console.WriteLine("Ошибка, не существует.");
                return;
            }

            var time = Stopwatch.StartNew(); // Начинаем засекать время.
            var hash =FindHash(path);// Выполнение.
            time.Stop(); // Заканчиваем замерять время

            Console.WriteLine("Время затрачено: {0}", time.Elapsed);

            Console.WriteLine("Хэш указанной папки:");
            Console.WriteLine(BitConverter.ToString(hash).Replace("-", "").ToLower());
            
        }
        
        
        public static byte[] FindHash(string dirPath) {           
            
            var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);// Считываем пути до всех файлов в папке и сортируем.
            Array.Sort(files);

            var hashes = new List<byte[]>();
            var hash = MD5.Create();
            
            using (hash) { 
                
                var tasks = new List<Task<byte[]>>();

                foreach (var file in files) { // Для каждого файла в массиве со всеми файлами папки ищем свой хэш.
                    
                    var str = File.OpenRead(file);
                    var task = Task.Run(() => FindLocalHash(str)); 
                    tasks.Add(task);
                    
                }
                
                Task.WaitAll(tasks.ToArray());// Чтобы последующее объединение хэшэй было последовательным ,а не зависело от скорости выполнение тасков.

                foreach (var task in tasks) {
                    
                    hashes.Add((task.Result));
                }

                var array = hashes.SelectMany(a => a).ToArray();

                return hash.ComputeHash(array); //Получаем хэш папки, вычислив хэш функцию от объединенных хешей всех файлов в папке.
            }
        }
        
        
        public static byte[] FindLocalHash(FileStream stream) //Само вычисление хэша, отдельно по причине того ,что имеются какие-то проблема с сочетанием с параллельностью(???)
        {
            
            using (var hash = MD5.Create())  return hash.ComputeHash(stream);
                
        }


      
    }
}