# Reparto y guiones de defensa

Duración propuesta: 15 minutos, aproximadamente 5 por persona, más preguntas. Es una propuesta de exposición para Alejandro Rojas, Matías Córdova y Sebastián Maldonado, los nombres del README. Pueden intercambiar las partes generales manteniendo los reportes que ya tienen asignados.

## 1. Alejandro: arquitectura, datos y registro del préstamo

### Qué debe dominar

- Objetivo del sistema y alcance funcional.
- MVC y recorrido de una petición.
- `Program.cs`: registro de MVC, inyección de `BibliotecaContext`, `UseSqlServer`, cultura `es-AR` y ruta predeterminada.
- Entidades `Socio`, `Autor`, `Libro`, `Prestamo`; claves primarias, foráneas y cardinalidades.
- `BibliotecaContext`: `DbSet`, relaciones, índice único de email, restricciones y datos iniciales.
- Migración `CreacionInicial` y diferencia entre definir el modelo y aplicar la migración.
- `PrestamosController.Create`: validaciones, inserción y reducción del stock.
- Panel en Development: 100 socios después de la carga de Bogus (5 originales y 95 generados), 8 títulos, 18 ejemplares disponibles y 4 préstamos pendientes. La migración original por sí sola carga 5 socios.

### Archivos para abrir, en orden

1. [Program.cs](../../BibliotecaMVC/Program.cs).
2. [BibliotecaContext.cs](../../BibliotecaMVC/Data/BibliotecaContext.cs).
3. [Prestamo.cs](../../BibliotecaMVC/Models/Prestamo.cs) y las otras tres entidades de `Models`.
4. [Migración inicial](../../BibliotecaMVC/Migrations/20261027005750_CreacionInicial.cs).
5. [PrestamosController.cs](../../BibliotecaMVC/Controllers/PrestamosController.cs), método POST `Create`.

### Guion para explicar con sus palabras

> El proyecto responde a la necesidad de reemplazar el registro manual de préstamos de la biblioteca José Hernández. Permite administrar socios, autores y libros, y registrar quién tiene cada libro y cuándo lo devuelve. También ofrece dos reportes para apoyar el trabajo de la biblioteca.
>
> Usamos ASP.NET Core MVC sobre .NET 9. El controlador recibe la petición, valida la operación y usa el contexto para consultar o guardar datos. Las vistas Razor generan la pantalla. Las entidades representan los datos persistentes, mientras que los ViewModels agrupan lo que necesita cada formulario o reporte.
>
> En Program se configura BibliotecaContext con SQL Server. Los controladores reciben ese contexto por el constructor mediante inyección de dependencias. La ruta predeterminada lleva a Home/Index, donde se calculan los indicadores del panel.
>
> La base tiene cuatro entidades. Un autor tiene muchos libros; un socio realiza muchos préstamos; un libro puede aparecer en muchos préstamos a lo largo del tiempo. Cada préstamo identifica un socio y un libro. No manejamos cada ejemplar físico individualmente: el libro tiene un número de ejemplares disponibles llamado Stock.
>
> El contexto configura claves foráneas y restricciones. El email del socio tiene un índice único; el stock no puede ser negativo; y una devolución no puede ser anterior al préstamo. Las relaciones restringen los borrados para evitar eliminar registros que todavía tienen dependencias.
>
> Al registrar un préstamo comprobamos que el socio y el libro existan, que haya stock, que la fecha sea válida y que ese socio no tenga ya el mismo libro pendiente. Si hay errores, mostramos nuevamente el formulario. Si todo está correcto, agregamos el préstamo y descontamos un ejemplar, guardando ambos cambios con SaveChangesAsync.

**Mini demo:** mostrar el panel, abrir el contexto y registrar un préstamo después de que los compañeros hayan mostrado los reportes iniciales. Para una exposición sin saltos, se puede mostrar el formulario ahora y confirmar el préstamo en la demo conjunta final.

**Pase de palabra:** “Matías explicará cómo se validan los socios y cómo el sistema identifica los libros que todavía no se devolvieron”.

## 2. Matías: socios, devoluciones y reporte de pendientes

### Qué debe dominar

