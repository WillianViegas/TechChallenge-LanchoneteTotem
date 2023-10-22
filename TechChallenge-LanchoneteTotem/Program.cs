using Application.UseCases;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static Domain.Entities.Pedido;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoClient>(_ => new MongoClient());
builder.Services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<MongoClient>().GetDatabase("LanchoneteTotem"));
builder.Services.AddSingleton<IMongoCollection<Usuario>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Usuario>("Usuario"));
builder.Services.AddSingleton<IMongoCollection<Categoria>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Categoria>("Categoria"));
builder.Services.AddSingleton<IMongoCollection<Produto>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Produto>("Produto"));
builder.Services.AddSingleton<IMongoCollection<Carrinho>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Carrinho>("Carrinho"));
builder.Services.AddSingleton<IMongoCollection<Pedido>>(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Pedido>("Pedido"));


builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IProdutoRepository, ProdutoRepository>();
builder.Services.AddTransient<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();

builder.Services.AddTransient<IUsuarioUseCase, UsuarioUseCase>();
builder.Services.AddTransient<ICategoriaUseCase, CategoriaUseCase>();
builder.Services.AddTransient<IProdutoUseCase, ProdutoUseCase>();
builder.Services.AddTransient<ICarrinhoUseCase, CarrinhoUseCase>();
builder.Services.AddTransient<IPedidoUseCase, PedidoUseCase>();

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(nameof(DatabaseConfig)));
builder.Services.AddSingleton<IDatabaseConfig>(sp => sp.GetRequiredService<IOptions<DatabaseConfig>>().Value);

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
categorias.MapGet("/nome/{nome}", GetCategoriaByNome).WithName("GetCategoriaByNome").WithOpenApi();
categorias.MapPost("/", CreateCategoria).WithName("CreateCategoria").WithOpenApi();
categorias.MapPut("/{id}", UpdateCategoria).WithName("UpdateCategoria").WithOpenApi();
categorias.MapDelete("/{id}", DeleteCategoria).WithName("DeleteCategoria").WithOpenApi();
#endregion

#region endpoint Produto
var produtos = app.MapGroup("/produto");

produtos.MapGet("/", GetAllProdutos).WithName("GetAllProdutos").WithOpenApi();
produtos.MapGet("/{id}", GetProdutoById).WithName("GetProdutoById").WithOpenApi();
produtos.MapGet("/nome/{id}", GetProdutoByNome).WithName("GetProdutoByNome").WithOpenApi();
produtos.MapGet("/categoria/{id}", GetAllProdutosPorCategoria).WithName("GetAllProdutosPorCategoria").WithOpenApi();
produtos.MapPost("/", CreateProduto).WithName("CreateProduto").WithOpenApi();
produtos.MapPut("/{id}", UpdateProduto).WithName("UpdateProduto").WithOpenApi();
produtos.MapDelete("/{id}", DeleteProduto).WithName("DeleteProduto").WithOpenApi();
#endregion

#region endpoint Carrinho
var carrinho = app.MapGroup("/carrinho");

carrinho.MapGet("/{id}", GetCarrinhoById).WithName("GetCarrinhoById").WithOpenApi();
carrinho.MapPost("/addProduto", AddProdutoCarrinho).WithName("AddProdutoCarrinho").WithOpenApi();
carrinho.MapPost("/RemoveProduto", RemoveProdutoCarrinho).WithName("RemoveProdutoCarrinho").WithOpenApi();
carrinho.MapPost("/", CreateCarrinho).WithName("CreateCarrinho").WithOpenApi();
carrinho.MapPut("/{id}", UpdateCarrinho).WithName("UpdateCarrinho").WithOpenApi();
carrinho.MapDelete("/{id}", DeleteCarrinho).WithName("DeleteCarrinho").WithOpenApi();
#endregion

#region endpoint Pedido
var pedido = app.MapGroup("/pedido");

pedido.MapGet("/ativos", GetAllPedidosAtivos).WithName("GetAllPedidosAtivos").WithOpenApi();
pedido.MapGet("/", GetAllPedidos).WithName("GetAllPedidos").WithOpenApi();
pedido.MapGet("/{id}", GetPedidoById).WithName("GetPedidoById").WithOpenApi();
pedido.MapPost("/", CreatePedido).WithName("CreatePedido").WithOpenApi();
pedido.MapPost("/confirmar/{id}", ConfirmarPedido).WithName("ConfirmarPedido").WithOpenApi();
pedido.MapPut("/{id}", UpdatePedido).WithName("UpdatePedido").WithOpenApi();
pedido.MapPut("/status/{id}", UpdateStatusPedido).WithName("UpdateStatusPedido").WithOpenApi();
pedido.MapDelete("/{id}", DeletePedido).WithName("DeletePedido").WithOpenApi();
#endregion

