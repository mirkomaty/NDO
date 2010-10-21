using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using ClassGenerator;

namespace Generator
{
	class CodeCheckForNullExpression : CodeSnippetExpression
	{
		public CodeCheckForNullExpression( string variableName, TargetLanguage targetLanguage )
		{
			SetValue( variableName, targetLanguage );
		}

		public CodeCheckForNullExpression( CodeVariableReferenceExpression variableReference, TargetLanguage targetLanguage )
		{
			SetValue(variableReference.VariableName, targetLanguage);
		}

		private void SetValue( string variableName, TargetLanguage targetLanguage )
		{
			if ( targetLanguage == TargetLanguage.VB )
				this.Value = variableName + " Is Nothing";
			else
				this.Value = variableName + " == null";
		}		
	}
}
