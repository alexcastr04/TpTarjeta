namespace TransporteUrbano
{
    public class Colectivo
    {
        private const decimal TarifaBasica = 940;

        public Boleto PagarCon(Tarjeta tarjeta, string linea)
        {
                decimal monto;
                string descripcionExtra = "";

            if (tarjeta is MedioBoleto)
            {
                monto = TarifaBasica / 2;
                tarjeta.DescontarSaldo(monto);
            }
            else if (tarjeta is TarjetaCompleta)
            {
                monto = 0;
                tarjeta.DescontarSaldo(monto);
            }
            else
            {
                monto = TarifaBasica;
                tarjeta.DescontarSaldo(monto);
            }

            if (tarjeta.DeudaPlus > 0)
            {
                descripcionExtra = $"Abona saldo plus de ${tarjeta.DeudaPlus}";
            }

            return new Boleto(monto, tarjeta.GetType().Name, linea, tarjeta.Saldo, tarjeta.GetHashCode().ToString(), descripcionExtra);
        }
    }
}
