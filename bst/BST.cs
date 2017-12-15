
using System;

namespace bst{
    
    public class BinarySearchTree<TK, TV> where TK : IComparable<TK> {
        
            object locker1 = new object();
            object locker2 = new object();
        
        public Node<TK, TV> Root;//Создаем корень.

        public void Insert(TK key, TV value) {//Вставка.
            

            
            Node<TK, TV> par = null;

            var current = Root;

            while (current != null) {//Ищем куда вставить.
                
                if (par == null) {//Если у текущего листа нет родителя(то родителя и не надо лочить).
                    
                    lock (locker1) {//Идем ниже.
                        
                        par = current;
                        
                        if (key.CompareTo(current.Key) < 0) current = current.Left;
                        
                        else if (key.CompareTo(current.Key) > 0) current = current.Right;
                        
                        else if (key.CompareTo(current.Key) == 0) {
                            
                            current.Value = value;
                            return;
                        }
                    }
                } else {//Если родитель есть(следовательно требукется его лок).
                    
                    lock (locker2) {//Идем ниже.
                        
                        lock (locker1) {
                            
                            par = current;
                            if (key.CompareTo(current.Key) < 0) current = current.Left;
                            else if (key.CompareTo(current.Key) > 0) current = current.Right;
                            else if (key.CompareTo(current.Key) == 0) {
                                
                                current.Value = value;
                                return;
                            }
                        }
                    }
                }
            }//Нашли место куда вставить.
            
            if (par == null) {//Нет родителя у найденного места(лист - корень).
                
                Root = new Node<TK, TV>(key, value, null);
                return;
            }
            
            lock (locker2){ //Когда есть родитель то добавляем лист на найденное место.
           
                if (key.CompareTo(par.Key) < 0) par.Left = new Node<TK, TV>(key, value, par);
                
                else par.Right = new Node<TK, TV>(key, value, par);
                
            }
        }

        public bool Find(TK key)
        {
            var result = FindNode(key);

            if (result == null) return false;

            return true;
        }

        private Node<TK, TV> FindNode(TK key) {//Поиск.
          
            
            var current = Root;

            while (current != null) {//Ищем пока не наткнемся на пустой лист(в таком случае нужного нет в дереве).
                
                if (current.Parent == null) {//Если нет родителя у текущего листа то его(родителя) не надо лочить.
                    
                    lock (locker1) {
                        
                        if (key.CompareTo(current.Key) == 0) return current;
                        if (key.CompareTo(current.Key) < 0) current = current.Left;
                        else if(key.CompareTo(current.Key)>0) current = current.Right;
                    }
                    
                } else {//Если родитель есть то требуется его лок.
                    
                    lock (locker2) {
                        
                        lock (locker1) {
                            
                            if (key.CompareTo(current.Key) == 0) return current;
                            if (key.CompareTo(current.Key) < 0) current = current.Left;
                            else if(key.CompareTo(current.Key) > 0) current = current.Right;
                        }
                    }
                }
            }

            return null;
        }

        public void Delete(TK key) {// Удаление.
            

            
            var node = FindNode(key);//Находим лист, который нужно удалить.

            if (node == null) return;

            if (node.Parent == null && node.Left == null && node.Right == null) {//Лист - корень.
                
                lock (locker1) {
                    
                    Root = null;
                    
                }
            }
            
            else if (node.Parent != null && node.Left == null && node.Right == null) { //Лист имеет только родителя.
                
                lock (locker2) {
                    
                    lock (locker1) {
                        
                        if (node == node.Parent.Left) node.Parent.Left = null;

                        if (node == node.Parent.Right) node.Parent.Right = null;
                        
                    }
                }
            }
            
            else if (node.Parent == null && node.Left == null && node.Right != null) {//Лист корень и имеет только правого ребенка.
            
                lock (locker1) {
                    
                    node.Right.Parent = null;
                    Root = node.Right;
                }
            }
            
            else if (node.Parent == null && node.Left != null && node.Right == null) {//Лист корень и имеет только левого ребенка.
            
                lock (locker1) {
                    
                    node.Left.Parent = null;
                    Root = node.Left;
                }
            }
            
            else if (node.Parent != null && (node.Left == null || node.Right == null)) {// Лист имеет родителя и только одного ребенка.
                
                lock (locker2) {
                    
                    lock (locker1) {
                        
                        if (node.Right != null) {//Если этот ребенок - правый.
                            
                            if (node.Parent.Left == node) {// Наш лист левый ребенок своего родителя.
                                node.Right.Parent = node.Parent;
                                node.Parent.Left = node.Right;
                            }
                            
                            else if (node.Parent.Right == node) {// Наш лист правый ребенок своего родителя.
                            
                                node.Right.Parent = node.Parent;
                                node.Parent.Right = node.Right;
                            }
                        } else if (node.Left != null) {//Если ребенок ношего листа левый.
                        
                            if (node.Parent.Left == node) {// Наш лист левый ребенок своего родителя.
                                node.Left.Parent = node.Parent;
                                node.Parent.Left = node.Left;
                            } else if (node.Parent.Right == node) {// Наш лист правый ребенок своего родителя.
                            
                                node.Left.Parent = node.Parent;
                                node.Parent.Right = node.Left;
                            }
                        }
                    }
                }
            } else {//Случаи исключающие предыдущие.
            
                if (node.Parent != null) {//При наличии родителя.
                    
                    lock (locker2) {
                        
                        lock (locker1) {
                            
                            var swaper = FindMin(node.Right);//  Ищем лист на замену нашего.
                            node.Key = swaper.Key;
                            node.Value = swaper.Value;
                            
                            if (swaper.Parent.Left == swaper) { 
                                
                                swaper.Parent.Left = swaper.Right;

                                if (swaper.Right != null) swaper.Right.Parent = swaper.Parent;
                                
                            } else {
                                
                                swaper.Parent.Right = swaper.Right;

                                if (swaper.Right != null) swaper.Right.Parent = swaper.Parent;
                            }
                        }
                    }
                }
                
                else if (node.Parent == null) {//При отстуствии родителя.
                    
                    lock (locker1) {
                        
                        var swaper = FindMin(node.Right);
                        node.Key = swaper.Key;
                        node.Value = swaper.Value;

                        if (swaper.Parent.Left == swaper) {
                            
                            swaper.Parent.Left = swaper.Right;

                            if (swaper.Right != null)
                                
                                swaper.Right.Parent = swaper.Parent;
                        } else {
                            
                            swaper.Parent.Right = swaper.Right;

                            if (swaper.Right != null) swaper.Right.Parent = swaper.Parent;
                        }
                    }
                }
            }
        }
        
        private Node<TK, TV> FindMin(Node<TK, TV> rootNode)//Для нахождения минимального ребенка при удалении.
        {
            return rootNode.Left == null ? rootNode : FindMin(rootNode.Left);
        }
         
    }
    
    
}
