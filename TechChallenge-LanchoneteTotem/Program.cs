using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using TechChallenge_LanchoneteTotem.Model;
using TechChallenge_LanchoneteTotem.Model.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoClient>(_ => new MongoClient());
builder.Services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<MongoClient>().GetDatabase("LanchoneteTotem"));
builder.Services.AddSingleton<IMongoCollection<Usuario>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Usuario>("Usuario"));
builder.Services.AddSingleton<IMongoCollection<Categoria>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Categoria>("Categoria"));
builder.Services.AddSingleton<IMongoCollection<Produto>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Produto>("Produto"));
//builder.Services.AddSingleton<IMongoCollection<Categoria>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Categoria>("Carrinho"));
//builder.Services.AddSingleton<IMongoCollection<Categoria>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Categoria>("Pedido"));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/teste", GetTeste).WithName("GetTeste").WithOpenApi();

#region endpoint Usuario
var usuarios = app.MapGroup("/usuario");

usuarios.MapGet("/", GetAllUsuarios).WithName("GetAllUsuarios").WithOpenApi();
usuarios.MapGet("/id/{id}", GetUsuarioById).WithName("GetUsuarioById").WithOpenApi();
usuarios.MapGet("/cpf/{cpf}", GetUsuarioByCPF).WithName("GetUsuarioByCPF").WithOpenApi();
usuarios.MapGet("/email/{email}", GetUsuarioByEmail).WithName("GetUsuarioByEmail").WithOpenApi();
usuarios.MapPost("/", CreateUsuario).WithName("CreateUsuario").WithOpenApi();
usuarios.MapPut("/{id}", UpdateUsuario).WithName("UpdateUsuario").WithOpenApi();
usuarios.MapDelete("/{id}", DeleteUsuario).WithName("DeleteUsuario").WithOpenApi();
#endregion

#region endpoint Categoria
var categorias = app.MapGroup("/categoria");

categorias.MapGet("/", GetAllCategorias).WithName("GetAllCategorias").WithOpenApi();
categorias.MapGet("/{id}", GetCategoriaById).WithName("GetCategoriaById").WithOpenApi();
categorias.MapPost("/", CreateCategoria).WithName("CreateCategoria").WithOpenApi();
categorias.MapPut("/{id}", UpdateCategoria).WithName("UpdateCategoria").WithOpenApi();
categorias.MapDelete("/{id}", DeleteCategoria).WithName("DeleteCategoria").WithOpenApi();
#endregion

#region endpoint Produto
var produtos = app.MapGroup("/produto");

produtos.MapGet("/", GetAllProdutos).WithName("GetAllProdutos").WithOpenApi();
produtos.MapGet("/{id}", GetProdutoById).WithName("GetProdutoById").WithOpenApi();
produtos.MapGet("/categoria/{id}", GetAllProdutosPorCategoria).WithName("GetAllProdutosPorCategoria").WithOpenApi();
produtos.MapPost("/", CreateProduto).WithName("CreateProduto").WithOpenApi();
produtos.MapPut("/{id}", UpdateProduto).WithName("UpdateProduto").WithOpenApi();
produtos.MapDelete("/{id}", DeleteProduto).WithName("DeleteProduto").WithOpenApi();
#endregion

app.Run();

static async Task<IResult> GetTeste(IMongoCollection<Categoria> collection)
{
    var categorias = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(categorias);
}

#region Usuario
static async Task<IResult> GetAllUsuarios(IMongoCollection<Usuario> collection)
{
    var usuarios = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(usuarios.Select(x => new UsuarioDTO(x)).ToArray());
}

static async Task<IResult> GetUsuarioById(string id, IMongoCollection<Usuario> collection)
{
    var usuario = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(new UsuarioDTO(usuario));
}

static async Task<IResult> GetUsuarioByCPF(string cpf, IMongoCollection<Usuario> collection)
{
    var usuario = await collection.Find(x => x.CPF == cpf).FirstOrDefaultAsync();

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(new UsuarioDTO(usuario));
}

static async Task<IResult> GetUsuarioByEmail(string email, IMongoCollection<Usuario> collection)
{
    var usuario = await collection.Find(x => x.Email == email).FirstOrDefaultAsync();

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(new UsuarioDTO(usuario));
}

