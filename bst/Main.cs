using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace bst
{
    class Program
    {   
        static void Main()
        {
            int min = 0;
            int max = 1000000;

            var keysToInsert = new int[1000000];
            var keysToDelete = new int[100000];
            var keysToFind = new int[1000000];

            var randNum = new Random();
            
            for (int i = 0; i < keysToInsert.Length; i++) //Генерируем случайные числа для вставки, удаления и поиска
            {
                keysToInsert[i] = randNum.Next(min, max);
            }
            
            for (int i = 0; i < keysToDelete.Length; i++)
            {
                int j = randNum.Next(min, max);
                keysToDelete[i] = keysToInsert[j];
            }
            
            for (int i = 0; i < keysToFind.Length; i++)
            {
                int j = randNum.Next(min, max);
                keysToFind[i] = keysToInsert[j];
            }                                             //Сгенерировали.

            var tree = new BinarySearchTree<int, char>(); //Дерево для последовательных операций.
            var pTree = new BinarySearchTree<int, char>();//Дерево для параллельных операций.
            
            var time1 = Stopwatch.StartNew();
           
            foreach (int key in keysToInsert) tree.Insert(key, 'a');
            
             time1.Stop();

            Console.WriteLine("Обычная вставка: {0}", time1.Elapsed);
            
            
            
            var time2 = Stopwatch.StartNew();
            
            foreach (int key in keysToDelete) tree.Delete(key);
            
            time2.Stop();

            Console.WriteLine("Обычное удаление: {0}", time2.Elapsed);

                
            
            
            var time3 = Stopwatch.StartNew();
            
            foreach (int key in keysToFind)  tree.Find(key);
            

            time3.Stop();

            Console.WriteLine("обычный поиск: {0}", time3.Elapsed);

            
            
                   
            var time4 = Stopwatch.StartNew();

            Parallel.ForEach (keysToInsert, key => { pTree.Insert(key, 'x');});
            
            time4.Stop();

            Console.WriteLine("Параллельная вставка: {0}", time4.Elapsed);
            
            
            
            
            var time5 = Stopwatch.StartNew();
            
            Parallel.ForEach (keysToDelete, key => { pTree.Delete(key);});
            
            time5.Stop();

            Console.WriteLine("Параллельное удаление: {0}", time5.Elapsed);

            
            
            
            var time6 = Stopwatch.StartNew();
            
            Parallel.ForEach (keysToFind, key => {pTree.Find(key);});
            
            time6.Stop();

            Console.WriteLine("Параллельный поиск: {0}", time6.Elapsed);
            

        }
    }
}