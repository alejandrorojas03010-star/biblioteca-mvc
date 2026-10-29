# Preparación y demostración en Rider

**Actualización — datos con Bogus:** en Development, el inicio completa la base hasta **100 socios** conservando los registros originales. Para la demo actual, esperar 100 en el panel y en el listado, en lugar de los 5 del seed original que se describe más abajo. No se agregan libros ni préstamos con esta carga; esos conteos permanecen iguales. Reiniciar vuelve a comprobar el total y no agrega duplicados.

## 1. Situación de este equipo

Se encontró Rider en `/Applications/Rider.app` y el SDK **.NET 9.0.320** en:

```text
/Users/alejandorojasespinoza/.dotnet/dotnet
```

El SDK existe aunque `dotnet` no esté en el PATH de esta terminal. No hace falta reinstalarlo para compilar este proyecto. Este equipo es macOS con arquitectura ARM64.

Se comprobó la compilación de `Biblioteca.sln`: **0 errores y 0 advertencias**. También se encontró `dotnet-ef` **9.0.10** instalado globalmente. Posteriormente se preparó SQL Server en OrbStack, se aplicó la migración inicial y se verificaron inicio, listados, formulario de préstamo y ambos reportes con HTTP 200. La aplicación se reinició desde Rider y el panel se comprobó en Chrome. El circuito de escritura se debe ensayar siguiendo los pasos de esta guía.

**Este Mac ya está configurado:** consultar [Ejecutar en Mac con Rider](../Ejecutar-en-Mac-con-Rider.md). Para usarlo, mantener OrbStack abierto e iniciar la base con `docker compose up -d` desde la raíz del repositorio. La conexión local está en User Secrets; no es necesario completar la plantilla de conexión que aparece más abajo.

La conexión del repositorio apunta a `(localdb)\\MSSQLLocalDB`. LocalDB es la variante de desarrollo de SQL Server para Windows; Rider en macOS necesita una instancia de SQL Server accesible con otra cadena de conexión. Instalar Rider no crea esa base de datos. [Referencia de LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).

## 2. Abrir y compilar desde Rider

1. Abrir **File → Open** y seleccionar [Biblioteca.sln](../../Biblioteca.sln), dentro de la copia `biblioteca-mvc` clonada en este espacio de trabajo.
2. Esperar la carga de la solución y la restauración de NuGet.
3. Si Rider no encuentra .NET, abrir sus ajustes y buscar **Toolset and Build**. En **.NET CLI executable path**, indicar `/Users/alejandorojasespinoza/.dotnet/dotnet`.
4. Compilar la solución desde el menú **Build**.
5. Revisar `BibliotecaMVC/Properties/launchSettings.json`. Seleccionar el perfil **http** para ejecutar en `http://localhost:5087`; el perfil **https** usa además el puerto 7268.

