
# Value Watcher Idea

### Objective
Function as a runtime debug tool that allows monitoring the value of node data in real time, with multiple visualization options such as plotting value over time, showing min/max value, etc.

### Ideas

- Add an annotation to any variable, like ```[DebugWatch]``` or similar
- Collect all the watched variables at compile time using analyzers
- At runtime, create a watcher object for each node that has watched variables
- Each frame, record every variable's value
- Add a new tab near the inspector that displays the watcher associated with the selected node

https://chatgpt.com/c/67ca63f2-0014-8007-979e-aa512e187693
