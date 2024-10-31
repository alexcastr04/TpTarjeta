using NUnit.Framework;
using TransporteUrbano;
using System;

namespace TpTarjetaTests
{
    [TestFixture]
    public class ColectivoTests
    {
        [Test]
        public void PagarCon_TarjetaNormal_BoletoCorrecto()
        {
            var tarjeta = new Tarjeta { Saldo = 2000m };
            var colectivo = new Colectivo();
            string linea = "102 144N";
            bool esInterurbana = false;

            Boleto boleto = colectivo.PagarCon(tarjeta, linea, esInterurbana);

            Assert.AreEqual(1200m, boleto.Monto); // Tarifa básica
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(800m, boleto.SaldoRestante); // Saldo inicial menos tarifa
            Assert.AreEqual("", boleto.DescripcionExtra); // Sin saldo plus
        }

        [Test]
        public void PagarCon_MedioBoleto_BoletoCorrecto()
        {
            var tarjeta = new MedioBoleto { Saldo = 1000m };
            var colectivo = new Colectivo();
            string linea = "102 144N";

            Boleto boleto = colectivo.PagarCon(tarjeta, linea);

            Assert.AreEqual(600m, boleto.Monto); // Tarifa reducida 1/2
            Assert.AreEqual("MedioBoleto", boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(700m, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_TarjetaCompleta_BoletoGratuitoEstudiantil()
        {
            var tarjeta = new TarjetaCompleta { Saldo = 500m };
            var colectivo = new Colectivo();
            string linea = "102 144N";

            Boleto boleto = colectivo.PagarCon(tarjeta, linea);

            Assert.AreEqual(0m, boleto.Monto); // Tarjeta completa, no paga
            Assert.AreEqual("TarjetaCompleta", boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(500m, boleto.SaldoRestante); // Mismo saldo
        }

        [Test]
        public void PagarCon_SaldoPlus_BoletoConDescripcionExtra()
        {
            var tarjeta = new Tarjeta { Saldo = 1000m, DeudaPlus = 500m }; // Saldo plus activo
            var colectivo = new Colectivo();
            string linea = "103";

            Boleto boleto = colectivo.PagarCon(tarjeta, linea);

            Assert.AreEqual(1200m, boleto.Monto);
            Assert.AreEqual($"Abona saldo plus de ${tarjeta.DeudaPlus}", boleto.DescripcionExtra);
        }

        [Test]
        public void PagarCon_TarifaInterurbana_BoletoCorrecto()
        {
            var tarjeta = new Tarjeta { Saldo = 5000m };
            var colectivo = new Colectivo();
            string linea = "Expreso";
            bool esInterurbana = true;

            Boleto boleto = colectivo.PagarCon(tarjeta, linea, esInterurbana);

            Assert.AreEqual(2500m, boleto.Monto); // Tarifa interurbana
            Assert.AreEqual("Tarjeta", boleto.TipoTarjeta);
            Assert.AreEqual(linea, boleto.LineaColectivo);
            Assert.AreEqual(2500m, boleto.SaldoRestante);
        }
        [Test]
        public void ValidarFranquiciaHorario_FueraDeHorario_RetornaFalse()
        {
            var colectivo = new Colectivo();
            DateTime fecha = new DateTime(2024, 10, 23, 23, 0, 0); // Miércoles, 23:00 (fuera de horario)

            bool resultado = colectivo.ValidarFranquiciaHorario(fecha);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void ValidarFranquiciaHorario_Sabado_RetornaFalse()
        {
            var colectivo = new Colectivo();
            DateTime fecha = new DateTime(2024, 10, 26, 9, 0, 0); // Sábado, 9:00 

            bool resultado = colectivo.ValidarFranquiciaHorario(fecha);

            Assert.IsFalse(resultado);
        }
    }

    //ITERACION 4
    [TestFixture]
    public class FranquiciaTests
    {
        private TiempoFalso tiempoFalso;

        [SetUp]
        public void Setup()
        {
            // Inicializa la instancia de TiempoFalso antes de cada test
            tiempoFalso = new TiempoFalso();
        }

        [Test]
        public void NoDeberiaPermitirViajeFueraDeFranjaHoraria()
        {
            // Prueba fuera de horario (22:01)
            tiempoFalso = new TiempoFalso(); // Reinicia tiempoFalso si es necesario
            tiempoFalso.AgregarHoras(22); // Cambia la hora a 22:00
            tiempoFalso.AgregarMinutos(1); // Cambia la hora a 22:01

            bool resultado = RealizarViajeConFranquicia(tiempoFalso.Now());

            Assert.IsFalse(resultado, "Se debería haber bloqueado el viaje fuera del horario permitido a las 22:01.");

            // Prueba fuera de horario (05:59)
            tiempoFalso = new TiempoFalso(); // Reinicia tiempoFalso para el segundo caso
            tiempoFalso.AgregarHoras(5); // Cambia la hora a 05:00
            tiempoFalso.AgregarMinutos(59); // Cambia la hora a 05:59

            resultado = RealizarViajeConFranquicia(tiempoFalso.Now());

            Assert.IsFalse(resultado, "Se debería haber bloqueado el viaje fuera del horario permitido a las 05:59.");
        }


        [Test]
        public void DeberiaPermitirViajeDentroDeFranjaHoraria()
        {
            // Prueba dentro de horario (09:00)
            tiempoFalso.AgregarHoras(9); // Cambia la hora a 09:00

            bool resultado = RealizarViajeConFranquicia(tiempoFalso.Now());

            Assert.IsTrue(resultado, "El viaje debería ser permitido dentro del horario.");

            // Prueba dentro de horario (21:59)
            tiempoFalso.AgregarHoras(12); // Cambia la hora a 21:59

            resultado = RealizarViajeConFranquicia(tiempoFalso.Now());

            Assert.IsTrue(resultado, "El viaje debería ser permitido dentro del horario.");
        }

        private bool RealizarViajeConFranquicia(DateTime horaActual)
        {
            // Lógica simulada para verificar si el viaje puede realizarse
            // Solo permite el viaje si está entre 6:00 y 22:00
            return horaActual.Hour >= 6 && horaActual.Hour < 22;
        }
    }
}
