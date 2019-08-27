// 
// TokenNode.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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

namespace ICSharpCode.NRefactory.Al
{
	/// <summary>
	/// Represents a token in C#. Note that the type of the token is defined through the TokenRole.
	/// </summary>
	/// <remarks>
	/// In all non null c# token nodes the Role of a AlToken must be a TokenRole.
	/// </remarks>
	public class AlTokenNode : AstNode
	{
		public static new readonly AlTokenNode Null = new NullAlTokenNode ();
		class NullAlTokenNode : AlTokenNode
		{
			public override bool IsNull {
				get {
					return true;
				}
			}
			
			public NullAlTokenNode () : base (TextLocation.Empty, null)
			{
			}
			
			public override void AcceptVisitor (IAstVisitor visitor)
			{
				visitor.VisitNullNode(this);
			}
			
			public override T AcceptVisitor<T> (IAstVisitor<T> visitor)
			{
				return visitor.VisitNullNode(this);
			}
			
			public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
			{
				return visitor.VisitNullNode(this, data);
			}
			
			protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
			{
				return other == null || other.IsNull;
			}
		}
		
		public override NodeType NodeType {
			get {
				return NodeType.Token;
			}
		}
		
		TextLocation startLocation;
		public override TextLocation StartLocation {
			get {
				return startLocation;
			}
		}

		int TokenLength {
			get {
				return TokenRole.TokenLengths [(int)(this.flags >> AstNodeFlagsUsedBits)];
			}
		}
		
		public override TextLocation EndLocation {
			get {
				return new TextLocation (StartLocation.Line, StartLocation.Column + TokenLength);
			}
		}

		public AlTokenNode (TextLocation location, TokenRole role)
		{
			this.startLocation = location;
			if (role != null)
				this.flags |= role.TokenIndex << AstNodeFlagsUsedBits;
		}

		public override string ToString(AlFormattingOptions formattingOptions)
		{
			return TokenRole.Tokens [(int)(this.flags >> AstNodeFlagsUsedBits)];
		}

		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitAlTokenNode (this);
		}
			
		public override T AcceptVisitor<T> (IAstVisitor<T> visitor)
		{
			return visitor.VisitAlTokenNode (this);
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitAlTokenNode (this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			AlTokenNode o = other as AlTokenNode;
			return o != null && !o.IsNull && !(o is AlModifierToken);
		}
	}
}

