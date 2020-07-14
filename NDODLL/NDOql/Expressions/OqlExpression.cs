using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace NDOql.Expressions
{
	internal interface IManageExpression
	{
		void SetParent( OqlExpression exp );
	}

    /// <summary>
    /// Base class for Oql expressions
    /// </summary>
    public class OqlExpression : IManageExpression
    {
        int line;
        int col;
        ExpressionType expressionType;
        string unaryOp;
        string op;
		OqlExpression parent = null;
        List<OqlExpression> children = new List<OqlExpression>();
        object value;
		Dictionary<string, object> annotations = new Dictionary<string, object>();

        /// <summary>
        /// Sets an annotation for a certain experession node
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public void SetAnnotation(string key, object value)
		{
			annotations[key] = value;
		}

        /// <summary>
        /// Gets an annotation for a node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
		public T GetAnnotation<T>( string key )
		{
			if (!annotations.ContainsKey( key ))
				return default(T);

			return (T)annotations[key];
		}

        /// <summary>
        /// Gets or sets the line number where the expression appears
        /// </summary>
		public int Line
        {
            get { return line; }
            set { line = value; }
        }
        /// <summary>
        /// Gets or sets the column in the line where the expression appears
        /// </summary>
        public int Column
        {
            get { return col; }
            set { col = value; }
        }

        /// <summary>
        /// Gets or sets the expression type
        /// </summary>
        public ExpressionType ExpressionType
        {
            get { return expressionType; }
            set { expressionType = value; }
        }

        void ThrowUnaryException()
        {
            throw new OqlExpressionException("Unary Operator doesn't fit to the type of the expression in '" + this.ToString() + "'. Line " + this.line + ", Col " + this.col);
        }

        /// <summary>
        /// Gets or sets an unary operator, if one exists
        /// </summary>
        public string UnaryOp
        {
            get { return unaryOp; }
            set 
            { 
                unaryOp = value;
                if (unaryOp == "NOT")
                {
                    if (this.expressionType != ExpressionType.Boolean
                        && this.expressionType != ExpressionType.Unknown)
                        ThrowUnaryException();
                }
                else if (unaryOp == "~" || unaryOp == "-" || unaryOp == "+")
                {
                    if (this.expressionType != ExpressionType.Number
                        && this.expressionType != ExpressionType.Unknown)
                        ThrowUnaryException();
                }
            }
        }

        /// <summary>
        /// Gets or sets an operator
        /// </summary>
        public string Operator
        {
            get { return op; }
            set { op = value; }
        }

        bool hasBrackets;
        /// <summary>
        /// Gets or sets a value, which determines, if an expression should be bracketed
        /// </summary>
        public bool HasBrackets
        {
            get { return hasBrackets; }
            set { hasBrackets = value; }
        }

        /// <summary>
        /// Gets the child expressions
        /// </summary>
        public List<OqlExpression> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets the parent expression
        /// </summary>
		public OqlExpression Parent
		{
			get { return this.parent; }
		}

		/// <summary>
		/// </summary>
		/// <param name="exp"></param>
		/// <remarks>
		/// Since there occurs a lot of shuffling around the children during the parse process
		/// we itererate through the tree after parsing and set the parent there.
		/// </remarks>
		void IManageExpression.SetParent( OqlExpression exp )
		{
			this.parent = exp;
		}

        /// <summary>
        /// Gets the value of the expression
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        public OqlExpression(int line, int col)
        {
            this.line = line;
            this.col = col;
        }

        /// <summary>
        /// Determines, if an expression is a leaf in the tree.
        /// </summary>
        public virtual bool IsTerminating
        {
            get { return this.children.Count == 0; }
        }

        /// <summary>
        /// Reduces an expression
        /// </summary>
        /// <returns></returns>
        public OqlExpression Simplify()
        {
            System.Diagnostics.Debug.Assert(this.children.Count != 0 || this.value != null);
            if (this.children.Count > 1 || this.IsTerminating)
                return this;
            if (unaryOp != null)
            {
                if (this.children[0].unaryOp == null && !this.children[0].hasBrackets)
                {
                    this.children[0].unaryOp = this.unaryOp;
                    return this.children[0];
                }
                else
                {
                    return this;
                }
            }
            return this.children[0];
        }

        /// <summary>
        /// Adds a child expressoin
        /// </summary>
        /// <param name="exp"></param>
        public virtual void Add(OqlExpression exp)
        {            
            this.children.Add(exp);
        }

        /// <summary>
        /// Adds a child expression and an operator
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="op"></param>
        public virtual void Add(OqlExpression exp, string op)
        {
            System.Diagnostics.Debug.Assert(this.children.Count > 0);
            if (this.Children.Count < 2)
            {
                this.children.Add(exp);
                this.op = op;
            }
            else if (this.op == op)
            {
                this.children.Add(exp);
            }
            else
            {
                OqlExpression left = this.Clone();
                this.children.Clear();
                this.children.Add(left);
                this.op = op;
                this.children.Add(exp);
            }
        }

        /// <summary>
        /// Gets all siblings of an expression
        /// </summary>
		public List<OqlExpression> Siblings
		{
			get
			{
				List<OqlExpression> result = new List<OqlExpression>();
				if ( this.parent != null )
				{
					foreach ( OqlExpression child in parent.children )
					{
						if ( Object.ReferenceEquals( child, this ) )
							continue;

						result.Add( child );
					}
				}
				return result;
			}
		}

        /// <summary>
        /// Gets the next sibling starting from the current expression
        /// </summary>
        public OqlExpression NextSibling
        {
            get
            {
                if (this.parent != null)
                {
                    bool foundMyself = false;
                    foreach (OqlExpression child in parent.children)
                    {
                        if (foundMyself)
                            return child;
                        if (Object.ReferenceEquals(child, this))
                            foundMyself = true;

                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Clones the expression
        /// </summary>
        /// <returns></returns>
        protected virtual OqlExpression Clone()
        {
            OqlExpression exp = new OqlExpression(this.line, this.col);
            exp.children = children;
            exp.op = op;
            exp.unaryOp = unaryOp;
            exp.value = value;
            return exp;
        }

		//public List<object> Serialize()
		//{
		//    List<object> list = new List<object>();
		//    Serialize(list);
		//}

		//private void Serialize(List<object> list)
		//{
		//    if (this.IsTerminating)
		//    {
		//        list.Add(this);
		//    }
		//    else
		//    {
		//        if (this.hasBrackets)
		//            list.Add("(");
		//        if (this.unaryOp != null)
		//            list.Add(this.unaryOp);
		//    }

		//}

        ///<inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (hasBrackets)
                sb.Append('(');
            if (unaryOp != null)
            {
                sb.Append(unaryOp);
                sb.Append(' ');
            }
			if (ExpressionType == Expressions.ExpressionType.Raw && value != null)
			{
				sb.Append( this.value.ToString() );
                if (!IsTerminating)
				    sb.Append( ' ' );
			}
            else if (this.IsTerminating)
            {
                if (this.value is Double)
                    sb.Append(((Double)this.value).ToString(CultureInfo.InvariantCulture));
                else
                    if (value != null)
                    sb.Append(this.value.ToString());
            }
            if (!this.IsTerminating)
            {
                string op1 = op;
                for (int i = 0; i < children.Count; i++)
                {
                    OqlExpression child = this.children[i];
                    sb.Append(child.ToString());
                    if (i < children.Count - 1)
                    {
                        if (!String.IsNullOrEmpty(op))
                        {
                            if (op != ",")
                                sb.Append(' ');
                            sb.Append(op1);
                            if (op1 == "BETWEEN")
                            {
                                op1 = "AND";
                            }
                            sb.Append(' ');
                        }
                        else
                        {
                            sb.Append(' ');
                        }
                    }
                }
            }
            if (hasBrackets)
                sb.Append(')');
            
            return sb.ToString();
        }

        /// <summary>
        /// Visits all expressions of the tree and returns the expressions to which the predicate applies
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
		public IEnumerable<OqlExpression> GetAll(Func<OqlExpression, bool>predicate)
		{
			if (predicate(this)) 
				yield return this;
			foreach (var child in children)
			{
				foreach (var exp in child.GetAll( predicate ))
				{
					yield return exp;
				}
			}
		}
    
        /// <summary>
        /// Constructs a deep clone of the expression
        /// </summary>
		public virtual OqlExpression DeepClone
		{
			get
			{
				OqlExpression clone = new OqlExpression( this.line, this.col );
				clone.expressionType = this.expressionType;
				clone.hasBrackets = this.hasBrackets;
				clone.op = this.op;
				clone.unaryOp = this.unaryOp;
				clone.value = this.value;
				clone.annotations = annotations;
				foreach(var child in children)
				{
					OqlExpression childClone = child.DeepClone;
					((IManageExpression)childClone).SetParent( clone );
					clone.Add( childClone );
				}
				return clone;
			}
		}
    }
}
