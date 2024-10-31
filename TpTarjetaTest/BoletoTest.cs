using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TpTarjetaTests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void CrearBoleto_TipoGeneral_DatosCorrectos()
        {
            //datos de prueba
            decimal monto = 50.00m;
            string tipoTarjeta = "General";
            string linea = "Línea 145";
            decimal saldoRestante = 200.00m;
            string idTarjeta = "1234567890";
            string descripcionExtra = "";

            //creo el boleto
            Boleto boleto = new Boleto(monto, tipoTarjeta, linea, saldoRestante, idTarjeta, descripcionExtra);

            Assert.AreEqual(monto, boleto.Monto);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.AreEqual(descripcionExtra, boleto.DescripcionExtra);
        }

        [Test]
        public void CrearBoleto_TipoEstudiante_ConDescuento()
        {
            //datos para un boleto de estudiante
            decimal monto = 50.00m; // Monto total antes de aplicar descuento
            string tipoTarjeta = "Estudiante";
            string linea = "Línea 123";
            decimal saldoRestante = 150.00m; // Saldo después del pago
            string idTarjeta = "1234567891";
            string descripcionExtra = "Descuento aplicado";

            Boleto boleto = new Boleto(monto * 0.5m, tipoTarjeta, linea, saldoRestante, idTarjeta, descripcionExtra);

            //verifico que el monto sea la mitad (50% de descuento) y los datos sean correctos
            Assert.AreEqual(monto * 0.5m, boleto.Monto);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.AreEqual(descripcionExtra, boleto.DescripcionExtra);
        }

        [Test]
        public void CrearBoleto_TipoGeneral_SaldoNegativoCancelado()
        {
            //datos para un boleto con saldo negativo cancelado
            decimal monto = 60.00m;
            string tipoTarjeta = "General";
            string linea = "Línea 200";
            decimal saldoRestante = 100.00m;
            string idTarjeta = "1234567892";
            string descripcionExtra = "Abona saldo 120";
            
            Boleto boleto = new Boleto(monto, tipoTarjeta, linea, saldoRestante, idTarjeta, descripcionExtra);

            Assert.AreEqual(monto, boleto.Monto);
            Assert.AreEqual(tipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(saldoRestante, boleto.SaldoRestante);
            Assert.AreEqual(idTarjeta, boleto.IdTarjeta);
            Assert.AreEqual(descripcionExtra, boleto.DescripcionExtra);
        }
    }
}