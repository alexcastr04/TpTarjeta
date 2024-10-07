namespace TransporteUrbano
{
    public class Colectivo
    {
        private const decimal TarifaBasica = 940;

        public Boleto PagarCon(Tarjeta tarjeta)
        {
            // Chequear el tipo de tarjeta y aplicar la lógica correspondiente
            if (tarjeta is MedioBoleto)
            {
                decimal tarifaMedioBoleto = TarifaBasica / 2;
                tarjeta.DescontarSaldo(tarifaMedioBoleto);
                return new Boleto(tarifaMedioBoleto);
            }
            else if (tarjeta is TarjetaCompleta)
            {
                tarjeta.DescontarSaldo(0); // Boleto gratuito
                return new Boleto(0);
            }
            else
            {
                // Tarjeta normal sin franquicia
                if (tarjeta.Saldo >= TarifaBasica)
                {
                    tarjeta.DescontarSaldo(TarifaBasica);
                    return new Boleto(TarifaBasica);
                }
                else
                {
                    throw new InvalidOperationException("Saldo insuficiente. No se puede realizar el viaje.");
                }
            }
        }
    }
}
