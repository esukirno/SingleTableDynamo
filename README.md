![.NET Core](https://github.com/esukirno/SingleTableDynamo/workflows/.NET%20Core/badge.svg)

# Single Table DynamoDb

If you are here, chances are that you have heard the concept from re:Invent DynamoDb Advanced Design Pattern talk.
This use of single table is mention in:
re:Invent 2018
https://www.youtube.com/watch?v=HaEPXoXVf2k

re:Invent 2019
https://www.youtube.com/watch?v=6yqfmXiZTlM

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

* dotnet core 2.1

* nuke.build tooling

```
dotnet tool install Nuke.GlobalTool --global

```

* Docker (to run component tests)

### Installing

Clone this repository onto your development environment.


## Running the tests

This solution contains 2 tests: unit tests and component tests. Each tests can be run using xunit test runner (via Visual Studio).
Or if cli is preferred, you can run the following nuke.build command from root repository folder.

```
.\build.ps1 --target UnitTest
```
```
.\build.ps1 --target ComponentTest
```

## Built With

* [nuke.build](https://nuke.build/)
* [Docker Compose](https://docs.docker.com/compose/)
* [AWS SDK](https://aws.amazon.com/sdk-for-net/)
* [xunit](https://xunit.net/)


## Authors

* **Edwin Sukirno**