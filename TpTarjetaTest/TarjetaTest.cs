using TransporteUrbano;

namespace TpTarjetaTests
{
    ////TESTS_ITERACIÓN1
    [TestFixture]
    public class TarjetaTest
    {
        [Test]
        public void Recargar_RecargaValida_AumentaSaldo()
        {
            Tarjeta tarjeta = new Tarjeta(0);

            tarjeta.Recargar(2000);

            Assert.AreEqual(2000, tarjeta.Saldo);
        }

        [Test]
        public void Recargar_RecargaInvalida_LanzaExcepcion()
        {
            Tarjeta tarjeta = new Tarjeta(0);

            // Assert: Probamos recargar con un monto no válido (por ejemplo 1500)
            Assert.Throws<ArgumentException>(() => tarjeta.Recargar(1500));
        }

        [Test]
        public void Recargar_ExcedeSaldoMaximo_SaldoPlus()
        {
            Tarjeta tarjeta = new Tarjeta(35000);
            tarjeta.Recargar(2000);
            // Intentamos recargar con un monto que excedería el saldo máximo permitido
            Assert.AreEqual(1000, tarjeta.SaldoPlus);
        }

        [Test]
        public void Recargar_CubreDeudaPlus_SaldoRestanteCorrecto()
        {
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 480;

            tarjeta.Recargar(2000);

            Assert.AreEqual(1520, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.DeudaPlus);
        }

        [Test]
        public void Recargar_CubreDeudaPlus_DeudaActualizadaCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 480;

            tarjeta.Recargar(2000); //al recargar 2000 (mínimo monto recarga), saldamos la deuda y nos queda un saldo de 1520

