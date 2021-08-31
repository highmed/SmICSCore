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

Clone these two repositories:
```
git clone https://github.com/highmed/SmICSCore.git
git clone https://github.com/highmed/SmICSVisualisierung.git
```

**Build & Run Process - Docker**

Within each local git repository following commands need to be executed. **You need to start with the SmICSCore Repository**

```
docker network create smics-net
docker build -t smics .
docker run --name smics_core --network smics-net -e OPENEHR_DB="http://localhost:8080/ehrbase/rest/openehr/v1" -e OPENEHR_USER="$USERNAME" -e OPENEHR_PASSWD="$PASSWORD" -d -p 9787:9787 smics
```

```http://localhost:8080/ehrbase/rest/openehr/v1``` must be exchanged for the valid link to the openEHR REST API from the openEHR repository.
```$USERNAME``` and ```$PASSWORD``` must be exchanged for valid user credentials from the openEHR repository.

Before building the container for the SmICS Visualization you need to change a variable within the ```src/server/config.ts```. In **line 76** you need to change the variable **hostname: "localhost"** to the DNS adress of you server where the SmICS shall run. 

```
docker build -t smicsvisualisierung .
docker run --name smics_visualisierung --network smics-net -d -p 3231:3231 smicsvisualisierung
```

If you want to change the ports through which the applications are accessible, you have to change the first port in ```-p 9787:9787``` and/or ```-p 3231:3231```.
<!--
**Build & Run Process - Docker Compose**
Edit the ```args: repo:``` in the ```docker-compose.yml``` and enter your connection string to you openEHR REST API.

```
docker-compose up -d
```

The SmICS Core Componentes should know be reachable via ```http://localhost:9787``` and the SmICS Visualisierungs Componentents via ```http://localhost:3231`` on the machine where you installed the Docker Container. -->
