using System;


namespace bst
{
    public class Node<TK, TV> where TK : IComparable<TK> {
        
        public TK Key;
        public TV Value;
        public Node<TK, TV> Parent;
        public Node<TK, TV> Left;
        public Node<TK, TV> Right;

        public Node(TK key, TV value, Node<TK, TV> parent) {
            Key = key;
            Value = value;
            Parent = parent;
        }
    }
}