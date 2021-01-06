# Caffe Menu Bot (Back-end)

This project is powered by ASP.NET Core 5.

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