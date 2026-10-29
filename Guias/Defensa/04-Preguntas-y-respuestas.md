# Preguntas de defensa y respuestas

Las respuestas describen el código revisado. Conviene ensayarlas explicando un ejemplo y señalando el archivo correspondiente, sin memorizar frases enteras.

## Preguntas que deben contestar los tres

### 1. ¿Qué problema resuelve el sistema?

Organiza el registro de socios, autores y libros y el circuito de préstamos y devoluciones de la biblioteca José Hernández. Actualiza la disponibilidad y permite consultar pendientes y demanda de libros mediante reportes.

### 2. ¿Qué significa MVC en este proyecto?

Los modelos representan los datos; los controladores reciben las peticiones y coordinan validaciones y operaciones; las vistas Razor generan HTML. Por ejemplo, `PrestamosController.Create` recibe el formulario, guarda mediante `BibliotecaContext` y devuelve una vista o una redirección.

### 3. ¿Model y ViewModel son lo mismo?

No. `Libro` representa un registro persistente. `LibroFormViewModel` contiene los campos del formulario y también las listas de autores y géneros. Los ViewModels de reportes agregan filtros, filas y resúmenes que no representan una sola tabla.

### 4. ¿Para qué sirve Entity Framework Core?

Permite trabajar con entidades y consultas LINQ, traduciéndolas a operaciones sobre SQL Server. `BibliotecaContext` expone los conjuntos de entidades y configura relaciones y restricciones. Guardamos cambios con `SaveChangesAsync`.

### 5. ¿Cuáles son las relaciones?

Autor a Libro es uno a muchos; Socio a Prestamo es uno a muchos; Libro a Prestamo es uno a muchos. Cada préstamo guarda el ID de un socio y un libro. Los préstamos registran la relación entre socios y libros a través del tiempo, incluyendo sus fechas.

### 6. ¿Qué significan `int?`, `DateTime?` y `null`?

El signo `?` permite representar ausencia de valor. En los formularios un `int?` ayuda a detectar que no se eligió una opción. En `FechaDevolucion`, `null` indica que el libro todavía no se devolvió; no significa una fecha vacía guardada como texto.

### 7. ¿Qué es una migración?

Un conjunto versionado de cambios para construir o modificar el esquema de la base. La inicial crea tablas, índices, claves, restricciones y datos de ejemplo. `dotnet ef database update` aplica las migraciones pendientes a la base configurada; compilar por sí solo no crea la base.

### 8. ¿Cómo se evita prestar un libro sin stock?

El desplegable muestra libros con stock mayor que cero y el POST vuelve a leer el libro y comprueba su disponibilidad. En el alta válida se descuenta uno. La base además impide stock negativo. Falta reforzar el control para peticiones simultáneas.

### 9. ¿Cómo se evita prestar dos veces el mismo libro al mismo socio?

El POST de alta consulta con `AnyAsync` si existe un préstamo con el mismo socio, el mismo libro y devolución nula. No prohíbe que el socio vuelva a pedir ese libro después de devolverlo. La edición que reabre un préstamo necesita reforzar este mismo control.

### 10. ¿Cómo se mantiene el stock cuando se devuelve?

Se carga el préstamo, se verifica que esté pendiente y que la fecha sea válida, se establece la devolución y se incrementa el stock en uno. Se guardan ambos cambios juntos. Una segunda devolución secuencial se rechaza porque el préstamo ya no está pendiente.

### 11. ¿Qué pasa al editar o eliminar un préstamo?

Al editar de pendiente a devuelto el stock suma uno; de devuelto a pendiente resta uno si hay disponibilidad; si se mantiene el estado no cambia. Al eliminar un pendiente se suma uno, porque el ejemplar deja de estar comprometido. Eliminar uno devuelto no cambia el stock.

### 12. ¿Qué representa el stock?

La cantidad de ejemplares disponibles ahora. No es el total de ejemplares que la biblioteca posee ni una identificación individual de cada copia. El modelo no tiene una entidad Ejemplar.

