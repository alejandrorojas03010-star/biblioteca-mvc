# Defensa de BibliotecaMVC entre tres personas

Preparado el 29/10/2026 a partir del código del repositorio [biblioteca-mvc](https://github.com/alejandrorojas03010-star/biblioteca-mvc), commit `9828def462464a450fdc18b302b443f95c1cbf8a`.

Este material propone cómo repartir **la exposición y el estudio**. No atribuye autoría de código que no esté documentada. Conserva las responsabilidades de reportes que ya figuran en el README: Matías, pendientes; Sebastián, ranking.

## Material para estudiar

**Actualización — Bogus:** la aplicación ahora completa la base hasta 100 socios al iniciar en Development. Conserva los 5 socios originales y agrega los 95 faltantes en la base de demostración. La carga está en `BibliotecaMVC/Data/SociosSeeder.cs`, se invoca desde `Program.cs` y no duplica registros al reiniciar. Los conteos de 5 socios que describen el seed de la migración corresponden a antes de esa carga adicional; el panel y la lista actuales muestran 100.

1. [Reparto, guiones individuales y esquema de diapositivas](01-Reparto-y-guiones.md).
2. [Explicación técnica y mapa de archivos](02-Guia-tecnica.md).
3. [Preparación y demostración paso a paso](03-Demostracion.md).
4. [Preguntas del docente y respuestas](04-Preguntas-y-respuestas.md).

## Reparto resumido

| Persona | Responsabilidad principal | Parte compartida | Tiempo sugerido |
|---|---|---|---|
| Alejandro Rojas | Problema, MVC, configuración, entidades, EF Core, relaciones y migración | Crear préstamo: validaciones y stock | 5 minutos |
| Matías Córdova | Socios, email único, validaciones y reporte de préstamos pendientes | Registrar devolución y comprobar el stock | 5 minutos |
| Sebastián Maldonado | Autores, libros, filtros, vistas y reporte de libros más prestados | Edición y eliminación de préstamos: efectos sobre stock | 5 minutos |

Las preguntas pueden dirigirse a cualquiera. Los tres deben poder seguir el recorrido formulario → controlador → EF Core → SQL Server → vista, explicar las cuatro entidades y describir cuándo cambia el stock.

## Idea central de la defensa

> El sistema administra los préstamos de la biblioteca José Hernández. Organiza socios, autores y libros, registra préstamos y devoluciones y mantiene el stock disponible. Usa ASP.NET Core MVC para separar la interfaz del procesamiento, y Entity Framework Core para guardar los datos en SQL Server. Los reportes permiten identificar préstamos pendientes y conocer los libros con mayor demanda.

## Qué está verificado y qué falta ejecutar

Se revisaron modelos, ViewModels, controladores, vistas principales, configuración, contexto, migración y guías existentes. Se compiló `Biblioteca.sln` con el SDK .NET 9.0.320: **0 errores y 0 advertencias**. Rider está instalado y se abrió la solución con él. También está instalada la herramienta global `dotnet-ef` 9.0.10.

En este Mac, el ejecutable .NET se encuentra en `/Users/alejandorojasespinoza/.dotnet/dotnet`, fuera del PATH de esta terminal. Se preparó SQL Server 2022 Developer en OrbStack, se aplicó la migración inicial y se configuró la conexión mediante User Secrets. Se reinició la aplicación desde Rider y se comprobó el panel en Chrome. Inicio, socios, autores, libros, préstamos, alta de préstamo y ambos reportes respondieron HTTP 200. Los dos reportes mostraron los datos iniciales esperados. El circuito de escritura de préstamo y devolución debe ensayarse siguiendo el documento 03; esta comprobación de conexión no equivale a probar todas las reglas del negocio.

Para volver a iniciar la base y consultar la configuración local: [Ejecutar en Mac con Rider](../Ejecutar-en-Mac-con-Rider.md).

## Orden de preparación

1. Cada integrante estudia su guion junto a los archivos indicados.
2. Todos estudian las reglas de préstamos y las preguntas comunes.
3. Preparan una base de ensayo y comprueban primero los dos reportes sin modificar datos.
4. Ensayan una exposición de 15 minutos con una sola persona manejando la computadora por turno.
5. Repasan las limitaciones reales: autenticación, concurrencia y validaciones cruzadas pendientes de reforzar.

La preparación de estos materiales no cambió las reglas de negocio. Para resolver el error de LocalDB en macOS se añadió configuración local de SQL Server, un identificador UserSecretsId en el proyecto y un script de preparación.
