PS4 - Spreadsheet

Written by Ian Rodriguez

Code completed and commented on 9/29/2016

I initially began this project with the idea that its purpose was to setup the general structure of the
spreadsheet using DependencyGraphs and Formulas. To do this, I included previous code from PS2 and PS3. I 
edited neither of these, as there were very few errors in PS2 (specifically having to do with incrementing 
the size of the graph) and no errors in PS3. As I structured the code, I built tests for each method I stubbed
out. This meant that I was able to easily do TDD (test driven development) and write better code faster.

There is only one spot in the code where the specification for the assignment seemed to almost need the 
functionality that will be added in PS5. When changing the value of a cell, I run the function 
GetCellsToRecalculate(), but do not use the returned list. However, this means that I never remove dependencies
from the spreadsheets dependency graph, as in PS5 I will be making a function that both evaluates the new 
contents of cells and removes/changes their associated dependencies. 

Please note that my code coverage is about 97%. The only portion of code not covered is technically reachable,
but not something that I will be able to test easily and similar logic is covered by a test involving changing
dependencies with cells containing Formulas.