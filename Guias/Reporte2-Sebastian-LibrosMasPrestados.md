# Guía — Reporte 2: Libros más prestados por período

**Responsable:** Sebastián Maldonado

## Objetivo

La biblioteca quiere saber **qué libros se piden más** en un período, para decidir qué ejemplares conviene comprar. El reporte agrupa los préstamos por libro y los ordena de mayor a menor cantidad.

## Qué tiene que mostrar

Una tabla con un título claro (por ejemplo, "Libros más prestados del 01/01/2026 al 31/10/2026") y estas columnas:

| Columna | De dónde sale |
|---|---|
| Puesto | 1, 2, 3… según el orden |
| Libro | `Libro.Titulo` |
| Autor | `Libro.Autor.Nombre` |
| Género | `Libro.Genero` |
| Cantidad de préstamos | Cuántos préstamos del período tiene ese libro |
| % del total | Cantidad del libro / total de préstamos del período × 100 |
| Stock disponible | `Libro.Stock` (sirve para ver si hace falta comprar más) |

**Filtros** (formulario con `method="get"`):

- **Desde / hasta:** dos `<input type="date">` sobre `FechaPrestamo`. Si vienen vacíos, usá por defecto el 1 de enero del año actual y hoy.
- **Género:** un `<select>` con los géneros existentes, más la opción "Todos". `LibrosController` ya tiene un método que obtiene los géneros distintos; podés ver cómo lo hace.
- **Mostrar los primeros:** cantidad máxima de filas (Top 5, 10 o todos).

**Resumen:**

- Total de préstamos en el período (con los filtros aplicados).
- Cantidad de títulos distintos que se prestaron.
- Género más pedido.

## Archivos que vas a crear

1. `ViewModels/LibrosMasPrestadosViewModel.cs`, con los filtros, la lista de filas, los totales y la lista de géneros para el `<select>`.
2. Una clase para cada fila, por ejemplo `LibroMasPrestadoFila`.
3. Una acción `LibrosMasPrestados(...)` en `Controllers/ReportesController.cs`.
   - **Matías también usa este controlador.** El que llega primero lo crea y el otro hace `git pull` antes de agregar la suya.
4. `Views/Reportes/LibrosMasPrestados.cshtml`.
5. Un enlace en el menú, en `Views/Shared/_Layout.cshtml`. Pueden armar entre los dos un desplegable "Reportes" de Bootstrap.

## Pistas para la consulta (LINQ)

- Partí de `_context.Prestamos`, filtrá por `FechaPrestamo` entre las dos fechas y, si se eligió, por `Libro.Genero`.
- Agrupá por libro con `GroupBy` (por ejemplo, por `IdLibro`, `Titulo`, `Autor.Nombre`, `Genero` y `Stock`) y en el `Select` contá los préstamos con `Count()`.
- Ordená con `OrderByDescending` por la cantidad y, si empatan, por título (`ThenBy`).
- El porcentaje y el puesto calculalos **después** de traer la lista, en C#. Ojo con dividir por cero si no hay préstamos.
- El "género más pedido" sale de agrupar las filas por género y sumar sus cantidades.
- Validá que "desde" no sea mayor que "hasta" y mostrá un mensaje si pasa.

## Cómo probarlo

Con los datos de ejemplo, del 01/01/2026 a hoy:

- "Cien años de soledad" debe aparecer primero con 3 préstamos.
- "Ficciones" y "Rayuela" tienen 2 cada uno.
- El total del período es 11 (o más, si registraste préstamos de prueba).
- Al filtrar por género "Cuento" solo aparecen libros de cuentos, y los totales cambian.
- Un período sin préstamos muestra un mensaje en lugar de una tabla vacía.

## Al terminar

1. Completá la sección "Reporte 2" del `README.md`: qué muestra, qué filtros tiene y para qué le sirve a la biblioteca.
2. Hacé **varios commits chicos** con mensajes descriptivos mientras avanzás, no uno solo al final. Por ejemplo:
   - `Agrega ViewModel del reporte de libros más prestados`
   - `Agrega consulta agrupada por libro con filtros de fecha y género`
   - `Agrega vista del ranking con porcentajes y totales`
   - `Documenta reporte de libros más prestados en README`
3. Prepará la defensa. Tenés que poder explicar qué hace el `GroupBy`, cómo calculás el porcentaje y por qué el reporte le sirve a la biblioteca.
