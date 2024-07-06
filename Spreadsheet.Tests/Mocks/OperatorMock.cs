using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core.Parsers.Operators;

namespace Spreadsheet.Tests.Mocks
{
    internal class OperatorMock : IOperator
    {
        public int Priority => -1;

        public char OperatorCharacter => '@';

        public bool IsBinaryOperationSupported => true;

        public bool IsUnaryOperationSupported => true;

        public object Left { get; private set; }

        public object Right { get; private set; }

        public object Value { get; private set; }

        private readonly Func<object> _result;

        public OperatorMock(Func<object> result)
        {
            _result = result;
        }

        public object BinaryOperation(object left, object right)
        {
            Left = left;
            Right = right;
            return _result();
        }

        public object UnaryOperation(object value)
        {
            Value = value;
            return _result();
        }
    }
}
