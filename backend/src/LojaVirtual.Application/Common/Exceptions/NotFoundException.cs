namespace LojaVirtual.Application.Common.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"Entidade '{entityName}' com chave '{key}' não foi encontrada.");
