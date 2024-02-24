# Introduction <!-- omit in toc -->

Constraints is a object based mechanism for designing expressions for checking
inputs (in this case, anything that derives from an `ITraceLine`) against a set
of user defined rules.

- [1. Usage](#1-usage)
  - [1.1. Boolean Conditions](#11-boolean-conditions)
    - [1.1.1. OR conditions](#111-or-conditions)
    - [1.1.2. AND conditions](#112-and-conditions)
    - [1.1.3. NOT conditions](#113-not-conditions)
    - [1.1.4. Combining conditions](#114-combining-conditions)
  - [1.2. Constraints](#12-constraints)
    - [1.2.1. Left to Right Evaluation](#121-left-to-right-evaluation)
    - [1.2.2. Side Effects in Constraints](#122-side-effects-in-constraints)
- [2. Design](#2-design)
  - [2.1. Unit Test Framework Inspiration](#21-unit-test-framework-inspiration)
  - [2.2. Expressions](#22-expressions)
  - [2.3. List Interpretation Mechanism](#23-list-interpretation-mechanism)
  - [2.4. Making Things Faster](#24-making-things-faster)
    - [2.4.1. Binary Expression Trees](#241-binary-expression-trees)
    - [2.4.2. Inline Compilation](#242-inline-compilation)
    - [2.4.3. Dynamic Method Preparation](#243-dynamic-method-preparation)
    - [2.4.4. Operations in IL](#244-operations-in-il)
      - [2.4.4.1. AND](#2441-and)
      - [2.4.4.2. OR](#2442-or)
      - [2.4.4.3. NOT](#2443-not)
    - [2.4.5. Optimizations in IL](#245-optimizations-in-il)
      - [2.4.5.1. a AND b AND c](#2451-a-and-b-and-c)
      - [2.4.5.2. a AND b OR c](#2452-a-and-b-or-c)
      - [2.4.5.3. a OR b AND c](#2453-a-or-b-and-c)
      - [2.4.5.4. a OR b OR c](#2454-a-or-b-or-c)
      - [2.4.5.5. NOT a](#2455-not-a)
    - [2.4.6. Summary](#246-summary)
      - [2.4.6.1. 3.4.6.1 Branch Always](#2461-3461-branch-always)
      - [2.4.6.2. Branch Never](#2462-branch-never)
    - [2.4.7. Debugging](#247-debugging)
      - [2.4.7.1. Compilation](#2471-compilation)
      - [2.4.7.2. Run Time](#2472-run-time)
- [3. Work to be Done](#3-work-to-be-done)

## 1. Usage

An example of the simplest program that will check if a `TraceLine` text has the
string "foo" in it.

```csharp
using RJCP.Diagnostics.Log.Constraints;

namespace MyProgram {
  class Program {
    Constraint c = new Constraint().TextString("foo");
    TraceLine line = new TraceLine("foobar");
    Console.WriteLine("match: {0}", c.Check(line));
  }
}
```

You can apply boolean conditions and subexpressions with the `.And`, `.Or` and
`.Not` properties.

### 1.1. Boolean Conditions

#### 1.1.1. OR conditions

To test either/or conditions, (boolean `OR`) use a sample such as:

```csharp
Constraint c = new Constraint().TextString("foo").Or.TextString("bar");
```

#### 1.1.2. AND conditions

Chaining operations one after the other implicitly applies the boolean `AND`
operator.

```csharp
Constraint c = new Constraint().TextString("foo").TextString("bar");
```

#### 1.1.3. NOT conditions

To check that a condition does not occur:

```csharp
Constraint c = new Constraint().Not.TextString("foo");
```

#### 1.1.4. Combining conditions

The conditions can be combined, the rules for evaluation is the same as for any
other boolean expression. NOT has the highest precedence, followed by `AND` and
then `OR`. You can create subexpressions with the `Expr()` method, which is
similar to using parenthesis in a boolean expression.

### 1.2. Constraints

A constraint is simply something that derives from the `IMatchConstraint`
interface. It takes a `ITraceLine` as an input, does some checks against its own
local state and can return either `true` or `false`. You can apply your own
expressions also by using the `Expr()` method.

```csharp
public class IsEmpty : IMatchConstraint {
  public IsEmpty() { }

  public bool Check(ITraceLine line) {
    return string.IsNullOrEmpty(line.Text);
  }
}
```

Then it can be used as:

```csharp
Constraint c = new Constraint().Not.Expr(new IsEmpty());
```

If you want to make the code a little more readable, you could go to the effort
of creating extension methods such as:

```csharp
public Constraint IsEmpty(this Constraint constraint) {
  return constraint.Expr(new IsEmpty());
}
```

Then you can write the expression as:

```csharp
Constraint c = new Constraint().Not.IsEmpty();
```

#### 1.2.1. Left to Right Evaluation

Calculations of the logical expression are done from left to right and only
calculates constraints as required. That means:

- For `OR`, if the left is `TRUE`, the right is skipped as the result is known
  to be `TRUE`.
- For `AND`, if the left is `FALSE`, the right is skipped as the result is known
  to be `FALSE`.

#### 1.2.2. Side Effects in Constraints

By using the left to right evaluation of constraints, one can also define
constraints that return `true`, but which can cause side effects, such as
counting how often an expression is evaluated.

Thus:

```csharp
public class Ctr : IMatchConstraint {
  public long Count { get; private set; }
  public bool Check(ITraceLine line) {
    Count++;
    return true;
  }
}
```

So if you have the constraint:

```csharp
IMatchConstraint ctr = new Ctr();
Constraint c = new Constraint().Not.IsEmpty().Expr(ctr);
```

you can determine how many times `c.Check(line)` has been called where the
string is not empty. Because if it were empty, the left to right ordering of
constraints means that the term `Expr(ctr)` is not evaluated.

## 2. Design

This section is dedicated to the internal design of the software.

### 2.1. Unit Test Framework Inspiration

The implementation is inspired by how the NUnit test framework defines
constraints for testing with the `Assert` method.

### 2.2. Expressions

It is important for the `Constraint` class, that methods and properties return
itself (the `this` pointer). This allows chaining of operations one after the
other, to be able to construct a readable expression.

As methods and properties are chained one after the other, a list of "tokens" is
built up. This is nothing unlike how a tokenizer reads a string and generates a
list of tokens. Except we're leaving it to your favorite compiler to do the
tokenization for us, and the `Constraints` class itself builds up the list of
tokens.

### 2.3. List Interpretation Mechanism

The simplest mechanism for implementation is to parse tokens from left to right,
and when an operator is observed, the next values of higher precedence is
processed in the list until there is a left value and a right value and then the
result is calculated.

This is implemented using the `ConstraintList` class.

The implementation is quite fast and specific to the rules for calculating the
result of a constraint.

### 2.4. Making Things Faster

#### 2.4.1. Binary Expression Trees

When one has a [binary expression
tree](https://en.wikipedia.org/wiki/Binary_expression_tree), there are multiple
ways in which the result can be calculated. By doing a Depth First Search (also
known as Postfix) from left to right, a stack based list of tokens to be
calculated can be generated as with Reverse Polish Notation.

However, to avoid unnecessary calculations, one should do a Depth First Search,
but only move to the right of the node if the result of the left requires
calculation of the right. In the list implementation, all elements of higher
precedence were skipped. With a binary expression tree, one just ignores that
node and it is never processed.

To test that mechanism, the `ConstraintExprTree` class was made that can build
expression trees from elements in the `Constraint` class, including
sub-expressions.

For performance, each node in the expression is counted and assigned an
identifier. That identifier maps to an array element which keeps track if the
node was visited during the DFS algorithm, as well as the result of the
calculation.

#### 2.4.2. Inline Compilation

The algorithm for performing the calculation for binary search trees shows
itself to be slightly slower than that for the list for smaller expressions. The
reason is mostly to do with the number of instructions used rather than the
algorithm itself, and that actually testing the constraint if it is `true` or
`false` is longer than actually parsing through the list.

Since .NET 2.0 there is a mechanism called
[DynamicMethod](https://msdn.microsoft.com/en-us/library/system.reflection.emit.dynamicmethod%28v=vs.110%29.aspx)
which can be used to compile expressions into IL for the fastest possible speed.

So instead of parsing the binary expression tree to generate results, the binary
expression tree will be parsed using a depth first search algorithm, but to
generate Intermediate Language (IL). This allows for the fastest possible code,
without having to iterate through internal data structures such as lists or
trees.

Using IL brings quite a few benefits for performance:

- IL is a stack based language. Binary expression trees can naturally generate
  expressions that use a stack for their evaluation. Thus there is no need to
  keep stack data structures, use the model of IL directly.
- Parsing through the binary expression tree only needs to be done once to
  generate the IL. Then when invoking the check, the instructions pass from top
  to bottom in order, allowing for smaller code and inlining. Branching occurs
  based on the previous result only if required.
- All values can be cached and organized ahead of time for the fastest possible
  memory accesses.

#### 2.4.3. Dynamic Method Preparation

The Binary Expression Tree is used to generate the IL, as well as to populate an
array containing all the expression checks required. By having an array if
`IMatchConstraint` checks, one for each check that needs to be performed, IL
just needs to index that array to get the object to do the check.

Assume the first argument to the `DynamicMethod` is the array of
`IMatchConstraint` objects. To get the first element in the array:

```ilasm
ldarg.0
ldc.i4 <index>
ldelem.ref
```

By doing this, we save instructions of having to iterate through the tree
structure which would contain significantly more instructions.

Then to do the check, where the second argument to the `DynamicMethod` is the
`line` to check:

```ilasm
ldarg.1
callvirt IMatchConstraint.Check(ITraceLine)
```

The result is placed on the stack.

Of course, using `il.Emit(OpCodes.Ldc_I4_0)` or similar functions can be decided
as the binary tree is iterated over, and it can prepare the array in advance.

#### 2.4.4. Operations in IL

There are three operations that need to be performed (apart from the check
itself). That is AND, OR and NOT. One should not use bit operations, as the
results are not the same as their boolean bit-wise counterparts.

In the following subsections, we'll use the opcode `Check(X)` to indicate the
loading of the function from the array, and calling the operation, which was
given in the previous section.

##### 2.4.4.1. AND

Operation: l AND r

```ilasm
      check(l)
      brfalse.s L11
      check(r)
      br.s L10
L11:  ldc.i4.0
L10:
```

If the first check for "l" is `FALSE`, then we don't need to check "r" as the
result will remain `FALSE`.

##### 2.4.4.2. OR

Operation: l OR r

```ilasm
      check(l)
      brtrue.s L21
      check(r)
      br.s L20
L21:  ldc.i4.1
L20:
```

The `AND` and `OR` operations are very similar. If the first check for "l" is
`TRUE`, then we don't need to check "r" as the result will remain `TRUE`.

##### 2.4.4.3. NOT

Operation: NOT r

```ilasm
      check(r)
      ldc.i4.1
      xor
```

#### 2.4.5. Optimizations in IL

As IL is a stack based machine, evaluating an expression is as simple as
chaining results one after the other, with the previous result pushed onto the
stack.

By analyzing chains of operations, we can begin to look for optimizations. The
following sections are to be read from left to right the ordering in which the
binary expression tree is parsed.

The most basic operation is to see that results of one operation can be used for
the input of the next operation, which may result in known outcomes when
performing static analysis.

##### 2.4.5.1. a AND b AND c

Operation: a AND b AND c

```ilasm
      check(a)         |       check(a)
      brfalse.s L11    | L10 = BeginBrFalse()
      check(b)         |       check(b)
      br.s L10         |       EndSetFalse(L10)
L11:  ldc.i4.0         |
L10:                   |
      brfalse.s L21    | L20 = BeginBrFalse()
      check(c)         |       check(c)
      br.s L20         |       EndSetFalse(L20)
L21:  ldc.i4.0         |
L20:                   |
```

The instructions at L11 will always evaluate so the branch is made. So instead
of branching to L11, that instruction can branch immediately to L21, thus we can
remove the two instructions at L11.

On the right, code that generates the IL makes it more obvious we set the result
to false, and then branch if false. Obviously this code generation isn't needed
and we can jump to the correct label.

```ilasm
      check(a)         |       check(a)
      brfalse.s L21    | L10 = BeginBrFalse()
      check(b)         |       check(b)
      brfalse.s L21    | L20 = BeginBrFalse()
      check(c)         |       check(c)
      br.s L20         |       EndSetFalse(L10, L20)
L21:  ldc.i4.0         |
L20:                   |
```

The `EndSetFalse(L10)` followed by `L20=BeginBrFalse()` can be combined, so that
the `L10=BeginBrFalse()` can be written to point to L20 instead.

##### 2.4.5.2. a AND b OR c

Operation: a AND b OR c

```ilasm
      check(a)         |       check(a)
      brfalse.s L11    | L10 = BeginBrFalse()
      check(b)         |       check(b)
      br.s L10         |       EndSetFalse(L10)
L11:  ldc.i4.0         |
L10:                   |
      brtrue.s L21     | L20 = BeginBrTrue()
L12:  check(c)         |       check(c)
      br.s L20         |       EndSetTrue(L20)
L21:  ldc.i4.1         |
L20:                   |
```

As instructions from L11 do nothing more than push a value on the stack and pop
that value from the stack, we can jump to L12 instead. This renders instructions
at L11 no longer required, so it can be removed and the instruction prior also.

```ilasm
      check(a)         |       check(a)
      brfalse.s L12    | L10 = BeginBrFalse()
      check(b)         |       check(b)
      brtrue.s L21     | L20 = BeginBrTrue(); EndBr(L10);
L12:  check(c)         |       check(c)
      br.s L20         |       EndSetTrue(L20)
L21:  ldc.i4.1
L20:
```

The `EndSetFalse(L10)` followed by `L20=BeginBrTrue()` can be combined, so that
only the `L20=BeginBrTrue()` is needed and the marker label is moved to be
immediately after the check that will always be false (push false, branch if
true is an effective no operation).

##### 2.4.5.3. a OR b AND c

Operation: (a OR b) AND c

```ilasm
      check(a)         |       check(a)
      brtrue.s L21     | L20 = BeginBrTrue()
      check(b)         |       check(b)
      br.s L20         |       EndSetTrue(L20)
L21:  ldc.i4.1         |
L20:  brfalse.s L11    | L10 = BeginBrFalse()
L22:  check(c)         |       check(c)
      br.s L10         |       EndSetFalse(L10)
L11:  ldc.i4.0         |
L10:                   |
```

This is the same effect as for `a AND b OR c` where jumping to L21 has no effect
and we can jump to L22 instead. That renders the instruction at L21 no longer
required and we can also remove the instruction prior to it.

```ilasm
      check(a)         |       check(a)
      brtrue.s L22     | L20 = BeginBrTrue()
      check(b)         |       check(b)
L20:  brfalse.s L11    | L10 = BeginBrFalse(); EndBr(L20);
L22:  check(c)         |       check(c)
      br.s L10         |       EndSetFalse(L10);
L11:  ldc.i4.0         |
L10:                   |
```

Like the case in `a AND b OR c` earlier, setting the result to true, and then
branching if false does nothing. So the first branch should end immediately
after the second check.

##### 2.4.5.4. a OR b OR c

Operation: a OR b OR c

```ilasm
      check(a)         |       check(a)
      brtrue.s L11     | L10 = BeginBrTrue()
      check(b)         |       check(b)
      br.s L10         |       EndSetTrue(L10)
L11:  ldc.i4.1         |
L10:  brtrue.s L21     | L20 = BeginBrTrue()
      check(c)         |       check(c)
      br.s L20         |       EndSetTrue(L20)
L21:  ldc.i4.1         |
L20:                   |
```

Can be similarly reduced to:

```ilasm
      check(a)         |       check(a)
      brtrue.s L21     | L10 = BeginBrTrue()
      check(b)         |       check(b)
      brtrue.s L21     | L20 = BeginBrTrue()
      check(c)         |       check(c)
      br.s L20         |       EndSetTrue(L10, L20)
L21:  ldc.i4.1         |
L20:                   |
```

The case shown here is similar to the case for `a AND b AND c`.

##### 2.4.5.5. NOT a

Operation: NOT a

When executing the code, the value to invert is already on the stack, the
operation is expected to change the outcome

```ilasm
      check(a)
      ldc.i4.1
      xor
```

We can change this, so that if the previous operation is a `EndSetXXXX`, we can
immediately invert the operation reducing the branches and the code required.
Otherwise, we emit the code that is listed.

#### 2.4.6. Summary

Even though we have not analyzed the case of `NOT`, there is a pattern. The
portion that should be optimized is the boundary from the result of the previous
operation with the check of the next operation.

##### 2.4.6.1. 3.4.6.1 Branch Always

If the sequence is

```ilasm
      ...                            |
      brXXXX.s L11   brXXXX.s L11    | L10 = BeginBrXXXX()
      check(b)       check(b)        |       check(b)
      br.s L10       br.s L10        |       EndSetXXXX(L10)
L11:  ldc.i4.1       ldc.i4.0        |
                                     |
L10:  brtrue.s L21   brfalse.s L21   | L20 = BeginBrXXXX()
L12:  check(c)       check(c)        |       check(c)
      br.s L20       br.s L20        |       EndSetXXXX(L20)
L21:  ldc.i4.X       ldc.i4.X        |
L20:                                 |
```

this can be reduced to:

```ilasm
      ...                            |
      brXXXX.s L21   brXXXX.s L21    | L10 = BeginBrXXXX()
      check(b)       check(b)        |       check(b)
                                     |
L10:  brtrue.s L21   brfalse.s L21   | L20 = BeginBrXXXX()
L12:  check(c)       check(c)        |       check(c)
      br.s L20       br.s L20        |       EndSetXXXX(L10, L20)
L21:  ldc.i4.X       ldc.i4.X        |
L20:                                 |
```

##### 2.4.6.2. Branch Never

If the sequence is

```ilasm
      ...            ...             |
      brXXXX.s L11   brXXXX.s L11    | L10 = BeginBrXXXX()
      check(b)       check(b)        |       check(b)
      br.s L10       br.s L10        |       EndSetXXXX(L10)
L11:  ldc.i4.0       ldc.i4.1        |
L10:  brtrue.s L21   brfalse.L21     | L20 = BeginBrYYYY()
L12:  check(c)       check(c)        |       check(c)
      br.s L20       br.s L20        |       EndSetYYYY(L20)
L21:  ldc.i4.X       ldc.i4.X        |
L20:                                 |
```

this can be reduced to:

```ilasm
      ...            ...             |
      brXXXX.s L12   brXXXX.s L12    | L10 = BeginBrXXXX()
      check(b)       check(b)        |       check(b)
L10:  brtrue.s L21   brfalse.L21     | L20 = BeginBrYYYY(); EndBr(L10)
L12:  check(c)       check(c)        |       check(c)
      br.s L20       br.s L20        |       EndSetYYYY()
L21:  ldc.i4.X       ldc.i4.X        |
L20:                                 |
```

#### 2.4.7. Debugging

Working on low level instructions has invariably problems and so mechanisms are
required to debug the work flow. These are the two methods used:

##### 2.4.7.1. Compilation

Trace can be inserted during the expression tree parsing phase to print out the
instructions being performed. The current Node ID (used when parsing the initial
token list) can be used to help identify specific labels. This can be used to
ensure the correct sequence of IL is being generated.

##### 2.4.7.2. Run Time

Often, while the parsing trace may appear to be correct, one wants to check the
actual instructions being executed and insert trace points at specific locations
within the IL.

.NET makes this easy, one simply needs to insert into the generated IL:

```csharp
m_IlGenerator.Emit(OpCodes.Ldstr, "BRANCH TRUE");
m_IlGenerator.Emit(OpCodes.Call,
    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
```

For example, this can be used to show the current instruction about to be
executed. This has helped to identify incorrect sequences in the IL for test
cases that fail.

## 3. Work to be Done

- The number of nodes for the Check functions can be reduced, by counting only
  the number of check nodes. This means we can reduce memory requirements a
  little bit.
- To implement the simplest `AND`, `OR`, `NOT` operations without optimizations
  and test the speed.
- To figure out how to implement the optimization. I don't think we'll be able
  to emit the IL directly as we did before, but need to generate now a list that
  keeps tracks of the labels and can relabel based on the optimizations.
