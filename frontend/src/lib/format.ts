export function formatKwanza(value: number): string {
  return new Intl.NumberFormat("pt-AO", {
    style: "currency",
    currency: "AOA",
    minimumFractionDigits: 2,
  }).format(value);
}

export function formatDateTime(value: string): string {
  return new Intl.DateTimeFormat("pt-AO", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

const orderStatusLabels: Record<string, string> = {
  Pendente: "Pendente",
  Pago: "Pago",
  Preparando: "Preparando",
  ProntoParaEntregaOuRetirada: "Pronto para entrega/retirada",
  Concluido: "Concluído",
  Cancelado: "Cancelado",
};

export function formatOrderStatus(status: string): string {
  return orderStatusLabels[status] ?? status;
}

const paymentMethodLabels: Record<string, string> = {
  MulticaixaExpress: "Multicaixa Express",
  ReferenciaDePagamento: "Referência de pagamento",
  TransferenciaBancaria: "Transferência bancária",
};

export function formatPaymentMethod(method: string): string {
  return paymentMethodLabels[method] ?? method;
}
