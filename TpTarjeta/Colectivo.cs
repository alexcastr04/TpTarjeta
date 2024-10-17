namespace TransporteUrbano
{
    public class Colectivo
    {
        private const decimal TarifaBasica = 1200;
        private const decimal TarifaInterurbana = 2500;

        public Boleto PagarCon(Tarjeta tarjeta, string linea, bool esInterurbana = false)
        {
            decimal monto;
            string descripcionExtra = "";

            if (esInterurbana)
            {
                monto = TarifaInterurbana;
            }
            else
            {
                monto = TarifaBasica;
            }

            if (tarjeta is MedioBoleto)
            {
                monto /= 2;
            }
            else if (tarjeta is TarjetaCompleta)
            {
                monto = 0;
            }

            tarjeta.DescontarSaldo(monto);

            if (tarjeta.DeudaPlus > 0)
            {
                descripcionExtra = $"Abona saldo plus de ${tarjeta.DeudaPlus}";
            }

            return new Boleto(monto, tarjeta.GetType().Name, linea, tarjeta.Saldo, tarjeta.GetHashCode().ToString(), descripcionExtra);
        }

        public bool ValidarFranquiciaHorario(DateTime fecha)
        {
            return fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday &&
                   fecha.TimeOfDay >= new TimeSpan(6, 0, 0) && fecha.TimeOfDay <= new TimeSpan(22, 0, 0);
        }
    }
}
