namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal SaldoMaximo = 36000;
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

                decimal montoPendiente = 0;
            if (Saldo + monto > SaldoMaximo)
            {
                montoPendiente = (Saldo + monto) - SaldoMaximo;
                monto = SaldoMaximo - Saldo; // Solo acreditamos hasta el máximo permitido
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
                    return;
                }
            }

            Saldo += monto;

            // Si hay saldo pendiente, guardarlo para futuras acreditaciones
            if (montoPendiente > 0)
            {
                Console.WriteLine($"La recarga excedió el saldo máximo. Monto pendiente de acreditación: ${montoPendiente}");
            }
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
    private int viajesHoy;
    private DateTime ultimoViaje;

    public TarjetaCompleta(decimal saldoInicial = 0) : base(saldoInicial)
    {
        viajesHoy = 0;
        ultimoViaje = DateTime.MinValue;
    }

    public new void DescontarSaldo(decimal monto)
    {
        if (ultimoViaje.Date != DateTime.Now.Date)
        {
            viajesHoy = 0; // Resetear el contador de viajes diarios al cambiar el día
        }

        if (viajesHoy >= 2)
        {
            throw new InvalidOperationException("Solo se permiten 2 viajes gratis por día.");
        }

        base.DescontarSaldo(0); // Viaje gratuito
        viajesHoy++;
        ultimoViaje = DateTime.Now;
    }
    }


    // Tarjeta con franquicia parcial (medio boleto estudiantil, universitario)
    public class MedioBoleto : Tarjeta
    {
    private DateTime ultimoViaje;

    public MedioBoleto(decimal saldoInicial = 0) : base(saldoInicial)
    {
        ultimoViaje = DateTime.MinValue;
    }

    public new void DescontarSaldo(decimal monto)
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
