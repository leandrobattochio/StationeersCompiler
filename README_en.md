# Stationeers Compiler

![Build and Deploy](https://github.com/leandrobattochio/StationeersCompiler/actions/workflows/build-and-deploy.yml/badge.svg)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![GitHub Pages](https://img.shields.io/badge/demo-live-success)](https://leandrobattochio.github.io/StationeersCompiler/)

A complete compiler for the Stationeers game programming language, developed in C# with .NET 9.0. This compiler transforms high-level code into assembly that can be executed on the game's Integrated Circuits (ICs).

> 🌐 **[Try it online on GitHub Pages](https://leandrobattochio.github.io/StationeersCompiler/)** - Interactive web interface with code editor!

## 📋 Table of Contents

- [Overview](#overview)
- [Requirements](#requirements)
- [Installation and Setup](#installation-and-setup)
- [How to Use](#how-to-use)
- [Compiler Architecture](#compiler-architecture)
- [Compilation Pipeline](#compilation-pipeline)
- [Detailed Components](#detailed-components)
  - [1. Lexer (Lexical Analysis)](#1-lexer-lexical-analysis)
  - [2. Parser (Syntax Analysis)](#2-parser-syntax-analysis)
  - [3. Semantic Analyzer (Semantic Analysis)](#3-semantic-analyzer-semantic-analysis)
  - [4. IR Builder (Intermediate Representation)](#4-ir-builder-intermediate-representation)
  - [5. IR Optimizer (Optimization)](#5-ir-optimizer-optimization)
  - [6. Code Generator (Code Generation)](#6-code-generator-code-generation)
- [Developer Guide](#developer-guide)
  - [Add New Token](#add-new-token)
  - [Add New Expression](#add-new-expression)
  - [Add New Statement](#add-new-statement)
  - [Add New Operator](#add-new-operator)
  - [Add New Built-in Function](#add-new-built-in-function)
  - [Add New IR Instruction](#add-new-ir-instruction)
- [Language Documentation](#language-documentation)
- [Contributing](#contributing)
- [Useful Links](#-useful-links)

---

## 🎯 Overview

The **Stationeers Compiler** is a multi-stage compiler that converts high-level code into optimized assembly for the Stationeers game.

### About the Project

This project consists of four main components:

#### **Compiler.Core**
Main library with all compilation logic, organized in:
- `Lexer/` - Lexical analysis and tokens
- `Parser/` - Syntax analysis
- `SemanticAnalysis/` - Semantic analysis and symbol table
- `CodeGeneration/` - IR generation, optimization and assembly code
- `Domain/` - Domain models (AST, IR, Types)

#### **CompilerCLI**
Command-line interface to use the compiler. Demonstrates complete compilation examples.

#### **CompilerWeb**
Interactive web interface developed in Blazor WebAssembly with:
- Code editor with syntax highlighting (Monaco Editor)
- Generated AST visualization
- Real-time assembly output
- Built-in language documentation

#### **Compiler.Tests**
Unit test suite to validate compiler functionality.

### Compilation Phases

1. **Lexical Analysis** (Tokenization)
2. **Syntax Analysis** (AST Construction)
3. **Semantic Analysis** (Type Checking)
4. **Intermediate Representation Generation** (IR)
5. **IR Optimization** (Optional)
6. **Code Generation** (Assembly)

---

## ⚙️ Requirements

- **.NET 9.0 SDK** or higher
- **Operating System**: Windows, Linux or macOS
- **Recommended IDE**: 
  - JetBrains Rider
  - Visual Studio 2022
  - Visual Studio Code with C# Dev Kit

---

## 🚀 Installation and Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd StationeersCompiler
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run Tests (if available)

```bash
dotnet test
```

---

## 💻 How to Use

### CLI (Command Line Interface)

```bash
cd CompilerCLI
dotnet run
```

### Web Interface

```bash
cd CompilerWeb
dotnet run
```

Then, open your browser at `https://localhost:5001` (or the port indicated in the terminal).

### As a Library

```csharp
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.SemanticAnalysis;
using Compiler.CodeGeneration;

var code = """
    StationeersDevice device = referenceDevice(d0);
    Float temp = device.Temperature;
    if (temp >= 300) {
        device.On = true;
    } else {
        device.On = false;
    }
    """;

// Lexer
var lexer = new StationeerLexer(code);

// Parser
var parser = new StationeerParser(lexer);
var ast = parser.Parse();

// Semantic Analysis
var semantic = new SemanticAnalyzer();
semantic.Analyze(ast);

// IR Generation
var irBuilder = new IrBuilder();
var ir = irBuilder.Build(ast);

// IR Optimization (optional)
var optimizer = new IrOptimizer();
ir = optimizer.Optimize(ir);

// Code Generation
var codegen = new CodeGenerator();
var assembly = codegen.Generate(ir);

// Output
foreach (var line in assembly)
    Console.WriteLine(line);
```

**Generated Assembly:**
```assembly
start:
l r0 d0 Temperature
mov r1 300
sge r2 r0 r1
beqz r2 L0
sb d0 On 1
j L1
L0:
sb d0 On 0
L1:
j start
```

### Quick Example

**Input (High-Level Code):**
```javascript
StationeersDevice device = referenceDevice(d0);
Float temp = device.Temperature;
if (temp >= 300) {
    device.On = true;
} else {
    device.On = false;
}
```

**Output (Assembly):**
```assembly
start:
l r0 d0 Temperature
mov r1 300
sge r2 r0 r1
beqz r2 L0
s d0 On 1
j L1
L0:
s d0 On 0
L1:
j start
```

---

## 📁 Project Structure

```
StationeersCompiler/
├── Compiler.Core/                    # Main compiler library
│   ├── Lexer/                        # Lexical analysis
│   │   ├── Lexer.cs                  # Main scanner
│   │   └── Tokens/                   # Token definitions
│   ├── Parser/                       # Syntax analysis
│   │   └── StationeerParser.cs       # Recursive descent parser
│   ├── SemanticAnalysis/             # Semantic analysis
│   │   ├── SemanticAnalyzer.cs       # Type checker
│   │   └── SymbolTable.cs            # Symbol table
│   ├── CodeGeneration/               # Code generation
│   │   ├── IrBuilder.cs              # IR builder
│   │   ├── IrOptimizer.cs            # IR optimizer
│   │   ├── CodeGenerator.cs          # Assembly generator
│   │   └── RegisterAllocator.cs      # Register allocator
│   └── Domain/                       # Domain models
│       ├── Ast/                      # Abstract Syntax Tree
│       │   ├── Expressions/          # Expression nodes
│       │   ├── Statements/           # Statement nodes
│       │   └── Visitors/             # Visitor interfaces
│       ├── IR/                       # Intermediate Representation
│       │   └── *.Instruction.cs      # IR instructions
│       └── Types/                    # Type system
│           ├── TypeInfo.cs
│           ├── TypeKind.cs
│           └── MemberInfo.cs
├── CompilerCLI/                      # Command-line interface
│   └── Program.cs
├── CompilerWeb/                      # Blazor web interface
│   ├── Pages/
│   │   └── StCompiler.razor          # Main page
│   ├── Components/
│   │   └── AstNodeComponent.razor    # AST visualizer
│   └── wwwroot/                      # Static assets
├── Compiler.Tests/                   # Unit tests
│   └── CompilerTests.cs
└── README.md                         # This file
```

---

## 🏗️ Compiler Architecture

```
┌─────────────────┐
│  Source Code    │
└────────┬────────┘
         │
         ▼
┌───────────────────┐
│     LEXER         │ ◄── Converts text to tokens
│ (StationeerLexer) │
└────────┬──────────┘
         │
         ▼
┌───────────────────────┐
│     PARSER            │ ◄── Builds the syntax tree (AST)
│  (StationeerParser)   │
└────────┬──────────────┘
         │
         ▼
┌─────────────────┐
│  SEMANTIC       │ ◄── Verifies types and symbols
│  ANALYZER       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   IR BUILDER    │ ◄── Generates intermediate representation
│   (IrBuilder)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  IR OPTIMIZER   │ ◄── Optimizes intermediate code
│  (IrOptimizer)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  CODE GENERATOR │ ◄── Generates final assembly
│ (CodeGenerator) │
└────────┬────────┘
         │
         ▼
┌────────────┐
│  Assembly  │
└────────────┘
```

---

## 🔄 Compilation Pipeline

### Complete Flow Example

**Source Code:**
```javascript
StationeersDevice sensor = referenceDevice(d0);
Float temp = sensor.Temperature;

temp = Math.convertToCelsius(temp);
if (temp >= 25) {
    sensor.On = true;
} else {
    sensor.On = false;
}
```

**1. Tokens (Lexer):**
```
IdentifierToken { Type = Identifier, Position = 0, Value = StationeersDevice }
IdentifierToken { Type = Identifier, Position = 18, Value = sensor }
EqualsToken { Type = Equals, Position = 25, Value = = }
IdentifierToken { Type = Identifier, Position = 27, Value = referenceDevice }
LeftParentToken { Type = LParen, Position = 42, Value = ( }
IdentifierToken { Type = Identifier, Position = 43, Value = d0 }
RightParentToken { Type = RParen, Position = 45, Value = ) }
SemicolonToken { Type = Semicolon, Position = 46, Value = ; }
IdentifierToken { Type = Identifier, Position = 48, Value = Float }
IdentifierToken { Type = Identifier, Position = 54, Value = temp }
EqualsToken { Type = Equals, Position = 59, Value = = }
IdentifierToken { Type = Identifier, Position = 61, Value = sensor }
DotToken { Type = Dot, Position = 67, Value = . }
IdentifierToken { Type = Identifier, Position = 68, Value = Temperature }
...
```

**2. AST (Parser):**
```
VarDeclarationStmt {
    Name = "sensor",
    ExplicitType = "StationeersDevice",
    Initializer = CallExpr {
        Callee = IdentifierExpr { Name = "referenceDevice" },
        Arguments = [
            IdentifierExpr { Name = "d0" }
        ]
    }
}
VarDeclarationStmt {
    Name = "temp",
    ExplicitType = "Float",
    Initializer = MemberAccessExpr {
        Object = IdentifierExpr { Name = "sensor" },
        MemberName = "Temperature"
    }
}
AssignmentExpr {
    Name = "temp",
    Value = StaticMethodCallExpr {
        ObjectName = "Math",
        MethodName = "convertToCelsius",
        Arguments = [IdentifierExpr { Name = "temp" }]
    }
}
IfStmt {
    Condition = BinaryExpr {
        Left = IdentifierExpr { Name = "temp" },
        Op = ">=",
        Right = FloatExpr { Value = 25 }
    },
    ThenBranch = MemberAssignmentExpr {
        Object = IdentifierExpr { Name = "sensor" },
        MemberName = "On",
        Value = BooleanExpr { Value = true }
    },
    ElseBranch = MemberAssignmentExpr {
        Object = IdentifierExpr { Name = "sensor" },
        MemberName = "On",
        Value = BooleanExpr { Value = false }
    }
}
```

**3. Semantic Analysis:**
- Checks if `referenceDevice` is declared ✓
- Checks if `d0` is a valid device identifier ✓
- Validates explicit type `StationeersDevice` ✓
- Checks if `Temperature` is a valid property of `StationeersDevice` ✓
- Checks if `On` is a valid property and can be assigned ✓
- Validates that `Math.convertToCelsius` exists and accepts Float/Int ✓
- Checks operator and comparison types ✓

**4. IR (Intermediate Representation):**
```
LoadFromDeviceInstruction { Target = "t0", Device = "d0", Property = "Temperature" }
LoadConstInstruction { Target = "t1", Value = 273.15 }
BinaryOpInstruction { Target = "t2", Left = "t0", Op = "-", Right = "t1" }
LoadConstInstruction { Target = "t3", Value = 25 }
CompareInstruction { Target = "t4", Left = "t2", Right = "t3", Op = ">=" }
BranchIfZeroInstruction { Condition = "t4", Label = "L0" }
LoadConstInstruction { Target = "t5", Value = 1 }
StoreToDeviceInstruction { Device = "d0", Property = "On", Value = "t5" }
JumpInstruction { Label = "L1" }
LabelInstruction { Name = "L0" }
LoadConstInstruction { Target = "t6", Value = 0 }
StoreToDeviceInstruction { Device = "d0", Property = "On", Value = "t6" }
LabelInstruction { Name = "L1" }
```

**5. Final Assembly:**
```assembly
start:
l r0 d0 Temperature
sub r1 r0 273.15
mov r0 r1
mov r2 25
sge r3 r0 r2
beqz r3 L0
s d0 On 1
j L1
L0:
s d0 On 0
L1:
j start
```

---

## 🔍 Detailed Components

### 1. Lexer (Lexical Analysis)

**File:** `Lexer/Lexer.cs`

The Lexer (also called Scanner or Tokenizer) is responsible for converting source code into a sequence of tokens.

#### Responsibilities:
- Read characters from source code
- Identify lexical patterns (keywords, identifiers, numbers, operators)
- Create appropriate tokens for each pattern
- Ignore whitespace
- Report lexical errors

#### Supported Tokens:

| Token | Description | Example |
|-------|-----------|---------|
| `VarKeywordToken` | var keyword | `var` |
| `IfKeywordToken` | if keyword | `if` |
| `ElseKeywordToken` | else keyword | `else` |
| `TrueKeywordToken` | true keyword | `true` |
| `FalseKeywordToken` | false keyword | `false` |
| `IdentifierToken` | Identifier | `sensor`, `temp`, `d0` |
| `NumberToken` | Integer number | `42`, `300` |
| `FloatToken` | Decimal number | `27.5`, `101325.0` |
| `EqualsToken` | Assignment | `=` |
| `EqualsEqualsToken` | Equality | `==` |
| `NotEqualsToken` | Inequality | `!=` |
| `LessThanToken` | Less than | `<` |
| `LessThanOrEqualToken` | Less than or equal | `<=` |
| `GreaterThanToken` | Greater than | `>` |
| `GreaterThanOrEqualToken` | Greater than or equal | `>=` |
| `LogicalAndToken` | Logical AND | `&&` |
| `LogicalOrToken` | Logical OR | `||` |
| `PlusToken` | Addition | `+` |
| `MinusToken` | Subtraction | `-` |
| `MultiplyToken` | Multiplication | `*` |
| `DivideToken` | Division | `/` |
| `PlusEqualsToken` | Addition with assignment | `+=` |
| `MinusEqualsToken` | Subtraction with assignment | `-=` |
| `MultiplyEqualsToken` | Multiplication with assignment | `*=` |
| `DivideEqualsToken` | Division with assignment | `/=` |
| `PlusPlusToken` | Increment | `++` |
| `MinusMinusToken` | Decrement | `--` |
| `LeftParentToken` | Left parenthesis | `(` |
| `RightParentToken` | Right parenthesis | `)` |
| `LeftBraceToken` | Left brace | `{` |
| `RightBraceToken` | Right brace | `}` |
| `SemicolonToken` | Semicolon | `;` |
| `CommaToken` | Comma | `,` |
| `DotToken` | Dot (member access) | `.` |
| `EndOfFileToken` | End of file | (automatic) |
| `LexerErrorToken` | Lexical error | (error) |

#### Token Structure:

```csharp
public abstract class Token
{
    public TokenType Type { get; }
    public int Position { get; }
    public string Value { get; }
}
```

#### Usage Example:

```csharp
var code = "var x = 42;";
var lexer = new StationeerLexer(code);

Token token;
while ((token = lexer.NextToken()).Type != TokenType.EndOfFile)
{
    Console.WriteLine(token);
}
```

---

### 2. Parser (Syntax Analysis)

**File:** `Parser/StationeerParser.cs`

The Parser builds an Abstract Syntax Tree (AST) from the token sequence.

#### Responsibilities:
- Consume tokens from the Lexer
- Check the language grammar
- Build AST nodes
- Report syntax errors

#### Language Grammar:

```
program         → statement* EOF

statement       → varDeclaration
                | ifStatement
                | blockStatement
                | exprStatement

varDeclaration  → (TYPE_NAME | "var") IDENTIFIER "=" expression ";"

ifStatement     → "if" "(" expression ")" statement ("else" statement)?

blockStatement  → "{" statement* "}"

exprStatement   → expression ";"

expression      → assignment

assignment      → IDENTIFIER "=" expression
                | IDENTIFIER "." IDENTIFIER "=" expression
                | logicalOr

logicalOr       → logicalAnd ( "||" logicalAnd )*

logicalAnd      → comparison ( "&&" comparison )*

comparison      → term ( (">" | ">=" | "<" | "<=" | "==" | "!=") term )*

term            → factor ( ("+" | "-") factor )*

factor          → unary ( ("*" | "/") unary )*

unary           → ("-" | "++" | "--") unary
                | primary ("++" | "--")?

primary         → NUMBER
                | FLOAT
                | "true"
                | "false"
                | IDENTIFIER "." IDENTIFIER "(" arguments? ")"
                | IDENTIFIER "." IDENTIFIER
                | IDENTIFIER "(" arguments? ")"
                | IDENTIFIER
                | "(" expression ")"

arguments       → expression ( "," expression )*

TYPE_NAME       → "Int" | "Float" | "Boolean" | "StationeersDevice"
```

#### AST Structure:

**Expressions (Expr):**
- `NumberExpr` - Integer literal
- `FloatExpr` - Decimal literal
- `BooleanExpr` - Boolean value (true/false)
- `IdentifierExpr` - Identifier
- `BinaryExpr` - Binary operation
- `AssignmentExpr` - Variable assignment
- `GroupExpr` - Parenthesized expression
- `CallExpr` - Function call
- `MemberAccessExpr` - Property access (object.property)
- `MemberAssignmentExpr` - Property assignment (object.property = value)
- `MethodCallExpr` - Method call
- `StaticMethodCallExpr` - Static method call (e.g., Math.convertToCelsius)
- `DeviceExpr` - Device
- `DevicePropertyExpr` - Device property
- `LoadDeviceExpr` - Device loading

**Statements (Stmt):**
- `VarDeclarationStmt` - Variable declaration (with or without explicit type)
- `ExprStmt` - Expression as statement
- `BlockStmt` - Code block delimited by { }
- `IfStmt` - If/else

The AST uses the Visitor pattern to separate data structure from operations:

```csharp
public interface IExprVisitor<T>
{
    T VisitNumber(NumberExpr expr);
    T VisitFloat(FloatExpr expr);
    T VisitBoolean(BooleanExpr expr);
    T VisitBinary(BinaryExpr expr);
    T VisitGroup(GroupExpr expr);
    T VisitCall(CallExpr expr);
    T VisitAssignment(AssignmentExpr expr);
    T VisitCompoundAssignment(CompoundAssignmentExpr expr);
    T VisitIncrementDecrement(IncrementDecrementExpr expr);
    T VisitMemberAccess(MemberAccessExpr expr);
    T VisitMemberAssignment(MemberAssignmentExpr expr);
    T VisitMethodCall(MethodCallExpr expr);
    T VisitStaticMethodCall(StaticMethodCallExpr expr);
    T VisitLoadDevice(LoadDeviceExpr expr);
    T VisitIdentifier(IdentifierExpr expr);
}

public interface IStmtVisitor<T>
{
    T VisitVarDeclaration(VarDeclarationStmt stmt);
    T VisitExprStmt(ExprStmt stmt);
    T VisitIf(IfStmt stmt);
    T VisitBlock(BlockStmt stmt);
}
```

---

### 3. Semantic Analyzer (Semantic Analysis)

**File:** `SemanticAnalysis/SemanticAnalyzer.cs`

The Semantic Analyzer verifies the semantic correctness of the program.

#### Responsibilities:
- Type checking
- Symbol table management
- Variable declaration and usage validation
- Function call validation
- Stationeers device validation

#### Type System:

```csharp
public enum TypeKind
{
    Void,     // Void type (no return)
    Int,      // Integer type
    Float,    // Decimal type
    Boolean,  // Boolean type
    Device,   // Stationeers device type
    Error,    // Error type
    Function  // Function type
}

public class TypeInfo
{
    public TypeKind Kind { get; }
    public List<TypeInfo>? ParameterTypes { get; }  // For functions
    public TypeInfo? ReturnType { get; }            // For functions
    public Dictionary<string, MemberInfo>? Members { get; }  // For objects with properties
    
    public MemberInfo? GetMember(string memberName) => Members?.GetValueOrDefault(memberName);
    public static readonly TypeInfo Void = new(TypeKind.Void);
    public static readonly TypeInfo Int = new(TypeKind.Int);
    public static readonly TypeInfo Boolean = new(TypeKind.Boolean);
    public static readonly TypeInfo Float = new(TypeKind.Float);
    public static readonly TypeInfo Error = new(TypeKind.Error);
    public static readonly TypeInfo Device = new(TypeKind.Device);
}
```

#### Symbol Table:

```csharp
public class SymbolTable
{
    private readonly Dictionary<string, TypeInfo> _symbols = new();
    private readonly Dictionary<string, TypeInfo> _functions = new();

    public void Declare(string name, TypeInfo type);
    public TypeInfo Lookup(string name);
    public void DeclareFunction(string name, List<TypeInfo> parameterTypes, TypeInfo returnType);
    public TypeInfo LookupFunction(string name);
}
```

#### Built-in Functions:

The semantic analyzer registers the following built-in functions:

| Function | Parameters | Return | Description                           |
|--------|-----------|---------|-------------------------------------|
| `referenceDevice` | (device: Device) | StationeersDevice | Creates a reference to a device |
| `sleep` | () | Void | Equivalent to `yield` instruction      |

#### Static Methods:

| Class.Method | Parameters | Return | Description |
|---------------|-----------|---------|-----------|
| `Math.convertToCelsius` | (temp: Int/Float) | Int/Float | Converts Kelvin to Celsius (subtracts 273.15) |

#### Special Validations:

**1. Devices:**

Validates that device identifiers follow the pattern: `d0`, `d1`, `d2`, `d3`, `d4`, `d5`, `db`

```csharp
private bool IsDeviceIdentifier(string name)
{
    if (name == "db") return true;
    if (name.Length == 2 && name[0] == 'd' && char.IsDigit(name[1]))
    {
        int digit = name[1] - '0';
        return digit >= 0 && digit <= 5;
    }
    return false;
}
```

**2. Member Verification:**

Validates if properties and methods exist on the object's type before allowing access.

```csharp
public TypeInfo VisitMemberAccess(MemberAccessExpr expr)
{
    var objectType = expr.Object.Accept(this);
    
    if (objectType.Members == null)
        throw new Exception($"Type '{objectType.Kind}' does not have accessible members");
    
    if (!objectType.HasMember(expr.MemberName))
    {
        var availableMembers = string.Join(", ", objectType.Members.Keys);
        throw new Exception(
            $"Type '{objectType.Kind}' does not have member '{expr.MemberName}'. " +
            $"Available members: {availableMembers}");
    }
    
    var member = objectType.GetMember(expr.MemberName);
    return member switch
    {
        PropertyInfo property => property.PropertyType,
        MethodInfo => throw new Exception(
            $"'{expr.MemberName}' is a method and must be called with parentheses"),
        _ => throw new Exception($"Unknown member type for '{expr.MemberName}'")
    };
}
```

**3. Return Value Usage Validation:**

Ensures that functions with return values are not called as statements without using the returned value.

```csharp
public TypeInfo VisitExprStmt(ExprStmt stmt)
{
    var resultType = stmt.Expression.Accept(this);
    
    // If it's a function call and returns a value, error!
    if (stmt.Expression is CallExpr call && resultType.Kind != TypeKind.Void)
    {
        throw new Exception(
            $"Function returns a value of type '{resultType.Kind}' " +
            $"but the result is not being used. " +
            $"Assign it to a variable or use it in an expression.");
    }
    
    return resultType;
}
```

**4. Device Reference Tracking:**

Maintains a dictionary that maps variables to their physical devices.

```csharp
private readonly Dictionary<string, string> _deviceReferences = new();

// When processing: StationeersDevice sensor = referenceDevice(d0);
_deviceReferences["sensor"] = "d0";

// Later, when generating code for: sensor.Temperature
// We know it should be: l r0 d0 Temperature
```

---

### 4. IR Builder (Intermediate Representation)

**File:** `CodeGeneration/IrBuilder.cs`

The IR Builder converts the AST into an intermediate representation closer to assembly.

#### Responsibilities:
- Convert AST into IR instructions
- Generate temporary variables
- Generate labels for control flow
- Linearize the tree structure

#### IR Instructions:

| Instruction | Description | Example |
|-----------|-----------|---------|
| `LoadConstInstruction` | Load constant | `t0 = 42` |
| `BinaryOpInstruction` | Binary operation | `t2 = t0 + t1` |
| `CompareInstruction` | Comparison | `t3 = t0 >= t1` |
| `LogicalAndInstruction` | Logical AND | `t4 = t0 && t1` |
| `LogicalOrInstruction` | Logical OR | `t5 = t0 \|\| t1` |
| `LoadFromDeviceInstruction` | Load from device | `t6 = load d0.Temperature` |
| `StoreToDeviceInstruction` | Store to device | `store d0.On = 1` |
| `BranchIfZeroInstruction` | Branch if zero | `if t0 == 0 goto L0` |
| `JumpInstruction` | Unconditional jump | `goto L1` |
| `LabelInstruction` | Label | `L0:` |

#### IrProgram Structure:

```csharp
public class IrProgram
{
    public List<IrInstruction> Instructions { get; }
    public string ResultRegister { get; }
}
```

#### Temporary and Label Generation:

```csharp
private int _temp = 0;
private int _label = 0;

private string Temp() => $"t{_temp++}";  // t0, t1, t2, ...
private string Label() => $"L{_label++}"; // L0, L1, L2, ...
```

#### Conversion Example:

**AST:**
```csharp
BinaryExpr { Left = NumberExpr(10), Op = "+", Right = NumberExpr(20) }
```

**IR:**
```
LoadConstInstruction { Target = "t0", Value = 10 }
LoadConstInstruction { Target = "t1", Value = 20 }
BinaryOpInstruction { Target = "t2", Left = "t0", Op = "+", Right = "t1" }
```

---

### 5. IR Optimizer (Optimization)

**File:** `CodeGeneration/IrOptimizer.cs`

The IR Optimizer performs optimizations on the intermediate code before assembly generation.

#### Responsibilities:
- Eliminate redundant instructions
- Optimize common code patterns
- Reduce number of instructions
- Improve generated code efficiency

#### Implemented Optimizations:

**1. Redundant Move Elimination**

Detects the pattern where a binary operation is followed by a move to the same register as the left operand:

**Before:**
```
t0 = load d0.Temperature    // Load into t0
t1 = t0 - 273.15            // Operate and store in t1
t0 = t1                     // Move t1 back to t0 (redundant!)
```

**After:**
```
t0 = load d0.Temperature    // Load into t0
t0 = t0 - 273.15            // Operate directly on t0
```

#### Optimizer Structure:

The optimizer uses the **Strategy pattern** to apply multiple optimization strategies:

```csharp
public class IrOptimizer
{
    private readonly List<IOptimizationStrategy> _strategies =
    [
        new ConstantFoldingOptimize(),  // Eliminate unnecessary LoadConsts
        new BinOpMoveOptimize(),         // Eliminate redundant Moves
    ];

    public IrProgram Optimize(IrProgram program)
    {
        return _strategies.Aggregate(program, (current, strategy) => strategy.Apply(current));
    }
}
```

Each strategy implements the `IOptimizationStrategy` interface:

```csharp
public interface IOptimizationStrategy
{
    IrProgram Apply(IrProgram program);
}
```

For more details about optimizations, see the [Code Optimizations](#code-optimizations) section.

#### Usage:

```csharp
var irBuilder = new IrBuilder();
var ir = irBuilder.Build(ast);

var optimizer = new IrOptimizer();
var optimizedIr = optimizer.Optimize(ir);

var codegen = new CodeGenerator();
var assembly = codegen.Generate(optimizedIr);
```

---

### 6. Code Generator (Code Generation)

**File:** `CodeGeneration/CodeGenerator.cs`

The Code Generator converts IR into assembly for Stationeers.

#### Responsibilities:
- Convert IR instructions into assembly
- Allocate physical registers
- Optimize register usage
- Generate valid assembly code

#### Register Allocator:

**File:** `CodeGeneration/RegisterAllocator.cs`

Uses liveness analysis to efficiently allocate registers.

**Available Registers:**
- `r0` to `r15` (16 general-purpose registers)

**Algorithm:**
1. Analyze the lifetime of each temporary variable
2. Determine when each variable is last used
3. Allocate registers by reusing ones that are no longer needed
4. Minimize the number of registers used

#### IR → Assembly Mapping:

| IR Instruction | Assembly | Description |
|--------------|---------------|-----------|
| `LoadConst t0, 42` | `mov r0 42` | Move constant to register |
| `Move t0, t1` | `mov r0 r1` | Move value between registers |
| `BinaryOp t2, t0, +, t1` | `add r2 r0 r1` | Addition |
| `BinaryOp t2, t0, -, t1` | `sub r2 r0 r1` | Subtraction |
| `BinaryOp t2, t0, *, t1` | `mul r2 r0 r1` | Multiplication |
| `BinaryOp t2, t0, /, t1` | `div r2 r0 r1` | Division |
| `Compare t3, t0, >=, t1` | `sge r3 r0 r1` | Set if Greater or Equal |
| `Compare t3, t0, >, t1` | `sgt r3 r0 r1` | Set if Greater Than |
| `Compare t3, t0, <=, t1` | `sle r3 r0 r1` | Set if Less or Equal |
| `Compare t3, t0, <, t1` | `slt r3 r0 r1` | Set if Less Than |
| `Compare t3, t0, ==, t1` | `seq r3 r0 r1` | Set if Equal |
| `Compare t3, t0, !=, t1` | `sne r3 r0 r1` | Set if Not Equal |
| `LogicalAnd t4, t0, t1` | `mul r4 r0 r1` | Logical AND (using multiplication) |
| `LogicalOr t5, t0, t1` | `add r5 r0 r1; sgt r5 r5 0` | Logical OR |
| `LoadFromDevice t0, d0, Temperature` | `l r0 d0 Temperature` | Load device property |
| `StoreToDevice d0, On, t0` | `s d0 On r0` | Store value to property |
| `BranchIfZero t0, L0` | `beqz r0 L0` | Branch if register == 0 |
| `Jump L1` | `j L1` | Unconditional jump |
| `Label L0` | `L0:` | Define a label |
| `Yield` | `yield` | Pause execution for one tick |

---

## 🔧 Developer Guide

This section is a complete guide to extend the language with new features.

---

### Add New Token

To add support for a new symbol or keyword in the language.

#### Step 1: Add the token type in `TokenType.cs`

```csharp
public enum TokenType
{
    // ...existing types...
    NewType,  // Add here
}
```

#### Step 2: Create the token class

Create a new file in `Lexer/Tokens/NewToken.cs`:

```csharp
namespace Compiler.Lexer.Tokens;

public sealed class NewToken : Token
{
    public NewToken(int position) 
        : base(TokenType.NewType, position, "symbol")
    {
    }
}
```

#### Step 3: Add recognition in the Lexer

Edit `Lexer/Lexer.cs`:

**For keyword:**
```csharp
private Token ReadIdentifierOrKeyword()
{
    var start = _pos;
    while (char.IsLetterOrDigit(Current) || Current == '_')
        Advance();

    var text = _src[start.._pos];
    return text switch
    {
        // ...existing keywords...
        "newKeyword" => new NewToken(start),
        _ => new IdentifierToken(text, start)
    };
}
```

**For symbol:**
```csharp
public Token NextToken()
{
    // ...existing code...
    
    return Current switch
    {
        // ...existing cases...
        '@' => ConsumeAnd(new NewToken(_pos)),  // Example: @
        // ...
    };
}
```

**For multi-character symbol:**
```csharp
private Token HandleNewSymbol()
{
    var start = _pos;
    Advance(); // Consume first character
    
    if (Current == '=')  // Example: @=
    {
        Advance();
        return new NewToken(start);
    }
    
    return new LexerErrorToken(start);
}
```

#### Step 4: Test

```csharp
var code = "newKeyword @ value";
var lexer = new StationeerLexer(code);
lexer.DebugTokens();
```

---

### Add New Expression

To add a new type of expression (e.g., ternary operator, array access, etc).

#### Step 1: Create the expression class

Create a file in `Domain/Ast/Expressions/NewExpr.cs`:

```csharp
namespace Compiler.Domain.Ast.Expressions;

public sealed class NewExpr : Expr
{
    public Expr Operand { get; }
    public string Info { get; }
    
    public NewExpr(Expr operand, string info)
    {
        Operand = operand;
        Info = info;
    }
    
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitNew(this);
    
    public override string ToString()
        => $"NewExpr {{ Operand = {Operand}, Info = {Info} }}";
}
```

#### Step 2: Add to visitor

Edit `Domain/Ast/Visitors/IExprVisitor.cs`:

```csharp
public interface IExprVisitor<T>
{
    // ...existing methods...
    T VisitNew(NewExpr expr);
}
```

#### Step 3: Add parsing

Edit `Parser/Parser.cs`:

```csharp
private Expr ParsePrimary()
{
    // ...existing code...
    
    if (CurrentToken is NewToken)
    {
        Eat(TokenType.NewType);
        var operand = ParseExpression();
        // ...additional logic...
        return new NewExpr(operand, "info");
    }
    
    // ...rest of existing code...
}
```

#### Step 4: Implement semantic analysis

Edit `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public TypeInfo VisitNew(NewExpr expr)
{
    var operandType = expr.Operand.Accept(this);
    
    // Necessary validations
    if (operandType.Kind != TypeKind.Int)
        return TypeInfo.Error;
    
    return TypeInfo.Int; // or other appropriate type
}
```

#### Step 5: Implement IR generation

Edit `CodeGeneration/IrBuilder.cs`:

```csharp
public string VisitNew(NewExpr expr)
{
    var operand = expr.Operand.Accept(this);
    var result = Temp();
    
    _instructions.Add(new NewInstruction(result, operand, expr.Info));
    
    return result;
}
```

#### Step 6: Create IR instruction (if needed)

Create `Domain/IR/NewInstruction.cs`:

```csharp
namespace Compiler.Domain.IR;

public sealed class NewInstruction : IrInstruction
{
    public string Target { get; }
    public string Operand { get; }
    public string Info { get; }
    
    public NewInstruction(string target, string operand, string info)
    {
        Target = target;
        Operand = operand;
        Info = info;
    }
    
    public override string ToString()
        => $"{Target} = new {Operand} ({Info})";
}
```

#### Step 7: Implement code generation

Edit `CodeGeneration/CodeGenerator.cs`:

```csharp
public IReadOnlyList<string> Generate(IrProgram program)
{
    // ...existing code...
    
    switch (instr)
    {
        // ...existing cases...
        case NewInstruction newInstr:
            EmitNew(newInstr, i, regAlloc, lines);
            break;
    }
}

private static void EmitNew(
    NewInstruction newInstr,
    int instrIndex,
    RegisterAllocator regAlloc,
    List<string> lines)
{
    var target = regAlloc.GetRegister(newInstr.Target, instrIndex);
    var operand = ResolveOperand(newInstr.Operand, instrIndex, regAlloc);
    
    // Generate appropriate assembly
    lines.Add($"new {target} {operand} # {newInstr.Info}");
}
```

---

### Add New Statement

To add a new control structure (e.g., while, for, switch).

#### Step 1: Create the statement class

Create `Domain/Ast/Statements/NewStmt.cs`:

```csharp
namespace Compiler.Domain.Ast.Statements;

public sealed class NewStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt Body { get; }
    
    public NewStmt(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }
    
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitNew(this);
    
    public override string ToString()
        => $"NewStmt {{ Condition = {Condition}, Body = {Body} }}";
}
```

#### Step 2: Add to visitor

Edit `Domain/Ast/Visitors/IStmtVisitor.cs`:

```csharp
public interface IStmtVisitor<T>
{
    // ...existing methods...
    T VisitNew(NewStmt stmt);
}
```

#### Step 3: Add parsing

Edit `Parser/Parser.cs`:

```csharp
private Stmt ParseStatement()
{
    return CurrentToken switch
    {
        // ...existing cases...
        NewKeywordToken => ParseNewStatement(),
        _ => ParseExprStatement()
    };
}

private Stmt ParseNewStatement()
{
    Eat(TokenType.NewKeyword);
    Eat(TokenType.LParen);
    var condition = ParseExpression();
    Eat(TokenType.RParen);
    var body = ParseStatement();
    
    return new NewStmt(condition, body);
}
```

#### Step 4: Implement semantic analysis

Edit `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public TypeInfo VisitNew(NewStmt stmt)
{
    var conditionType = stmt.Condition.Accept(this);
    
    // Condition validation
    if (conditionType.Kind != TypeKind.Int)
        throw new Exception("Condition must be an integer");
    
    stmt.Body.Accept(this);
    
    return TypeInfo.Int;
}
```

#### Step 5: Implement IR generation

Edit `CodeGeneration/IrBuilder.cs`:

```csharp
public object VisitNew(NewStmt stmt)
{
    var loopLabel = Label();    // L0
    var endLabel = Label();     // L1
    
    // Loop label
    _instructions.Add(new LabelInstruction(loopLabel));
    
    // Condition
    var condition = stmt.Condition.Accept(this);
    _instructions.Add(new BranchIfZeroInstruction(condition, endLabel));
    
    // Body
    stmt.Body.Accept(this);
    
    // Jump back to start
    _instructions.Add(new JumpInstruction(loopLabel));
    
    // End label
    _instructions.Add(new LabelInstruction(endLabel));
    
    return new object();
}
```

---

### Add New Operator

To add a new operator (e.g., modulo %, exponentiation **, etc).

#### Step 1: Add token (if needed)

If the operator uses a new symbol, follow the steps in [Add New Token](#add-new-token).

#### Step 2: Add to parser

Edit `Parser/Parser.cs`:

```csharp
private Expr ParseFactor()
{
    var left = ParseUnary();
    
    while (CurrentToken.Type is TokenType.Multiply 
                              or TokenType.Divide 
                              or TokenType.Modulo)  // New operator
    {
        var op = CurrentToken.Value;
        Eat(CurrentToken.Type);
        var right = ParseUnary();
        left = new BinaryExpr(left, op, right);
    }
    
    return left;
}
```

#### Step 3: Implement in IrBuilder

Edit `CodeGeneration/IrBuilder.cs`:

```csharp
public string VisitBinary(BinaryExpr expr)
{
    var l = expr.Left.Accept(this);
    var r = expr.Right.Accept(this);
    var t = Temp();

    switch (expr.Op)
    {
        // ...existing operators...
        case "%":
            _instructions.Add(new BinaryOpInstruction(t, l, "%", r));
            break;
        default:
            throw new Exception($"Unknown operator: {expr.Op}");
    }

    return t;
}
```

#### Step 4: Implement in CodeGenerator

Edit `CodeGeneration/CodeGenerator.cs`:

```csharp
private static void EmitBinary(
    BinaryOpInstruction bo,
    int instrIndex,
    RegisterAllocator regAlloc,
    List<string> lines)
{
    var target = regAlloc.GetRegister(bo.Target, instrIndex);
    var left = ResolveOperand(bo.Left, instrIndex, regAlloc);
    var right = ResolveOperand(bo.Right, instrIndex, regAlloc);

    var instruction = bo.Op switch
    {
        // ...existing operators...
        "%" => "mod",  // Instruction for modulo
        _ => throw new Exception($"Unknown binary operator: {bo.Op}")
    };

    lines.Add($"{instruction} {target} {left} {right}");
}
```

---

### Add New Built-in Function

To add a new globally available function.

#### Step 1: Register in SemanticAnalyzer

Edit `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public SemanticAnalyzer()
{
    // ...existing functions...
    
    // Example: min(a, b) returns the smaller value
    _symbols.DeclareFunction(
        "min", 
        new List<TypeInfo> { TypeInfo.Int, TypeInfo.Int },  // Parameters
        TypeInfo.Int                                         // Return
    );
}
```

#### Step 2: Add specific validation (if needed)

```csharp
public TypeInfo VisitCall(CallExpr expr)
{
    // ...existing code...
    
    switch (id.Name)
    {
        // ...existing cases...
        case "min":
            // Specific validations for min, if needed
            break;
    }
    
    // ...rest of existing code...
}
```

#### Step 3: Implement in IrBuilder

Edit `CodeGeneration/IrBuilder.cs`:

```csharp
public string VisitCall(CallExpr expr)
{
    if (expr.Callee is not IdentifierExpr id)
        throw new Exception("Invalid function call");

    switch (id.Name)
    {
        // ...existing cases...
        
        case "min":
        {
            var arg1 = expr.Arguments[0].Accept(this);
            var arg2 = expr.Arguments[1].Accept(this);
            var result = Temp();
            
            // Generate code for min using comparison
            var cmp = Temp();
            var trueLabel = Label();
            var endLabel = Label();
            
            _instructions.Add(new CompareInstruction(cmp, arg1, "<", arg2));
            _instructions.Add(new BranchIfZeroInstruction(cmp, trueLabel));
            _instructions.Add(new LoadConstInstruction(result, arg2));
            _instructions.Add(new JumpInstruction(endLabel));
            _instructions.Add(new LabelInstruction(trueLabel));
            _instructions.Add(new LoadConstInstruction(result, arg1));
            _instructions.Add(new LabelInstruction(endLabel));
            
            return result;
        }
        
        default:
            throw new Exception($"Unknown function: {id.Name}");
    }
}
```

#### Step 4: (Alternative) Create dedicated IR instruction

If the function is complex, create a specific IR instruction:

```csharp
// Domain/IR/MinInstruction.cs
public sealed class MinInstruction : IrInstruction
{
    public string Target { get; }
    public string Arg1 { get; }
    public string Arg2 { get; }
    
    public MinInstruction(string target, string arg1, string arg2)
    {
        Target = target;
        Arg1 = arg1;
        Arg2 = arg2;
    }
}
```

And implement the corresponding code generation.

---

### Add New IR Instruction

To create a new intermediate representation instruction.

#### Step 1: Create the instruction class

Create `Domain/IR/NewInstruction.cs`:

```csharp
namespace Compiler.Domain.IR;

public sealed class NewInstruction : IrInstruction
{
    public string Target { get; }
    public string Source { get; }
    public int Value { get; }
    
    public NewInstruction(string target, string source, int value)
    {
        Target = target;
        Source = source;
        Value = value;
    }
    
    public override string ToString()
        => $"{Target} = new {Source} {Value}";
}
```

#### Step 2: Use in IrBuilder

```csharp
// Where appropriate in IrBuilder.cs
_instructions.Add(new NewInstruction(target, source, value));
```

#### Step 3: Add to CodeGenerator

Edit `CodeGeneration/CodeGenerator.cs`:

```csharp
public IReadOnlyList<string> Generate(IrProgram program)
{
    // ...existing code...
    
    switch (instr)
    {
        // ...existing cases...
        case NewInstruction newInstr:
            EmitNew(newInstr, i, regAlloc, lines);
            break;
    }
}

private static void EmitNew(
    NewInstruction newInstr,
    int instrIndex,
    RegisterAllocator regAlloc,
    List<string> lines)
{
    var target = regAlloc.GetRegister(newInstr.Target, instrIndex);
    var source = ResolveOperand(newInstr.Source, instrIndex, regAlloc);
    
    lines.Add($"new {target} {source} {newInstr.Value}");
}
```

#### Step 4: Update RegisterAllocator (if needed)

If the instruction uses temporary variables in a special way, it may be necessary to update `RegisterAllocator.cs` to consider the lifetime of these variables.

---

## 📖 Language Documentation

### Supported Data Types

- **Int** - Integer numbers
- **Float** - Decimal numbers
- **Boolean** - Boolean values (true/false)
- **StationeersDevice** - Reference to game devices

### Language Features

#### Variable Declaration

```javascript
var x = 42;                                          // Type inference
Int temperature = 300;                               // Explicit type
Float pressure = 101325.0;
Boolean active = true;
StationeersDevice sensor = referenceDevice(d0);
```

#### Supported Operators

**Arithmetic:** `+`, `-`, `*`, `/`
**Comparison:** `==`, `!=`, `<`, `<=`, `>`, `>=`
**Logical:** `&&`, `||`

#### Control Structures

```javascript
if (temperature > 300) {
    // then code
} else {
    // else code
}
```

#### Device Property Access

```javascript
StationeersDevice sensor = referenceDevice(d0);
Float temp = sensor.Temperature;
sensor.On = true;
```

#### Built-in Functions

| Function | Parameters | Return | Description |
|--------|-----------|---------|-----------|
| `referenceDevice(device)` | device identifier | StationeersDevice | Creates a reference to a device (d0-d5, db) |
| `sleep()` | - | Void | Pauses execution for one tick (equivalent to `yield`) |
| `Math.convertToCelsius(kelvin)` | Int/Float | Int/Float | Converts temperature from Kelvin to Celsius (subtracts 273.15) |

### Current Limitations

- No loop support (while, for)
- No user-defined functions
- No arrays or complex data structures
- Global scope only (no nested variable scopes)

---

## 🔬 Technical References

### Design Patterns Used

1. **Visitor Pattern** - For AST traversal
2. **Builder Pattern** - For IR construction
3. **Strategy Pattern** - For register allocation

### Code Optimizations

The compiler implements optimizations in the intermediate representation (IR) through the **Strategy** pattern, allowing multiple optimization strategies to be applied in a modular and extensible way.

#### Available Optimization Strategies

##### 1. **ConstantFoldingOptimize**
Removes unnecessary `LoadConst` instructions when constants can be used directly in operations.

**Example:**
```
Before optimization:
  mov t2 20        # Load constant into temporary register
  sub t3 r0 t2     # Use temporary register
  mov r0 t3        # Move result to final register

After ConstantFoldingOptimize:
  sub t3 r0 20     # Use constant directly
  mov r0 t3        # Move result to final register
```

**How it works:**
1. Identifies all `LoadConst` instructions that load constants into temporary registers
2. Replaces references to these temporary registers with the direct constant value
3. Removes `LoadConst` instructions that are no longer needed

##### 2. **BinOpMoveOptimize**
Eliminates redundant pairs of binary operation followed by `Move` when the result can be stored directly in the destination register.

**Example:**
```
Before optimization:
  sub t3 r0 20     # Binary operation
  mov r0 t3        # Unnecessary move (destination = left operand)

After BinOpMoveOptimize:
  sub r0 r0 20     # Direct operation on final register
```

**How it works:**
1. Detects patterns where a `BinaryOpInstruction` is followed by a `MoveInstruction`
2. Checks if the move destination is the same as the binary operation's left operand
3. Combines both into a single instruction that operates directly on the final register

#### Optimization Pipeline

Strategies are applied in sequential order:

```csharp
private readonly List<IOptimizationStrategy> _strategies =
[
    new ConstantFoldingOptimize(),  // 1st: Eliminate unnecessary LoadConsts
    new BinOpMoveOptimize(),         // 2nd: Eliminate redundant Moves
];
```

**Complete Example:**
```javascript
// Source code
var a = 10;
a -= 20;

// Unoptimized IR
mov t0 10        # LoadConst
mov t1 20        # LoadConst
sub t2 t0 t1     # BinaryOp
mov t0 t2        # Move

// After ConstantFoldingOptimize
sub t2 t0 20     # Inline constant

// After BinOpMoveOptimize (final result)
sub t0 t0 20     # Optimized direct operation
```

#### Extensibility

To add a new optimization strategy:

1. Create a class that implements `IOptimizationStrategy`
2. Implement the `Apply(IrProgram program)` method
3. Add the strategy to the list in `IrOptimizer`

```csharp
public class MyNewOptimization : IOptimizationStrategy
{
    public IrProgram Apply(IrProgram program)
    {
        // Your optimization logic here
        return program;
    }
}
```

### Implemented Algorithms

1. **Recursive Descent Parsing** - Top-down parser
2. **Liveness Analysis** - Variable lifetime analysis
3. **Register Allocation** - Linear register allocation
4. **Constant Folding** - Compile-time constant optimization
5. **Dead Code Elimination** - Removal of unused instructions (via optimizations)

### Compilation Phases

```
Source Code
    ↓
[Lexical Analysis]     ← Regex/DFA
    ↓
Token Stream
    ↓
[Syntax Analysis]      ← Context-Free Grammar
    ↓
Abstract Syntax Tree
    ↓
[Semantic Analysis]    ← Type System, Symbol Table
    ↓
Annotated AST
    ↓
[IR Generation]        ← Three-Address Code
    ↓
Intermediate Representation
    ↓
[IR Optimization]      ← Peephole Optimization
    ↓
Optimized IR
    ↓
[Code Generation]      ← Register Allocation, Instruction Selection
    ↓
Assembly Code
```

### Dependency Structure

```
Program.cs
    ├── StationeerLexer
    ├── StationeerParser
    │   └── StationeerLexer
    ├── SemanticAnalyzer
    │   └── SymbolTable
    ├── IrBuilder
    ├── IrOptimizer (optional)
    └── CodeGenerator
        └── RegisterAllocator
```

---

## 🤝 Contributing

Contributions are welcome! To contribute to this project:

### 1. Development Environment Setup

- Clone the repository
- Install .NET 9.0 SDK
- Open the solution in your favorite IDE
- Build and run the tests

### 2. Add New Features

Consult the [Developer Guide](#developer-guide) for detailed instructions on:
- Adding new tokens and operators
- Implementing new expressions and statements
- Creating built-in functions
- Adding IR instructions

### 3. Best Practices

- Follow existing code patterns
- Write tests for new features
- Document complex changes
- Use descriptive commits
- Update documentation when necessary

### 4. Contribution Process

1. Fork the project
2. Create a branch for your feature (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m 'Add MyFeature'`)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

---

## 🔗 Useful Links

- [Stationeers Wiki](https://stationeers-wiki.com/)
- [Stationeers Official Website](https://store.steampowered.com/app/544550/Stationeers/)
- [IC10 Assembly Documentation](https://stationeers-wiki.com/IC10)

---

