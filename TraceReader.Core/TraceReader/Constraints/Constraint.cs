#define USEBTREE

namespace RJCP.Diagnostics.Log.Constraints
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A class to define and check multiple constraints on an <see cref="ITraceLine"/>.
    /// </summary>
    /// <remarks>
    /// Constraints is a programmatic way using object chaining to define a constraint which should be applied to an
    /// arbitrary <see cref="ITraceLine"/> object via the <see cref="IMatchConstraint.Check(ITraceLine)"/> method. This
    /// class allows you to build up expressions that can be used to test any kind of condition you require against an
    /// <see cref="ITraceLine"/>.
    /// <para>
    /// There are three basic operators for this class: The AND operator (implicit, by simply chaining one constraint
    /// after the other), the OR operator with the <see cref="Or"/> property, and negation with the <see cref="Not"/>
    /// property.
    /// </para>
    /// <para>Constraint expressions can be grouped with the <see cref="Expr(IMatchConstraint)"/> method.</para>
    /// <para>
    /// Various methods are defined as convenience to shorten an expression string in your code without having to use
    /// <see cref="Expr(IMatchConstraint)"/> everywhere.
    /// </para>
    /// <para>
    /// The simplest example to using this class is to check if a <see cref="ITraceLine.Text"/> has a specific
    /// substring.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Constraint c = new Constraint().TextString("substring");
    /// ]]></code>
    /// <para>
    /// Using the method <see cref="Check(ITraceLine)"/>, you can test if the <see cref="ITraceLine.Text"/> contains the
    /// substring "substring". If that should contain two substrings (both must be present), you can use the "and"
    /// condition:
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Constraint c = new Constraint().TextString("sub1").TextString("sub2");
    /// ]]></code>
    /// <para>
    /// That checks that both substrings are present. If the condition should contain one of two substrings, then use
    /// the "or" condition:
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Constraint c = new Constraint().TextString("sub1").Or.TextString("sub2");
    /// ]]></code>
    /// <para>To test if a substring is not present:</para>
    /// <code language="csharp"><![CDATA[
    /// Constraint c = new Constraint().Not.TextString("sub");
    /// ]]></code>
    /// <para>
    /// A more complex example may require one of two substrings, and not another. This would require building up
    /// subexpressions:
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Constraint c = new Constraint()
    ///     .Expr(new Constraint().TextString("sub1").Or.TextString("sub2"))
    ///     .Not.TextString("sub3");
    /// ]]></code>
    /// <para>That above example checks that a line has either "sub1" or "sub2", but it may not have "sub3".</para>
    /// <para>
    /// The rules of precedence are the same as with any boolean expressions you're familiar with. Subexpressions are
    /// the highest priority and are created with <see cref="Expr(IMatchConstraint)"/>. Followed is <see cref="Not"/>,
    /// then "and" which is implicit by chaining operations, and the least precedent is <see cref="Or"/>.
    /// </para>
    /// <para>In the example above, we see that the boolean expression is like <c>("sub1"+"sub2").!"sub3";</c>.</para>
    /// </remarks>
    public class Constraint : IMatchConstraint
    {
        private readonly ConstraintTokens m_Tokens = new ConstraintTokens();
        private readonly ConstraintOptions m_Options = ConstraintOptions.Compiled;

        // This is the algorithm to use to parse the tokens and evaluate the response.
        IConstraintBase m_Constraints;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> class.
        /// </summary>
        public Constraint() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint"/> class.
        /// </summary>
        /// <param name="options">The options when constructing the constraint.</param>
        /// <remarks>
        /// You can explicitly state if this implementation should be interpreted, or if it should be compiled.
        /// </remarks>
        public Constraint(ConstraintOptions options)
        {
            m_Options = options;
        }

        /// <summary>
        /// Checks the specified line against the constraints defined in the object.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns><see langword="true"/> if the constraints are met, <see langword="false"/> otherwise.</returns>
        /// <exception cref="ConstraintException">
        /// No constraints defined.
        /// <para>- or -</para>
        /// Error evaluating expression.
        /// </exception>
        public bool Check(ITraceLine line)
        {
            try {
                if (!m_Compiled) Compile();
                return m_Constraints.Evaluate(line);
            } catch (ConstraintException) {
                throw;
            } catch (Exception ex) {
                throw new ConstraintException("Error evaluating expression", ex);
            }
        }

        /// <summary>
        /// Builds the internal constraints expression tree.
        /// </summary>
        /// <returns>An IConstraintBase object after building the internal expression tree.</returns>
        /// <remarks>
        /// This is an internal method to allow for optimization in building expressions for faster performance.
        /// </remarks>
        internal IConstraintBase Build()
        {
            if (m_Constraints == null) {
                if (!m_Options.HasFlag(ConstraintOptions.Compiled)) {
#if USEBTREE
                    m_Constraints = new ConstraintExprTree();
#else
                    m_Constraints = new ConstraintList();
#endif
                } else {
                    m_Constraints = new ConstraintCompiled();
                }
                m_Constraints.Build(m_Tokens.GetEnumerator());
            }
            return m_Constraints;
        }

        private bool m_Compiled;

        private void Compile()
        {
            Build().Compile();
            m_Compiled = true;
        }

        /// <summary>
        /// Adds an expression to the constraint expression tree.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="constraint"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        /// <value>The <see cref="Constraint"/> object for chaining.</value>
        public Constraint Expr(IMatchConstraint constraint)
        {
            if (constraint == null) throw new ArgumentNullException(nameof(constraint));
            if (m_Constraints != null) throw new InvalidOperationException("Constraints expression is read only");
            m_Tokens.Append(constraint);
            return this;
        }

        /// <summary>
        /// Gets a <see cref="IMatchConstraint"/> that has no constraints.
        /// </summary>
        /// <value>The none expression.</value>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Constraint None
        {
            get { return Expr(new None()); }
        }

        /// <summary>
        /// Applies an 'OR' expression using boolean logic precedence rules.
        /// </summary>
        /// <value>The <see cref="Constraint"/> object for chaining.</value>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Constraint Or
        {
            get
            {
                if (m_Constraints != null) throw new InvalidOperationException("Constraints expression is read only");
                m_Tokens.Or();
                return this;
            }
        }

        /// <summary>
        /// Inverts the boolean result of the next expression.
        /// </summary>
        /// <value>The <see cref="Constraint"/> object for chaining.</value>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Constraint Not
        {
            get
            {
                if (m_Constraints != null) throw new InvalidOperationException("Constraints expression is read only");
                m_Tokens.Not();
                return this;
            }
        }

        /// <summary>
        /// Checks if the line is <see langword="null"/>.
        /// </summary>
        /// <value>The <see cref="Constraint"/> object for chaining.</value>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Constraint Null
        {
            get
            {
                return Expr(new Null());
            }
        }

        /// <summary>
        /// Indicates the end of the expression.
        /// </summary>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <remarks>
        /// The <see cref="End"/> method indicates the end of the expression and marks it read only. It is beneficial to
        /// mark expressions as finished, so that they can be compiled immediately, not when the first evaluation takes
        /// place.
        /// <para>
        /// Marking constraints as <see langword="static"/> has the advantage that they are only compiled once, and so
        /// may save time running a program if a particular expression is used often.
        /// </para>
        /// </remarks>
        public Constraint End()
        {
            Compile();
            return this;
        }

        /// <summary>
        /// Checks that the <see cref="ITraceLine"/> being checked is of type <typeparamref name="T"/> or derived
        /// thereof.
        /// </summary>
        /// <typeparam name="T">The type the <see cref="ITraceLine"/> should be.</typeparam>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint InstanceOf<T>()
        {
            return Expr(new InstanceOf(typeof(T)));
        }

        /// <summary>
        /// Checks that the <see cref="ITraceLine"/> being checked is exactly of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type the <see cref="ITraceLine"/> should be.</typeparam>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint TypeOf<T>()
        {
            return Expr(new TypeOf(typeof(T)));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> contains the string specified.
        /// </summary>
        /// <param name="text">The text which should be a substring.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint TextString(string text)
        {
            return Expr(new TextString(text));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> contains the string specified using a case insensitive match.
        /// </summary>
        /// <param name="text">The text which should be a substring.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public Constraint TextIString(string text)
        {
            return Expr(new TextIString(text));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> exactly equals the string specified.
        /// </summary>
        /// <param name="text">The text which should exactly match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint TextEquals(string text)
        {
            return Expr(new TextEquals(text));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> exactly equals the string specified using a case insensitive
        /// match.
        /// </summary>
        /// <param name="text">The text which should exactly match.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public Constraint TextIEquals(string text)
        {
            return Expr(new TextIEquals(text));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> exactly equals the text which the line should start with.
        /// </summary>
        /// <param name="text">The text which the line should start with.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public Constraint TextStartsWith(string text)
        {
            return Expr(new TextStartsWith(text));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> satisfies a regular expression.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint TextRegEx(string regEx)
        {
            return Expr(new TextRegEx(regEx));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> satisfies a regular expression.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <param name="options">The options to apply to the regular expression.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        /// <exception cref="InvalidOperationException">Constraints expression is read only.</exception>
        public Constraint TextRegEx(string regEx, System.Text.RegularExpressions.RegexOptions options)
        {
            return Expr(new TextRegEx(regEx, options));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> satisfies a regular expression using a case insensitive match.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public Constraint TextIRegEx(string regEx)
        {
            return Expr(new TextIRegEx(regEx));
        }

        /// <summary>
        /// Checks that <see cref="ITraceLine.Text"/> satisfies a regular expression using a case insensitive match.
        /// </summary>
        /// <param name="regEx">The regular expression which should match the <see cref="ITraceLine.Text"/>.</param>
        /// <param name="options">The options to apply to the regular expression.</param>
        /// <returns>The <see cref="Constraint"/> object for chaining.</returns>
        public Constraint TextIRegEx(string regEx, System.Text.RegularExpressions.RegexOptions options)
        {
            return Expr(new TextIRegEx(regEx, options));
        }
    }
}
