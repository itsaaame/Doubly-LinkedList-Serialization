using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DoublyLinkedListSerializer
{
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }


    class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public ListRandom()
        {   
            // Constructor for the purpose of testing
            Head = new ListNode();
            Tail = Head;
            Count = 1;
        }

        public void Append(string Data)
        {
            // Method to add new Data into ListRandom
            ListNode newListNode = new ListNode();
            newListNode.Data = Data;
            newListNode.Previous = Tail;

            Tail.Next = newListNode;
            Tail = newListNode;

            Count++;
        }

        public void FillWithTestData()
        {
            // Fill linkedlist with data for testing
            this.Head.Data = "1";
            this.Append("2");
            this.Append("1");
            this.Append("1");
            this.Append("1");
            this.Append("1");
            this.Append("1");
            this.Append("4");
            this.Append("3");
            this.AssignRandom();
        }
        public void AssignRandom()
        {
            // Filling in random property for the tested LinkedList
            ListNode Current = Head;
            int[] randoms = new int[9] { 1, 4, 0, 0, 0, 0, -5, -3, 0 };
            int randind = 0;
            while (Current.Next != null)
            {
                Current.Random = Randomize(randoms[randind]);

                randind++;
                Current = Current.Next;
            }
        }
        private ListNode Randomize(int position)
        {
            // Helper method for assigning Random propery to nodes
            int counter = 0;
            if (position > 0)
            {
                ListNode Current = Head;
                while (counter < position)
                {
                    Current = Current.Next;
                    counter++;
                }
                return Current;
            }
            else if (position < 0)
            {
                ListNode Current = Tail;
                while (counter > position)
                {
                    Current = Current.Previous;
                    counter--;
                }
                return Current;
            }
            else
            {
                return null;
            }
        }

        protected int GetRandomNodePosition(ListNode RandomNode)
        {
            // Helper method to calculate and safe position of the random node
            int position = 0;
            ListNode Current = Head;
            while (Current != null)
            {
                if (Current == RandomNode)
                {
                    return position;
                }
                Current = Current.Next;
                position++;
            }
            return 0;
        }
        public void Serialize(Stream s)
        {
            // Method to serialize ListRandom into stream
            // The order of writing is following:
            // 1. Write the count of the linked list
            // 2. Write length of data of the node (byte length)
            // 3. Write data of the node
            // 4. Write position of the node from Random propery
            // 5. Continue writing remaining nodes into stream accodring to 2-5 steps order 

            byte[] WriteData;
            int DataLength;

            ListNode Current = Head;

            s.Write(BitConverter.GetBytes(Count));

            while (Current != null)
            {
                WriteData = Encoding.Default.GetBytes(Current.Data);
                DataLength = WriteData.Length;

                s.Write(BitConverter.GetBytes(DataLength));
                s.Write(WriteData);
                s.Write(BitConverter.GetBytes(GetRandomNodePosition(Current.Random)));

                Current = Current.Next;
            }

        }

        public ListRandom Deserialize(Stream s)
        {
            // Method to deserialize ListRandom from stream
            // 1. We create a new ListRandom and fill it with data
            //      (we safe positions of the Random nodes into queue) 
            // 2. We go through the ListRandom again and assign Random property
            //      according to queue order
            
            // return type is not part of the method signature

            ListRandom newListRandom = new();
            Queue<int> QueueRandom = new();

            byte[] ReadData;

            int ListLength = ReadIntFromStreamHelper(s);
            ReadData = new byte[ReadIntFromStreamHelper(s)];

            s.Read(ReadData, 0, ReadData.Length);
            newListRandom.Head.Data = Encoding.Default.GetString(ReadData);

            QueueRandom.Enqueue(ReadIntFromStreamHelper(s));
            ListLength -= 1;

            while (ListLength > 0)
            {
                ReadData = new byte[ReadIntFromStreamHelper(s)];

                s.Read(ReadData, 0, ReadData.Length);
                newListRandom.Append(Encoding.Default.GetString(ReadData));

                QueueRandom.Enqueue(ReadIntFromStreamHelper(s));
                ListLength--;
            }

            ListNode Current = newListRandom.Head;

            while (QueueRandom.Count > 0)
            {
                Current.Random = DeserializeRandomHelper(newListRandom, QueueRandom.Dequeue());
                Current = Current.Next;
            }

            return newListRandom;
        }

        private int ReadIntFromStreamHelper(Stream s)
        {
            //Helper method for reading bytes of integer type from stream
            byte[] ByteInt = new byte[4];
            s.Read(ByteInt, 0, ByteInt.Length);
            return BitConverter.ToInt32(ByteInt);
        }
        private ListNode DeserializeRandomHelper(ListRandom newListRandom, int position)
        {
            //Helper method for assigning Random property
            int counter = 0;
            ListNode Current = newListRandom.Head;
            while (counter < position)
            {
                Current = Current.Next;
                counter++;
            }
            return Current;
        }

        public void PrintListRandom()
        {
            ListNode Current = Head;
            while (Current != null)
            {
                Console.Write($"Node ({Current.Data};{GetRandomNodePosition(Current.Random)}) -> ");
                Current = Current.Next;
            }
            Console.Write("NULL\n");
        }
    }

}
