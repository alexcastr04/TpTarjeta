using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransporteUrbano.Tests
{
    [TestClass]       //TESTS_ITERACIÓN1
    public class TarjetaTest
    {
        [TestMethod]
        public void Recargar_RecargaValida_AumentaSaldo()
        {
            // Creamos una tarjeta con saldo inicial de 0
            Tarjeta tarjeta = new Tarjeta(0);

            // La recargamos con un monto válido (2000)
            tarjeta.Recargar(2000);

            // El saldo debe aumentar correctamente
            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Recargar_RecargaInvalida_LanzaExcepcion()
        {
            // Volvemos a crear una tarjeta con saldo inicial de 0
            Tarjeta tarjeta = new Tarjeta(0);

            // Probamos recargar con un monto no válido (por ejemplo 1500)
            tarjeta.Recargar(1500);

            // No escribimos la función 'Assert' ya que con el atributo '[ExpectedException(typeof(ArgumentException))]'
            // indicamos que la prueba pasará solo si se lanza una excepción del tipo ArgumentException. 
            // 
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Recargar_ExcedeSaldoMaximo_LanzaExcepcion()
        {
            // Creamos una tarjeta con saldo inicial cercano al máximo permitido (9000)
            Tarjeta tarjeta = new Tarjeta(9000);

            // Intentamos recargar con un monto que excedería el saldo máximo permitido
            tarjeta.Recargar(1000);

            // No escribimos la función 'Assert' ya que usamos el atributo '[ExpectedException(typeof(InvalidOperationException))]' el cual
            // indica que la prueba pasará solo si se lanza una excepción del tipo InvalidOperationException. 
            //        
        }

        [TestMethod]
        public void Recargar_CubreDeudaPlus_SaldoRestanteCorrecto()
        {
            // Creamos una tarjeta con deuda de 500 y la recargarmos
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 500;

            // Recargarmos con un monto suficiente para cubrir la deuda (ejemplo: 2000)
            tarjeta.Recargar(2000);

            // El saldo restante debería ser 1500 (2000 - 500), entonces hacemos la comparación 
            Assert.AreEqual(1500, tarjeta.Saldo);
            // La deuda debe ser eliminada
            Assert.AreEqual(0, tarjeta.DeudaPlus);
        }

        [TestMethod]
        public void Recargar_CubreParcialmenteDeudaPlus_DeudaActualizadaCorrectamente()
        {
            // Arrange: Se crea una tarjeta con deuda de 500 y la recaragamos parcialmente
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 500;

            // Recargamos con un monto insuficiente para poder cubrir la deuda (ejemplo: 300)
            tarjeta.Recargar(300);

            // Assert: La deuda debe disminuir a 200 y el saldo debe seguir en 0
            Assert.AreEqual(0, tarjeta.Saldo);
            Assert.AreEqual(200, tarjeta.DeudaPlus);
        }
    }

    [TestClass]       //TESTS_ITERACIÓN2
    public class TarjetaDescuentoSaldoTest
    {
        private const decimal Tarifa = 940; // Tarifa de un viaje

        [TestMethod]
        public void PagarConSaldo_SaldoDescontadoCorrectamente()
        {
            // Creamos una tarjeta con saldo suficiente para un viaje
            Tarjeta tarjeta = new Tarjeta(2000);

            // Realizamos el pago del boleto
            tarjeta.DescontarSaldo(Tarifa);

            // El saldo debería disminuir en el valor del viaje (2000 - 940 = 1060)
            Assert.AreEqual(1060, tarjeta.Saldo);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PagarSinSaldo_LanzaExcepcion()
        {
            // Creamos una tarjeta con saldo insuficiente
            Tarjeta tarjeta = new Tarjeta(0);

            // Se intenta realizar el pago del boleto sin saldo
            tarjeta.DescontarSaldo(Tarifa);

            // Se espera que lance una excepción de InvalidOperationException
        }
    }

    [TestClass]
    public class TarjetaSaldoNegativoTest
    {
        private const decimal Tarifa = 940; // Tarifa viaje
        private const decimal SaldoNegativoMaximo = 480; // Máximo permitido en saldo negativo

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Pagar_ExcedeSaldoNegativoMaximo_LanzaExcepcion()
        {
            // Creamos una tarjeta con saldo bajo (ejemplo 200) para simular que quedará con saldo negativo
            Tarjeta tarjeta = new Tarjeta(200);

            // Se realizará un viaje que exceda el saldo negativo 480 (tarifa viaje: 940, saldo disponible 200, saldo resultante negativo -740)
            tarjeta.DescontarSaldo(Tarifa); // 

            // Se espera que lance una excepción de InvalidOperationException ya que el saldo negativo excede el límite (-480)
        }

        [TestMethod]
        public void Recargar_ConSaldoNegativo_DescuentaDeudaCorrectamente()
        {
            // Creamos una tarjeta con saldo inicial de 0 y se simula un viaje que genera saldo negativo
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 480; // Simulamos un saldo negativo máximo de 480

            // Recargarmos la tarjeta con un monto mayor a la deuda (ejemplo: 2000)
            tarjeta.Recargar(2000);

            // El saldo final debe ser la recarga menos la deuda, es decir 1520 (2000 - 480)
            Assert.AreEqual(1520, tarjeta.Saldo);
            // La deuda debe haber sido eliminada
            Assert.AreEqual(0, tarjeta.DeudaPlus);
        }
    }

    [TestClass]
    public class TarjetaFranquiciaTest
    {
        private const decimal TarifaNormal = 940; // Tarifa de un viaje común
        private const decimal TarifaMedioBoleto = TarifaNormal / 2; // Tarifa con medio boleto

        [TestMethod]
        public void PagarConFranquiciaCompleta_SiemprePuedePagar()
        {
            // Creamos una tarjeta con franquicia completa sin importar el saldo
            TarjetaCompleta tarjetaFranquiciaCompleta = new TarjetaCompleta(0);

            // Se intenta realizar un pago de boleto con franquicia completa
            tarjetaFranquiciaCompleta.DescontarSaldo(TarifaNormal);

            // El saldo de la tarjeta debe seguir siendo 0 ya que la franquicia completa no paga nada
            Assert.AreEqual(0, tarjetaFranquiciaCompleta.Saldo);
        }

        [TestMethod]
        public void PagarConMedioBoleto_DescuentoCorrecto()
        {
            // Creamos una tarjeta con medio boleto con saldo suficiente para el viaje
            MedioBoleto tarjetaMedioBoleto = new MedioBoleto(2000);

            // Se realiza el pago de boleto con medio boleto
            tarjetaMedioBoleto.DescontarSaldo(TarifaNormal);

            // El saldo de la tarjeta se debe haber disminuido en solo la mitad del costo común (470)
            Assert.AreEqual(2000 - TarifaMedioBoleto, tarjetaMedioBoleto.Saldo);
        }
    }
}
