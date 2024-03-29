﻿// 
// UnsafeStatement.cs
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

namespace ICSharpCode.NRefactory.Al
{
	/// <summary>
	/// unsafe { Body }
	/// </summary>
	public class UnsafeStatement : Statement
	{
		public static readonly TokenRole UnsafeKeywordRole = new TokenRole ("unsafe");
		
		public AlTokenNode UnsafeToken {
			get { return GetChildByRole (UnsafeKeywordRole); }
		}
		
		public BlockStatement Body {
			get { return GetChildByRole (Roles.Body); }
			set { SetChildByRole (Roles.Body, value); }
		}
		
		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitUnsafeStatement (this);
		}
			
		public override T AcceptVisitor<T> (IAstVisitor<T> visitor)
		{
			return visitor.VisitUnsafeStatement (this);
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitUnsafeStatement (this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			UnsafeStatement o = other as UnsafeStatement;
			return o != null && this.Body.DoMatch(o.Body, match);
		}
	}


    /// <summary>
    /// safe { Body }
    /// </summary>
    public class SafeStatement : Statement
    {
        public static readonly TokenRole UnsafeKeywordRole = new TokenRole("safe");

        public AlTokenNode UnsafeToken
        {
            get { return GetChildByRole(UnsafeKeywordRole); }
        }

        public BlockStatement Body
        {
            get { return GetChildByRole(Roles.Body); }
            set { SetChildByRole(Roles.Body, value); }
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitSafeStatement(this);
        }

        public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitSafeStatement(this);
        }

        public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
        {
            return visitor.VisitSafeStatement(this, data);
        }

        protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
        {
            SafeStatement o = other as SafeStatement;
            return o != null && this.Body.DoMatch(o.Body, match);
        }
    }


    /// <summary>
    /// safe { Body }
    /// </summary>
    public class SymStatement : Statement
    {
        public static readonly TokenRole UnsafeKeywordRole = new TokenRole("sym");

        public AlTokenNode UnsafeToken
        {
            get { return GetChildByRole(UnsafeKeywordRole); }
        }

        public BlockStatement Body
        {
            get { return GetChildByRole(Roles.Body); }
            set { SetChildByRole(Roles.Body, value); }
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitSymStatement(this);
        }

        public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitSymStatement(this);
        }

        public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
        {
            return visitor.VisitSymStatement(this, data);
        }

        protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
        {
            SymStatement o = other as SymStatement;
            return o != null && this.Body.DoMatch(o.Body, match);
        }
    }

    /// <summary>
    /// safe { Body }
    /// </summary>
    public class AsmStatement : Statement
    {
        public static readonly TokenRole UnsafeKeywordRole = new TokenRole("asm");

        public AlTokenNode UnsafeToken
        {
            get { return GetChildByRole(UnsafeKeywordRole); }
        }

        public BlockStatement Body
        {
            get { return GetChildByRole(Roles.Body); }
            set { SetChildByRole(Roles.Body, value); }
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitAsmStatement(this);
        }

        public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitAsmStatement(this);
        }

        public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
        {
            return visitor.VisitAsmStatement(this, data);
        }

        protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
        {
            AsmStatement o = other as AsmStatement;
            return o != null && this.Body.DoMatch(o.Body, match);
        }
    }
    /// <summary>
    /// loop { Body }
    /// </summary>
    public class LoopStatement : Statement
    {
        public static readonly TokenRole UnsafeKeywordRole = new TokenRole("loop");

        public AlTokenNode UnsafeToken
        {
            get { return GetChildByRole(UnsafeKeywordRole); }
        }

        public BlockStatement Body
        {
            get { return GetChildByRole(Roles.Body); }
            set { SetChildByRole(Roles.Body, value); }
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitLoopStatement(this);
        }

        public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitLoopStatement(this);
        }

        public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
        {
            return visitor.VisitLoopStatement(this, data);
        }

        protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
        {
            LoopStatement o = other as LoopStatement;
            return o != null && this.Body.DoMatch(o.Body, match);
        }
    }
}
