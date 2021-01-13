# Caffe Menu Bot (Back-end)

This project is powered by ASP.NET Core 5.

### Build statuses
| Build Service | Status | Configuration |
| ------------- | ------ | ------------- |
| Azure Pipelines | **macOS**<br/>![Azure Pipelines - macOS](https://img.shields.io/azure-devops/build/vova-lantsov/0aba9e48-2760-46e5-977e-74cfad73d964/1/main?job=Test&logo=azure-devops&stage=macOS-latest&style=for-the-badge)<br/>**Windows**<br/>![Azure Pipelines - Windows](https://img.shields.io/azure-devops/build/vova-lantsov/0aba9e48-2760-46e5-977e-74cfad73d964/1/main?job=Test&logo=azure-devops&stage=windows-latest&style=for-the-badge) | [azure-pipelines.yml](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/azure-pipelines.yml) |
| TeamCity | **Debian**<br/>![TeamCity](https://img.shields.io/teamcity/build/s/CaffeMenuBot_CaffeMenuBotBackend_Compile?logo=teamcity&server=https%3A%2F%2Ftc.vova-lantsov.dev&style=for-the-badge) | [settings.kts](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/.teamcity/settings.kts) (Kotlin DSL) |
| GitHub Actions | **Windows, macOS, Ubuntu**<br/>![GitHub Actions](https://img.shields.io/github/workflow/status/krok-lives-matter/CaffeMenuBot_Back-End/build/main?logo=github&style=for-the-badge) | [build.yml](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/.github/workflows/build.yml) |
### Test statuses
| Test Service | Status | Configuration | Build Url |
| ------------ | ------ | ------------- | --------- |
| AppVeyor | **Ubuntu**<br/>![AppVeyor](https://img.shields.io/appveyor/tests/vova-lantsov-dev/caffemenubot-back-end?logo=appveyor&logoColor=white&style=for-the-badge) | [appveyor.yml](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/appveyor.yml) | [Link](https://ci.appveyor.com/project/vova-lantsov-dev/caffemenubot-back-end) |
| Azure Pipelines | **macOS, Windows**<br/>![Azure Pipelines - macOS](https://img.shields.io/azure-devops/tests/vova-lantsov/caffe-menu-bot/1/main?logo=azure-devops&style=for-the-badge) | [azure-pipelines.yml](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/azure-pipelines.yml) | [Link](https://dev.azure.com/vova-lantsov/caffe-menu-bot/_build?definitionId=1) |
| TeamCity | **Debian**<br/>![TeamCity - Debian](https://img.shields.io/teamcity/build/s/CaffeMenuBot_CaffeMenuBotBackend_Test?logo=teamcity&label=tests&server=https%3A%2F%2Ftc.vova-lantsov.dev&style=for-the-badge) | [settings.kts](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/.teamcity/settings.kts) (Kotlin DSL) | [Link](https://tc.vova-lantsov.dev/viewType.html?buildTypeId=CaffeMenuBot_CaffeMenuBotBackend_Test) |
| GitHub Actions | **Windows, macOS, Ubuntu**<br/>![GitHub Actions](https://img.shields.io/github/workflow/status/krok-lives-matter/CaffeMenuBot_Back-End/test/main?label=tests&logo=github&style=for-the-badge) | [build.yml](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/blob/main/.github/workflows/test.yml) | [Link](https://github.com/krok-lives-matter/CaffeMenuBot_Back-End/actions?query=workflow%3Atest) |

### Prerequisites
1. Install either the Visual Studio, the Visual Studio Code or the JetBrains Rider editor.
2. Install the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) on your PC.
3. Clone the project repository locally.
4. Install the [Docker for Windows](https://hub.docker.com/editions/community/docker-ce-desktop-windows/) on your PC (by pressing the _Get Docker_ button).
5. Ensure that `SVM mode` / `hypervisor` is enabled in your motherboard's BIOS.

### How To Run
To run this project locally, open a terminal (for example, `cmd.exe` or `Git Bash`) and navigate to the project's root directory (where the `docker-compose.yml` file is located).  
To build the containers, run the following command: `docker-compose build`  
To run the already built containers, run: `docker-compose up`  
You can also build the containers and run them simultaneously: `docker-compose up --build`  
If you want the containers to run in the background (even if you close the terminal), append the `-d` argument to the end of a command:
`docker-compose up -d` or `docker-compose up --build -d`  
To shut the containers down while they are working in the background, run:
`docker-compose down`  
To shut the containers down and destroy all the data stored in a database, run:
`docker-compose down -v`

As soon as Docker containers are started, you are able to call the API on 5000 port:
`http://localhost:5000/`.  
There is no HTTPS configured for the local web-host.
