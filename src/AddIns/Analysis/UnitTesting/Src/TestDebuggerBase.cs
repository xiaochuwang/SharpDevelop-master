﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.UnitTesting
{
	public abstract class TestDebuggerBase : TestRunnerBase
	{
		IUnitTestDebuggerService debuggerService;
		IMessageService messageService;
		IDebugger debugger;
		ITestResultsMonitor testResultsMonitor;
		
		public TestDebuggerBase()
			: this(new UnitTestDebuggerService(),
				ServiceManager.Instance.MessageService,
				new TestResultsMonitor())
		{
		}
		
		public TestDebuggerBase(IUnitTestDebuggerService debuggerService,
			IMessageService messageService,
			ITestResultsMonitor testResultsMonitor)
		{
			this.debuggerService = debuggerService;
			this.messageService = messageService;
			this.testResultsMonitor = testResultsMonitor;
			this.debugger = debuggerService.CurrentDebugger;
			
			testResultsMonitor.TestFinished += OnTestFinished;
		}
		
		protected ITestResultsMonitor TestResultsMonitor {
			get { return testResultsMonitor; }
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			if (IsDebuggerRunning) {
				if (CanStopDebugging()) {
					debugger.Stop();
					Start(startInfo);
				}
			} else {
				Start(startInfo);
			}
		}
		
		public bool IsDebuggerRunning {
			get { return debuggerService.IsDebuggerLoaded && debugger.IsDebugging; }
		}
		
		bool CanStopDebugging()
		{
			string question = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingQuestion}";
			string caption = "${res:XML.MainMenu.RunMenu.Compile.StopDebuggingTitle}";
			return messageService.AskQuestion(question, caption);
		}
		
		void Start(ProcessStartInfo startInfo)
		{
			testResultsMonitor.Start();
			StartDebugger(startInfo);
		}
		
		void StartDebugger(ProcessStartInfo startInfo)
		{
			LogCommandLine(startInfo);
			
			bool running = false;
			debugger.DebugStopped += DebugStopped;
			try {
				debugger.Start(startInfo);
				running = true;
			} finally {
				if (!running) {
					debugger.DebugStopped -= DebugStopped;
				}
			}
		}
		
		void DebugStopped(object source, EventArgs e)
		{
			debugger.DebugStopped -= DebugStopped;
			OnAllTestsFinished(source, e);
		}
		
		public override void Stop()
		{
			if (debugger.IsDebugging) {
				debugger.Stop();
			}
			
			testResultsMonitor.Stop();
			testResultsMonitor.Read();
		}
		
		public override void Dispose()
		{
			Stop();
			testResultsMonitor.Dispose();
		}
	}
}
