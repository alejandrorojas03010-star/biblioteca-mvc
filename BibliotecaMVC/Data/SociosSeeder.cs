using BibliotecaMVC.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Data;

public static class SociosSeeder
{
    public static async Task<int> CompletarAsync(BibliotecaContext context)
    {
        const int cantidadObjetivo = 100;
        var emailsExistentes = await context.Socios.Select(s => s.Email).ToListAsync();
        var faltantes = cantidadObjetivo - emailsExistentes.Count;

        if (faltantes <= 0)
        {
            return 0;
        }

        var emails = new HashSet<string>(emailsExistentes, StringComparer.OrdinalIgnoreCase);
        var hoy = DateTime.Today;
        var generador = new Faker<Socio>("es")
            .UseSeed(20261029)
            .RuleFor(s => s.Nombre, f => f.Name.FullName())
            .RuleFor(s => s.Email, f => f.Internet.Email(provider: "example.com").ToLowerInvariant())
            .RuleFor(s => s.FechaAlta, f => f.Date.Between(hoy.AddYears(-2), hoy).Date);

        var socios = new List<Socio>();
        while (socios.Count < faltantes)
        {
            var socio = generador.Generate();
            if (emails.Add(socio.Email))
            {
                socios.Add(socio);
            }
        }

        context.Socios.AddRange(socios);
        await context.SaveChangesAsync();
        return socios.Count;
    }
}
