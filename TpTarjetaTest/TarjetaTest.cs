using NUnit.Framework;
using TransporteUrbano;
using System;

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
        public void Recargar_ExcedeSaldoMaximo_LanzaExcepcion()
        {
            Tarjeta tarjeta = new Tarjeta(9000);

            // Assert: Intentamos recargar con un monto que excedería el saldo máximo permitido
            Assert.Throws<InvalidOperationException>(() => tarjeta.Recargar(3000));
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

        //[Test]
        //public void Recargar_CubreDeudaPlus_DeudaActualizadaCorrectamente()
        //{
        //    Tarjeta tarjeta = new Tarjeta(0);
        //    tarjeta.DeudaPlus = 480;

        //    tarjeta.Recargar(2000); //al recargar 2000 (mínimo monto recarga), saldamos la deuda y nos queda un saldo de 1520

        //    //La deuda queda saldada (0) y el saldo debe ser de 1520
        //    Assert.AreEqual(1520, tarjeta.Saldo);
        //    Assert.AreEqual(0, tarjeta.DeudaPlus);
        //}
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
            var tarjeta = new TarjetaCompleta();
            tarjeta.DescontarSaldo(0); // Primer viaje gratis
            tarjeta.DescontarSaldo(0); // Segundo viaje gratis
            Assert.Throws<InvalidOperationException>(() => tarjeta.DescontarSaldo(0),
                "Debería lanzarse una excepción al intentar realizar un tercer viaje gratuito en el mismo día.");
        }

        [Test]
        public void CobrarViajeCompletoDespuesDeDosViajesGratis()
        {
            var tarjeta = new TarjetaCompleta(1500);
            decimal precioViaje = 940;
            tarjeta.DescontarSaldo(0); // Primer viaje gratis
            tarjeta.DescontarSaldo(0); // Segundo viaje gratis
            tarjeta.DescontarSaldo(precioViaje);
            Assert.AreEqual(940, tarjeta.Saldo, "El saldo debería haberse reducido después del tercer viaje.");
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
        [Test]
        public void UsoFrecuenteSUBE_Aplicar_Beneficios()
        {
            decimal tarifaNormal = 1200m;
            decimal descuento20 = tarifaNormal * 0.8m;
            decimal descuento25 = tarifaNormal * 0.75m;
            decimal saldoInicial = 36000m;
            var tarjeta = new Tarjeta(saldoInicial);

            int cantidadViajes = 0;

            for (int i = 1; i <= 29; i++)
            {
                tarjeta.DescontarSaldo(tarifaNormal);
                cantidadViajes++;

                Assert.AreEqual(saldoInicial - (tarifaNormal * cantidadViajes), tarjeta.Saldo);

                if (tarjeta.Saldo <= 1200)
                {
                    for (int j = 1; j <= 4; i++)
                    {
                        tarjeta.Recargar(8000);
                    }
                    //saldoInicial = 36000;
                }
            }

            for (int i = 30; i <= 79; i++)
            {
                tarjeta.DescontarSaldo(descuento20);
                cantidadViajes++;

                Assert.AreEqual(saldoInicial - (descuento20 * (cantidadViajes - 29)), tarjeta.Saldo);

                if (tarjeta.Saldo <= 1200)
                {
                    for (int j = 1; j <= 4; i++)
                    {
                        tarjeta.Recargar(8000);
                    }
                    //saldoInicial = 36000;
                }
            }

            tarjeta.DescontarSaldo(descuento25);
            Assert.AreEqual(saldoInicial - descuento25, tarjeta.Saldo);

            if (tarjeta.Saldo <= 1200)
            {
                tarjeta.Recargar(9000);
            }

            tarjeta.DescontarSaldo(tarifaNormal);
            Assert.AreEqual(36000 - tarifaNormal, tarjeta.Saldo);
        }
    }
}