# Spreadsheet Simulator

Simple OOP demo that simulates spreadsheets processing

## I. General description, structure of solution, configuration
Spreadsheet processing consist of three stages:
### 1. Reading of spreadsheet
Spreadsheet could be read from stream using `SpreadsheetStreamReader` which includes `SpreadsheetStreamTokenizer` for reading tokens from stream, and `SpreadsheetStreamParser` for combining tokens in cells expressions.
By default the reader is configured to support expressions with:
* Multiletter cell address like `AA78` or `BVZ197`.  
* Integer operators `+`, `-`, `*`, `/`, `^ `(exponentiation), according to its priority.
* Parenthesis for changing operation priority.
* Unary operators `+`, `-`.	
Set of operators or it's priority could be changed by creating OperatorManager or modifying configuring default one. String operators also could be easily added there, for example: 
```csharp 
	new Operator<string>('&', 1, (l, r) => l + r) 
```
Character responsible for them parenthesis, column separators and others could be changed in 'SpesialCharactersSettings'. 

### 2. Processing of spreadsheet
Evaluation of every spreadsheet cells done in `SpreadsheetProcessor`, it's created based on `Spreadsheet` and can accept `ISpreadsheetValidator` for spreadsheet checks before evaluation. For example `RecursionDetectionValidator` which checks cells for circular references. `SpreadsheetProcessor` is also able to detect circular references during evaluation but in multithreaded environment it could cause deadlock. 
Spreadsheet evaluation could be customizing by passing processing strategy to SpreadsheetProcessor, currently exists:
* `SimpleProcessingStrategy` - processing done in one thread.
* `ParallelProcessingStrategy` - parallel multithread processing using TPL.
Result of evaluation stored in SpreadsheetEvaluationResult, important to remember that its enumerable so will be executed only when really required

### 3. Writing result of spreadsheet processing
`SpreadsheetWriter` responsible for `SpreadsheetProcessingResult` output, logic inside it:
* Convert processed objects to string representation. 
* Split cell results by rows.
* Add error marks for exceptions.
* Display cells call stack for exceptions if it’s available.
Any other formatting of processed values could be done here.

### Simplified scheme of solution
![Simple solution scheme](https://cdn.rawgit.com/AndreyTretyak/SpreadsheetSimulator/master/simple_scheme.svg)
 
## II. Performance optimization

### 1. Manual integer parsing
For optimization of memory traffic integers are parsing manually while reading input stream char by char, without creation of redundant string for function `int.Parse`.

### 2. Redundant delegate creation
To prevent creating of new delegate on every method call, that takes it as parameter, delegate stored as private variables, for example in `SpreadsheetProcessor` class. This situation well displayed in  
[decompilation of delegate passing example](http://tryroslyn.azurewebsites.net/#K4Zwlgdg5gBAygTxAFwKYFsDcAoUlaIoYB0AMpAI44AOwARgDZgDGMzDAhiCDAMIzYA3jBGiBY2oxYwAbgHswAExgBZABQBKcaMHYxYgGLAIzADzMAFhwBOAGjpy5DAHwwAZsebIYAXjZXrYgBJEAARMCgwZEwxPX0RGRsYZABGXxgAIgziAHULVGtUNUsbYLCIqI0ceISk5AAmdKzc/MK1DxNkKriRAF9sXqAAA).

### 3. Closure of cell variable
Class `ExtendedLazy` is slightly modified version of standard `Lazy` [source code](https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Lazy.cs). 

`Lazy` uses parameterless delegate for object initialization, 
`ExtendedLazy` receive in constructor delegate with parameter and parameter itself. 

Those changes helps to prevent closure of cell variable and creation of extra types for holding closure variable, as it's displayed in [decompilation of local variable closure](http://tryroslyn.azurewebsites.net/#K4Zwlgdg5gBAygTxAFwKYFsDcAoADsAIwBswBjGUogQxBBgGEYBvbGN919/YsmANwD2YACYwAsgAoAlDE5sW7RTEjJ+VIjAC8MAIyZZS9nyoAnGKu0RUAdxgAZKgC8EAHgEEAVqlLIAfBOktXzUNAGoYAGYpHEUAX2xYoAAA).

### 4. Creation of Hashsets
Classes `PooledHashSet` and its base class `ObjectPool` copied from Roslyn [source code](http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis/PooledHashSet.cs) for reusing HashSets during recursion checking.   

 
## III. Possible problems
### 1. Spreadsheet size limitation
Spreadsheet stored in array so size more than couple hungered millions cells could case `OutOfMemoryException`. 
Problem could be solved by changing way of storage in `Spreadsheet` class and cash storage inside `SpreadsheetProcessor`. 

### 2. Integer size limitation
Integers in expressions have standard limitations for signed 32 bit's numbers. 
So if bigger numbers required long should be used or even BigInt, but it will increase memory usage.

### 3. Boxing of value types
All expression stored and processed as objects so value types will be boxed during evaluation, which results in creation many redundant objects.

### 4. Double memory usage
Beside of storing spreadsheet in memory `SpreadsheetProcessor` also store evaluated values, which results in more memory usage, but prevents evaluation of cell value couple of times.

### 5. Reading slower than calculation
Reading of expression takes more time than its evaluation. Way to minimize time lose connected with problem descried in _Start calculation during reading_.

### 6. Multithreading slow down calculation for simple expressions
On a spreadsheet with simple expressions, more recourses spend for synchronizing multiple threads than gained from splitting execution between threads.

 
## IV. Ways to optimize solution

### 1. Start calculation during reading
As it was sad before reading of spreadsheet takes much more time than evaluating of it, and it can be read only in one thread. So possible way to speed up solution is starting cell value evaluation is separate thread, just after it was read.
This could give speed improvement for spreadsheets where cells mostly referenced to cells that were read before.

### 2. Expression cashing
Large amount of memory used for storing expressions could be reduced by using expression cashing similar to how it`s done in Roslyn ([source code](https://github.com/dotnet/roslyn/blob/1c74e1b4698881d87870cf53fef06549ea348763/src/Compilers/CSharp/Portable/Syntax/InternalSyntax/SyntaxNodeCache.cs), [discussion](https://roslyn.codeplex.com/discussions/541953)).

### 3. Expression simplification
If memory used for storing spreadsheet needs to be reduced, and there is no need in displaying spreadsheet how it is, expressions can be simplified during parsing or after it.

For example if binary expression has only constant values it could be replaced with constant, and one object will be stored instead of three.

### 4. Error handling
In case of error during cell parsing or evaluating exception are thrown and every cell catch it do add address in error stack (converting cell address to string also takes many resources). 

So processing of spreadsheets with many errors could be optimized by:
* Remove cells call stack creation in exception, this will allow to get rid of cell address converting, catching and rethrowing exception in every cell.
* Processing exceptions like regular results of evaluation, without throwing and catching them.