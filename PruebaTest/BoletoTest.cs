using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransporteUrbano;

namespace TransporteUrbanoTest
{
    [TestClass]
    public class BoletoTest
    {
        [TestMethod]
        public void CrearBoleto_AsignarMontoYFecha()
        {
            //Inicializamos el objeto Boleto con un monto
            decimal montoEsperado = 50.00m;
            DateTime antesDeCreacion = DateTime.Now; // Guardamos el tiempo antes de crear el boleto

            //Creamos el objeto Boleto
            Boleto miBoleto = new Boleto(montoEsperado);

            //Verificamos que el monto asignado sea el esperado
            Assert.AreEqual(montoEsperado, miBoleto.Monto, "El monto del boleto no es el correcto.");

            //Verificamos que la fecha esté dentro del rango esperado (momento de la creación del boleto)
            Assert.IsTrue(miBoleto.Fecha >= antesDeCreacion && miBoleto.Fecha <= DateTime.Now,
                          "La fecha del boleto no es la esperada.");
        }
    }
}

