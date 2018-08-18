using System;
using System.Collections.Generic;
using System.Linq;

namespace RWT.ArgParse
{
    public class ArgParser
    {
        /// <summary>
        /// the number of extras that are required and allowed during the parse.
        /// Fewer extras than the required amount will cause an exception.
        /// More extras than the allowed amount will cause an exception.
        /// </summary>
        public (int Required, int Allowed) ExtrasRange { get; set; }

        private readonly Dictionary<String, Switch> Switches;

        public ArgParser(params Switch[] args)
        {
            Switches = args.ToDictionary(sw => sw.Name);
            ExtrasRange = (Required: 0, Allowed: Int32.MaxValue);
        }

        /// <summary>
        /// Activate the named switch with a given value, outside the normal Parse() workflow.
        /// </summary>
        /// <param name="name">the name of the switch to activate</param>
        /// <param name="arg">the string argument to give the switch (default null)</param>
        public void ActivateSwitch(String name, String arg = null)
        {
            if (Switches.TryGetValue(name, out var sw))
            {
                sw.Accept(this, arg);
            }
            else
            {
                throw new ArgParseException($"<{name}> could not activate: it is not a switch!");
            }
        }

        /// <summary>
        /// Parse the given command-line arguments, activating switches as needed, and returning a list
        /// of all arguments which did not invoke a switch.
        /// </summary>
        /// <param name="args">an array of strings representing command-line arguments</param>
        /// <returns>a List of all strings which did not activate a switch</returns>
        public List<String> Parse(String[] args)
        {
            var extras = new List<String>();
            var idx = 0;
            var alen = args.Length;

            // loop through the arguments collecting switches
            while (idx < alen)
            {
                var hd = args[idx++];
                if (Switches.TryGetValue(hd, out var s))
                {
                    if (s.NeedsArg)
                    {
                        if (idx == alen) throw new ArgParseException($"Switch <{s.Name}> expects an argument!");
                        s.Accept(this, args[idx++]);
                    }
                    else
                    {
                        s.Accept(this, null);
                    }
                }
                else
                {
                    extras.Add(hd);
                }
            }

            // check that the extras are in the [required,allowed] range:
            var ecnt = extras.Count;
            if (ecnt < ExtrasRange.Required)
                throw new ArgParseException($"Not enough arguments (got {ecnt} but need {ExtrasRange.Required})!");
            if (ecnt > ExtrasRange.Allowed)
                throw new ArgParseException($"Too many arguments (got {ecnt} when maximum is {ExtrasRange.Allowed})!");

            // now that we are out of arguments, apply defaults to any we haven't seen.
            foreach (var s in Switches.Values)
            {
                s.ApplyDefault();
            }

            // give back any extra (non-switch) args in a list.
            return extras;
        }

        /// <summary>
        /// Generates a helpful list of the configured options
        /// </summary>
        /// <param name="tw">a TextWriter to use for output</param>
        public void ShowOptions(System.IO.TextWriter tw)
        {
            tw.WriteLine("OPTIONS");
            foreach (var s in Switches.Values.OrderBy(s => s.Name))
            {
                var lhs = s.Name;
                var rhs = s.Help;
                if (rhs.StartsWith("<"))
                {
                    var endArg = rhs.IndexOf('>') + 1;
                    lhs = $"{lhs} {rhs.Substring(0, endArg).Trim()}";
                    rhs = rhs.Substring(endArg).TrimStart();
                }
                if (lhs.Length <= 5)
                {
                    tw.WriteLine($"  {lhs,-5}  {rhs}");
                }
                else
                {
                    tw.WriteLine($"  {lhs}");
                    tw.WriteLine($"         {rhs}");
                }
                tw.WriteLine();
            }
        }

    }
}

