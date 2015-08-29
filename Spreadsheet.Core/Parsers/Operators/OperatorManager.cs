using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Spreadsheet.Core.Parsers.Operators
{
    internal class OperatorManager
    {
        public IReadOnlyDictionary<char, IOperator> Operators { get; }

        public IReadOnlyCollection<int> Priorities { get; }

        public OperatorManager(IList<IOperator> operators)
        {
            Operators = operators.ToDictionary(o => o.OperatorCharacter, o => o);
            Priorities = operators.Select(o => o.Priority)
                                  .Distinct()
                                  .OrderBy(g => g)
                                  .ToArray();
        }

        private readonly static Lazy<OperatorManager> _default;

        static OperatorManager() 
        {
            _default = new Lazy<OperatorManager>(
                    () => new OperatorManager(new List<IOperator>
                    {
                        new Operator<int>('+', 0, (l, r) => l + r, v => v),
                        new Operator<int>('-', 0, (l, r) => l - r, v => -v),
                        new Operator<int>('*', 1, (l, r) => l * r),
                        new Operator<int>('/', 1, (l, r) => l / r),
                        new Operator<int>('^', 2, (l, r) => (int) Math.Pow(l, r))
                    }), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static OperatorManager Default => _default.Value;
    }
}