### 13. ¿Por qué hay validaciones en la interfaz y en el servidor?

La interfaz da respuesta rápida, pero sus restricciones se pueden omitir. El servidor debe comprobar los datos y las reglas antes de guardar. Algunas garantías también existen en SQL, como email único, claves foráneas y stock no negativo.

### 14. ¿Qué contiene `ModelState`?

Los valores y errores del enlace y validación del formulario. `ModelState.IsValid` permite decidir si guardar o volver a mostrar el formulario. `AddModelError` incorpora reglas del negocio, como la existencia de un préstamo pendiente duplicado.

### 15. ¿Qué hace `NoFuturaAttribute`?

Hereda de `ValidationAttribute` y compara una fecha con `DateTime.Today`. Es validación de servidor. Algunas vistas también usan `max` de HTML, pero la clase no incorpora un validador propio de JavaScript.

### 16. ¿Qué hacen `Include`, `ThenInclude` y `AsNoTracking`?

`Include` carga relaciones, como el socio y el libro del préstamo; `ThenInclude` continúa hacia otra relación, como el autor de ese libro. `AsNoTracking` evita registrar entidades para detectar cambios cuando solo se van a leer. Para modificar, el código usa entidades con seguimiento.

### 17. ¿Qué hacen `async` y `await`?

Permiten esperar operaciones de entrada/salida, como consultas a la base, sin mantener ocupado el hilo durante toda la espera. No implican que varias consultas del mismo contexto se ejecuten automáticamente en paralelo ni que la consulta SQL sea más rápida.

### 18. ¿Para qué sirven `TempData` y la redirección después del POST?

Después de guardar, el controlador redirige al listado para evitar que refrescar ese listado reenvíe el formulario. `TempData` permite mostrar allí el mensaje de éxito o error que se creó antes de redirigir.

### 19. ¿Qué aporta el token antifalsificación?

Ayuda a rechazar solicitudes de escritura falsificadas desde otro origen cuando no llevan un token válido. Los POST tienen `[ValidateAntiForgeryToken]`. Esto no implementa login ni roles: el proyecto todavía no restringe las acciones por usuario.

### 20. ¿Por qué no se puede borrar un socio con préstamos ya devueltos?

El controlador comprueba cualquier préstamo del socio para conservar sus relaciones históricas; no solo los pendientes. La base refuerza las dependencias con borrado restringido. Sin embargo, el proyecto sí permite eliminar préstamos directamente, por lo que no debe afirmarse que el historial es inalterable.

## Preguntas para Alejandro

### 21. ¿Por qué `BibliotecaContext` se recibe por constructor?

Porque `Program.cs` lo registra en el contenedor de dependencias. ASP.NET Core entrega el contexto configurado al crear el controlador, facilitando configuración centralizada y evitando construirlo manualmente en cada acción.

### 22. ¿Qué diferencia hay entre unicidad de email y validación de formato?

`[EmailAddress]` comprueba formato. El controlador consulta si otro socio ya utiliza el email. El índice único en SQL impide guardar duplicados incluso si se omite esa comprobación. Son reglas distintas.

### 23. ¿Guardar préstamo y stock juntos impide cualquier inconsistencia?

La llamada conjunta a `SaveChangesAsync` protege la atomicidad de ese guardado con SQL Server: no se debe confirmar solo una parte. Las comprobaciones previas y los cambios de otras peticiones requieren controles adicionales de concurrencia. En el código no hay un token `rowversion` configurado.

### 24. ¿Por qué no funciona directamente LocalDB en este Mac con Rider?

Rider puede compilar y ejecutar ASP.NET Core en macOS, pero LocalDB corresponde al entorno Windows. Hace falta indicar una instancia SQL Server accesible y aplicar allí las migraciones. El IDE y el servidor de base de datos cumplen tareas diferentes.

## Preguntas para Matías

### 25. ¿Pendiente y atrasado significan lo mismo?

Pendiente significa que no tiene devolución. Atrasado significa que, además de estar pendiente, supera el límite de días elegido en el reporte. No hay una fecha de vencimiento almacenada para cada préstamo; el umbral se elige al consultar.

