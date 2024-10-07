namespace PruebaTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string result = PruebaTesting.Program.Something();
            Assert.AreEqual("Algo", result);

        }
    }
}