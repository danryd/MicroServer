using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Tarro.Logging
{
    internal class ColoredConsoleTraceListener : TextWriterTraceListener
    {
        private readonly ConsoleColor initColor;
        public ColoredConsoleTraceListener()
            : base(Console.Out)
        {
            initColor = Console.ForegroundColor;

        }

        public ColoredConsoleTraceListener(bool useErrorStream)
            : base(useErrorStream ? Console.Error : Console.Out)
        {
        }

        public override void Close()
        {
        }

        readonly Dictionary<TraceEventType, ConsoleColor> colorMap = new Dictionary<TraceEventType, ConsoleColor>
        {
            {TraceEventType.Critical, ConsoleColor.Red},
            {TraceEventType.Error, ConsoleColor.Red},
            {TraceEventType.Warning, ConsoleColor.Yellow},
            {TraceEventType.Information, ConsoleColor.Green},
            {TraceEventType.Verbose, ConsoleColor.Green},
            {TraceEventType.Stop, ConsoleColor.Magenta},
            {TraceEventType.Start, ConsoleColor.Magenta},
            {TraceEventType.Suspend, ConsoleColor.Magenta},
            {TraceEventType.Transfer, ConsoleColor.Magenta},
            {TraceEventType.Resume, ConsoleColor.Magenta},

        };
        private void SetColor(TraceEventType eventType)
        {
            Console.ForegroundColor = colorMap[eventType];
        }
        private void ResetColor()
        {
            Console.ForegroundColor = initColor;
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            SetColor(eventType);
            base.TraceEvent(eventCache, source, eventType, id);
            ResetColor();
        }




        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format,
            params object[] args)
        {
            SetColor(eventType);
            if (this.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                return;
            this.Write(source + ": ");
            if (args != null)
                this.WriteLine(String.Format((IFormatProvider)CultureInfo.InvariantCulture, format, args));
            else
                this.WriteLine(format);
            ResetColor();
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            SetColor(eventType);
            if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
                return;
            this.Write(source + ": ");
            base.WriteLine(message);

            ResetColor();
        }
    }
}