using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Domain.Entities;
using System.Collections.Concurrent;

namespace OrderAccumulator.Application.Services
{
    public class ExposureAccumulator : IExposureAccumulator
    {
        /*
         * Armazena a exposição atual de cada símbolo em memória,
         * utilizando o ConcurrentDictionary para acessar/manipular
         * o dicionário de forma segura em caso de várias requisições simultâneas e
         * mantendo um objeto de sincronização para cada símbolo, de forma que simbolos iguais
         * sejam processadas sequencialmente e os diferentes sendo processados em paralelo.
         */

        private readonly ConcurrentDictionary<string, SymbolExposure> _exposures = new();
        private readonly ConcurrentDictionary<string, object> _locks = new();

        public Task<bool> TryApplyExposureAsync(Order order, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Obtém ou cria o objeto de sincronização por símbolo
            var sync = _locks.GetOrAdd(order.Symbol, _ => new object());

            //Somente uma thread por símbolo pode executar a lógica simultaneamente
            lock (sync)
            {
                //Obtém o estado do símbolo ou cria uma nova caso não exista
                var exposure = _exposures.GetOrAdd(
                    order.Symbol,
                    symbol => new SymbolExposure(symbol));

                //Calcular a nova exposição
                var newValue = exposure.CalculateNewValue(order);

                //Verifica se a nova exposição está dentro do limite
                if (!exposure.HasLimit(newValue))
                {
                    //Não havendo limite, a ordem é rejeitada e o resultado não é alterado
                    return Task.FromResult(false);
                }

                //Havendo limite, efetiva a atualização do valor de exposição do símbolo
                exposure.ApplyNewValue(newValue);

                //Retorna que a ordem foi aceita
                return Task.FromResult(true);
            }
        }
    }
}
