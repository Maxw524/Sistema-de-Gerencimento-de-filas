using Microsoft.EntityFrameworkCore;
using QueueSystemBackend.Data;
using QueueSystemBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure o DbContext com a string de conexão (use SQLite para simplicidade)
builder.Services.AddDbContext<QueueContext>(options =>
    options.UseSqlite("Data Source=queuesystem.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint para criar um serviço
app.MapPost("/services", async (Service service, QueueContext db) =>
{
    if (service is null)
    {
        return Results.BadRequest("Serviço não pode ser nulo.");
    }

    db.Services.Add(service);
    await db.SaveChangesAsync();

    return Results.Created($"/services/{service.Id}", service);
});

// Endpoint para listar serviços
app.MapGet("/services", async (QueueContext db) =>
{
    var services = await db.Services.ToListAsync();
    return Results.Ok(services);
});

// Endpoint para atualizar um serviço
app.MapPut("/services/{id}", async (int id, Service updatedService, QueueContext db) =>
{
    var service = await db.Services.FindAsync(id);
    if (service is null) return Results.NotFound();

    service.Name = updatedService.Name; // Atualiza o nome do serviço
    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content
});

// Endpoint para deletar um serviço
app.MapDelete("/services/{id}", async (int id, QueueContext db) =>
{
    var service = await db.Services.FindAsync(id);
    if (service is null) return Results.NotFound();

    db.Services.Remove(service);
    await db.SaveChangesAsync();
    return Results.NoContent(); // Retorna 204 No Content
});

// Endpoint para criar um ticket com número automático
app.MapPost("/tickets", async (Ticket ticket, QueueContext db) =>
{
    if (ticket.ServiceId <= 0) // Verifica se o ServiceId é válido
    {
        return Results.BadRequest("O ServiceId deve ser fornecido.");
    }

    // Busca o último ticket criado
    var lastTicket = await db.Tickets.OrderByDescending(t => t.Id).FirstOrDefaultAsync();

    // Gera o próximo número sequencial
    var nextTicketNumber = lastTicket == null ? "0001" : (int.Parse(lastTicket.Number) + 1).ToString("D4");

    var newTicket = new Ticket
    {
        Number = nextTicketNumber,
        IssuedAt = DateTime.Now,
        Status = TicketStatus.Waiting,
        ServiceId = ticket.ServiceId // Adiciona o ServiceId ao ticket
    };

    db.Tickets.Add(newTicket);
    await db.SaveChangesAsync();

    return Results.Created($"/tickets/{newTicket.Id}", newTicket);
});

// Endpoint para listar tickets
app.MapGet("/tickets", async (QueueContext db) =>
{
    var tickets = await db.Tickets.ToListAsync();
    return Results.Ok(tickets);
});

// Endpoint para chamar um ticket
app.MapPut("/tickets/call/{id}", async (int id, QueueContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();

    ticket.Status = TicketStatus.Called; // Atualiza o status para "Called"
    ticket.CalledAt = DateTime.Now;      // Atualiza a hora de chamada do ticket
    await db.SaveChangesAsync();

    return Results.NoContent(); // Retorna 204 No Content
});

// Endpoint para cancelar um ticket
app.MapPut("/tickets/cancel/{id}", async (int id, QueueContext db) =>
{
    var ticket = await db.Tickets.FindAsync(id);
    if (ticket is null) return Results.NotFound();

    ticket.Status = TicketStatus.Cancelled; // Atualiza o status para "Cancelled"
    await db.SaveChangesAsync();

    return Results.NoContent(); // Retorna 204 No Content
});

// Endpoint para chamar o próximo ticket de um serviço
app.MapPut("/services/{serviceId}/next-ticket", async (int serviceId, QueueContext db) =>
{
    var nextTicket = await db.Tickets
        .Where(t => t.ServiceId == serviceId && t.Status == TicketStatus.Waiting)
        .OrderBy(t => t.IssuedAt)
        .FirstOrDefaultAsync();

    if (nextTicket is null)
    {
        return Results.NotFound("Nenhum ticket em espera para esse serviço.");
    }

    nextTicket.Status = TicketStatus.Called;
    nextTicket.CalledAt = DateTime.Now;

    await db.SaveChangesAsync();
    return Results.Ok(nextTicket);
});

// Endpoint para listar tickets por status para um serviço
app.MapGet("/services/{serviceId}/tickets", async (int serviceId, TicketStatus status, QueueContext db) =>
{
    var tickets = await db.Tickets
        .Where(t => t.ServiceId == serviceId && t.Status == status)
        .ToListAsync();

    if (tickets.Count == 0)
    {
        return Results.NotFound($"Nenhum ticket com status {status} para o serviço {serviceId}.");
    }

    return Results.Ok(tickets);
});

app.Run();
