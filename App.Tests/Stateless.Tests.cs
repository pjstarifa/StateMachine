using System;
using Stateless;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.ServiceInterface;

namespace App.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Open_Reassign_and_Close_Bug()
        {
            var bug = new Bug("Incorrect stock count");
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());

            bug.Assign("Joe");
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());

            bug.Defer();
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());

            var state = bug.CurrentState();
            

            bug.Assign("Harry");
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());

            bug.Assign("Fred");
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());

            bug.Close();
            Console.WriteLine("{0} {1}\n", bug.CurrentState(), bug.Assignee());           
        }
    }
}
