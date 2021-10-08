using System;
using System.IO;

namespace DoublyLinkedListSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            ListRandom test = new();
            test.FillWithTestData();
            test.PrintListRandom();

            ListRandom newtest;
            MemoryStream f = new MemoryStream();

            test.Serialize(f);

            f.Position = 0;
            newtest = test.Deserialize(f);

            newtest.PrintListRandom();
        }
    }
}
