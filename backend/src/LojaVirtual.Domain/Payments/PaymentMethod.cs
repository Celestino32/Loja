namespace LojaVirtual.Domain.Payments;

public enum PaymentMethod
{
    MulticaixaExpress = 0,
    ReferenciaDePagamento = 1,
    TransferenciaBancaria = 2
}

public enum PaymentStatus
{
    Pendente = 0,
    Confirmado = 1,
    Falhado = 2
}
