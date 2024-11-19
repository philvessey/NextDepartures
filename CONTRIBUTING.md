# Contributing to NextDepartures

Thank you for your interest in contributing to NextDepartures! We welcome contributions from everyone.

## Getting Started

To get started, follow these steps:

* Fork the repository to your own GitHub account.
* Clone the forked repository to your local machine or use a GitHub codespace.
* Create a new branch for your changes.
* Make your changes and commit them locally.
* Push your changes to your forked repository on GitHub (ex: username/NextDepartures).
* Open a pull request to the main repository (ex: philvessey/NextDepartures).

## Code Style

We follow the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) for our C# code. Please make sure your code adheres to this style guide before submitting a pull request.  Pull requests will not be accepted without a passing build from Azure DevOps. Tests will run automatically when you create a pull request.

## Unit Tests

We use VSTest for our unit tests. Please make sure your changes include appropriate unit tests in the `NextDepartures.Test` project. Tests are setup for each endpoint in the library. If you create a new endpoint, create a new test for it. If you modify an existing endpoint, please make sure your changes are reflected in the existing test.

## Reporting Bugs

If you find a bug, please open an issue in the repository and include as much detail as possible about the bug and how to reproduce it.