namespace TransporteUrbano
{
    public class Tarjeta
    {
        private const decimal SaldoMaximo = 36000;
        private static readonly decimal[] RecargasValidas = { 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000 };

        public decimal Saldo { get; protected set; }
        public decimal DeudaPlus { get; protected set; } = 0; // Monto acumulado en concepto de viaje plus
        public int CantidadViajesMes { get; protected set; } = 0; // Contador de viajes mensuales

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

            if (montoPendiente > 0)
            {
                Console.WriteLine($"La recarga excedió el saldo máximo. Monto pendiente de acreditación: ${montoPendiente}");
            }
        }

        public virtual void DescontarSaldo(decimal monto)
        {
            decimal montoDescuento = CalcularDescuento(monto);
            if (Saldo < montoDescuento)
            {
                decimal deuda = montoDescuento - Saldo;
                if (deuda <= 480)
                {
                    DeudaPlus += deuda;
                    Saldo = 0;
                }
                else
                {
                    throw new InvalidOperationException("Saldo negativo máximo alcanzado. No se puede realizar el viaje.");
                }
            }
            else
            {
                Saldo -= montoDescuento;
            }
            CantidadViajesMes++;
        }

        protected virtual decimal CalcularDescuento(decimal monto)
        {
            // Descuento basado en la cantidad de viajes mensuales
            if (CantidadViajesMes >= 30 && CantidadViajesMes <= 79)
            {
                return monto * 0.80m; // 20% de descuento
            }
            if (CantidadViajesMes == 80)
            {
                return monto * 0.75m; // 25% de descuento
            }
            return monto; // Tarifa normal
        }
    }

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
                throw new InvalidOperationException("Solo se permiten 2 viajes gratis por día.");
            }

            base.DescontarSaldo(0); // Viaje gratuito
            viajesHoy++;
            ultimoViaje = DateTime.Now;
        }
    }

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
