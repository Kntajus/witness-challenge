# Assistant for The Witness challenge

## Disclaimer
This is entirely based on the good work already done by [Michael Gerasimenko](https://github.com/gerasimenko) over 5 years ago. You can find his original version in Java
[here](https://github.com/gerasimenko/witness-challenge).

## About
After struggling with the [The Witness challenge section](https://www.ign.com/wikis/the-witness/The_Caves#The_Challenge) for a while, I searched the web for any tips and tricks.
(To be fair, I was able to make it to the final 2 pillars a few times, so I wasn't completely useless.) That's when I came upon Michael's work that solves some of the puzzles
along the way for you from screenshots.

Intrigued about how it worked, I figured the best way for me to properly understand it was to port it across to a language I was more familiar with, as the process of getting
it to work would require me to dig into the detail a bit to fully grok it.

However once I got the initial port working I still had questions, so I started refactoring it to a structure that made a bit more sense to me. Only once I got started, I didn't
really stop... so here's my own version now, that at first glance looks almost nothing like the code that inspired it. However the actual brains of it (in terms of the approach
used to identify and solve the puzzles) is all still pretty much intact from the original.

## How to build/run
Easiest approach is to build with Visual Studio (remember you can always get the [Community Edition](https://visualstudio.microsoft.com/vs/community/) for free if you don't
have it already). Just like the original, it requires the input (screenshot) and output directories to be passed in as arguments on the command line, although you can always
populate them in the Debug section of the Project Properties and just hit the green "Play" button in Visual Studio if you prefer.