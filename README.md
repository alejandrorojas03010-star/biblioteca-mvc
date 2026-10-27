# Biblioteca José Hernández — Sistema de préstamos

Aplicación web en **ASP.NET Core MVC** que reemplaza el cuaderno de préstamos de la biblioteca municipal "José Hernández". Permite gestionar socios, autores y libros, y registrar préstamos y devoluciones. El stock de cada libro se actualiza automáticamente.

El sistema está construido a partir del Diagrama Entidad-Relación del equipo (entidades **Socio**, **Autor**, **Libro** y **Préstamo**).

## Integrantes

| Integrante | Usuario de GitHub |
|---|---|
| Alejandro Rojas | `@completar` |
| Matías Córdova | `@completar` |
| Sebastián Maldonado | `@completar` |

## Funcionalidades

- **Socios:** alta, consulta, edición y baja. El email no puede repetirse. No se puede eliminar un socio que tiene préstamos, para conservar el historial.
- **Autores:** ABM completo. No se puede eliminar un autor que tiene libros en el catálogo.
- **Libros:** ABM completo, con filtro por título, autor y género. Cada libro muestra el nombre de su autor y su historial de préstamos.
- **Préstamos:**
  - Al registrar un préstamo, el stock del libro baja en 1. Solo se ofrecen libros con stock disponible.
  - Al registrar la devolución, el stock vuelve a subir.
  - No se puede prestar a un socio un libro que ya tiene prestado y no devolvió.
  - Las fechas no pueden ser futuras ni anteriores al alta del socio, y la devolución no puede ser anterior al préstamo.
- **Reportes:** ver la sección [Reportes](#reportes).

## Tecnologías

- .NET 9 / ASP.NET Core MVC
- Entity Framework Core 9 (SQL Server)
- SQL Server LocalDB (se instala con Visual Studio)
- Bootstrap 5

## Requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB, o cualquier SQL Server (hay que ajustar la cadena de conexión)
- Visual Studio 2022 (17.12 o superior) o VS Code
- Herramienta de EF Core para la línea de comandos:

  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Cómo ejecutar

1. Clonar el repositorio:

   ```bash
   git clone <url-del-repositorio>
   ```

2. Revisar la cadena de conexión en `BibliotecaMVC/appsettings.json`. Por defecto usa LocalDB:

   ```json
   "BibliotecaConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BibliotecaJoseHernandez;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   ```

3. Crear la base de datos aplicando las migraciones:

   ```bash
   cd BibliotecaMVC
   dotnet ef database update
   ```

   En Visual Studio también se puede hacer desde la *Consola del Administrador de paquetes* con `Update-Database`.

   La migración `CreacionInicial` crea las tablas `Socio`, `Autor`, `Libro` y `Prestamo` con sus claves, relaciones y restricciones. También carga datos de ejemplo: 5 autores, 8 libros, 5 socios y 11 préstamos.

4. Ejecutar la aplicación:

   ```bash
   dotnet run
   ```

   O abrir `Biblioteca.sln` en Visual Studio y presionar F5. La app queda en `http://localhost:5087`.

## Estructura del proyecto

```
BibliotecaMVC/
├── Controllers/    Controladores (Home, Socios, Autores, Libros, Prestamos)
├── Data/           BibliotecaContext: DbContext, relaciones y datos iniciales
├── Migrations/     Migraciones de Entity Framework Core
├── Models/         Entidades del DER (Socio, Autor, Libro, Prestamo)
├── Validations/    Validaciones personalizadas (NoFutura)
├── ViewModels/     Modelos para formularios y pantallas que lo necesitan
└── Views/          Vistas Razor por controlador y parciales compartidas
```

## Del DER a la base de datos

| Entidad (DER) | Tabla | Clave primaria | Claves foráneas |
|---|---|---|---|
| SOCIO | `Socio` | `IdSocio` | — |
| AUTOR | `Autor` | `IdAutor` | — |
| LIBRO | `Libro` | `IdLibro` | `IdAutor` → Autor (ESCRIBE, N:1) |
| PRESTAMO | `Prestamo` | `IdPrestamo` | `IdSocio` → Socio (REALIZA, N:1), `IdLibro` → Libro (INCLUYE, N:1) |

Las relaciones usan `DeleteBehavior.Restrict`: no se borra un registro que tiene otros que dependen de él.

**Cambios respecto del DER original:** no se modificaron entidades, atributos ni relaciones. Los nombres de columnas se adaptaron a la convención de C# (por ejemplo, `id_socio` → `IdSocio` y `fecha_alta` → `FechaAlta`). Además se agregaron restricciones a nivel de base de datos: email de socio único, stock no negativo y devolución no anterior al préstamo.

## Reportes

> Pendiente: cada responsable completa la descripción de su reporte.

### Reporte 1 — Préstamos pendientes (responsable: Matías Córdova)

Menú **Reportes → Préstamos pendientes**.

Lista los préstamos que todavía no tienen fecha de devolución, del más antiguo al más reciente. Por cada uno muestra:

- socio y su email
- libro y autor
- fecha de préstamo
- días que lleva afuera
- si está **atrasado**, es decir, si supera los días permitidos

**Filtros:**
- socio
- rango de fechas de préstamo
- días permitidos (por defecto 14)
- "solo atrasados"

**Resumen:** cantidad de préstamos pendientes, cantidad de atrasados y promedio de días afuera.

**Utilidad:** la bibliotecaria ve de un vistazo qué ejemplares faltan y a quién reclamárselos. Puede escribirle al socio desde el mismo reporte y registrar la devolución con un clic.

### Reporte 2 — Libros más prestados por período (responsable: Sebastián Maldonado)

Menú **Reportes → Libros más prestados**.

Arma un ranking de los libros según la cantidad de préstamos en un período, de mayor a menor. Por cada libro muestra:

- puesto
- título, autor y género
- cantidad de préstamos
- porcentaje sobre el total del período
- stock disponible

**Filtros:**
- rango de fechas (por defecto, del 1 de enero del año actual a hoy)
- género
- cantidad de filas (Top 5, Top 10 o todos)

**Resumen:** total de préstamos del período, cantidad de títulos distintos prestados y género más pedido.

**Utilidad:** muestra qué libros y géneros piden más los socios. Ayuda a decidir qué ejemplares conviene comprar, sobre todo los más pedidos que tienen poco stock.
