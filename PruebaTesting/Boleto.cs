namespace TransporteUrbano
{
    public class Boleto
    {
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; private set; }

        public Boleto(decimal monto)
        {
            Monto = monto;
            Fecha = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Boleto - Fecha: {Fecha}, Monto: ${Monto}";
        }
    }
}
