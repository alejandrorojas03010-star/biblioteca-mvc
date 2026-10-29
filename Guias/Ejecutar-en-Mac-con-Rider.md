# Ejecutar BibliotecaMVC en macOS con Rider

## Por qué aparecía el error

`PlatformNotSupportedException: LocalDB is not supported on this platform` significa que la conexión predeterminada intenta abrir SQL Server LocalDB, disponible en Windows. El servidor web sí arrancó; la consulta inicial a socios falla al abrir esa conexión.

Para desarrollar en este Mac se usa SQL Server 2022 Developer en un contenedor de OrbStack, accesible solo desde `127.0.0.1:14333`. La aplicación conserva el proveedor SQL Server y sus migraciones originales.

## Preparación

Abrir una terminal en la carpeta que contiene `Biblioteca.sln` y ejecutar:

```bash
bash scripts/preparar-mac.sh
```

El script:

1. Busca el SDK de .NET, incluyendo la instalación en `~/.dotnet`.
2. Inicia OrbStack si es necesario.
3. Genera una contraseña local aleatoria en `.env`, ignorado por Git.
4. Inicia el servicio SQL Server y espera a que acepte consultas.
5. Guarda la conexión con `dotnet user-secrets`, asociada a este proyecto.
6. Aplica la migración inicial y los datos de ejemplo.

La primera ejecución descarga la imagen de SQL Server y puede tardar varios minutos. Las siguientes reutilizan la imagen, el contenedor y el volumen.

## Ejecutar en Rider

1. Abrir `Biblioteca.sln`.
2. Si Rider no detecta .NET, configurar **.NET CLI executable path** en **Toolset and Build** con `/Users/alejandorojasespinoza/.dotnet/dotnet`.
3. Detener la ejecución anterior con el botón Stop.
4. Seleccionar el perfil **http** y volver a ejecutar. Ese perfil establece `ASPNETCORE_ENVIRONMENT=Development`, que permite cargar la conexión local desde User Secrets.
5. Abrir [la aplicación](http://localhost:5087).

Después de modificar el identificador UserSecretsId del proyecto, es necesario reconstruir y reiniciar el proceso anterior. Recargar solo el navegador no actualiza un proceso que sigue usando la configuración anterior.

## Mantener la base disponible

Dejar OrbStack en ejecución mientras se usa la aplicación. Para iniciar de nuevo la base:

```bash
docker compose up -d
```

Para detener sus contenedores sin borrar los datos:

```bash
docker compose stop
```

Los datos están en el volumen `biblioteca-mvc-local_sqlserver-data`. No usar `docker compose down -v` para un apagado normal: elimina el volumen. No cambiar ni eliminar `.env` sin conservar su contraseña, porque regenerarla no cambia la contraseña de una base existente.

## Alcance de la configuración

La cadena de LocalDB de `appsettings.json` se conserva para Windows. En este Mac, User Secrets sustituye esa cadena en Development; la contraseña no está escrita en archivos versionados. User Secrets es una configuración local de desarrollo, no un almacén cifrado para producción.

El contenedor usa `linux/amd64` mediante Rosetta en Apple Silicon. OrbStack documenta esa emulación; Microsoft no ofrece soporte oficial de SQL Server para ejecutar sus contenedores mediante emulación. Esta configuración sirve como entorno de ensayo local; para un despliegue soportado se debe usar un servidor x86-64 compatible.

Referencias: [OrbStack y emulación Intel](https://docs.orbstack.dev/docker/#intel-x86-emulation), [contenedores de SQL Server y plataformas compatibles](https://learn.microsoft.com/en-us/sql/linux/containers/deploy?view=sql-server-ver16).
