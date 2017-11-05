using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Prime {
    
    internal class Program {
        
        public static void Main() {
            int start = 1;
            int end = 1000000;// установим сами начальное и конечное значения отрезка.
           
            Console.WriteLine("Thread");
            
            for (int i = 0; i < 8; i++) { // Подсчет разным количеством потоков( по степеням двойки ). 
            
                int threadNumber = (int) Math.Pow(2, i);
            
                var time1 = Stopwatch.StartNew(); // Подсчитываем время для всех количеств потоков.
                var primeThreads = Findthreads(start, end,threadNumber ); 
                time1.Stop();      

                Console.WriteLine("Time on {0} threads: {1}", threadNumber, time1.Elapsed);//Вывод только времени для сравнения эффективности способов.
            }

            Console.WriteLine("Tasks"); // Подсчет тасками(для двух).
            
            var time2 = Stopwatch.StartNew();
            var tasks = new Task[2];
            
            tasks[0] = Task.Factory.StartNew(() => PrimeInRange(start, end/2));
            tasks[1] = Task.Factory.StartNew(() => PrimeInRange(end/2 + 1, end));
            
            Task.WaitAll(tasks);
            time2.Stop();
           
            Console.WriteLine("Time on 2 tasks: {0}", time2.Elapsed);

            
            Console.WriteLine("Thread pool"); //Подсчет пулом потоков.
            
            var time3 = Stopwatch.StartNew();
            
            var myEvent = new ManualResetEvent(false);
            
            ThreadPool.QueueUserWorkItem(delegate{PrimeInRange(start, end);myEvent.Set();}); 
            myEvent.WaitOne();
            time3.Stop();
          
            Console.WriteLine("Time on thread pool: {0}", time3.Elapsed);
        }
        
        
        public static List<int> Findthreads(int start, int end, int threadNumber) {
            
            var thr = Thread.CurrentThread.ManagedThreadId; //Идентификатор(номер) текущего управляемого потока.
            var primes = new List<int>();

            if (thr < threadNumber) {// Создаем новые потоки, разбивая отрезок снова и снова пополам.
                
                int mid = (start + end) / 2;

                var first = new List<int>();
                var second = new List<int>();

                var thr1 = new Thread(() => { first.AddRange(Findthreads(start, mid, threadNumber)); });
                thr1.Start();

                var thr2 = new Thread(() => { second.AddRange(Findthreads(mid + 1, end, threadNumber)); });
                thr2.Start();

                thr1.Join();
                thr2.Join();

                primes.AddRange(first);
                primes.AddRange(second);
                
            }
            else {// Сам поиск простых чисел.
                for (int i = start; i <= end; i++) {
                    
                    if (IsPrime(i)) primes.Add(i);
                    
                }
            }

            return primes;
        }

       
        
        public static bool IsPrime(int num) {
            
            if (num == 1)  return false;
            
            for (int i = 2; i <= Math.Sqrt(num); i++) {
                
             if (num % i == 0)  return false;
                
            }

            return true;
        }

        private static List<int> PrimeInRange(int start, int end)
        {
            var result = new List<int>();

            for (int i = start; i < end; i++) {
                if (IsPrime(i))  result.Add(i);
                
            }

            return result;
        }

        
        
    }
}