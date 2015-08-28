using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal class OperatorManager
    {
        public IReadOnlyDictionary<char, IOperator> Operators { get; }

        public IReadOnlyCollection<int> Priorities { get; }

        public OperatorManager(IList<IOperator> operators)
        {
            Operators = operators.ToDictionary(o => o.OperatorCharacter, o => o);
            Priorities = operators.GroupBy(o => o.Priority, o => o)
                                  .Select(g => g.Key)
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
                        new Operator<int>('*', 1, (l, r) => l* r),
                        new Operator<int>('/', 1, (l, r) => l / r),
                        new Operator<int>('^', 2, (l, r) => (int) Math.Pow(l, r))
                    }), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static OperatorManager Default => _default.Value;
    }
}
