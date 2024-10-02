namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal SaldoMaximo = 9900;
        private static readonly decimal[] RecargasValidas = { 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };

        public decimal Saldo { get; protected set; }
        public decimal DeudaPlus { get; protected set; } = 0; // Monto acumulado en concepto de viaje plus

        public Tarjeta(decimal saldoInicial = 0)
        {
            if (saldoInicial > SaldoMaximo)
                throw new ArgumentException("El saldo inicial no puede exceder el límite de saldo permitido.");
            Saldo = saldoInicial;
        }

        public void Recargar(decimal monto)
        {
            if (!RecargasValidas.Contains(monto))
            {
                throw new ArgumentException("Monto de recarga no válido.");
            }

            if (Saldo + monto > SaldoMaximo)
            {
                throw new InvalidOperationException("La recarga excede el saldo máximo permitido.");
            }

            // Descontar deuda de saldo negativo antes de agregar la recarga
            if (DeudaPlus > 0)
            {
                decimal deudaRestante = DeudaPlus - monto;
                if (deudaRestante <= 0)
                {
                    monto -= DeudaPlus;
                    DeudaPlus = 0;
                }
                else
                {
                    DeudaPlus = deudaRestante;
                    return; // Si la deuda no se cubre, no se suma saldo a la tarjeta
                }
            }

            Saldo += monto;
        }

        public void DescontarSaldo(decimal monto)
        {
            if (Saldo < monto)
            {
                // Permitir saldo negativo para viajes plus hasta un máximo de -480
                decimal deuda = monto - Saldo;
                if (deuda <= 480)
                {
                    DeudaPlus += deuda;
                    Saldo = 0; // El saldo queda en 0, y se acumula la deuda
                }
                else
                {
                    throw new InvalidOperationException("Saldo negativo máximo alcanzado. No se puede realizar el viaje.");
                }
            }
            else
            {
                Saldo -= monto;
            }
        }
    }

    // Tarjeta con franquicia completa (jubilados, estudiantes con gratuidad)
    public class TarjetaCompleta : Tarjeta
    {
        public TarjetaCompleta(decimal saldoInicial = 0) : base(saldoInicial) { }

        public new void DescontarSaldo(decimal monto)
        {
            // Tarjetas con franquicia completa no pagan
            Saldo -= 0;
        }
    }

    // Tarjeta con franquicia parcial (medio boleto estudiantil, universitario)
    public class MedioBoleto : Tarjeta
    {
        public MedioBoleto(decimal saldoInicial = 0) : base(saldoInicial) { }

        public new void DescontarSaldo(decimal monto)
        {
            // El costo del pasaje es la mitad
            base.DescontarSaldo(monto / 2);
        }
    }
}
