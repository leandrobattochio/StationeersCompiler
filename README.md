# Stationeers Compiler

![Build and Deploy](https://github.com/leandrobattochio/StationeersCompiler/actions/workflows/build-and-deploy.yml/badge.svg)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![GitHub Pages](https://img.shields.io/badge/demo-live-success)](https://leandrobattochio.github.io/StationeersCompiler/)

Um compilador completo para a linguagem de programação do jogo Stationeers, desenvolvido em C# com .NET 9.0. Este compilador transforma código de alto nível em assembly que pode ser executado nos Integrated Circuits (ICs) do jogo.

> 🌐 **[Experimente agora em GitHub Pages](https://leandrobattochio.github.io/StationeersCompiler/)** - Interactive web interface with code editor!

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Requisitos](#requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Como Usar](#como-usar)
- [Arquitetura do Compilador](#arquitetura-do-compilador)
- [Pipeline de Compilação](#pipeline-de-compilação)
- [Componentes Detalhados](#componentes-detalhados)
  - [1. Lexer (Análise Léxica)](#1-lexer-análise-léxica)
  - [2. Parser (Análise Sintática)](#2-parser-análise-sintática)
  - [3. Semantic Analyzer (Análise Semântica)](#3-semantic-analyzer-análise-semântica)
  - [4. IR Builder (Representação Intermediária)](#4-ir-builder-representação-intermediária)
  - [5. IR Optimizer (Otimização)](#5-ir-optimizer-otimização)
  - [6. Code Generator (Geração de Código)](#6-code-generator-geração-de-código)
- [Guia do Desenvolvedor](#guia-do-desenvolvedor)
  - [Adicionar Novo Token](#adicionar-novo-token)
  - [Adicionar Nova Expressão](#adicionar-nova-expressão)
  - [Adicionar Novo Statement](#adicionar-novo-statement)
  - [Adicionar Novo Operador](#adicionar-novo-operador)
  - [Adicionar Nova Função Built-in](#adicionar-nova-função-built-in)
  - [Adicionar Nova Instrução IR](#adicionar-nova-instrução-ir)
- [Documentação da Linguagem](#documentação-da-linguagem)
- [Contribuindo](#contribuindo)
- [Links úteis](#-links-úteis)

---

## 🎯 Visão Geral

O **Stationeers Compiler** é um compilador de múltiplos estágios que converte código de alto nível em assembly otimizado para o jogo Stationeers. 

### Sobre o Projeto

Este projeto consiste em quatro componentes principais:

#### **Compiler.Core**
Biblioteca principal com toda a lógica de compilação, organizada em:
- `Lexer/` - Análise léxica e tokens
- `Parser/` - Análise sintática
- `SemanticAnalysis/` - Análise semântica e tabela de símbolos
- `CodeGeneration/` - Geração de IR, otimização e código assembly
- `Domain/` - Modelos de domínio (AST, IR, Types)

#### **CompilerCLI**
Interface de linha de comando para usar o compilador. Demonstra exemplos completos de compilação.

#### **CompilerWeb**
Interface web interativa desenvolvida em Blazor WebAssembly com:
- Editor de código com syntax highlighting (Monaco Editor)
- Visualização da AST gerada
- Output de assembly em tempo real
- Documentação embutida da linguagem

#### **Compiler.Tests**
Suite de testes unitários para validar a funcionalidade do compilador.

### Fases de Compilação

1. **Análise Léxica** (Tokenização)
2. **Análise Sintática** (Construção da AST)
3. **Análise Semântica** (Verificação de tipos)
4. **Geração de Representação Intermediária** (IR)
5. **Otimização IR** (Opcional)
6. **Geração de Código** (Assembly)

---

## ⚙️ Requisitos

- **.NET 9.0 SDK** ou superior
- **Sistema Operacional**: Windows, Linux ou macOS
- **IDE recomendada**: 
  - JetBrains Rider
  - Visual Studio 2022
  - Visual Studio Code com C# Dev Kit

---

## 🚀 Instalação e Configuração

### 1. Clonar o Repositório

```bash
git clone <repository-url>
cd StationeersCompiler
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Compilar o Projeto

```bash
dotnet build
```

### 4. Executar os Testes (se disponíveis)

```bash
dotnet test
```

---

## 💻 Como Usar

### CLI (Interface de Linha de Comando)

```bash
cd CompilerCLI
dotnet run
```

### Web Interface

```bash
cd CompilerWeb
dotnet run
```

Em seguida, abra seu navegador em `https://localhost:5001` (ou a porta indicada no terminal).

### Como Biblioteca

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

**Assembly gerado:**
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

### Exemplo Rápido

**Entrada (Código de Alto Nível):**
```javascript
StationeersDevice device = referenceDevice(d0);
Float temp = device.Temperature;
if (temp >= 300) {
    device.On = true;
} else {
    device.On = false;
}
```

**Saída (Assembly):**
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

## 📁 Estrutura do Projeto

```
StationeersCompiler/
├── Compiler.Core/                    # Biblioteca principal do compilador
│   ├── Lexer/                        # Análise léxica
│   │   ├── Lexer.cs                  # Scanner principal
│   │   └── Tokens/                   # Definições de tokens
│   ├── Parser/                       # Análise sintática
│   │   └── StationeerParser.cs       # Parser recursivo descendente
│   ├── SemanticAnalysis/             # Análise semântica
│   │   ├── SemanticAnalyzer.cs       # Verificador de tipos
│   │   └── SymbolTable.cs            # Tabela de símbolos
│   ├── CodeGeneration/               # Geração de código
│   │   ├── IrBuilder.cs              # Construtor de IR
│   │   ├── IrOptimizer.cs            # Otimizador de IR
│   │   ├── CodeGenerator.cs          # Gerador de assembly
│   │   └── RegisterAllocator.cs      # Alocador de registradores
│   └── Domain/                       # Modelos de domínio
│       ├── Ast/                      # Abstract Syntax Tree
│       │   ├── Expressions/          # Nós de expressão
│       │   ├── Statements/           # Nós de statement
│       │   └── Visitors/             # Interfaces visitor
│       ├── IR/                       # Intermediate Representation
│       │   └── *.Instruction.cs      # Instruções IR
│       └── Types/                    # Sistema de tipos
│           ├── TypeInfo.cs
│           ├── TypeKind.cs
│           └── MemberInfo.cs
├── CompilerCLI/                      # Interface de linha de comando
│   └── Program.cs
├── CompilerWeb/                      # Interface web Blazor
│   ├── Pages/
│   │   └── StCompiler.razor          # Página principal
│   ├── Components/
│   │   └── AstNodeComponent.razor    # Visualizador de AST
│   └── wwwroot/                      # Assets estáticos
├── Compiler.Tests/                   # Testes unitários
│   └── CompilerTests.cs
└── README.md                         # Este arquivo
```

---

## 🏗️ Arquitetura do Compilador

```
┌─────────────────┐
│  Código Fonte   │
└────────┬────────┘
         │
         ▼
┌───────────────────┐
│     LEXER         │ ◄── Converte texto em tokens
│ (StationeerLexer) │
└────────┬──────────┘
         │
         ▼
┌───────────────────────┐
│     PARSER            │ ◄── Constrói a árvore sintática (AST)
│  (StationeerParser)   │
└────────┬──────────────┘
         │
         ▼
┌─────────────────┐
│  SEMANTIC       │ ◄── Verifica tipos e símbolos
│  ANALYZER       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   IR BUILDER    │ ◄── Gera representação intermediária
│   (IrBuilder)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  IR OPTIMIZER   │ ◄── Otimiza código intermediário
│  (IrOptimizer)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  CODE GENERATOR │ ◄── Gera assembly final
│ (CodeGenerator) │
└────────┬────────┘
         │
         ▼
┌────────────┐
│  Assembly  │
└────────────┘
```

---

## 🔄 Pipeline de Compilação

### Fluxo Completo de Exemplo

**Código Fonte:**
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

**3. Análise Semântica:**
- Verifica se `referenceDevice` está declarada ✓
- Verifica se `d0` é um identificador de dispositivo válido ✓
- Valida tipo explícito `StationeersDevice` ✓
- Verifica se `Temperature` é uma propriedade válida de `StationeersDevice` ✓
- Verifica se `On` é uma propriedade válida e pode ser atribuída ✓
- Valida que `Math.convertToCelsius` existe e aceita Float/Int ✓
- Verifica tipos de operadores e comparações ✓

**4. IR (Representação Intermediária):**
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

**5. Assembly Final:**
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

## 🔍 Componentes Detalhados

### 1. Lexer (Análise Léxica)

**Arquivo:** `Lexer/Lexer.cs`

O Lexer (também chamado de Scanner ou Tokenizer) é responsável por converter o código fonte em uma sequência de tokens.

#### Responsabilidades:
- Ler caracteres do código fonte
- Identificar padrões léxicos (palavras-chave, identificadores, números, operadores)
- Criar tokens apropriados para cada padrão
- Ignorar espaços em branco
- Reportar erros léxicos

#### Tokens Suportados:

| Token | Descrição | Exemplo |
|-------|-----------|---------|
| `VarKeywordToken` | Palavra-chave var | `var` |
| `IfKeywordToken` | Palavra-chave if | `if` |
| `ElseKeywordToken` | Palavra-chave else | `else` |
| `TrueKeywordToken` | Palavra-chave true | `true` |
| `FalseKeywordToken` | Palavra-chave false | `false` |
| `IdentifierToken` | Identificador | `sensor`, `temp`, `d0` |
| `NumberToken` | Número inteiro | `42`, `300` |
| `FloatToken` | Número decimal | `27.5`, `101325.0` |
| `EqualsToken` | Atribuição | `=` |
| `EqualsEqualsToken` | Igualdade | `==` |
| `NotEqualsToken` | Diferença | `!=` |
| `LessThanToken` | Menor que | `<` |
| `LessThanOrEqualToken` | Menor ou igual | `<=` |
| `GreaterThanToken` | Maior que | `>` |
| `GreaterThanOrEqualToken` | Maior ou igual | `>=` |
| `LogicalAndToken` | E lógico | `&&` |
| `LogicalOrToken` | OU lógico | `||` |
| `PlusToken` | Adição | `+` |
| `MinusToken` | Subtração | `-` |
| `MultiplyToken` | Multiplicação | `*` |
| `DivideToken` | Divisão | `/` |
| `PlusEqualsToken` | Adição com atribuição | `+=` |
| `MinusEqualsToken` | Subtração com atribuição | `-=` |
| `MultiplyEqualsToken` | Multiplicação com atribuição | `*=` |
| `DivideEqualsToken` | Divisão com atribuição | `/=` |
| `PlusPlusToken` | Incremento | `++` |
| `MinusMinusToken` | Decremento | `--` |
| `LeftParentToken` | Parêntese esquerdo | `(` |
| `RightParentToken` | Parêntese direito | `)` |
| `LeftBraceToken` | Chave esquerda | `{` |
| `RightBraceToken` | Chave direita | `}` |
| `SemicolonToken` | Ponto e vírgula | `;` |
| `CommaToken` | Vírgula | `,` |
| `DotToken` | Ponto (acesso a membro) | `.` |
| `EndOfFileToken` | Fim do arquivo | (automático) |
| `LexerErrorToken` | Erro léxico | (erro) |
#### Estrutura de Token:

```csharp
public abstract class Token
{
    public TokenType Type { get; }
    public int Position { get; }
    public string Value { get; }
}
```

#### Exemplo de Uso:

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

### 2. Parser (Análise Sintática)

**Arquivo:** `Parser/StationeerParser.cs`

O Parser constrói uma Árvore Sintática Abstrata (AST) a partir da sequência de tokens.

#### Responsabilidades:
- Consumir tokens do Lexer
- Verificar a gramática da linguagem
- Construir nós da AST
- Reportar erros de sintaxe

#### Gramática da Linguagem:

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

#### Estrutura da AST:

**Expressões (Expr):**
- `NumberExpr` - Número inteiro literal
- `FloatExpr` - Número decimal literal
- `BooleanExpr` - Valor booleano (true/false)
- `IdentifierExpr` - Identificador
- `BinaryExpr` - Operação binária (+, -, *, /, ==, !=, <, >, <=, >=, &&, ||)
- `AssignmentExpr` - Atribuição a variável (=)
- `CompoundAssignmentExpr` - Atribuição composta (+=, -=, *=, /=)
- `IncrementDecrementExpr` - Incremento/decremento (++, --)
- `GroupExpr` - Expressão agrupada com parênteses
- `CallExpr` - Chamada de função
- `MemberAccessExpr` - Acesso a propriedade (objeto.propriedade)
- `MemberAssignmentExpr` - Atribuição a propriedade (objeto.propriedade = valor)
- `MethodCallExpr` - Chamada de método
- `StaticMethodCallExpr` - Chamada de método estático (ex: Math.convertToCelsius)
- `DeviceExpr` - Dispositivo
- `DevicePropertyExpr` - Propriedade de dispositivo
- `LoadDeviceExpr` - Carregamento de dispositivo

**Statements (Stmt):**
- `VarDeclarationStmt` - Declaração de variável (com ou sem tipo explícito)
- `ExprStmt` - Expressão como statement
- `BlockStmt` - Bloco de código delimitado por { }
- `IfStmt` - If/else

A AST usa o padrão Visitor para separar a estrutura dos dados das operações:

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

### 3. Semantic Analyzer (Análise Semântica)

**Arquivo:** `SemanticAnalysis/SemanticAnalyzer.cs`

O Semantic Analyzer verifica a corretude semântica do programa.

#### Responsabilidades:
- Verificação de tipos
- Gerenciamento de tabela de símbolos
- Validação de declarações e usos de variáveis
- Validação de chamadas de funções
- Validação de dispositivos Stationeers

#### Sistema de Tipos:

```csharp
public enum TypeKind
{
    Void,     // Tipo void (sem retorno)
    Int,      // Tipo inteiro
    Float,    // Tipo decimal
    Boolean,  // Tipo booleano
    Device,   // Tipo dispositivo Stationeers
    Error,    // Tipo de erro
    Function  // Tipo função
}

public class TypeInfo
{
    public TypeKind Kind { get; }
    public List<TypeInfo>? ParameterTypes { get; }  // Para funções
    public TypeInfo? ReturnType { get; }            // Para funções
    public Dictionary<string, MemberInfo>? Members { get; }  // Para objetos com propriedades
    
    public MemberInfo? GetMember(string memberName) => Members?.GetValueOrDefault(memberName);
    public static readonly TypeInfo Void = new(TypeKind.Void);
    public static readonly TypeInfo Int = new(TypeKind.Int);
    public static readonly TypeInfo Boolean = new(TypeKind.Boolean);
    public static readonly TypeInfo Float = new(TypeKind.Float);
    public static readonly TypeInfo Error = new(TypeKind.Error);
    public static readonly TypeInfo Device = new(TypeKind.Device);
}
```

#### Tabela de Símbolos:

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

#### Funções Built-in:

O analisador semântico registra as seguintes funções built-in:

| Função | Parâmetros | Retorno | Descrição                           |
|--------|-----------|---------|-------------------------------------|
| `referenceDevice` | (device: Device) | StationeersDevice | Cria referência para um dispositivo |
| `sleep` | () | Void | Equivalente a instrução `yield`      |

#### Métodos Estáticos:

| Classe.Método | Parâmetros | Retorno | Descrição |
|---------------|-----------|---------|-----------|
| `Math.convertToCelsius` | (temp: Int/Float) | Int/Float | Converte Kelvin para Celsius (subtrai 273.15) |

#### Validações Especiais:

**1. Dispositivos:**

Valida que identificadores de dispositivos seguem o padrão: `d0`, `d1`, `d2`, `d3`, `d4`, `d5`, `db`

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

**2. Verificação de Membros:**

Valida se propriedades e métodos existem no tipo do objeto antes de permitir acesso.

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

**3. Validação de Uso de Retorno:**

Garante que funções com retorno não sejam chamadas como statements sem usar o valor retornado.

```csharp
public TypeInfo VisitExprStmt(ExprStmt stmt)
{
    var resultType = stmt.Expression.Accept(this);
    
    // Se é uma chamada de função e retorna valor, erro!
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

**4. Rastreamento de Referências de Dispositivos:**

Mantém um dicionário que mapeia variáveis para seus dispositivos físicos.

```csharp
private readonly Dictionary<string, string> _deviceReferences = new();

// Ao processar: StationeersDevice sensor = referenceDevice(d0);
_deviceReferences["sensor"] = "d0";

// Depois, ao gerar código para: sensor.Temperature
// Sabemos que deve ser: l r0 d0 Temperature
```

---

### 4. IR Builder (Representação Intermediária)

**Arquivo:** `CodeGeneration/IrBuilder.cs`

O IR Builder converte a AST em uma representação intermediária mais próxima do assembly.

#### Responsabilidades:
- Converter AST em instruções IR
- Gerar variáveis temporárias
- Gerar labels para controle de fluxo
- Linearizar a estrutura de árvore

#### Instruções IR:

| Instrução | Descrição | Exemplo |
|-----------|-----------|---------|
| `LoadConstInstruction` | Carrega constante | `t0 = 42` |
| `BinaryOpInstruction` | Operação binária | `t2 = t0 + t1` |
| `CompareInstruction` | Comparação | `t3 = t0 >= t1` |
| `LogicalAndInstruction` | AND lógico | `t4 = t0 && t1` |
| `LogicalOrInstruction` | OR lógico | `t5 = t0 \|\| t1` |
| `LoadFromDeviceInstruction` | Carregar de dispositivo | `t6 = load d0.Temperature` |
| `StoreToDeviceInstruction` | Armazenar em dispositivo | `store d0.On = 1` |
| `BranchIfZeroInstruction` | Branch se zero | `if t0 == 0 goto L0` |
| `JumpInstruction` | Jump incondicional | `goto L1` |
| `LabelInstruction` | Label | `L0:` |

#### Estrutura do IrProgram:

```csharp
public class IrProgram
{
    public List<IrInstruction> Instructions { get; }
    public string ResultRegister { get; }
}
```

#### Geração de Temporários e Labels:

```csharp
private int _temp = 0;
private int _label = 0;

private string Temp() => $"t{_temp++}";  // t0, t1, t2, ...
private string Label() => $"L{_label++}"; // L0, L1, L2, ...
```

#### Exemplo de Conversão:

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

### 5. IR Optimizer (Otimização)

**Arquivo:** `CodeGeneration/IrOptimizer.cs`

O IR Optimizer realiza otimizações no código intermediário antes da geração de assembly.

#### Responsabilidades:
- Eliminar instruções redundantes
- Otimizar padrões comuns de código
- Reduzir número de instruções
- Melhorar eficiência do código gerado

#### Otimizações Implementadas:

**1. Eliminação de Move Redundante**

Detecta o padrão onde uma operação binária é seguida por um move para o mesmo registrador do operando esquerdo:

**Antes:**
```
t0 = load d0.Temperature    // Carrega em t0
t1 = t0 - 273.15            // Opera e armazena em t1
t0 = t1                     // Move t1 de volta para t0 (redundante!)
```

**Depois:**
```
t0 = load d0.Temperature    // Carrega em t0
t0 = t0 - 273.15            // Opera diretamente em t0
```

#### Estrutura do Optimizer:

O otimizador usa o **padrão Strategy** para aplicar múltiplas estratégias de otimização:

```csharp
public class IrOptimizer
{
    private readonly List<IOptimizationStrategy> _strategies =
    [
        new ConstantFoldingOptimize(),  // Elimina LoadConst desnecessários
        new BinOpMoveOptimize(),         // Elimina Moves redundantes
    ];

    public IrProgram Optimize(IrProgram program)
    {
        return _strategies.Aggregate(program, (current, strategy) => strategy.Apply(current));
    }
}
```

Cada estratégia implementa a interface `IOptimizationStrategy`:

```csharp
public interface IOptimizationStrategy
{
    IrProgram Apply(IrProgram program);
}
```

Para mais detalhes sobre as otimizações, consulte a seção [Otimizações de Código](#otimizações-de-código).

#### Uso:

```csharp
var irBuilder = new IrBuilder();
var ir = irBuilder.Build(ast);

var optimizer = new IrOptimizer();
var optimizedIr = optimizer.Optimize(ir);

var codegen = new CodeGenerator();
var assembly = codegen.Generate(optimizedIr);
```

---

### 6. Code Generator (Geração de Código)

**Arquivo:** `CodeGeneration/CodeGenerator.cs`

O Code Generator converte IR em assembly para Stationeers.

#### Responsabilidades:
- Converter instruções IR em assembly
- Alocar registradores físicos
- Otimizar uso de registradores
- Gerar código assembly válido

#### Register Allocator:

**Arquivo:** `CodeGeneration/RegisterAllocator.cs`

Usa análise de tempo de vida (liveness analysis) para alocar registradores de forma eficiente.

**Registradores Disponíveis:**
- `r0` até `r15` (16 registradores de propósito geral)

**Algoritmo:**
1. Analisa o tempo de vida de cada variável temporária
2. Determina quando cada variável é usada pela última vez
3. Aloca registradores reutilizando os que não são mais necessários
4. Minimiza o número de registradores usados

#### Mapeamento IR → Assembly:

| Instrução IR | Assembly | Descrição |
|--------------|---------------|-----------|
| `LoadConst t0, 42` | `mov r0 42` | Move constante para registrador |
| `Move t0, t1` | `mov r0 r1` | Move valor entre registradores |
| `BinaryOp t2, t0, +, t1` | `add r2 r0 r1` | Adição |
| `BinaryOp t2, t0, -, t1` | `sub r2 r0 r1` | Subtração |
| `BinaryOp t2, t0, *, t1` | `mul r2 r0 r1` | Multiplicação |
| `BinaryOp t2, t0, /, t1` | `div r2 r0 r1` | Divisão |
| `Compare t3, t0, >=, t1` | `sge r3 r0 r1` | Set if Greater or Equal |
| `Compare t3, t0, >, t1` | `sgt r3 r0 r1` | Set if Greater Than |
| `Compare t3, t0, <=, t1` | `sle r3 r0 r1` | Set if Less or Equal |
| `Compare t3, t0, <, t1` | `slt r3 r0 r1` | Set if Less Than |
| `Compare t3, t0, ==, t1` | `seq r3 r0 r1` | Set if Equal |
| `Compare t3, t0, !=, t1` | `sne r3 r0 r1` | Set if Not Equal |
| `LogicalAnd t4, t0, t1` | `mul r4 r0 r1` | AND lógico (usando multiplicação) |
| `LogicalOr t5, t0, t1` | `add r5 r0 r1; sgt r5 r5 0` | OR lógico |
| `LoadFromDevice t0, d0, Temperature` | `l r0 d0 Temperature` | Carrega propriedade do dispositivo |
| `StoreToDevice d0, On, t0` | `s d0 On r0` | Armazena valor em propriedade |
| `BranchIfZero t0, L0` | `beqz r0 L0` | Branch se registrador == 0 |
| `Jump L1` | `j L1` | Jump incondicional |
| `Label L0` | `L0:` | Define um label |
| `Yield` | `yield` | Pausa execução por um tick |

---

## 🔧 Guia do Desenvolvedor

Esta seção é um guia completo para estender a linguagem com novos recursos.

---

### Adicionar Novo Token

Para adicionar suporte a um novo símbolo ou palavra-chave na linguagem.

#### Passo 1: Adicionar o tipo do token em `TokenType.cs`

```csharp
public enum TokenType
{
    // ...existing types...
    NovoTipo,  // Adicione aqui
}
```

#### Passo 2: Criar a classe do token

Crie um novo arquivo em `Lexer/Tokens/NovoToken.cs`:

```csharp
namespace Compiler.Lexer.Tokens;

public sealed class NovoToken : Token
{
    public NovoToken(int position) 
        : base(TokenType.NovoTipo, position, "simbolo")
    {
    }
}
```

#### Passo 3: Adicionar reconhecimento no Lexer

Edite `Lexer/Lexer.cs`:

**Para palavra-chave:**
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
        "novaPalavra" => new NovoToken(start),
        _ => new IdentifierToken(text, start)
    };
}
```

**Para símbolo:**
```csharp
public Token NextToken()
{
    // ...existing code...
    
    return Current switch
    {
        // ...existing cases...
        '@' => ConsumeAnd(new NovoToken(_pos)),  // Exemplo: @
        // ...
    };
}
```

**Para símbolo de múltiplos caracteres:**
```csharp
private Token HandleNovoSimbolo()
{
    var start = _pos;
    Advance(); // Consome primeiro caractere
    
    if (Current == '=')  // Exemplo: @=
    {
        Advance();
        return new NovoToken(start);
    }
    
    return new LexerErrorToken(start);
}
```

#### Passo 4: Testar

```csharp
var code = "novaPalavra @ valor";
var lexer = new StationeerLexer(code);
lexer.DebugTokens();
```

---

### Adicionar Nova Expressão

Para adicionar um novo tipo de expressão (ex: operador ternário, acesso a array, etc).

#### Passo 1: Criar a classe da expressão

Crie um arquivo em `Domain/Ast/Expressions/NovaExpr.cs`:

```csharp
namespace Compiler.Domain.Ast.Expressions;

public sealed class NovaExpr : Expr
{
    public Expr Operando { get; }
    public string Info { get; }
    
    public NovaExpr(Expr operando, string info)
    {
        Operando = operando;
        Info = info;
    }
    
    public override T Accept<T>(IExprVisitor<T> visitor)
        => visitor.VisitNova(this);
    
    public override string ToString()
        => $"NovaExpr {{ Operando = {Operando}, Info = {Info} }}";
}
```

#### Passo 2: Adicionar ao visitor

Edite `Domain/Ast/Visitors/IExprVisitor.cs`:

```csharp
public interface IExprVisitor<T>
{
    // ...existing methods...
    T VisitNova(NovaExpr expr);
}
```

#### Passo 3: Adicionar parsing

Edite `Parser/Parser.cs`:

```csharp
private Expr ParsePrimary()
{
    // ...existing code...
    
    if (CurrentToken is NovoToken)
    {
        Eat(TokenType.NovoTipo);
        var operando = ParseExpression();
        // ...lógica adicional...
        return new NovaExpr(operando, "info");
    }
    
    // ...rest of existing code...
}
```

#### Passo 4: Implementar análise semântica

Edite `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public TypeInfo VisitNova(NovaExpr expr)
{
    var operandType = expr.Operando.Accept(this);
    
    // Validações necessárias
    if (operandType.Kind != TypeKind.Int)
        return TypeInfo.Error;
    
    return TypeInfo.Int; // ou outro tipo apropriado
}
```

#### Passo 5: Implementar geração de IR

Edite `CodeGeneration/IrBuilder.cs`:

```csharp
public string VisitNova(NovaExpr expr)
{
    var operand = expr.Operando.Accept(this);
    var result = Temp();
    
    _instructions.Add(new NovaInstruction(result, operand, expr.Info));
    
    return result;
}
```

#### Passo 6: Criar instrução IR (se necessário)

Crie `Domain/IR/NovaInstruction.cs`:

```csharp
namespace Compiler.Domain.IR;

public sealed class NovaInstruction : IrInstruction
{
    public string Target { get; }
    public string Operand { get; }
    public string Info { get; }
    
    public NovaInstruction(string target, string operand, string info)
    {
        Target = target;
        Operand = operand;
        Info = info;
    }
    
    public override string ToString()
        => $"{Target} = nova {Operand} ({Info})";
}
```

#### Passo 7: Implementar geração de código

Edite `CodeGeneration/CodeGenerator.cs`:

```csharp
public IReadOnlyList<string> Generate(IrProgram program)
{
    // ...existing code...
    
    switch (instr)
    {
        // ...existing cases...
        case NovaInstruction nova:
            EmitNova(nova, i, regAlloc, lines);
            break;
    }
}

private static void EmitNova(
    NovaInstruction nova,
    int instrIndex,
    RegisterAllocator regAlloc,
    List<string> lines)
{
    var target = regAlloc.GetRegister(nova.Target, instrIndex);
    var operand = ResolveOperand(nova.Operand, instrIndex, regAlloc);
    
    // Gere o assembly apropriado
    lines.Add($"nova {target} {operand} # {nova.Info}");
}
```

---

### Adicionar Novo Statement

Para adicionar uma nova estrutura de controle (ex: while, for, switch).

#### Passo 1: Criar a classe do statement

Crie `Domain/Ast/Statements/NovoStmt.cs`:

```csharp
namespace Compiler.Domain.Ast.Statements;

public sealed class NovoStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt Body { get; }
    
    public NovoStmt(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }
    
    public override T Accept<T>(IStmtVisitor<T> visitor)
        => visitor.VisitNovo(this);
    
    public override string ToString()
        => $"NovoStmt {{ Condition = {Condition}, Body = {Body} }}";
}
```

#### Passo 2: Adicionar ao visitor

Edite `Domain/Ast/Visitors/IStmtVisitor.cs`:

```csharp
public interface IStmtVisitor<T>
{
    // ...existing methods...
    T VisitNovo(NovoStmt stmt);
}
```

#### Passo 3: Adicionar parsing

Edite `Parser/Parser.cs`:

```csharp
private Stmt ParseStatement()
{
    return CurrentToken switch
    {
        // ...existing cases...
        NovoKeywordToken => ParseNovoStatement(),
        _ => ParseExprStatement()
    };
}

private Stmt ParseNovoStatement()
{
    Eat(TokenType.NovoKeyword);
    Eat(TokenType.LParen);
    var condition = ParseExpression();
    Eat(TokenType.RParen);
    var body = ParseStatement();
    
    return new NovoStmt(condition, body);
}
```

#### Passo 4: Implementar análise semântica

Edite `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public TypeInfo VisitNovo(NovoStmt stmt)
{
    var conditionType = stmt.Condition.Accept(this);
    
    // Validação da condição
    if (conditionType.Kind != TypeKind.Int)
        throw new Exception("Condition must be an integer");
    
    stmt.Body.Accept(this);
    
    return TypeInfo.Int;
}
```

#### Passo 5: Implementar geração de IR

Edite `CodeGeneration/IrBuilder.cs`:

```csharp
public object VisitNovo(NovoStmt stmt)
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

### Adicionar Novo Operador

Para adicionar um novo operador (ex: módulo %, exponenciação **, etc).

#### Passo 1: Adicionar token (se necessário)

Se o operador usa um novo símbolo, siga os passos de [Adicionar Novo Token](#adicionar-novo-token).

#### Passo 2: Adicionar ao parser

Edite `Parser/Parser.cs`:

```csharp
private Expr ParseFactor()
{
    var left = ParseUnary();
    
    while (CurrentToken.Type is TokenType.Multiply 
                              or TokenType.Divide 
                              or TokenType.Modulo)  // Novo operador
    {
        var op = CurrentToken.Value;
        Eat(CurrentToken.Type);
        var right = ParseUnary();
        left = new BinaryExpr(left, op, right);
    }
    
    return left;
}
```

#### Passo 3: Implementar em IrBuilder

Edite `CodeGeneration/IrBuilder.cs`:

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

#### Passo 4: Implementar em CodeGenerator

Edite `CodeGeneration/CodeGenerator.cs`:

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
        "%" => "mod",  // Instrução para módulo
        _ => throw new Exception($"Unknown binary operator: {bo.Op}")
    };

    lines.Add($"{instruction} {target} {left} {right}");
}
```

---

### Adicionar Nova Função Built-in

Para adicionar uma nova função disponível globalmente.

#### Passo 1: Registrar no SemanticAnalyzer

Edite `SemanticAnalysis/SemanticAnalyzer.cs`:

```csharp
public SemanticAnalyzer()
{
    // ...existing functions...
    
    // Exemplo: min(a, b) retorna o menor valor
    _symbols.DeclareFunction(
        "min", 
        new List<TypeInfo> { TypeInfo.Int, TypeInfo.Int },  // Parâmetros
        TypeInfo.Int                                         // Retorno
    );
}
```

#### Passo 2: Adicionar validação específica (se necessário)

```csharp
public TypeInfo VisitCall(CallExpr expr)
{
    // ...existing code...
    
    switch (id.Name)
    {
        // ...existing cases...
        case "min":
            // Validações específicas para min, se necessário
            break;
    }
    
    // ...rest of existing code...
}
```

#### Passo 3: Implementar em IrBuilder

Edite `CodeGeneration/IrBuilder.cs`:

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
            
            // Gera código para min usando comparação
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

#### Passo 4: (Alternativa) Criar instrução IR dedicada

Se a função for complexa, crie uma instrução IR específica:

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

E implemente a geração de código correspondente.

---

### Adicionar Nova Instrução IR

Para criar uma nova instrução de representação intermediária.

#### Passo 1: Criar a classe da instrução

Crie `Domain/IR/NovaInstruction.cs`:

```csharp
namespace Compiler.Domain.IR;

public sealed class NovaInstruction : IrInstruction
{
    public string Target { get; }
    public string Source { get; }
    public int Value { get; }
    
    public NovaInstruction(string target, string source, int value)
    {
        Target = target;
        Source = source;
        Value = value;
    }
    
    public override string ToString()
        => $"{Target} = nova {Source} {Value}";
}
```

#### Passo 2: Usar no IrBuilder

```csharp
// Onde for apropriado em IrBuilder.cs
_instructions.Add(new NovaInstruction(target, source, value));
```

#### Passo 3: Adicionar ao CodeGenerator

Edite `CodeGeneration/CodeGenerator.cs`:

```csharp
public IReadOnlyList<string> Generate(IrProgram program)
{
    // ...existing code...
    
    switch (instr)
    {
        // ...existing cases...
        case NovaInstruction nova:
            EmitNova(nova, i, regAlloc, lines);
            break;
    }
}

private static void EmitNova(
    NovaInstruction nova,
    int instrIndex,
    RegisterAllocator regAlloc,
    List<string> lines)
{
    var target = regAlloc.GetRegister(nova.Target, instrIndex);
    var source = ResolveOperand(nova.Source, instrIndex, regAlloc);
    
    lines.Add($"nova {target} {source} {nova.Value}");
}
```

#### Passo 4: Atualizar RegisterAllocator (se necessário)

Se a instrução usa variáveis temporárias de forma especial, pode ser necessário atualizar o `RegisterAllocator.cs` para considerar o tempo de vida dessas variáveis.

---

## 📖 Documentação da Linguagem

### Tipos de Dados Suportados

- **Int** - Números inteiros
- **Float** - Números decimais
- **Boolean** - Valores booleanos (true/false)
- **StationeersDevice** - Referência a dispositivos do jogo

### Recursos da Linguagem

#### Declaração de Variáveis

```javascript
var x = 42;                                          // Inferência de tipo
Int temperatura = 300;                               // Tipo explícito
Float pressao = 101325.0;
Boolean ligado = true;
StationeersDevice sensor = referenceDevice(d0);
```

#### Operadores Suportados

**Aritméticos:** `+`, `-`, `*`, `/`
**Comparação:** `==`, `!=`, `<`, `<=`, `>`, `>=`
**Lógicos:** `&&`, `||`
**Atribuição Composta:** `+=`, `-=`, `*=`, `/=`
**Incremento/Decremento:** `++`, `--` (prefixo e sufixo)

#### Operadores de Atribuição Composta

Os operadores compostos permitem realizar uma operação aritmética e atribuição de forma mais concisa:

```javascript
var a = 10;
a += 5;   // equivalente a: a = a + 5;  (a = 15)
a -= 3;   // equivalente a: a = a - 3;  (a = 12)
a *= 2;   // equivalente a: a = a * 2;  (a = 24)
a /= 4;   // equivalente a: a = a / 4;  (a = 6)
```

#### Operadores de Incremento e Decremento

```javascript
var contador = 0;

// Pós-incremento/decremento (usa o valor atual, depois incrementa)
contador++;  // equivalente a: contador = contador + 1;
contador--;  // equivalente a: contador = contador - 1;

// Pré-incremento/decremento (incrementa, depois usa o valor)
++contador;  // equivalente a: contador = contador + 1;
--contador;  // equivalente a: contador = contador - 1;
```

**Nota:** Tanto a forma prefixo quanto sufixo incrementam/decrementam a variável. A diferença no valor retornado só é relevante quando usados em expressões, mas atualmente o compilador trata ambos de forma equivalente em statements.

#### Estruturas de Controle

```javascript
if (temperatura > 300) {
    // código then
} else {
    // código else
}
```

#### Acesso a Propriedades de Dispositivos

```javascript
StationeersDevice sensor = referenceDevice(d0);
Float temp = sensor.Temperature;
sensor.On = true;
```

#### Funções Built-in

| Função | Parâmetros | Retorno | Descrição |
|--------|-----------|---------|-----------|
| `referenceDevice(device)` | device identifier | StationeersDevice | Cria referência para um dispositivo (d0-d5, db) |
| `sleep()` | - | Void | Pausa a execução por um tick (equivalente a `yield`) |
| `Math.convertToCelsius(kelvin)` | Int/Float | Int/Float | Converte temperatura de Kelvin para Celsius (subtrai 273.15) |

### Limitações Atuais

- Não suporta loops (while, for)
- Não suporta funções definidas pelo usuário
- Não suporta arrays ou estruturas de dados complexas
- Escopo global apenas (sem blocos de escopo aninhados)

---

## 🔬 Referências Técnicas

### Padrões de Design Utilizados

1. **Visitor Pattern** - Para traversal da AST
2. **Builder Pattern** - Para construção de IR
3. **Strategy Pattern** - Para otimizações de IR e alocação de registradores

### Otimizações de Código

O compilador implementa otimizações na representação intermediária (IR) através do padrão **Strategy**, permitindo aplicar múltiplas estratégias de otimização de forma modular e extensível.

#### Estratégias de Otimização Disponíveis

##### 1. **ConstantFoldingOptimize**
Remove instruções `LoadConst` desnecessárias quando constantes podem ser usadas diretamente nas operações.

**Exemplo:**
```
Antes da otimização:
  mov t2 20        # Carrega constante em registrador temporário
  sub t3 r0 t2     # Usa o registrador temporário
  mov r0 t3        # Move resultado para registrador final

Após ConstantFoldingOptimize:
  sub t3 r0 20     # Usa constante diretamente
  mov r0 t3        # Move resultado para registrador final
```

**Como funciona:**
1. Identifica todas as instruções `LoadConst` que carregam constantes em registradores temporários
2. Substitui referências a esses registradores temporários pelo valor constante direto
3. Remove as instruções `LoadConst` que não são mais necessárias

##### 2. **BinOpMoveOptimize**
Elimina pares redundantes de operação binária seguida de `Move` quando o resultado pode ser armazenado diretamente no registrador de destino.

**Exemplo:**
```
Antes da otimização:
  sub t3 r0 20     # Operação binária
  mov r0 t3        # Move desnecessário (destino = operando esquerdo)

Após BinOpMoveOptimize:
  sub r0 r0 20     # Operação direta no registrador final
```

**Como funciona:**
1. Detecta padrões onde uma `BinaryOpInstruction` é seguida por uma `MoveInstruction`
2. Verifica se o destino do `Move` é o mesmo que o operando esquerdo da operação binária
3. Combina ambas em uma única instrução que opera diretamente no registrador final

#### Pipeline de Otimização

As estratégias são aplicadas em ordem sequencial:

```csharp
private readonly List<IOptimizationStrategy> _strategies =
[
    new ConstantFoldingOptimize(),  // 1º: Elimina LoadConst desnecessários
    new BinOpMoveOptimize(),         // 2º: Elimina Moves redundantes
];
```

**Exemplo Completo:**
```javascript
// Código fonte
var a = 10;
a -= 20;

// IR não otimizado
mov t0 10        # LoadConst
mov t1 20        # LoadConst
sub t2 t0 t1     # BinaryOp
mov t0 t2        # Move

// Após ConstantFoldingOptimize
sub t2 t0 20     # Constante inline

// Após BinOpMoveOptimize (resultado final)
sub t0 t0 20     # Operação direta otimizada
```

#### Extensibilidade

Para adicionar uma nova estratégia de otimização:

1. Crie uma classe que implemente `IOptimizationStrategy`
2. Implemente o método `Apply(IrProgram program)`
3. Adicione a estratégia à lista em `IrOptimizer`

```csharp
public class MinhaNovaOtimizacao : IOptimizationStrategy
{
    public IrProgram Apply(IrProgram program)
    {
        // Sua lógica de otimização aqui
        return program;
    }
}
```

### Algoritmos Implementados

1. **Recursive Descent Parsing** - Parser top-down
2. **Liveness Analysis** - Análise de tempo de vida de variáveis
3. **Register Allocation** - Alocação linear de registradores
4. **Constant Folding** - Otimização de constantes em tempo de compilação
5. **Dead Code Elimination** - Remoção de instruções não utilizadas (via otimizações)

### Fases de Compilação

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

### Estrutura de Dependências

```
Program.cs
    ├── StationeerLexer
    ├── StationeerParser
    │   └── StationeerLexer
    ├── SemanticAnalyzer
    │   └── SymbolTable
    ├── IrBuilder
    ├── IrOptimizer (opcional)
    └── CodeGenerator
        └── RegisterAllocator
```

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir com este projeto:

### 1. Setup do Ambiente de Desenvolvimento

- Clone o repositório
- Instale o .NET 9.0 SDK
- Abra a solution no seu IDE favorito
- Compile e execute os testes

### 2. Adicionar Novas Funcionalidades

Consulte o [Guia do Desenvolvedor](#guia-do-desenvolvedor) para instruções detalhadas sobre:
- Adicionar novos tokens e operadores
- Implementar novas expressões e statements
- Criar funções built-in
- Adicionar instruções IR

### 3. Boas Práticas

- Siga os padrões de código existentes
- Escreva testes para novas funcionalidades
- Documente mudanças complexas
- Use commits descritivos
- Atualize a documentação quando necessário

### 4. Processo de Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 🔗 Links Úteis

- [Stationeers Wiki](https://stationeers-wiki.com/)
- [Stationeers Official Website](https://store.steampowered.com/app/544550/Stationeers/)
- [IC10 Assembly Documentation](https://stationeers-wiki.com/IC10)

---