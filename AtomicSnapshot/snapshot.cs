using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AtomicSnapshot
{
    public class SingleWriterMultiReader{
        
        private readonly int registersCount;

        private bool[,] q;
        private Register[] registers;
        private Stopwatch timer = new Stopwatch();
        private Dictionary<TimeSpan, int>[] logWrite;
        private Dictionary<TimeSpan, int[]> logRead = new Dictionary<TimeSpan, int[]>();
        
        public SingleWriterMultiReader(int regcount) {
            
            registersCount = regcount;
            registers = new Register[registersCount];
            q = new bool[registersCount, registersCount];
            logWrite = new Dictionary<TimeSpan, int>[registersCount];
            
            for (var i = 0; i < registersCount; i++) {
                
                registers[i] = new Register(regcount);
                logWrite[i] = new Dictionary<TimeSpan, int>();    
            }           
            timer.Start();                                      // Initializing registers.
            
        }
        
       
        
        public void Update(int id, int value) {// Updates the registers with the data-value, "write-state" of all registers, invert the toggle bit and the embedded scan.
            
            var f = new bool[registersCount];
            for (var j = 0; j < registersCount; j++) {
                
                f[j] = !q[j, id];
            }

            var snapshot = Scan(id, false);
            registers[id].snapshot = snapshot;
            
            registers[id].value = value;
            registers[id].handshakes = f;
            registers[id].toggle = !registers[id].toggle;
            
            logWrite[id].Add(timer.Elapsed, value);
        }
        
        
        public int[] Scan(int id, bool flag = true)  { // Returns a consistent view of the memory.
            
            var moved = new int[registersCount]; 

            while (true) {
            
                for (var j = 0; j < registersCount; j++) {
                    
                    q[id, j] = registers[j].handshakes[id];
                }
                
                
                var a = (Register[]) registers.Clone(); 
                var b = (Register[]) registers.Clone();

                var foreachCond = true;
                
                for (var j = 0; j < registersCount; j++) {
                   
                    if (a[j].handshakes[id] != q[id, j] ||  b[j].handshakes[id] != q[id, j] ||a[j].toggle != b[j].toggle)  { //  Process j performed an update.    
                        
                        if (moved[j] == 1) {
                            
                            if (flag) logRead.Add(timer.Elapsed, b[j].snapshot);
                            return b[j].snapshot;
                        }

                        foreachCond = false;
                        moved[j]++;
                    }
                   
                }

                if (foreachCond) {
                    
                    var snapshot = new int[registersCount];
                    
                    for (var j = 0; j < registersCount; j++) {
                        
                        snapshot[j] = b[j].value;
                    }

                    if (flag) logRead.Add(timer.Elapsed, snapshot);
                    
                    return snapshot;
                }
            }
        }
        
        public void Print() {
            
            for (var i = 0; i < registersCount; i++) {
                
                Console.WriteLine("register #{0} and his log:", i);
                
                Console.WriteLine("({0}, [{1}], {2}, [{3}])", registers[i].value, string.Join(",", registers[i].handshakes), 
                    registers[i].toggle, string.Join(",", registers[i].snapshot));
                
                foreach (var change in logWrite[i]) {
                    
                    Console.WriteLine(change);
                }
                
                Console.WriteLine("----------------------------");
            }
            Console.WriteLine("read-log:");
            foreach (var scan in logRead) {
                
                Console.Write("< values = (" + string.Join(", ", scan.Value));
                Console.WriteLine("), time = {0} >", scan.Key);
            }
            Console.WriteLine("********************************");   
        }
        
    }
    
     public class Register {
        
        public int value;
        public bool[] handshakes;
        public bool toggle;
        public int[] snapshot;

        public Register(int regcount) {
            
            handshakes = new bool[regcount];
            snapshot = new int[regcount];
        }
    }
    
}