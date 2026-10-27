# Guía — Reporte 1: Préstamos pendientes

**Responsable:** Matías Córdova

## Objetivo

La bibliotecaria necesita saber **qué libros están afuera y desde hace cuánto**, para reclamar los atrasados. El reporte lista todos los préstamos que todavía no tienen fecha de devolución.

## Qué tiene que mostrar

Una tabla ordenada de **más antiguo a más nuevo**, con un título claro y estas columnas:

| Columna | De dónde sale |
|---|---|
| Socio | `Prestamo.Socio.Nombre` |
| Email | `Prestamo.Socio.Email` (para contactarlo) |
| Libro | `Prestamo.Libro.Titulo` |
| Autor | `Prestamo.Libro.Autor.Nombre` |
| Fecha de préstamo | `Prestamo.FechaPrestamo`, en formato `dd/MM/yyyy` |
| Días afuera | Días entre `FechaPrestamo` y hoy |
| Estado | "Atrasado" si supera los días permitidos, "En término" si no |

**Filtros** (formulario con `method="get"`):

- **Socio:** un `<select>` con todos los socios, más la opción "Todos".
- **Prestado desde / hasta:** dos `<input type="date">` sobre `FechaPrestamo`.
- **Días permitidos:** número, con valor por defecto 14. Sirve para decidir qué cuenta como "Atrasado".
- **Solo atrasados:** un checkbox.

**Resumen** arriba o abajo de la tabla:

- Cantidad total de préstamos pendientes que cumplen los filtros.
- Cantidad de atrasados.
- Promedio de días afuera.

## Archivos que vas a crear

1. `ViewModels/PrestamosPendientesViewModel.cs`
   - Propiedades para cada filtro (`IdSocio`, `Desde`, `Hasta`, `DiasPermitidos`, `SoloAtrasados`).
   - Una lista de filas.
   - Los totales.
   - Un `IEnumerable<SelectListItem>` con los socios para el `<select>`.
2. Una clase para cada fila del reporte, por ejemplo `PrestamoPendienteFila`, con las columnas de la tabla. Puede ir en el mismo archivo o en uno aparte dentro de `ViewModels/`.
3. `Controllers/ReportesController.cs`, con una acción `PrestamosPendientes(...)` que recibe los filtros como parámetros.
   - **Sebastián va a agregar su acción en este mismo controlador.** Pónganse de acuerdo: el que llega primero lo crea y el otro hace `git pull` antes de agregar la suya.
4. `Views/Reportes/PrestamosPendientes.cshtml`.
5. Un enlace en el menú, en `Views/Shared/_Layout.cshtml`. Pueden armar entre los dos un desplegable "Reportes" de Bootstrap con un ítem para cada reporte.

## Pistas para la consulta (LINQ)

- Partí de `_context.Prestamos` e incluí `Socio` y `Libro`, y el `Autor` del libro con `ThenInclude`. Mirá cómo lo hace `PrestamosController`.
- "Pendiente" significa `FechaDevolucion == null`.
- Aplicá cada filtro solo si viene con valor (`if (idSocio.HasValue) consulta = consulta.Where(...)`). `SociosController.Index` hace lo mismo con el buscador.
- Los días afuera se pueden calcular **después** de traer los datos (`ToListAsync()`), con `(DateTime.Today - p.FechaPrestamo).Days`. Calcularlo en C# es más simple que hacerlo en SQL.
- Para los totales usá `Count()`, `Count(condición)` y `Average(...)` sobre la lista ya filtrada. Ojo: `Average` sobre una lista vacía lanza una excepción.
- Validá que "desde" no sea mayor que "hasta". Si lo es, mostrá un mensaje con `ModelState.AddModelError` o un aviso en la vista.

## Cómo probarlo

Con los datos de ejemplo hay 4 préstamos pendientes, del 20/09, 01/10, 14/10 y 20/10 de 2026. Comprobá que:

- Sin filtros, salen los 4, el más viejo primero.
- Con "Días permitidos = 14", el del 20/09 aparece como "Atrasado". Contá los días a mano para revisar.
- Al filtrar por Laura Gómez sale solo "Rayuela".
- Si registrás la devolución de uno desde Préstamos, desaparece del reporte.
- Si no hay resultados, se muestra un mensaje en lugar de una tabla vacía.

## Al terminar

1. Completá la sección "Reporte 1" del `README.md`: qué muestra, qué filtros tiene y para qué le sirve a la biblioteca.
2. Hacé **varios commits chicos** con mensajes descriptivos mientras avanzás, no uno solo al final. Por ejemplo:
   - `Agrega ViewModel del reporte de préstamos pendientes`
   - `Agrega consulta y filtros del reporte de pendientes`
   - `Agrega vista del reporte de pendientes con totales`
   - `Documenta reporte de préstamos pendientes en README`
3. Prepará la defensa. Tenés que poder explicar cómo armaste la consulta, por qué `FechaDevolucion == null` significa pendiente y cómo calculás los días.
