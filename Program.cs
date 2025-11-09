// -----------------------------------------------------------------------------
// SWIA – Sistema Web de Identificação de Árvores
// Arquitetura: .NET 8 Minimal API + PostgreSQL
// -----------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using SWIA.Models;
using System.Text.Json.Serialization;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// ----------------------------- CONFIGURAÇÃO DO BANCO -------------------------
var host = Environment.GetEnvironmentVariable("DB_HOST");
var db = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";

var connectionString = $"Host={host};Database={db};Username={user};Password={pass};Port={port};SSL Mode=Require;Trust Server Certificate=True";


builder.Services.AddDbContext<TreeDbContext>(options =>
 options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "SWIA API - Árvores";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SWIA API v1");
    });
}

// ----------------------------- ENDPOINT /IDENTIFY ----------------------------
app.MapPost("/identify", async (Tree request, TreeDbContext db) =>
{
    var query = db.Trees.AsQueryable();

    // Aplica filtros apenas para campos informados
    if (!string.IsNullOrEmpty(request.LeafFormat)) query = query.Where(t => t.LeafFormat == request.LeafFormat);
    if (!string.IsNullOrEmpty(request.LeafMargin)) query = query.Where(t => t.LeafMargin == request.LeafMargin);
    if (!string.IsNullOrEmpty(request.LeafArrangement)) query = query.Where(t => t.LeafArrangement == request.LeafArrangement);
    if (!string.IsNullOrEmpty(request.LeafType)) query = query.Where(t => t.LeafType == request.LeafType);
    if (!string.IsNullOrEmpty(request.WoodType)) query = query.Where(t => t.WoodType == request.WoodType);
    if (!string.IsNullOrEmpty(request.TrunkType)) query = query.Where(t => t.TrunkType == request.TrunkType);
    if (!string.IsNullOrEmpty(request.CrownShape)) query = query.Where(t => t.CrownShape == request.CrownShape);
    if (!string.IsNullOrEmpty(request.FlowerColor)) query = query.Where(t => t.FlowerColor == request.FlowerColor);
    if (!string.IsNullOrEmpty(request.FruitType)) query = query.Where(t => t.FruitType == request.FruitType);
    if (!string.IsNullOrEmpty(request.Biome)) query = query.Where(t => t.Biome == request.Biome);
    if (!string.IsNullOrEmpty(request.Region)) query = query.Where(t => t.Region == request.Region);
    if (request.HasFlower.HasValue) query = query.Where(t => t.HasFlower == request.HasFlower);
    if (request.HasFruit.HasValue) query = query.Where(t => t.HasFruit == request.HasFruit);

    var results = await query.ToListAsync();

    // Calcula a pontuação de similaridade
    var weightedResults = results.Select(r =>
    {
        double matches = 0;
        double total = 0;

        void Check(string? req, string? val) { if (!string.IsNullOrEmpty(req)) { total++; if (req == val) matches++; } }
        void CheckBool(bool? req, bool? val) { if (req.HasValue) { total++; if (req == val) matches++; } }

        Check(request.LeafFormat, r.LeafFormat);
        Check(request.LeafMargin, r.LeafMargin);
        Check(request.LeafArrangement, r.LeafArrangement);
        Check(request.LeafType, r.LeafType);
        Check(request.WoodType, r.WoodType);
        Check(request.TrunkType, r.TrunkType);
        Check(request.CrownShape, r.CrownShape);
        Check(request.FlowerColor, r.FlowerColor);
        Check(request.FruitType, r.FruitType);
        Check(request.Biome, r.Biome);
        Check(request.Region, r.Region);
        CheckBool(request.HasFlower, r.HasFlower);
        CheckBool(request.HasFruit, r.HasFruit);

        var score = total == 0 ? 0 : Math.Round(matches / total, 2);
        return new
        {
            r.CommonName,
            r.ScientificName,
            r.Family,
            r.Biome,
            r.Region,
            r.CrownShape,
            r.TrunkType,
            r.Description,
            Score = score
        };
    })
    .OrderByDescending(r => r.Score);

    return Results.Ok(weightedResults);
});

app.MapGet("/identify", async () =>
{    
    return Results.Ok("API conectada com sucesso!");
});


app.Run();
// ----------------------------- MODELOS E DB CONTEXT --------------------------
namespace SWIA.Models
{
    public class Tree
    {
        public int Id { get; set; }
        public string ScientificName { get; set; } = "";
        public string? CommonName { get; set; }
        public string? Family { get; set; }
        public string? Description { get; set; }
        public string? LeafFormat { get; set; }
        public string? LeafMargin { get; set; }
        public string? LeafArrangement { get; set; }
        public string? LeafType { get; set; }
        public string? WoodType { get; set; }
        public string? TrunkType { get; set; }
        public string? CrownShape { get; set; }
        public string? FlowerColor { get; set; }
        public string? FruitType { get; set; }
        public bool? HasFlower { get; set; }
        public bool? HasFruit { get; set; }
        public string? Biome { get; set; }
        public string? Region { get; set; }
    }
    public class TreeDbContext : DbContext
    {
        public TreeDbContext(DbContextOptions<TreeDbContext> options) : base(options) { }

        public DbSet<Tree> Trees => Set<Tree>();
    }
}

