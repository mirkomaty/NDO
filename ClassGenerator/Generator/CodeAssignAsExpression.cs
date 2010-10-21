using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using ClassGenerator;

namespace Generator
{
	class CodeAssignAsExpression : CodeSnippetExpression
	{
		public CodeAssignAsExpression( string targetObject, string type, TargetLanguage targetLanguage )
		{
			if (targetLanguage == TargetLanguage.VB)
				base.Value = "TryCast(" + targetObject + ", " + type + ")";
			else
				base.Value = targetObject + " as " + type;
		}
	}
}
