# Contributing to NextDepartures

Thank you for your interest in contributing to NextDepartures! We 
welcome contributions from everyone.

## Prerequisites

If you want to develop on your local machine you will need the 
following prerequisites for local development. If you are 
developing in the cloud then GitHub Codespaces will install 
what you need when you create the codespace. For local development:

* Ensure you have a .NET 8 and .NET 9 SDK installed on your 
  machine. The [.NET Website](https://dotnet.microsoft.com/en-us/download) has what you need.
* Ensure you have a code editor installed such as 
  [Visual Studio](https://visualstudio.com), [Visual Studio Code](https://code.visualstudio.com) or [Rider](https://www.jetbrains.com/rider).

## Getting Started

To get started, follow these steps:

* Fork the repository to your own GitHub account.
* Clone the forked repository to your local machine or use a 
  GitHub codespace.

```
git clone https://github.com/your-username/NextDepartures.git
```

* Create a new branch for your changes.

```
git checkout -b feature/feature-name
git checkout -b fix/fix-name
```

* Make your changes and commit them locally.

```
git commit -m "Your commit message."
```

* Push your changes to your forked repository on 
  GitHub (ex: https://github.com/your-username/NextDepartures).

```
git push
```

* Open a pull request against the `master` branch of the original 
  repository (ex: https://github.com/philvessey/NextDepartures).

## Code Style

We follow the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) for our C# 
code. Please make sure your code adheres to this style guide 
before submitting a pull request.  Pull requests will not be 
accepted without a passing build from Azure DevOps. Tests will 
run automatically when you create a pull request against 
the `master` branch of the original repository.

## Unit Tests

We use VSTest for our unit tests. Please make sure your changes 
include appropriate unit tests in the `NextDepartures.Test` 
project. Tests are set up for each endpoint in the library. If 
you create a new endpoint, create a new test for it. If you 
modify an existing endpoint, please make sure your changes are 
reflected in the existing test.

## Reporting Bugs

If you find a bug, please open an issue in the repository and 
include as much detail as possible about the bug and how to 
reproduce it.

## Lead Project Maintainer

[Phil Vessey](https://github.com/philvessey) is the lead project maintainer. Feel free to 
reach out to him for any significant inquiries or guidance 
regarding NextDepartures.

## Code of Conduct

We adhere to the Contributor Covenant [Code of Conduct](./CODE_OF_CONDUCT.md). By 
participating, you are expected to uphold this code.

## License

By contributing to NextDepartures, you agree that your 
contributions will be licensed under the [MIT License](./LICENSE).