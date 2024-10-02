using TransporteUrbano;
namespace TpTarjetaTest
{
    public class TarjetaTest
    {
        Tarjeta tarjeta;
        [SetUp]
        public void Setup()
        {
            tarjeta = new Tarjeta(0);
        }

        [Test]
        [TestCase(2000)]
        [TestCase(3000)]
        [TestCase(4000)]
        [TestCase(5000)]
        public void CargaSaldo(decimal monto)
        {
            tarjeta.Recargar(monto);
            Assert.That(tarjeta.Saldo, Is.EqualTo(monto));
        } 
    }
}