app.Run();

static async Task<IResult> GetTeste(IMongoCollection<Categoria> collection)
{
    return TypedResults.Ok();
}

#region Usuario
static async Task<IResult> GetAllUsuarios(IUsuarioUseCase usuarioUseCase)
{
    var usuarios = await usuarioUseCase.GetAllUsuarios();
    return TypedResults.Ok(usuarios.Select(x => new UsuarioDTO(x)).ToArray());
}

static async Task<IResult> GetUsuarioById(string id, IUsuarioUseCase usuarioUseCase)
{
    var usuario = await usuarioUseCase.GetUsuarioById(id);

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(usuario);
}

static async Task<IResult> GetUsuarioByCPF(string cpf, IUsuarioUseCase usuarioUseCase)
{
    var usuario = await usuarioUseCase.GetUsuarioByCPF(cpf);

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(usuario);
}

static async Task<IResult> GetUsuarioByEmail(string email, IUsuarioUseCase usuarioUseCase)
{
    var usuario = await usuarioUseCase.GetUsuarioByEmail(email);

    if (usuario is null) return TypedResults.NotFound();

    return TypedResults.Ok(usuario);
}

static async Task<IResult> CreateUsuario(UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase)
{
    usuarioDTO = await usuarioUseCase.CreateUsuario(usuarioDTO);

    return TypedResults.Created($"/usuario/{usuarioDTO.Id}", usuarioDTO);
}

