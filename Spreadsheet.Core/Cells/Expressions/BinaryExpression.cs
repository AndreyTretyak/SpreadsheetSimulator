using System.Collections.Generic;
using System.Linq;
using Spreadsheet.Core.Parsers.Operators;
using static Spreadsheet.Core.Parsers.Tokenizers.SpesialCharactersSettings;

namespace Spreadsheet.Core.Cells.Expressions
{
    internal class BinaryExpression : IExpression
    {
        public IExpression Left { get; }

        public IExpression Right { get; }

        public IOperator Operation { get; }
        
        public BinaryExpression(IExpression left, IOperator operation, IExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public object Evaluate(SpreadsheetProcessor processor)
        {
            return Operation.BinaryOperation(Left.Evaluate(processor), Right.Evaluate(processor));
        }

        public override string ToString() => $"{LeftParanthesis}{Left}{Operation}{Right}{RightParanthesis}";
    }
}