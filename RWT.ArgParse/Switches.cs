using System;
using System.Collections.Generic;
using System.Linq;

namespace RWT.ArgParse
{
    public abstract class Switch
    {
        public String Name { get; protected set; }
        public String Help { get; protected set; }
        public Boolean NeedsArg { get; protected set; }
        protected Boolean Seen;
        public Boolean Required { get; set; }

        internal abstract void Accept(ArgParser ap, String v);
        internal abstract void ApplyDefault();

        protected Switch(String n, String h)
        {
            Name = n;
            Help = h;
            NeedsArg = true;
            Seen = false;
            Required = false;
        }
    }

    public abstract class Switch<T> : Switch
    {
        public Action<T> Command { get; set; }
        public IEnumerable<T> Options { get; set; }
        public T Default { get; set; }

        protected Switch(String n, String h) : base(n, h)
        {
            Command = null;
            Options = null;
            Default = default(T);
        }

        protected abstract T Parse(String s);

        internal override void Accept(ArgParser ap, String v)
        {
            Seen = true;
            var current = Parse(v);

            if (Options == null || Options.Contains(current))
            {
                Command(current);
            }
            else
            {
                throw new ArgParseException($"<{current}> isn't a valid argument to switch <{Name}>!");
            }
        }


        internal override void ApplyDefault()
        {
            if (!Seen)
            {
                if (!Required)
                {
                    Command(Default);
                }
                else
                {
                    throw new ArgParseException($"<{Name}> is a required option, but did not get a value!");
                }
            }
        }

    }

    public class Int32Arg : Switch<Int32>
    {
        public Int32Arg(string n, string h) : base(n, h)
        {
        }

        protected override int Parse(string s)
        {
            if (Int32.TryParse(s, out var result))
            {
                return result;
            }
            throw new ArgParseException($"Switch <{Name}> expected an integer, and got <{s}> instead.");
        }
    }

    public class DoubleArg : Switch<Double>
    {
        public DoubleArg(string n, string h) : base(n, h)
        {
        }

        protected override Double Parse(string s)
        {
            if (Double.TryParse(s, out var result))
            {
                return result;
            }
            throw new ArgParseException($"Switch <{Name}> expected a number, and got <{s}> instead.");
        }
    }

    public class StrArg : Switch<String>
    {
        public StrArg(string n, string h) : base(n, h)
        {
        }

        protected override string Parse(string s) => s;
    }


    /// A class for switches that are no-arg flags 
    public class FlagArg : Switch<Boolean>
    {
        public FlagArg(string n, string h) : base(n, h)
        {
            NeedsArg = false;
        }

        protected override bool Parse(string s) => true;
    }

    /// <summary>
    ///  A slightly different kind of Switch, to support calls back into the
    ///  ArgParser for the option list.
    /// </summary>
    public class HelpArg : Switch
    {
        public Action<Action<System.IO.TextWriter>> Command { get; set; }

        public HelpArg(string n) : base(n, "displays this help message")
        {
            NeedsArg = false;
        }

        internal override void Accept(ArgParser ap, string v)
        {
            Command(ap.ShowOptions);
        }

        internal override void ApplyDefault() { }
    }
}
