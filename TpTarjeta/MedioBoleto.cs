namespace TransporteUrbano
{
public class MedioBoleto : Tarjeta
    {
        private DateTime ultimoViaje;

        public MedioBoleto(decimal saldoInicial = 0) : base(saldoInicial)
        {
            ultimoViaje = DateTime.MinValue;
        }

        public override void DescontarSaldo(decimal monto)
        {
            if ((DateTime.Now - ultimoViaje).TotalMinutes < 5)
            {
                throw new InvalidOperationException("No se puede realizar otro viaje con medio boleto en menos de 5 minutos.");
            }

            base.DescontarSaldo(monto / 2);
            ultimoViaje = DateTime.Now;
        }
    }
}
