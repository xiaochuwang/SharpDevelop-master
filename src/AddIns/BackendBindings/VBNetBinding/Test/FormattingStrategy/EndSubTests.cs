﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
{
	/// <summary>
	/// Tests that Operator overrides have "End Operator" added after the user presses the return key.
	/// </summary>
	[TestFixture]
	public class EndInsertionTests
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, "VBNetBindingTests");
			}
		}

		[Test]
		public void EndSub()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"End Class";
			
			string bar = "Bar\r\n";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\t\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Sub Bar\r\n" +
			                      "\t\t").Length;

			RunTest(code, cursorOffset, expectedCode, expectedOffset, '\n');
		}


		void RunTest(string code, int cursorOffset, string expectedCode, int expectedOffset, char keyPressed)
		{
			AvalonEditTextEditorAdapter editor = new AvalonEditTextEditorAdapter(new TextEditor());
			editor.Document.Text = code;
			editor.Caret.Offset = cursorOffset;
			VBNetFormattingStrategy formattingStrategy = new VBNetFormattingStrategy();
			formattingStrategy.FormatLine(editor, keyPressed);
			Assert.AreEqual(expectedCode, editor.Document.Text);
			Assert.AreEqual(expectedOffset, editor.Caret.Offset);
		}
		
		[Test]
		public void EndIf()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd Sub\r\n" +
				"End Class";
			
			string bar = "Then\r\n";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then\r\n" +
				"\t\t\t\r\n" +
				"\t\tEnd If\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Sub Bar\r\n" +
			                      "\t\tIf True Then\r\n" +
			                      "\t\t\t").Length;
			
			RunTest(code, cursorOffset, expectedCode, expectedOffset, '\n');
		}
		
		[Test]
		public void SingleLineIf()
		{
			string code = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then _\r\n" +
				"\r\n" + // This extra new line is required. This is the new line just entered by the user.
				"\tEnd Sub\r\n" +
				"End Class";
			
			string bar = "Then _\r\n";
			int cursorOffset = code.IndexOf(bar) + bar.Length;
			
			string expectedCode = "Public Class Foo\r\n" +
				"\tPublic Sub Bar\r\n" +
				"\t\tIf True Then _\r\n" +
				"\t\t\t\r\n" +
				"\tEnd Sub\r\n" +
				"End Class";
			
			int expectedOffset = ("Public Class Foo\r\n" +
			                      "\tPublic Sub Bar\r\n" +
			                      "\t\tIf True Then _\r\n" +
			                      "\t\t\t").Length;
			
			RunTest(code, cursorOffset, expectedCode, expectedOffset, '\n');
		}
	}
}