- ABM/CRUD de socios y búsqueda por nombre o email.
- `[Required]`, `[EmailAddress]`, `[StringLength]`, `[NoFutura]` y `ModelState`.
- Comprobación de email duplicado en el controlador y unicidad en la base.
- Restricción de borrar socios con cualquier préstamo registrado, incluso devuelto.
- `Devolver`: validación de fecha, cambio de estado, incremento del stock y control de una segunda devolución secuencial.
- Reporte de pendientes: `Include`, `ThenInclude`, `Where`, `OrderBy`, `AsNoTracking` y `ToListAsync`.
- Días afuera y condición estricta `dias > DiasPermitidos`.
- Resúmenes calculados sobre las filas que quedan después de todos los filtros.

### Archivos para abrir, en orden

1. [SociosController.cs](../../BibliotecaMVC/Controllers/SociosController.cs).
2. [Socio.cs](../../BibliotecaMVC/Models/Socio.cs) y [NoFuturaAttribute.cs](../../BibliotecaMVC/Validations/NoFuturaAttribute.cs).
3. [ReportesController.cs](../../BibliotecaMVC/Controllers/ReportesController.cs), acción `PrestamosPendientes`.
4. [PrestamosPendientesViewModel.cs](../../BibliotecaMVC/ViewModels/PrestamosPendientesViewModel.cs).
5. [Vista del reporte](../../BibliotecaMVC/Views/Reportes/PrestamosPendientes.cshtml).
6. [PrestamosController.cs](../../BibliotecaMVC/Controllers/PrestamosController.cs), métodos `Devolver`.

### Guion para explicar con sus palabras

> El módulo de socios permite crear, consultar, editar y eliminar registros cuando no tienen préstamos asociados. Podemos buscar por nombre o email. El nombre y el correo son obligatorios, se valida el formato del email y la fecha de alta no puede ser futura.
>
> El email se comprueba en dos niveles: el controlador consulta si otro socio ya lo utiliza para dar un mensaje comprensible, y la base tiene un índice único. Al editar excluimos el ID del mismo socio de esa búsqueda, para que pueda conservar su email.
>
> Mi reporte muestra los préstamos cuya FechaDevolucion es nula. Incluye el socio, su email, el libro y su autor, ordenando primero los préstamos más antiguos. Se puede filtrar por socio, fechas de préstamo, días permitidos y la opción de mostrar únicamente atrasados.
>
> La consulta aplica los filtros de socio y fechas antes de traer los datos. Include carga el socio y el libro; ThenInclude carga el autor del libro. AsNoTracking sirve para esta lectura, porque no vamos a modificar las entidades desde el reporte.
>
> Después de ToListAsync calculamos los días afuera en C#. Un préstamo está atrasado cuando supera los días permitidos. Si el límite es 14, exactamente 14 días todavía está en término y 15 ya es atraso. Los totales se calculan sobre las filas filtradas. Si no hay filas, el promedio devuelve cero.
>
> El enlace de devolución abre una pantalla de confirmación. Al confirmar una fecha válida, el préstamo deja de estar pendiente y el stock aumenta en uno. En una nueva petición, si ya está devuelto, el controlador evita volver a incrementar el stock.

**Mini demo:** mostrar pendientes sin filtros, filtrar por Laura Gómez y probar un email duplicado. En la demo conjunta, devolver el préstamo recién creado y comprobar que desaparece del reporte.

**Pase de palabra:** “Sebastián mostrará cómo se administra el catálogo y cómo obtenemos el ranking de libros más prestados”.

## 3. Sebastián: catálogo, interfaz y ranking

### Qué debe dominar

- CRUD de autores y libros, filtros por título/autor y género.
- Autor obligatorio y validación de su existencia.
- `LibroFormViewModel`: datos del libro, lista de autores y sugerencias de géneros.
- Vistas Razor, Tag Helpers, parciales de formularios, `_Layout` y mensajes con `TempData`.
- Reporte de libros más prestados: rango de fechas, género, agrupación, conteo, orden y Top.
- Porcentajes y resúmenes calculados antes de recortar el Top.
- El ranking cuenta préstamos devueltos y pendientes; el stock mostrado es el actual.
- Edición/eliminación de préstamos y sus consecuencias sobre stock.

### Archivos para abrir, en orden

