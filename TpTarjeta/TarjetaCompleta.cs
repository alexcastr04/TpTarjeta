namespace TransporteUrbano
{
    public class TarjetaCompleta : Tarjeta
    {
        private int viajesHoy;
        private DateTime ultimoViaje;

        public TarjetaCompleta(decimal saldoInicial = 0) : base(saldoInicial)
        {
            viajesHoy = 0;
            ultimoViaje = DateTime.MinValue;
        }

        public override void DescontarSaldo(decimal monto)
        {
            if (ultimoViaje.Date != DateTime.Now.Date)
            {
                viajesHoy = 0;
            }

            if (viajesHoy >= 2)
            {
                base.DescontarSaldo(monto);
            }

            viajesHoy++;
            ultimoViaje = DateTime.Now;
        }
    }
}
