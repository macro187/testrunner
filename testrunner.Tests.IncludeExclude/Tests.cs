using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A
{
    [TestClass] public class A
    {
        [TestMethod] public void a() { Console.WriteLine("A.A.a() is running"); }
        [TestMethod] public void b() { Console.WriteLine("A.A.b() is running"); }
    }
    [TestClass] public class B
    {
        [TestMethod] public void a() { Console.WriteLine("A.B.a() is running"); }
        [TestMethod] public void b() { Console.WriteLine("A.B.b() is running"); }
    }
}
namespace B
{
    [TestClass] public class A
    {
        [TestMethod] public void a() { Console.WriteLine("B.A.a() is running"); }
        [TestMethod] public void b() { Console.WriteLine("B.A.b() is running"); }
    }
    [TestClass] public class B
    {
        [TestMethod] public void a() { Console.WriteLine("B.B.a() is running"); }
        [TestMethod] public void b() { Console.WriteLine("B.B.b() is running"); }
    }
}
