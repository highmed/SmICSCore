# SmICS

The Smart Infection Control System (SmICS) is an application for the support of the infection control units in clinics. For the general use it is necessary to connect the application to an openEHR Repository like [ehrbase](https://github.com/ehrbase/ehrbase). It offers different statistics, a patient timeline of the patients locations and a contact network for patients to track possible transmission paths. 

___
## Requirements

### Repository

Installed and functional openEHR Repository which provides the basic REST API from the openEHR Reference Model.

The openEHR Repository needs to be prefilled with following templates and compositions for these templates:

- [Stationärer Versorgungsfall](https://ckm.highmed.org/ckm/templates/1246.169.620)
- [Patientenaufenthalt](https://ckm.highmed.org/ckm/templates/1246.169.590) *(Altough "Station" is no mandatory field in the template, it is necessary for the full functionality for the SmICS)*
- [Virologischer Befund](https://ckm.highmed.org/ckm/templates/1246.169.636)

*Upcoming:*
- *[Imfpstatus](https://ckm.highmed.org/ckm/templates/1246.169.1187)*
- *[Symptome](https://ckm.highmed.org/ckm/templates/1246.169.1109)*
- *[Mikrobiologischer Befund](https://ckm.highmed.org/ckm/templates/1246.169.69)*

### Hardware

#### Server 
- CPU: 4 Cores<sup>1</sup> 
- RAM: 4 GB<sup>1</sup> 
- Storage: 5 GB<sup>1</sup> 
- OS: Linux *(recommended)*<sup>2</sup> 

<sup>1</sup> *Estimated Requirements*

<sup>2</sup>*Although you could use it with Windows if your Docker is able to work with Linux Docker Container. On Windows Server there is a LinuxKit necessary which is available for Windows Server 2019*

### Workstation
- Full HD Monitor (or higher)
- Google Chrome Browser Version 88 (or newer)

### Docker

Docker or docker-compose tool. 

Installation: https://docs.docker.com/engine/install/ and if necessary: https://docs.docker.com/compose/install/

___
## Execution

Clone this Repository as whole and build form the root folder.

**Build Process - Docker**

```
docker build --build-arg repo="http://localhost:8080/ehrbase/rest/openehr/v1"  -t smics .
docker build -t smics-visualisierung .
```
The argument variable ```repo``` contains the connection string to the openEHR REST API.

**Run Process - Docker**
```
docker network create smics-net
docker run --name smics_core --network smics-net -d -p 80:80 smics
docker run --name smics_visualisierung --network smics-net -d -p 8080:3231 smicsvisualisierung
```

If you want to change the ports through which the applications are accessible, you have to change the first port in ```-p 80:80``` and/or ```-p 8080:3231```.

**Build & Run Process - Docker Compose**
Edit the ```args: repo:``` in the ```docker-compose.yml``` and enter your connection string to you openEHR REST API.

```
docker-compose up -d
```

The SmICS Core Componentes should know be reachable via ```http://localhost``` and the SmICS Visualisierungs Componentents via ```http://localhost:8080``` on the machine where you installed the Docker Container. 