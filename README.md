# TorreDeControlAerolinea
API desarrollada en ASP.NET 7

# API de simulación de aerolínea

Esta API simula el funcionamiento de una aerolínea y proporciona endpoints para interactuar con aviones, aeropuertos y pasajeros. Está desarrollada utilizando ASP.NET 7 y SQL Server como base de datos. A continuación, se describen los aspectos clave de la API y cómo utilizarla.

## Requisitos

Para ejecutar esta API, debes tener instalado:

- ASP.NET 7
- SQL Server

## Configuración de la base de datos

Antes de utilizar la API, asegúrate de configurar correctamente la cadena de conexión a la base de datos. Para ello, sigue los pasos a continuación:

1. Abre el archivo `appsettings.json`.
2. Busca la sección `ConnectionStrings`.
3. Reemplaza el valor de `DefaultConnection` con la cadena de conexión a tu base de datos.

## Script de la base de datos

El script para crear la base de datos y sus tablas se encuentra en la carpeta "scriptsAerolinea". Sigue estos pasos para ejecutar el script:

1. Abre SQL Server Management Studio u otra herramienta de administración de bases de datos.
2. Conéctate a tu instancia de SQL Server.
3. Abre el archivo de script proporcionado en la carpeta "scriptsAerolinea".
4. Ejecuta el script para crear la base de datos y sus tablas.

## Endpoints

A continuación se detallan los endpoints disponibles en la API:

### Aviones

- `GET /api/Aviones`: Obtiene la lista de todos los aviones.
- `GET /api/Aviones/{id}`: Obtiene los detalles de un avión específico por su ID.
- `POST /api/Aviones`: Agrega uno o varios aviones. Recibe una lista de objetos Avion en el cuerpo de la solicitud.
- `DELETE /api/Aviones/{id}`: Elimina un avión específico por su ID.
- `GET /api/Aviones/avion/{avionId}`: Obtiene los pasajeros de un avión específico por su ID.
- `POST /api/Aviones/{avionId}/pasajeros`: Agrega un pasajero a un avión específico por su ID.

### Aeropuertos

- `GET /api/Aeropuertos`: Obtiene la lista de todos los aeropuertos.
- `GET /api/Aeropuertos/{id}`: Obtiene los detalles de un aeropuerto específico por su ID.
- `GET /api/Aeropuertos/GetAeropuertoPorId/{id}`: Obtiene los detalles de un aeropuerto específico por su ID.
- `POST /api/Aeropuertos`: Agrega uno o varios aeropuertos. Recibe una lista de objetos Aeropuerto en el cuerpo de la solicitud.
- `DELETE /api/Aeropuertos`: Elimina todos los aeropuertos.
- `GET /api/Aeropuertos/{id}/AvionesSalida`: Obtiene la lista de aviones que salen de un aeropuerto específico por su ID.
- `GET /api/Aeropuertos/{id}/AvionesLlegada`: Obtiene la lista de aviones que llegan a un aeropuerto específico por su ID.

## Notas adicionales

- Para interactuar con la API, utiliza herramientas como Postman o cURL.
- Asegúrate de enviar los datos correctamente estructurados en el cuerpo de las solicitudes POST.
- La API utiliza los verbos HTTP adecuados para cada operación (GET, POST, DELETE).
- Ten en cuenta las restricciones de horarios y límites de aviones y pasajeros al agregar registros.

Si tienes alguna pregunta o problema, no dudes en contactarme.

## pd: Puede que algunas cosas no funcionen como se espera.