static async Task<IResult> UpdateUsuario(string id, UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase)
{
    if (await usuarioUseCase.GetUsuarioById(id) is UsuarioDTO usuario)
    {
        usuarioUseCase.UpdateUsuario(id, usuarioDTO);
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}

static async Task<IResult> DeleteUsuario(string id, IUsuarioUseCase usuarioUseCase)
{
    if (await usuarioUseCase.GetUsuarioById(id) is UsuarioDTO usuario)
    {
        usuarioUseCase.DeleteUsuario(id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Categoria

static async Task<IResult> GetAllCategorias(ICategoriaUseCase categoriaUseCase)
{
    var categorias = await categoriaUseCase.GetAllCategorias();
    return TypedResults.Ok(categorias.Select(x => new CategoriaDTO(x)).ToArray());
}

static async Task<IResult> GetCategoriaById(string id, ICategoriaUseCase categoriaUseCase)
{
    var categoria = await categoriaUseCase.GetCategoriaById(id);

    if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

    return TypedResults.Ok(categoria);
}

static async Task<IResult> GetCategoriaByNome(string nome, ICategoriaUseCase categoriaUseCase)
{
    var categoria = await categoriaUseCase.GetCategoriaByNome(nome);

    if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

    return TypedResults.Ok(categoria);
}

static async Task<IResult> CreateCategoria(Categoria categoria, ICategoriaUseCase categoriaUseCase)
{
    await categoriaUseCase.CreateCategoria(categoria);

    return TypedResults.Created($"/categoria/{categoria.Id}", categoria);
}

static async Task<IResult> UpdateCategoria(string id, Categoria categoriaInput, ICategoriaUseCase categoriaUseCase)
{
    var categoria = await categoriaUseCase.GetCategoriaById(id);

    if (categoria is null) return TypedResults.NotFound();

    await categoriaUseCase.UpdateCategoria(id, categoriaInput);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCategoria(string id, ICategoriaUseCase categoriaUseCase)
{
    if (await categoriaUseCase.GetCategoriaById(id) is CategoriaDTO categoria)
    {
        await categoriaUseCase.DeleteCategoria(id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Produto

static async Task<IResult> GetAllProdutos(IProdutoUseCase produtoUseCase)
{
    var produtos = await produtoUseCase.GetAllProdutos();
    return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
}

static async Task<IResult> GetProdutoById(string id, IProdutoUseCase produtoUseCase)
{
    var produto = await produtoUseCase.GetProdutoById(id);

    if (produto is null) return TypedResults.NotFound();

    return TypedResults.Ok(produto);
}


static async Task<IResult> GetAllProdutosPorCategoria(string id, IProdutoUseCase produtoUseCase)
{
    var produtos = await produtoUseCase.GetAllProdutosPorCategoria(id);
    return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
}

static async Task<IResult> GetProdutoByNome(string nome, IProdutoUseCase produtoUseCase)
{
    var produto = await produtoUseCase.GetProdutoByNome(nome);

    if (produto is null) return TypedResults.NotFound();

    return TypedResults.Ok(produto);
}

static async Task<IResult> CreateProduto(Produto produto, IProdutoUseCase produtoUseCase)
{
    await produtoUseCase.CreateProduto(produto);
    return TypedResults.Created($"/categoria/{produto.Id}", produto);
}

static async Task<IResult> UpdateProduto(string id, Produto produtoInput, IProdutoUseCase produtoUseCase)
{
    var produto = await produtoUseCase.GetProdutoById(id);

    if (produto is null) return TypedResults.NotFound();

    await produtoUseCase.UpdateProduto(id, produtoInput);

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduto(string id, IProdutoUseCase produtoUseCase)
{
    if (await produtoUseCase.GetProdutoById(id) is ProdutoDTO produto)
    {
        await produtoUseCase.DeleteProduto(id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Carrinho

static async Task<IResult> GetCarrinhoById(string id, ICarrinhoUseCase carrinhoUseCase)
{
    var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
    if (carrinho is null) return TypedResults.NotFound();
    return TypedResults.Ok(carrinho);
}


static async Task<IResult> CreateCarrinho(Carrinho carrinho, ICarrinhoUseCase carrinhoUseCase)
{
    await carrinhoUseCase.CreateCarrinho(carrinho);
    return TypedResults.Created($"/carrinho/{carrinho.Id}", carrinho);
}


static async Task<IResult> AddProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, string idProduto, string idCarrinho, int quantidade = 1)
{
    //melhorar
    try
    {
        var carrinho = await carrinhoUseCase.AddProdutoCarrinho(idProduto, idCarrinho, quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch(Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
   
}

static async Task<IResult> RemoveProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, string idProduto, string idCarrinho, int quantidade = 1)
{
    //melhorar
    try
    {
        var carrinho = await carrinhoUseCase.RemoveProdutoCarrinho(idProduto, idCarrinho, quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }

}

static async Task<IResult> UpdateCarrinho(string id, Carrinho carrinhoInput, ICarrinhoUseCase carrinhoUseCase)
{
    if (await carrinhoUseCase.GetCarrinhoById(id) is Carrinho carrinho)
    {
        await carrinhoUseCase.UpdateCarrinho(id, carrinhoInput);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

static async Task<IResult> DeleteCarrinho(string id, ICarrinhoUseCase carrinhoUseCase)
{
    if (await carrinhoUseCase.GetCarrinhoById(id) is Carrinho carrinho)
    {
        await carrinhoUseCase.DeleteCarrinho(id);
        return TypedResults.NoContent();
    }
   
    return TypedResults.NotFound();
}
#endregion

#region Pedido
static async Task<IResult> GetAllPedidosAtivos(IPedidoUseCase pedidoUseCase)
{
    var pedidos = await pedidoUseCase.GetAllPedidosAtivos();
    return TypedResults.Ok(pedidos);
}

static async Task<IResult> GetAllPedidos(IPedidoUseCase pedidoUseCase)
{
    var pedidos = await pedidoUseCase.GetAllPedidos();
    return TypedResults.Ok(pedidos);
}

static async Task<IResult> GetPedidoById(string id, IPedidoUseCase pedidoUseCase)
{
    var pedido = await pedidoUseCase.GetPedidoById(id);

    if (pedido is null) return TypedResults.NotFound();

    return TypedResults.Ok(pedido);
}


static async Task<IResult> CreatePedido(string idCarrinho, IPedidoUseCase pedidoUseCase)
{
    var pedido = await pedidoUseCase.CreatePedido(idCarrinho);
    return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
}


static async Task<IResult> ConfirmarPedido(string id, IPedidoUseCase pedidoUseCase)
{
    var pedido = await pedidoUseCase.ConfirmarPedido(id);
    return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
}

static async Task<IResult> UpdatePedido(string id, Pedido pedidoInput, IPedidoUseCase pedidoUseCase)
{
    pedidoUseCase.UpdatePedido(id, pedidoInput);
    return TypedResults.NoContent();
}

static async Task<IResult> UpdateStatusPedido(string id, int status, IPedidoUseCase pedidoUseCase)
{
    if (await pedidoUseCase.GetPedidoById(id) is Pedido pedido)
    {
        pedidoUseCase.UpdateStatusPedido(id, status);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

static async Task<IResult> DeletePedido(string id, IPedidoUseCase pedidoUseCase)
{
    if (await pedidoUseCase.GetPedidoById(id) is Pedido pedido)
    {
        pedidoUseCase.DeletePedido(id);
        return TypedResults.NoContent();
    }
    
    return TypedResults.NotFound();
}
#endregion