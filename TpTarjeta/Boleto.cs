namespace TransporteUrbano
{
    public class Boleto
    {
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; set; }
        public string TipoTarjeta { get; private set; }
        public string LineaColectivo { get; private set; }
        public decimal SaldoRestante { get; private set; }
        public string IdTarjeta { get; private set; }
        public string DescripcionExtra { get; private set; }

        public Boleto(decimal monto, string tipoTarjeta, string linea, decimal saldoRestante, string idTarjeta, string descripcionExtra = "")
        {
            Monto = monto;
            Fecha = DateTime.Now;
            TipoTarjeta = tipoTarjeta;
            LineaColectivo = linea;
            SaldoRestante = saldoRestante;
            IdTarjeta = idTarjeta;
            DescripcionExtra = descripcionExtra;
        }

        public override string ToString()
        {
            return $"Boleto - Fecha: {Fecha}, Tipo Tarjeta: {TipoTarjeta}, Línea: {LineaColectivo}, Monto: ${Monto}, Saldo Restante: ${SaldoRestante}. {DescripcionExtra}";
        }
    }
}