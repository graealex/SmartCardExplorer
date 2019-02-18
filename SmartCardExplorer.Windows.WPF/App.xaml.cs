using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SmartCardReader.Windows.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public static string LogFile
		{
			get
			{
				return Assembly.GetExecutingAssembly().Location;
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Logger.Info("Application: '{0}'", Assembly.GetExecutingAssembly().Location);
			Logger.Info("Application Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
			Logger.Info("OS: '{0}'", Environment.OSVersion.VersionString);
		}

		public static void SetupLoggingConfig()
		{
			LoggingConfiguration config = new LoggingConfiguration();

			string outdatedLogFile = Path.ChangeExtension(LogFile, "archive.1.log");
			if (File.Exists(outdatedLogFile))
			{
				File.Delete(outdatedLogFile);
			}

			FileTarget fileTarget = new FileTarget
			{
				FileName = Path.ChangeExtension(LogFile, "log"),
				ArchiveFileName = Path.ChangeExtension(LogFile, "archive.{#}.log"),
				ArchiveNumbering = ArchiveNumberingMode.Rolling,
				MaxArchiveFiles = 2,
				ArchiveAboveSize = 8 * 1024 * 1024,
				KeepFileOpen = true,
				Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss}\t${threadid}\t${level}\t${logger}\t${message}\t${onexception:EXCEPTION\\: ${exception:format=tostring}}"
			};
			config.AddTarget("logfile", fileTarget);

#if DEBUG
			LoggingRule ruleFile = new LoggingRule("*", LogLevel.Trace, fileTarget);
			config.LoggingRules.Add(ruleFile);
#else
			LoggingRule ruleFile = new LoggingRule("*", LogLevel.Info, fileTarget);
			config.LoggingRules.Add(ruleFile);				
#endif

			EventLogTarget eventLogTarget = new EventLogTarget()
			{
				Log = "SmartCard Explorer",
				Source = "SmartCardReader.Windows.WPF",
				Layout = "${level}: ${logger} / ${threadid}\n${message}\n${onexception:EXCEPTION\\: ${exception:format=tostring}}"
			};
			config.AddTarget("eventlog", eventLogTarget);

#if DEBUG
			LoggingRule ruleDebugger = new LoggingRule("*", LogLevel.Trace,
				new DebuggerTarget()
				{
					OptimizeBufferReuse = true,
					Layout = "${date:format=HH\\:mm\\:ss} ${threadid} ${level} ${logger:shortName=true}\t${message}\t${onexception:EXCEPTION\\: ${exception:format=tostring}}",
				}
			);
			config.LoggingRules.Add(ruleDebugger);
			LoggingRule ruleConsole = new LoggingRule("*", LogLevel.Trace,
				new ColoredConsoleTarget()
				{
					OptimizeBufferReuse = true,
					Layout = "${date:format=HH\\:mm\\:ss} ${threadid} ${level} ${logger:shortName=true}\t${message}\t${onexception:EXCEPTION\\: ${exception:format=tostring}}",
				}
			);
			config.LoggingRules.Add(ruleConsole);
			LoggingRule ruleEventLog = new LoggingRule("*", LogLevel.Trace, eventLogTarget);
			config.LoggingRules.Add(ruleEventLog);
#else
			LoggingRule ruleEventLog = new LoggingRule("*", LogLevel.Info, eventLogTarget);
			config.LoggingRules.Add(ruleEventLog);
#endif

			LogManager.Configuration = config;


			AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs exArgs)
			{
				Logger.Fatal(exArgs.ExceptionObject as Exception, "OnUnhandledException");
			};
		}
	}
}
