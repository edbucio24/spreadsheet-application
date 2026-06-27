# Spreadsheet Application

This project is a desktop-based spreadsheet application built in C# that resembles the functionality of Microsoft Excel. This application demonstrates event-driven programming, basica formula parsing, and object oriented programming.

# Core Architecture & Software Engineering Patterns

This application serves as a practical implementation of several advanced programming techniques:
* **The Command Design Pattern:** Powers the robust Undo/Redo engine by encapsulating user operations as execution objects.
* **Expression Trees & Stacks:** Parses and evaluates complex mathematical formulas dynamically at runtime.
* **Reflection & Assembly Inspection:** Dynamically loads supported mathematical operators from the project's assembly, enabling open-closed principle compliance.
* **Event-Driven Programming & Delegates:** Handles UI synchronization, data model propagation, and reactive cell dependency updates.
* **LINQ (Language Integrated Query):** Utilized for efficient data filtering, error state analysis, and XML node traversal.
* **Unit Testing:** Built with **NUnit** to ensure complete regression coverage.

# Features
* **Advanced Equation Engine:** Evaluates equations starting with `=` by converting standard mathematical infix text into postfix notation using the Shunting-yard algorithm, creating and executing a binary expression tree that supports variable cell references (e.g., `=A1 * 28`).
* **Comprehensive Operator Support:** Built-in calculation support includes addition (`+`), subtraction (`-`), multiplication (`*`), division (`/`), and exponentiation (`^`) handled natively through modular node evaluation math classes.
* **Extensible Operator System:** Uses C# Reflection at runtime to inspect the project assembly and dynamically load isolated mathematical operator classes inheriting from a base `OperatorNode`, facilitating the seamless introduction of new functions without rewriting core logic.
* **Custom Cell Color Formatting:** Enables users to change the background colors of any highlighted cell selection dynamically using either the top menu options or an intuitive right-click context menu.
* **Smart Error Detection:** Continuously validates cell dependency chains to intercept and flag structural calculation risks, including uninitialized variables, invalid cell names, self-references, and arbitrary-length circular reference paths.
* **Robust Undo/Redo Engine:** Implements the Command Design Pattern backed by dual execution stacks to let users toggle back and forth through text edits and cell color formatting changes via standard desktop hotkeys (`Ctrl + Z` and `Ctrl + Shift + Z`).
* **XML File Serialization:** Features resilient saving and loading engines via `Ctrl + S` and `Ctrl + O` that serialize spreadsheet states into standard XML files while automatically discarding corrupted formatting, irrelevant metadata, or scrambled tag structures safely.

* # Getting Started

### Prerequisites
* **.NET 8.0 SDK** or later
* Visual Studio 2022 (with the .NET Desktop Development workload installed)

Clone the repository:
```bash
git clone [https://github.com/edbucio24/spreadsheet-application.git](https://github.com/edbucio24/spreadsheet-application.git)
```

Navigate to the solution directory:
```bash
cd spreadsheet-application
```

Restore dependencies and run the application:
```bash
dotnet run --project SpreadsheetApp
```

Run unit tests:
```bash
dotnet test SpreadsheetSheetTests
```