static async Task<IResult> CreateUsuario(UsuarioDTO usuarioDTO, IMongoCollection<Usuario> collection)
{
    var usuario = new Usuario
    {
        Nome = usuarioDTO.Nome,
        CPF = usuarioDTO.CPF,
        Email = usuarioDTO.Email
    };

    await collection.InsertOneAsync(usuario);

    usuarioDTO = new UsuarioDTO(usuario);

    return TypedResults.Created($"/usuario/{usuarioDTO.Id}", usuarioDTO);
}

static async Task<IResult> UpdateUsuario(string id, UsuarioDTO usuarioDTO, IMongoCollection<Usuario> collection)
{
    var usuario = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (usuario is null) return TypedResults.NotFound();

    usuario.Nome = usuarioDTO.Nome;
    usuario.Email = usuarioDTO.Email;
    usuario.CPF = usuarioDTO.CPF;

    await collection.ReplaceOneAsync(x => x.Id == id, usuario);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteUsuario(string id, IMongoCollection<Usuario> collection)
{
    if (await collection.Find(x => x.Id == id).FirstOrDefaultAsync() is Usuario usuario)
    {
        await collection.DeleteOneAsync(x => x.Id == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Categoria

static async Task<IResult> GetAllCategorias(IMongoCollection<Categoria> collection)
{
    var categorias = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(categorias.Select(x => new CategoriaDTO(x)).ToArray());
}

static async Task<IResult> GetCategoriaById(string id, IMongoCollection<Categoria> collection)
{
    var categoria = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (categoria is null) return TypedResults.NotFound();

    return TypedResults.Ok(new CategoriaDTO(categoria));
}


///TODO Buscar categoria pelo nome///


static async Task<IResult> CreateCategoria(Categoria categoria, IMongoCollection<Categoria> collection)
{
    await collection.InsertOneAsync(categoria);

    return TypedResults.Created($"/usuario/{categoria.Id}", categoria);
}

static async Task<IResult> UpdateCategoria(string id, Categoria categoriaInput, IMongoCollection<Categoria> collection)
{
    var categoria = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (categoria is null) return TypedResults.NotFound();

    categoria.Nome = categoriaInput.Nome;
    categoria.Ativa = categoriaInput.Ativa;

    await collection.ReplaceOneAsync(x => x.Id == id, categoria);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCategoria(string id, IMongoCollection<Categoria> collection)
{
    if (await collection.Find(x => x.Id == id).FirstOrDefaultAsync() is Categoria categoria)
    {
        await collection.DeleteOneAsync(x => x.Id == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Produto

static async Task<IResult> GetAllProdutos(IMongoCollection<Produto> collection)
{
    var produtos = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
}

static async Task<IResult> GetProdutoById(string id, IMongoCollection<Produto> collection)
{
    var produto = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (produto is null) return TypedResults.NotFound();

    return TypedResults.Ok(new ProdutoDTO(produto));
}


static async Task<IResult> GetAllProdutosPorCategoria(string id, IMongoCollection<Produto> collection)
{
    var produtos = await collection.Find(x => x.CategoriaId == id).ToListAsync();
    return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
}

///TODO Buscar produto pelo nome///


static async Task<IResult> CreateProduto(Produto produto, IMongoCollection<Produto> collection)
{
    await collection.InsertOneAsync(produto);

    return TypedResults.Created($"/usuario/{produto.Id}", produto);
}

static async Task<IResult> UpdateProduto(string id, Produto produtoInput, IMongoCollection<Produto> collection)
{
    var produto = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    if (produto is null) return TypedResults.NotFound();

    produto.Nome = produtoInput.Nome;
    produto.Descricao = produtoInput.Descricao;
    produto.Preco = produtoInput.Preco;
    produto.CategoriaId = produtoInput.CategoriaId is null ? produto.CategoriaId : produtoInput.CategoriaId; //fazer uma validação melhor dps

    await collection.ReplaceOneAsync(x => x.Id == id, produto);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduto(string id, IMongoCollection<Produto> collection)
{
    if (await collection.Find(x => x.Id == id).FirstOrDefaultAsync() is Produto produto)
    {
        await collection.DeleteOneAsync(x => x.Id == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion