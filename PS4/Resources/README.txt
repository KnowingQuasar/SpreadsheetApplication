PS5 - Spreadsheet Logic Extensions

Written by Ian Rodriguez

Initial code completed and commented on 9/29/2016
PS5's additions completed and commented on 10/07/2016

I initially began this project with the idea that its purpose was to setup the general structure of the
spreadsheet using DependencyGraphs and Formulas. To do this, I included previous code from PS2 and PS3. I 
edited neither of these, as there were very few errors in PS2 (specifically having to do with incrementing 
the size of the graph) and no errors in PS3. As I structured the code, I built tests for each method I stubbed
out. This meant that I was able to easily do TDD (test driven development) and write better code faster.

As this program branched into PS5, I was very confused by the amount of things I needed to track for the 
spreadsheet logic. However, as time progressed, I began to understand more and more how the spreadsheet 
could be made to work efficiently. I believe I have understood the specifications to their full extent and have
implemented all necessary methods correctly. 

Please note that my code coverage is about 97%. The only portion of code not covered is technically reachable,
but not something that I will be able to test easily and similar logic is covered by a test involving changing
dependencies with cells containing Formulas.