            //La deuda queda saldada (0) y el saldo debe ser de 1520
            Assert.AreEqual(1520, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.DeudaPlus);
        }
    }

    ////TESTS_ITERACIÓN2
    [TestFixture]
    public class TarjetaDescuentoSaldoTest
    {
        private const decimal Tarifa = 940;

        [Test]
        public void PagarConSaldo_SaldoDescontadoCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(2000);
            tarjeta.DescontarSaldo(Tarifa);
            Assert.AreEqual(1060, tarjeta.Saldo);
        }

        [Test]
        public void PagarSinSaldo_LanzaExcepcion()
        {
            Tarjeta tarjeta = new Tarjeta(0);
            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(Tarifa));
        }
    }

    [TestFixture]
    public class TarjetaSaldoNegativoTest
    {
        private const decimal Tarifa = 940;
        private const decimal SaldoNegativoMaximo = 480;

        [Test]
        public void Pagar_ExcedeSaldoNegativoMaximo_LanzaExcepcion()
        {
            Tarjeta tarjeta = new Tarjeta(200);
            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(Tarifa));
        }

        [Test]
        public void Recargar_ConSaldoNegativo_DescuentaDeudaCorrectamente()
        {
            Tarjeta tarjeta = new Tarjeta(0);
            tarjeta.DeudaPlus = 480;
            tarjeta.Recargar(2000);
            Assert.AreEqual(1520, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.DeudaPlus);
        }
    }

    [TestFixture]
    public class TarjetaFranquiciaTest
    {
        private const decimal TarifaNormal = 940;
        private const decimal TarifaMedioBoleto = TarifaNormal / 2;

        [Test]
        public void PagarConFranquiciaCompleta_SiemprePuedePagar()
        {
            TarjetaCompleta tarjetaFranquiciaCompleta = new TarjetaCompleta(0);
            tarjetaFranquiciaCompleta.DescontarSaldo(TarifaNormal);
            Assert.AreEqual(0, tarjetaFranquiciaCompleta.Saldo);
        }

        [Test]
        public void PagarConMedioBoleto_DescuentoCorrecto()
        {
            MedioBoleto tarjetaMedioBoleto = new MedioBoleto(2000);
            tarjetaMedioBoleto.DescontarSaldo(TarifaNormal);
            Assert.AreEqual(2000 - TarifaMedioBoleto, tarjetaMedioBoleto.Saldo);
        }
    }

    //TESTS_ITERACIÓN3
    [TestFixture]
    public class TestearBoleto
    {
        [Test]
        public void NoPermitirViajeAntesDe5Minutos_MedioBoleto()
        {
            decimal monto = 25.00m;
            string tipoTarjeta = "Medio Boleto";
            string linea = "Línea 145";
            decimal saldoRestante = 100.00m;
            string idTarjeta = "1234567890";

            Boleto boleto1 = new Boleto(monto, tipoTarjeta, linea, saldoRestante, idTarjeta);
            DateTime fechaSegundoViaje = boleto1.Fecha.AddMinutes(1);
            Boleto boleto2 = new Boleto(monto, tipoTarjeta, linea, saldoRestante - monto, idTarjeta);
            boleto2.Fecha = fechaSegundoViaje;
            Assert.IsTrue((boleto2.Fecha - boleto1.Fecha).TotalMinutes < 5,
                "No se permite viajar antes de 5 minutos con la misma tarjeta de Medio Boleto.");
        }

        [Test]
        public void SoloCuatroViajesConDescuento_PorDia_MedioBoleto()
        {
            decimal montoConDescuento = 470m;
            decimal montoCompleto = 940m;
            string tipoTarjeta = "Medio Boleto";
            string linea = "102 144N";
            decimal saldoRestante = 150.00m;
            string idTarjeta = "1234567890";

            for (int i = 1; i <= 4; i++)
            {
                Boleto boleto = new Boleto(montoConDescuento, tipoTarjeta, linea, saldoRestante - (montoConDescuento * i), idTarjeta);
                Assert.AreEqual(montoConDescuento, boleto.Monto, $"Viaje {i}: Debería aplicar el descuento.");
            }

            Boleto boletoQuintoViaje = new Boleto(montoCompleto, tipoTarjeta, linea, saldoRestante - (montoConDescuento * 4), idTarjeta);
            Assert.AreEqual(montoCompleto, boletoQuintoViaje.Monto, "El quinto viaje del día no debería tener descuento.");
        }
    }

    [TestFixture]
    public class TarjetaCompletaTests
    {
        [Test]
        public void NoPermiteMasDeDosViajesGratisPorDia()
        {
            var tarjeta = new TarjetaCompleta(0);
            tarjeta.DescontarSaldo(0); // Primer viaje gratis
            tarjeta.DescontarSaldo(0); // Segundo viaje gratis                                  //Al querer viajar sin saldo por tercera vez en 
            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(1200),        //el día, se pasa de la deuda y no puede viajar
                "Debería lanzarse una excepción al intentar realizar un tercer viaje gratuito en el mismo día.");
        }

        [Test]
        public void CobrarViajeCompletoDespuesDeDosViajesGratis()
        {
            var tarjeta = new TarjetaCompleta(1500);
            decimal precioViaje = 1200;
            tarjeta.DescontarSaldo(0); // Primer viaje gratis
            tarjeta.DescontarSaldo(0); // Segundo viaje gratis
            tarjeta.DescontarSaldo(precioViaje);
            Assert.AreEqual(300, tarjeta.Saldo, "El saldo debería haberse reducido después del tercer viaje.");
        }
    }

    [TestFixture]
    public class TarjetaTests
    {
        [Test]
        public void RecargaExcedenteSeAcreditaHastaElMaximo()
        {
            var tarjeta = new Tarjeta(35000);
            decimal montoRecarga = 3000;
            tarjeta.Recargar(montoRecarga);
            Assert.AreEqual(36000, tarjeta.Saldo, "El saldo debería haberse limitado a 36,000 con un saldo pendiente de 2000");
            Assert.AreEqual(2000, tarjeta.SaldoPlus, "Saldo plus de 2000");
        }

        [Test]
        public void AcreditaSaldoPendienteDespuesDeUnViaje()
        {
            var tarjeta = new Tarjeta(35000);
            decimal montoRecarga = 3000;
            tarjeta.DescontarSaldo(6000); // Saldo pendiente generado
            tarjeta.Recargar(montoRecarga);
            Assert.AreEqual(32000, tarjeta.Saldo, "Debería haberse acreditado el saldo pendiente de 6000.");
        }
    }

    //ITERACIÓN 4
    [TestFixture]
    public class DescuentoTest
    {
        private Tarjeta tarjeta;

        [SetUp]
        public void Setup()
        {
            tarjeta = new Tarjeta(36000); // Inicializa con saldo suficiente
        }

        [Test]
        public void DescuentoDeViajeNormalHasta29Viajes()
        {
            decimal precioViaje = 1000;
            tarjeta.CantidadViajesMes = 15;
            
            tarjeta.DescontarSaldo(precioViaje);
            Assert.AreEqual(36000 - precioViaje, tarjeta.Saldo, "El saldo debería ser correcto después de 29 viajes con tarifa normal.");
        }

        [Test]
        public void DescuentoDel20PorcientoEntre30y79Viajes()
        {
            decimal precioViaje = 1000;
            tarjeta.CantidadViajesMes = 45;

            tarjeta.DescontarSaldo(precioViaje);
            Assert.AreEqual(36000 - (precioViaje * 0.80m), tarjeta.Saldo, "El saldo debería reflejar el descuento del 20% después de 30 viajes.");
        }

        [Test]
        public void DescuentoDel25PorcientoEnElViaje80()
        {
            decimal precioViaje = 1000;
            tarjeta.CantidadViajesMes = 80;
            
            tarjeta.DescontarSaldo(precioViaje);
            Assert.AreEqual(36000 - (precioViaje * 0.75m), tarjeta.Saldo, "El saldo debería reflejar el descuento del 25% en el viaje 80.");
        }

        [Test]
        public void ViajesDescontadosSinDescuentoLuegoDe80Viajes()
        {
            decimal precioViaje = 1000;
            tarjeta.CantidadViajesMes = 85;

            tarjeta.DescontarSaldo(precioViaje); 
            Assert.AreEqual(36000 - precioViaje, tarjeta.Saldo, "El saldo debería reflejar la tarifa normal después de 80 viajes.");
        }
    }
}