Rider permite configurar la ruta del ejecutable .NET y usar perfiles de `launchSettings.json`. Los nombres o la ubicación exacta de los menús pueden variar según la versión instalada. [Toolset and Build](https://www.jetbrains.com/help/rider/Settings_Toolset_and_Build.html), [perfiles de ejecución](https://www.jetbrains.com/help/rider/Running_LaunchSettings.html).

En la terminal integrada de Rider, desde la raíz que contiene `Biblioteca.sln`, también se puede usar:

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"
"$HOME/.dotnet/dotnet" build Biblioteca.sln
"$HOME/.dotnet/dotnet" run --project BibliotecaMVC/BibliotecaMVC.csproj --launch-profile http
```

Los dos `export` preparan solo esa sesión de terminal: permiten encontrar .NET y sus herramientas globales. El último comando inicia la aplicación; las pantallas que consultan datos requieren una conexión SQL Server válida.

## 3. Preparar la base de datos

### Si presentan desde Windows con LocalDB

Revisar que LocalDB esté instalado y disponible. Mantener la cadena incluida si corresponde a ese equipo. Desde la terminal, con .NET 9 y `dotnet-ef` 9.0.10 disponibles:

```bash
dotnet tool install --global dotnet-ef --version 9.0.10
dotnet ef database update --project BibliotecaMVC/BibliotecaMVC.csproj
dotnet run --project BibliotecaMVC/BibliotecaMVC.csproj --launch-profile http
```

El comando de instalación se usa solo si la herramienta no está instalada. Si ya existe, comprobar su versión antes de cambiarla. `database update` aplica migraciones a la base indicada por la conexión: usar una base de ensayo para la defensa.

### Si presentan desde este Mac con Rider

Usar una instancia de SQL Server accesible, por ejemplo un servidor del equipo o de la institución. Esta guía no presupone que exista una instancia configurada. No se ha creado ni reemplazado una base automáticamente.

En la configuración de ejecución de Rider se puede definir la variable **ConnectionStrings__BibliotecaConnection** con la conexión de desarrollo correspondiente. Así se sustituye la conexión de LocalDB para esa ejecución.

Plantilla orientativa, con todos sus valores de acceso por completar:

```text
Server=SERVIDOR,1433;Database=BibliotecaDefensa;User Id=USUARIO;Password=CONTRASENA;Encrypt=True;TrustServerCertificate=True
```

`TrustServerCertificate=True` sirve para un entorno de desarrollo con certificado no validado; usar la configuración TLS indicada por el administrador del servidor. No publicar contraseñas en el repositorio.

Para aplicar migraciones desde la terminal integrada, esa terminal también debe recibir la conexión correcta: las variables de una configuración Run no se trasladan automáticamente a otra terminal. Una vez configurada la conexión, ejecutar los dos `export` del apartado 2 si todavía no se ejecutaron en esa terminal. Se comprobó que con ellos `dotnet ef --version` reconoce la versión instalada, 9.0.10. Después:

```bash
"$HOME/.dotnet/dotnet" ef database update --project BibliotecaMVC/BibliotecaMVC.csproj
```

La migración carga los datos iniciales al crear una base nueva. No ejecutar comandos para eliminar o reiniciar una base con información que se deba conservar.

## 4. Resultados iniciales para comprobar

Estos resultados se deducen del seed y de las consultas. Corresponden a una base recién creada, sin préstamos añadidos ni editados. El cálculo de atrasos usa como fecha de referencia **29/10/2026**, con límite de **14 días**; cambiará si el día del servidor es otro.

| Indicador inicial | Resultado esperado |
|---|---:|
| Autores | 5 |
| Socios | 5 |
| Títulos en catálogo | 8 |
| Ejemplares disponibles, suma de Stock | 18 |
| Préstamos históricos | 11 |
| Préstamos pendientes | 4 |
| Préstamos devueltos | 7 |

### Reporte de pendientes

| Socio | Libro | Prestado el | Días al 29/10/2026 | Estado con límite 14 |
|---|---|---|---:|---|
| Sofía Díaz | Bestiario | 20/09/2026 | 39 | Atrasado |
| Laura Gómez | Rayuela | 01/10/2026 | 28 | Atrasado |
| Carlos Ruiz | La casa de los espíritus | 14/10/2026 | 15 | Atrasado |
| Ana Torres | Cien años de soledad | 20/10/2026 | 9 | En término |

Resumen: 4 pendientes, 3 atrasados y promedio 22,75 días; la vista redondea a un decimal. Al activar “Solo atrasados” quedan 3 filas y el promedio se calcula sobre esas tres. Al filtrar por Laura queda Rayuela.

### Ranking del 01/01/2026 al 29/10/2026, todos los géneros

| Libro | Cantidad | Porcentaje aproximado |
|---|---:|---:|
| Cien años de soledad | 3 | 27,3 % |
| Ficciones | 2 | 18,2 % |
| Rayuela | 2 | 18,2 % |
| Bestiario | 1 | 9,1 % |
| El Aleph | 1 | 9,1 % |
| La casa de los espíritus | 1 | 9,1 % |
| Martín Fierro | 1 | 9,1 % |

Resumen: 11 préstamos, 7 títulos distintos, género más pedido **Novela** con 6 préstamos. Los demás géneros suman Cuento 4 y Poesía 1. `Crónica de una muerte anunciada` no aparece porque no tiene préstamos en el seed.

## 5. Demo conjunta: primero reportes, después cambios

| Paso | Responsable | Acción | Qué comprobar |
|---|---|---|---|
| 1 | Alejandro | Mostrar inicio | 5 socios, 8 títulos, 18 disponibles y 4 pendientes |
| 2 | Matías | Abrir pendientes, filtrar por Laura y quitar filtros | Rayuela para Laura; 4 filas sin filtros |
| 3 | Sebastián | Abrir ranking con fechas explícitas; alternar Todos y Top 5 | 11 préstamos y 7 títulos; los totales no cambian al limitar filas |
| 4 | Sebastián | Buscar libros por Borges y filtrar Cuento | Se muestran los títulos que cumplen ambos filtros |
| 5 | Alejandro | Registrar Ficciones para Martín Pérez con fecha de hoy | Stock de Ficciones pasa de 3 a 2; pendientes de 4 a 5 |
| 6 | Alejandro | Intentar de nuevo el mismo préstamo para Martín | Error de duplicado pendiente; stock sigue en 2 |
| 7 | Matías | Devolver ese préstamo con fecha de hoy | Stock vuelve a 3; pendientes vuelven a 4 |
| 8 | Sebastián | Volver al ranking, con fecha final incluyendo hoy | El préstamo devuelto sigue contando: ahora hay 12 préstamos |
| 9 | Los tres | Explicar las mejoras identificadas | Concurrencia, permisos y validaciones cruzadas |

La combinación Martín/Ficciones es válida en el seed. Antes de ensayar sobre una base usada, comprobar que no exista ya un pendiente para esa combinación. Los conteos de los primeros pasos dejan de ser los originales después de modificar datos.

## 6. Pruebas manuales complementarias

| Caso | Resultado esperado |
|---|---|
| Crear socio con email `laura.gomez@mail.com` | Error de email repetido |
| Fecha de alta futura | Rechazo por validación |
| Laura intenta pedir Rayuela antes de devolverla | Rechazo por duplicado pendiente |
| Fecha de préstamo anterior al alta del socio | Rechazo |
| Fecha de devolución anterior al préstamo o futura | Rechazo |
| Abrir alta de préstamo con el seed | Bestiario no aparece: stock 0 |
| Intentar borrar Laura, un libro con historial o un autor con libros | Rechazo y mensaje de dependencia |
| Reporte con Desde posterior a Hasta | Mensaje de rango inválido |
| Ranking de un período sin préstamos | Mensaje sin resultados; total 0 |
| Editar devuelto a pendiente con stock 0 | Rechazo |

Para mostrar un CRUD completo, crear datos de ensayo identificables: autor “Autor de ensayo”, libro “Libro de ensayo” y socio con email propio de ensayo. Editarlos y eliminarlos antes de asociar préstamos. Así se demuestra el borrado permitido y luego el bloqueo de dependencias con los registros iniciales.

## 7. Depuración útil en Rider

Colocar un breakpoint en el POST `PrestamosController.Create`, antes de `ModelState.IsValid`, y otro antes de `SaveChangesAsync`. En una prueba pausada inspeccionar `modelo`, `socio`, `libro.Stock` y los errores de `ModelState`. Continuar y comprobar el stock en el catálogo.

Ensayar primero esta parte; durante la exposición basta un recorrido corto. Conservar una captura de los reportes y del DER como apoyo si falla la conexión del aula, dejando claro que una captura no equivale a una ejecución en vivo.
