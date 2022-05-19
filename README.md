he Smart Infection Control System (SmICS) is an application for the support of the infection control units in clinics. For the general use it is necessary to connect the application to an openEHR Repository like [ehrbase](https://github.com/ehrbase/ehrbase). It offers different statistics, a patient timeline of the patients locations and a contact network for patients to track possible transmission paths. 

___
## Requirements

### Repository

<br>

**IMPORTANT:** If you want to run the SmICS in its authentication version your openEHR Respository needs to be configured to work with oauth2 authentication


Installed and functional openEHR Repository which provides the basic REST API from the openEHR Reference Model.

The openEHR Repository needs to be prefilled with following templates and compositions for these templates:

- [Stationärer Versorgungsfall](https://ckm.highmed.org/ckm/templates/1246.169.620)
- [Patientenaufenthalt](https://ckm.highmed.org/ckm/templates/1246.169.590) *(Altough "Station" is no mandatory field in the template, it is necessary for the full functionality for the SmICS)*
- [Virologischer Befund](https://ckm.highmed.org/ckm/templates/1246.169.636)
- [Impfstatus](https://ckm.highmed.org/ckm/templates/1246.169.1187)
- [Symptome](https://ckm.highmed.org/ckm/templates/1246.169.1109)
- [Mikrobiologischer Befund](https://ckm.highmed.org/ckm/templates/1246.169.69)

### Hardware

#### Server 
- CPU: 4 Cores<sup>1</sup> 
- RAM: 4 GB<sup>1</sup> 
- Storage: 30 GB<sup>1</sup> 
- OS: Linux *(recommended)*<sup>2</sup> 

<sup>1</sup> *Estimated Requirements*

<sup>2</sup>*Although you could use it with Windows if your Docker is able to work with Linux Docker Container. On Windows Server there is a LinuxKit necessary which is available for Windows Server 2019*

### Workstation
- Full HD Monitor (or higher)
- Google Chrome Browser Version 88 (or newer)

### Docker

Docker and docker-compose tool. 

Installation: https://docs.docker.com/engine/install/ and https://docs.docker.com/compose/install/

___
## Preparation

### 1. Get the Software

Download the latest versions of the SmICSCore and the SmICS Visualization (without the authentication feature) in the same directory and unpack it.

SmICSCore: https://github.com/highmed/SmICSCore/releases <br>
SmICS Visualization: https://github.com/highmed/SmICSVisualisierung/releases

### 2. Prepare Certificates

You need:
<ul>
<li>Root Certificates</li>
</ul>

If the SmICSCore Directory doesn't contain a <i>Certificates</i> folder, create one within the SmICSCore Directory.
```
mkdir SmICSCore/Certificates
```

<ul>
<li>Copy the root certificates to the Certificates Folder</li>
</ul>

### 3.Prepare Resources Volume

Create a "SmICSData" folder at the same path where the SmICSCore is located.

```
mkdir SmICSData
```

### 4. Editing docker-compose.yml

The docker-compose.yml is located in the SmICSCore folder.

Every Environment Variable which contains an expression with ```<>``` needs to be replaced by your local settings.

### 5. Running docker-compose

Enter the SmICSCore directory and enter following command. 

```
docker-compose up --build -d
```
