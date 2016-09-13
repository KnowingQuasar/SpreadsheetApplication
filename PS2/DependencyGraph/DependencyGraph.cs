// SpreadsheetUtilities written by Ian Rodriguez (u1071551)
// Purpose: To create and manage dependency graphs for later use in creating spreadsheets.
// Version 1.0 - passes all tests and works as intended.

using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// Dictionary holding the dependents of given strings.
        /// </summary>
        private SortedDictionary<string, List<string>> Dependents;
        /// <summary>
        /// Dictionary holding the dependees of given strings.
        /// </summary>
        private SortedDictionary<string, List<string>> Dependees;
        /// <summary>
        /// Integer size of the dependency graph.
        /// </summary>
        private int sizeOfDG;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            Dependents = new SortedDictionary<string, List<string>>();
            Dependees = new SortedDictionary<string, List<string>>();
            sizeOfDG = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        /// <returns>Returns the size of the dependency graph.</returns>
        public int Size
        {
            get { return sizeOfDG; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        /// <param name="s">String used to get the size of the dependees list.</param>
        /// <returns>Returns the size of the dependees list of the given list.</returns>
        public int this[string s]
        {
            get { return GetDependees(s).Count(); }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        /// <param name="s">String to check for dependents.</param>
        /// <returns>Returns true if given string has dependents, false if it doesn't.</returns>
        public bool HasDependents(string s)
        {
            if (Dependents.ContainsKey(s) && Dependents[s].Count > 0)
                return true;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        /// <param name="s">String to check for dependees.</param>
        /// <returns>Returns true if given string has dependees, false if it doesn't.</returns>
        public bool HasDependees(string s)
        {
            if (Dependees.ContainsKey(s) && Dependees[s].Count > 0)
                return true;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        /// <param name="s">String to get the dependents of.</param>
        /// <returns>Returns a list representing the dependents for the given string.</returns>
        public IEnumerable<string> GetDependents(string s)
        {
            if (Dependents.ContainsKey(s) && Dependents[s].Count > 0)
                return Dependents[s];
            return new List<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        /// <param name="s">String to get the dependees of.</param>
        /// <returns>Returns a list representing the dependees for the given string.</returns>
        public IEnumerable<string> GetDependees(string s)
        {
            if (Dependees.ContainsKey(s) && Dependees[s].Count > 0)
                return Dependees[s];
            return new List<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        
        public void AddDependency(string s, string t)
        {
            if (sizeOfDG == 0)
            {
                Dependents.Add(s, new List<string>(new string[] { t }));
                Dependees.Add(t, new List<string>(new string[] { s }));
                sizeOfDG++;
                return;
            }
            else
            {
                if (GetDependents(s).Count() > 0)
                {
                    if (!Dependents[s].Contains(t))
                        Dependents[s].Add(t);
                    else
                        return;
                }
                else
                    Dependents.Add(s, new List<string>(new string[] { t }));
                if (GetDependees(t).Count() > 0)
                {
                    if (!Dependees[t].Contains(s))
                        Dependees[t].Add(s);
                    else
                        return;
                }
                else
                    Dependees.Add(t, new List<string>(new string[] { s }));
            }
            sizeOfDG++;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (GetDependents(s).Count() > 0 && GetDependents(s).Contains(t) && GetDependees(t).Count() > 0 && GetDependees(t).Contains(s))
            {
                Dependents[s].Remove(t);
                Dependees[t].Remove(s);
                sizeOfDG--;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        /// <param name="s">String to replace the dependents of.</param>
        /// <param name="newDependents">List of new dependents to replace the old ones.</param>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (HasDependents(s))
            {
                sizeOfDG -= Dependents[s].Count();
                foreach (string dep in Dependents[s])
                    Dependees[dep].Remove(s);
                Dependents.Remove(s);
            }
            foreach (string dependent in newDependents)
            {
                AddDependency(s, dependent);
                sizeOfDG++;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        /// <param name="s">String to replace the dependees of.</param>
        /// <param name="newDependees">List of new dependees to replace the old ones.</param>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (HasDependees(s))
            {
                sizeOfDG -= Dependees[s].Count();
                foreach (string dep in Dependees[s])
                    Dependents[dep].Remove(s);
                Dependees.Remove(s);
            }
            foreach (string dependent in newDependees)
            {
                AddDependency(dependent, s);
                sizeOfDG++;
            }
        }

    }
}