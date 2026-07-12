using System.Net;
using FluentValidation;
using LojaVirtual.Application.Common.Exceptions;
using LojaVirtual.Domain.Common;

namespace LojaVirtual.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Erro de validação", string.Join(" ", ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (DomainException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Regra de negócio violada", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, "Recurso não encontrado", ex.Message);
        }
        catch (PaymentGatewayException ex)
        {
            logger.LogError(ex, "Falha ao comunicar com o gateway de pagamento");
            await WriteProblemAsync(context, HttpStatusCode.BadGateway, "Falha no gateway de pagamento", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado ao processar a requisição {Path}", context.Request.Path);
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Erro interno do servidor", "Ocorreu um erro inesperado. Tente novamente mais tarde.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new
        {
            title,
            status = (int)statusCode,
            detail
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
