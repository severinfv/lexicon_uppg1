# Personal Register APP
A simple C# console application to load, view, search, modify, and extend CSV data.

## Features
- Load and parse CSV file
- Print schema and stats
- Add, edit, delete records
- Full table view

## Architecture

- `main`: Entry point to initialize the app
- `UserInterface`: UI loop, holds a `CsvRegister` object
- `CsvRegister`: Responsible for reading from, editing, and writing to the CSV file.
- `Record`: Stores values and provides methods for editing and displaying the record.

## Setup

1. Run the app: dotnet run