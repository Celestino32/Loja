namespace LojaVirtual.Domain.Ordering;

public enum OrderStatus
{
    Pendente = 0,
    Pago = 1,
    Preparando = 2,
    ProntoParaEntregaOuRetirada = 3,
    Concluido = 4,
    Cancelado = 5
}

public enum FulfillmentType
{
    Entrega = 0,
    RetiradaNaLoja = 1
}