1. [LibrosController.cs](../../BibliotecaMVC/Controllers/LibrosController.cs) y [AutoresController.cs](../../BibliotecaMVC/Controllers/AutoresController.cs).
2. [LibroFormViewModel.cs](../../BibliotecaMVC/ViewModels/LibroFormViewModel.cs) y [formulario de libros](../../BibliotecaMVC/Views/Libros/_Formulario.cshtml).
3. [ReportesController.cs](../../BibliotecaMVC/Controllers/ReportesController.cs), acción `LibrosMasPrestados`.
4. [LibrosMasPrestadosViewModel.cs](../../BibliotecaMVC/ViewModels/LibrosMasPrestadosViewModel.cs).
5. [Vista del ranking](../../BibliotecaMVC/Views/Reportes/LibrosMasPrestados.cshtml).
6. [PrestamosController.cs](../../BibliotecaMVC/Controllers/PrestamosController.cs), métodos POST `Edit` y `DeleteConfirmed`.

### Guion para explicar con sus palabras

> El catálogo administra autores y libros. Cada libro pertenece a un autor y tiene título, género y stock disponible. La búsqueda permite encontrar libros por título o nombre del autor y filtrar además por género. No se puede eliminar un autor con libros ni un libro con préstamos registrados.
>
> Para el formulario de libros usamos un ViewModel: además de los datos que se guardan, incluye las opciones de autores y las sugerencias de géneros. Las vistas usan Tag Helpers como asp-for y asp-items para vincular los campos con ese modelo. Los formularios reutilizan parciales y el menú está en un layout compartido.
>
> Mi reporte responde qué libros se prestaron más en un período. Sin filtros toma desde el primero de enero del año actual hasta hoy. Puede limitarse a un género y mostrar los primeros cinco, diez o todos los títulos.
>
> Partimos de los préstamos, filtramos por fecha y género y agrupamos por libro. Count obtiene los préstamos de cada grupo. Después ordenamos de mayor a menor cantidad y por título cuando hay empate. Los libros que no tuvieron préstamos en ese período no aparecen.
>
> Antes de aplicar el Top calculamos el total, la cantidad de títulos y el género más pedido. El porcentaje es la cantidad de ese libro multiplicada por cien y dividida por el total filtrado. Por eso mostrar solo cinco filas no cambia el denominador. En los datos iniciales, Cien años de soledad tiene tres de once préstamos, aproximadamente 27,3 por ciento.
>
> El stock del ranking es el disponible actualmente, no una reconstrucción histórica. También revisamos el stock al editar o eliminar préstamos: pasar de pendiente a devuelto suma uno, volver a pendiente resta uno si hay disponibilidad, y eliminar un préstamo pendiente devuelve el ejemplar al stock.

**Mini demo:** buscar un libro por autor, filtrar por género, mostrar el ranking completo y luego Top 5. Señalar que los totales permanecen iguales.

**Cierre sugerido:** “El sistema conecta la gestión diaria de préstamos con información para recuperar ejemplares y reconocer la demanda. Como siguientes mejoras proponemos reforzar concurrencia, permisos de acceso y algunas validaciones entre registros”.

## Esquema de 10 diapositivas

| Nº | Diapositiva | Qué mostrar | Responsable |
|---|---|---|---|
| 1 | Problema y objetivo | Registro manual, socios, catálogo y préstamos | Alejandro |
| 2 | Arquitectura | MVC → EF Core → SQL Server | Alejandro |
| 3 | Modelo de datos | Cuatro entidades, PK, FK y relaciones | Alejandro |
| 4 | Reglas del préstamo | Existencia, fechas, duplicado pendiente, stock | Alejandro |
| 5 | Socios y validaciones | Email único, fecha de alta, conservación de historial | Matías |
| 6 | Préstamos pendientes | Filtros, días afuera, límite de atraso y resumen | Matías |
| 7 | Devolución | Fecha válida y stock antes/después | Matías |
| 8 | Catálogo e interfaz | Autores, libros, filtros, ViewModels y Razor | Sebastián |
| 9 | Libros más prestados | GroupBy, Count, porcentaje y Top | Sebastián |
| 10 | Demo integrada y mejoras | Un préstamo completo y límites actuales | Los tres |

## Acuerdo de estudio común

Cada persona debe responder sin leer: qué resuelve el sistema, cuáles son las cuatro entidades, qué significa `FechaDevolucion == null`, cuándo cambia el stock, para qué sirve `ModelState`, qué diferencia hay entre Model y ViewModel y qué hace su propio reporte. El reparto sirve para organizar la exposición; todos deben entender el circuito completo.
