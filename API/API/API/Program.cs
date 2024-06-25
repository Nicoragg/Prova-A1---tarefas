using System.ComponentModel;
using System.Text;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Nicolas>();

var app = builder.Build();

// builder.Services.AddCors(
//     options =>
//     {
//         options.AddPolicy("Acesso",
//             builder => builder.
//                 AllowAnyOrigin().
//                 AllowAnyHeader().
//                 AllowAnyMethod());
//     }
// );

app.MapGet("/", () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5273/categoria/listar
app.MapGet("/categoria/listar", ([FromServices] Nicolas ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5273/categoria/cadastrar
app.MapPost("/categoria/cadastrar", ([FromServices] Nicolas ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/tarefas/listar
app.MapGet("/tarefas/listar", ([FromServices] Nicolas ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5273/tarefas/cadastrar
app.MapPost("/tarefas/cadastrar", ([FromServices] Nicolas ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

//PUT: http://localhost:5273/tarefas/alterar/{id}
app.MapPut("/tarefas/alterar/{id}", ([FromServices] Nicolas ctx, [FromRoute] string id) =>
{
    Tarefa? tarefa = ctx.Tarefas.Find(id);
    if (tarefa is null)
    {
        return Results.NotFound("Tarefa não encontrada!");
    }

    if(tarefa.Status == "Não Iniciada"){
        tarefa.Status = "Em andamento";
    }

    if(tarefa.Status == "Em andamento"){
        tarefa.Status = "Concluída";
    }

    ctx.Tarefas.Update(tarefa);
    ctx.SaveChanges();

    return Results.Ok("Tarefa alterada com sucesso!" + "\n" + tarefa);
});

app.MapGet("/tarefas/naoconcluidas", ([FromServices] Nicolas ctx) =>
{
    var tarefas = ctx.Tarefas.Where(t => t.Status == "Em andamento" || t.Status == "Não Iniciadas").ToListAsync();

    if (tarefas is not null)
    {
        return Results.Ok(tarefas);
    }
    return Results.NotFound("Não existem tarefas não concluídas");
});

//GET: http://localhost:5000/tarefas/concluidas
app.MapGet("/tarefas/concluidas", ([FromServices] Nicolas ctx) =>
{
    var tarefas = ctx.Tarefas.Where(t => t.Status == "Concluída").ToListAsync();
    if (tarefas is not null)
    {
        return Results.Ok(tarefas);
    }
    return Results.NotFound("Não existem tarefas concluídas");
});

// app.UseCors("Acesso");
app.Run();