### 26. ¿Un préstamo de exactamente 14 días está atrasado?

Con límite 14, no. La condición es `dias > DiasPermitidos`. Con 15 días sí. Si se cambia el límite del reporte, también cambia qué filas se consideran atrasadas; no se modifica el préstamo.

### 27. ¿Dónde se calculan los días y el promedio?

Se filtra la consulta por pendiente, socio y fechas, y se ejecuta con `ToListAsync`. Después se calculan los días en C#, se aplica “Solo atrasados” y el ViewModel calcula el promedio de las filas restantes. Si no quedan filas, devuelve cero.

### 28. ¿El reporte envía correos o devuelve el libro con un clic?

El enlace de correo es `mailto:` y abre el cliente de correo. El enlace de devolución abre una pantalla para confirmar fecha; el cambio se guarda en el POST de confirmación. El reporte no envía correos por un servidor SMTP.

## Preguntas para Sebastián

### 29. ¿Qué hace `GroupBy` en el ranking?

Forma un grupo por libro sobre los préstamos del período filtrado. `Count` cuenta los registros de cada grupo. No suma el stock ni cuenta solo los préstamos pendientes.

### 30. ¿Por qué se calcula el total antes de aplicar el Top?

Para mantener como referencia todos los préstamos que cumplen fecha y género. Si se aplicara el Top primero, el porcentaje expresaría participación dentro de las filas mostradas y cambiaría al modificar el límite.

### 31. ¿Por qué multiplicar por `100.0`?

Para realizar una división con decimales. Con 3 préstamos de 11, el porcentaje es aproximadamente 27,3 %. Usar división entera perdería la parte fraccionaria.

### 32. ¿Devolver un libro cambia el ranking?

No elimina ese préstamo del conteo: sigue formando parte de la demanda histórica. Sí cambia el stock actual que aparece en la fila. Eliminar el préstamo o mover su fecha fuera del período sí puede cambiar el conteo.

### 33. ¿Por qué el ranking no necesita `Include` como el reporte de pendientes?

Porque proyecta directamente los valores y agregados con `GroupBy` y `Select`; EF Core traduce los accesos relacionados necesarios a la consulta SQL. No se están trayendo entidades completas para recorrer sus relaciones después.

### 34. ¿Qué pasa si no hay datos o hay empates?

Sin préstamos se obtiene una lista vacía y la vista muestra un aviso; el total es cero. Los empates por cantidad se ordenan por título y se asignan puestos consecutivos, no puestos compartidos.

## Preguntas sobre mejoras

### 35. ¿Qué mejorarían primero?

Reforzaríamos la concurrencia del stock, repetiríamos la validación de duplicado al reabrir un préstamo y comprobaríamos el historial al editar el alta de un socio. Para uso real añadiríamos login y permisos, auditoría y pruebas de integración. Estas son mejoras propuestas, no funcionalidades ya terminadas.

### 36. ¿Cómo comprobaron el proyecto para esta preparación?

Se revisó el código del commit `9828def` y se compiló con .NET 9.0.320, sin errores ni advertencias. Después se conectó SQL Server 2022 en OrbStack y se aplicó la migración inicial. Inicio, socios, autores, libros, préstamos, alta de préstamo y ambos reportes respondieron HTTP 200; también se comprobó el panel en Chrome tras ejecutar desde Rider. Esto verifica la conexión y esas lecturas. Las operaciones de escritura y los casos de validación deben ensayarse con las pruebas manuales; no debe presentarse esta comprobación como si verificara todo el circuito.

## Respuesta final de 30 segundos

> El sistema integra catálogo, socios, préstamos y devoluciones con persistencia en SQL Server. Las validaciones del servidor y las restricciones de la base protegen las operaciones principales. El reporte de pendientes ayuda a recuperar libros y el ranking muestra la demanda. El circuito de stock está implementado para las operaciones habituales; quedan como mejoras el control de concurrencia, los permisos y algunas validaciones cruzadas.
