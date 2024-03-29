﻿// 
// OperatorDeclaration.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.ComponentModel;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Al
{
	public enum OperatorType
	{
		// Values must correspond to ICSharpCode.NRefactory.MonoAl.Operator.OpType
		// due to the casts used in OperatorDeclaration.
		
		// Unary operators
		LogicalNot = ICSharpCode.NRefactory.MonoAl.Operator.OpType.LogicalNot,
		OnesComplement = ICSharpCode.NRefactory.MonoAl.Operator.OpType.OnesComplement,
		Increment = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Increment,
		Decrement = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Decrement,
		True = ICSharpCode.NRefactory.MonoAl.Operator.OpType.True,
		False = ICSharpCode.NRefactory.MonoAl.Operator.OpType.False,

		// Unary and Binary operators
		Addition = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Addition,
		Subtraction = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Subtraction,

		UnaryPlus = ICSharpCode.NRefactory.MonoAl.Operator.OpType.UnaryPlus,
		UnaryNegation = ICSharpCode.NRefactory.MonoAl.Operator.OpType.UnaryNegation,
		
		// Binary operators
		Multiply = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Multiply,
		Division = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Division,
		Modulus = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Modulus,
		BitwiseAnd = ICSharpCode.NRefactory.MonoAl.Operator.OpType.BitwiseAnd,
		BitwiseOr = ICSharpCode.NRefactory.MonoAl.Operator.OpType.BitwiseOr,
		ExclusiveOr = ICSharpCode.NRefactory.MonoAl.Operator.OpType.ExclusiveOr,
		LeftShift = ICSharpCode.NRefactory.MonoAl.Operator.OpType.LeftShift,
		RightShift = ICSharpCode.NRefactory.MonoAl.Operator.OpType.RightShift,
		Equality = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Equality,
		Inequality = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Inequality,
		GreaterThan = ICSharpCode.NRefactory.MonoAl.Operator.OpType.GreaterThan,
		LessThan = ICSharpCode.NRefactory.MonoAl.Operator.OpType.LessThan,
		GreaterThanOrEqual = ICSharpCode.NRefactory.MonoAl.Operator.OpType.GreaterThanOrEqual,
		LessThanOrEqual = ICSharpCode.NRefactory.MonoAl.Operator.OpType.LessThanOrEqual,

		// Implicit and Explicit
		Implicit = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Implicit,
		Explicit = ICSharpCode.NRefactory.MonoAl.Operator.OpType.Explicit
	}
	
	public class OperatorDeclaration : EntityDeclaration
	{
		public static readonly TokenRole OperatorKeywordRole = new TokenRole ("operator");
		
		// Unary operators
		public static readonly TokenRole LogicalNotRole = new TokenRole ("!");
		public static readonly TokenRole OnesComplementRole = new TokenRole ("~");
		public static readonly TokenRole IncrementRole = new TokenRole ("++");
		public static readonly TokenRole DecrementRole = new TokenRole ("--");
		public static readonly TokenRole TrueRole = new TokenRole ("true");
		public static readonly TokenRole FalseRole = new TokenRole ("false");

		// Unary and Binary operators
		public static readonly TokenRole AdditionRole = new TokenRole ("+");
		public static readonly TokenRole SubtractionRole = new TokenRole ("-");

		// Binary operators
		public static readonly TokenRole MultiplyRole = new TokenRole ("*");
		public static readonly TokenRole DivisionRole = new TokenRole ("/");
		public static readonly TokenRole ModulusRole = new TokenRole ("%");
		public static readonly TokenRole BitwiseAndRole = new TokenRole ("&");
		public static readonly TokenRole BitwiseOrRole = new TokenRole ("|");
		public static readonly TokenRole ExclusiveOrRole = new TokenRole ("^");
		public static readonly TokenRole LeftShiftRole = new TokenRole ("<<");
		public static readonly TokenRole RightShiftRole = new TokenRole (">>");
		public static readonly TokenRole EqualityRole = new TokenRole ("==");
		public static readonly TokenRole InequalityRole = new TokenRole ("!=");
		public static readonly TokenRole GreaterThanRole = new TokenRole (">");
		public static readonly TokenRole LessThanRole = new TokenRole ("<");
		public static readonly TokenRole GreaterThanOrEqualRole = new TokenRole (">=");
		public static readonly TokenRole LessThanOrEqualRole = new TokenRole ("<=");
		
		public static readonly TokenRole ExplicitRole = new TokenRole ("explicit");
		public static readonly TokenRole ImplicitRole = new TokenRole ("implicit");
		
		public override SymbolKind SymbolKind {
			get { return SymbolKind.Operator; }
		}
		
		OperatorType operatorType;
		
		public OperatorType OperatorType {
			get { return operatorType; }
			set {
				ThrowIfFrozen();
				operatorType = value;
			}
		}
		
		public AlTokenNode OperatorToken {
			get { return GetChildByRole (OperatorKeywordRole); }
		}
		
		public AlTokenNode OperatorTypeToken {
			get { return GetChildByRole (GetRole (OperatorType)); }
		}
		
		public AlTokenNode LParToken {
			get { return GetChildByRole (Roles.LPar); }
		}
		
		public AstNodeCollection<ParameterDeclaration> Parameters {
			get { return GetChildrenByRole (Roles.Parameter); }
		}
		
		public AlTokenNode RParToken {
			get { return GetChildByRole (Roles.RPar); }
		}
		
		public BlockStatement Body {
			get { return GetChildByRole (Roles.Body); }
			set { SetChildByRole (Roles.Body, value); }
		}
		
		/// <summary>
		/// Gets the operator type from the method name, or null, if the method does not represent one of the known operator types.
		/// </summary>
		public static OperatorType? GetOperatorType(string methodName)
		{
			return (OperatorType?)ICSharpCode.NRefactory.MonoAl.Operator.GetType(methodName);
		}
		
		public static TokenRole GetRole (OperatorType type)
		{
			switch (type) {
			case OperatorType.LogicalNot:
				return LogicalNotRole;
			case OperatorType.OnesComplement:
				return OnesComplementRole;
			case OperatorType.Increment:
				return IncrementRole;
			case OperatorType.Decrement:
				return DecrementRole;
			case OperatorType.True:
				return TrueRole;
			case OperatorType.False:
				return FalseRole;
			
			case OperatorType.Addition:
			case OperatorType.UnaryPlus:
				return AdditionRole;
			case OperatorType.Subtraction:
			case OperatorType.UnaryNegation:
				return SubtractionRole;
			
			case OperatorType.Multiply:
				return MultiplyRole;
			case OperatorType.Division:
				return DivisionRole;
			case OperatorType.Modulus:
				return ModulusRole;
			case OperatorType.BitwiseAnd:
				return BitwiseAndRole;
			case OperatorType.BitwiseOr:
				return BitwiseOrRole;
			case OperatorType.ExclusiveOr:
				return ExclusiveOrRole;
			case OperatorType.LeftShift:
				return LeftShiftRole;
			case OperatorType.RightShift:
				return RightShiftRole;
			case OperatorType.Equality:
				return EqualityRole;
			case OperatorType.Inequality:
				return InequalityRole;
			case OperatorType.GreaterThan:
				return GreaterThanRole;
			case OperatorType.LessThan:
				return LessThanRole;
			case OperatorType.GreaterThanOrEqual:
				return GreaterThanOrEqualRole;
			case OperatorType.LessThanOrEqual:
				return LessThanOrEqualRole;
			
			case OperatorType.Implicit:
				return ImplicitRole;
			case OperatorType.Explicit:
				return ExplicitRole;
			
			default:
				throw new System.ArgumentOutOfRangeException ();
			}
		}
		
		/// <summary>
		/// Gets the method name for the operator type. ("op_Addition", "op_Implicit", etc.)
		/// </summary>
		public static string GetName (OperatorType type)
		{
			return ICSharpCode.NRefactory.MonoAl.Operator.GetMetadataName ((ICSharpCode.NRefactory.MonoAl.Operator.OpType)type);
		}
		
		/// <summary>
		/// Gets the token for the operator type ("+", "implicit", etc.)
		/// </summary>
		public static string GetToken (OperatorType type)
		{
			return ICSharpCode.NRefactory.MonoAl.Operator.GetName ((ICSharpCode.NRefactory.MonoAl.Operator.OpType)type);
		}
		
		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitOperatorDeclaration (this);
		}
			
		public override T AcceptVisitor<T> (IAstVisitor<T> visitor)
		{
			return visitor.VisitOperatorDeclaration (this);
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitOperatorDeclaration (this, data);
		}
		
		public override string Name {
			get { return GetName (this.OperatorType); }
			set { throw new NotSupportedException(); }
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Identifier NameToken {
			get { return Identifier.Null; }
			set { throw new NotSupportedException(); }
		}
		
		protected internal override bool DoMatch (AstNode other, PatternMatching.Match match)
		{
			OperatorDeclaration o = other as OperatorDeclaration;
			return o != null && this.MatchAttributesAndModifiers (o, match) && this.OperatorType == o.OperatorType
				&& this.ReturnType.DoMatch (o.ReturnType, match)
				&& this.Parameters.DoMatch (o.Parameters, match) && this.Body.DoMatch (o.Body, match);
		}
	}
}
