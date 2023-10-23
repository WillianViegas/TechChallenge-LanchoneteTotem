using Application.UseCases;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Infra.Configurations;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using TechChallenge_LanchoneteTotem.Model;
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
builder.Services.AddSwaggerGen(opts => opts.EnableAnnotations());

builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("token_admin", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "token"));

//torna obrigatorio o authorize em todas as requisições
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//    .RequireAuthenticatedUser()
//    .Build();
//});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthentication();
    app.UseAuthorization();
}

var teste = app.MapGroup("/").WithTags("Requisiçõe de teste");
teste.MapGet("/teste", GetTeste).WithName("GetTeste").WithOpenApi().RequireAuthorization("token_admin");

#region endpoint Usuario
var usuarios = app.MapGroup("/usuario").WithTags("Usuario");

usuarios.MapGet("/", GetAllUsuarios).WithName("GetAllUsuarios").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os usuários", description: "Retorna uma lista de usuários"));
usuarios.MapGet("/id/{id}", GetUsuarioById).WithName("GetUsuarioById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usuário pelo id", description: "Retorna o usuário encontrado"));
usuarios.MapGet("/cpf/{cpf}", GetUsuarioByCPF).WithName("GetUsuarioByCPF").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usuário pelo cpf", description: "Retorna o usuário encontrado"));
usuarios.MapGet("/email/{email}", GetUsuarioByEmail).WithName("GetUsuarioByEmail").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usuário pelo e-mail", description: "Retorna o usuário encontrado"));
usuarios.MapPost("/", CreateUsuario).WithName("CreateUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar novo usuário", description: "Cria um novo usuário e retorna o usuário cadastrado"));
usuarios.MapPut("/{id}", UpdateUsuario).WithName("UpdateUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar usuário existente", description: "Atualiza os dados do usuário"));
usuarios.MapDelete("/{id}", DeleteUsuario).WithName("DeleteUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar usuário", description: "Deleta o usuário"));
#endregion

#region endpoint Categoria
var categorias = app.MapGroup("/categoria").WithTags("Categoria");

categorias.MapGet("/", GetAllCategorias).WithName("GetAllCategorias").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todas as categorias", description: "Retorna uma lista de categorias"));
categorias.MapGet("/{id}", GetCategoriaById).WithName("GetCategoriaById").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter categoria pelo id", description: "Retorna uma categoria"));
categorias.MapGet("/nome/{nome}", GetCategoriaByNome).WithName("GetCategoriaByNome").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter categoria pelo nome", description: "Retorna uma categoria"));
categorias.MapPost("/", CreateCategoria).WithName("CreateCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar nova categoria", description: "Cria uma nova categoria"));
categorias.MapPut("/{id}", UpdateCategoria).WithName("UpdateCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar categoria existente", description: "Atualizar uma categoria existente"));
categorias.MapDelete("/{id}", DeleteCategoria).WithName("DeleteCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deleta uma categoria", description: "Deleta categoria existente"));
#endregion

#region endpoint Produto
var produtos = app.MapGroup("/produto").WithTags("Produto");

produtos.MapGet("/", GetAllProdutos).WithName("GetAllProdutos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os produtos", description: "Retorna uma lista de produtos"));
produtos.MapGet("/{id}", GetProdutoById).WithName("GetProdutoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter produto por id", description: "Retorna um produto pelo seu id"));
produtos.MapGet("/nome/{nome}", GetProdutoByNome).WithName("GetProdutoByNome").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter produto por nome", description: "Retorna um produto pelo seu nome"));
produtos.MapGet("/categoria/{id}", GetAllProdutosPorCategoria).WithName("GetAllProdutosPorCategoria").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os produtos de uma categoria", description: "Retorna uma lista de produtos com base no id de uma categoria"));
produtos.MapPost("/", CreateProduto).WithName("CreateProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar produto", description: "Cria um novo produto"));
produtos.MapPut("/{id}", UpdateProduto).WithName("UpdateProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar produto", description: "Atualiza um produto existente"));
produtos.MapDelete("/{id}", DeleteProduto).WithName("DeleteProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar produto", description: "Deleta um produto pelo id"));
#endregion

#region endpoint Carrinho
var carrinho = app.MapGroup("/carrinho").WithTags("Carrinho");

carrinho.MapGet("/{id}", GetCarrinhoById).WithName("GetCarrinhoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter carrinho", description: "Obter carrinho pelo id"));
carrinho.MapPost("/addProduto", AddProdutoCarrinho).WithName("AddProdutoCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Adicionar produto ao carrinho", description: "Adiciona um produto ao carrinho com base no id do produto, id do carrinho e quantidade"));
carrinho.MapPost("/RemoveProduto", RemoveProdutoCarrinho).WithName("RemoveProdutoCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Remover produto do carrinho", description: "Remove um produto ao carrinho com base no id do produto, id do carrinho"));
carrinho.MapPost("/", CreateCarrinho).WithName("CreateCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar carrinho", description: "Cria um novo carrinho"));
carrinho.MapPut("/{id}", UpdateCarrinho).WithName("UpdateCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar carrinho", description: "Atualiza as informações do carrinho"));
carrinho.MapDelete("/{id}", DeleteCarrinho).WithName("DeleteCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar carrinho", description: "Deleta o carrinho pelo id"));
#endregion

#region endpoint Pedido
var pedido = app.MapGroup("/pedido").WithTags("Pedido");

pedido.MapGet("/ativos", GetAllPedidosAtivos).WithName("GetAllPedidosAtivos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos ativos", description: "Retorna uma lista de pedidos ativos"));
pedido.MapGet("/", GetAllPedidos).WithName("GetAllPedidos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos", description: "Retorna uma lista de pedidos"));
pedido.MapGet("/{id}", GetPedidoById).WithName("GetPedidoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedido por id", description: "Retorna um pedido pelo id"));
pedido.MapPost("/", CreatePedido).WithName("CreatePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido", description: "Cria um novo pedido"));
pedido.MapPost("/fromCarrinho", CreatePedidoFromCarrinho).WithName("CreatePedidoFromCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido a partir do carrinho", description: "Cria um novo pedido utilizando o id de um carrinho"));
pedido.MapPost("/confirmar/{id}", ConfirmarPedido).WithName("ConfirmarPedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Confirmar pedido", description: "Confirma o pedido finalizando a compra"));
pedido.MapPut("/{id}", UpdatePedido).WithName("UpdatePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar pedido", description: "Atualizar informações do pedido"));
pedido.MapPut("/status/{id}", UpdateStatusPedido).WithName("UpdateStatusPedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar status pedido", description: "Atualiza o status do pedido"));
pedido.MapDelete("/{id}", DeletePedido).WithName("DeletePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar pedido", description: "Deleta o pedido pelo id"));
#endregion

app.Run();

static async Task<IResult> GetTeste(IMongoCollection<Categoria> collection)
{
    return TypedResults.Ok();
}

#region Usuario
static async Task<IResult> GetAllUsuarios(IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuarios = await usuarioUseCase.GetAllUsuarios();
        if (!usuarios.Any()) return TypedResults.NotFound("Nenhum usuário encontrado");

        return TypedResults.Ok(usuarios.Select(x => new UsuarioDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetUsuarioById(string id, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usuário não encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetUsuarioByCPF(string cpf, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuario = await usuarioUseCase.GetUsuarioByCPF(cpf);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usuário não encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetUsuarioByEmail(string email, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuario = await usuarioUseCase.GetUsuarioByEmail(email);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usuário não encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> CreateUsuario(UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        usuarioDTO = await usuarioUseCase.CreateUsuario(usuarioDTO);
        return TypedResults.Created($"/usuario/{usuarioDTO.Id}", usuarioDTO);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> UpdateUsuario(string id, UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usuário não encontrado");

        await usuarioUseCase.UpdateUsuario(id, usuarioDTO);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> DeleteUsuario(string id, IUsuarioUseCase usuarioUseCase)
{
    try
    {
        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usuário não encontrado");

        await usuarioUseCase.DeleteUsuario(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}
#endregion

#region Categoria

static async Task<IResult> GetAllCategorias(ICategoriaUseCase categoriaUseCase)
{
    try
    {
        var categorias = await categoriaUseCase.GetAllCategorias();
        return TypedResults.Ok(categorias.Select(x => new CategoriaDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetCategoriaById(string id, ICategoriaUseCase categoriaUseCase)
{
    try
    {
        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(categoria);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetCategoriaByNome(string nome, ICategoriaUseCase categoriaUseCase)
{
    try
    {
        var categoria = await categoriaUseCase.GetCategoriaByNome(nome);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(categoria);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> CreateCategoria(Categoria categoria, ICategoriaUseCase categoriaUseCase)
{
    try
    {
        await categoriaUseCase.CreateCategoria(categoria);
        return TypedResults.Created($"/categoria/{categoria.Id}", categoria);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> UpdateCategoria(string id, Categoria categoriaInput, ICategoriaUseCase categoriaUseCase)
{
    try
    {
        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        await categoriaUseCase.UpdateCategoria(id, categoriaInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> DeleteCategoria(string id, ICategoriaUseCase categoriaUseCase)
{
    try
    {
        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        await categoriaUseCase.DeleteCategoria(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}
#endregion

#region Produto

static async Task<IResult> GetAllProdutos(IProdutoUseCase produtoUseCase)
{
    try
    {
        var produtos = await produtoUseCase.GetAllProdutos();
        return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
    }
    catch(Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetProdutoById(string id, IProdutoUseCase produtoUseCase)
{
    try
    {
        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(produto);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}


static async Task<IResult> GetAllProdutosPorCategoria(string id, IProdutoUseCase produtoUseCase)
{
    try
    {
        var produtos = await produtoUseCase.GetAllProdutosPorCategoria(id);
        return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetProdutoByNome(string nome, IProdutoUseCase produtoUseCase)
{
    try
    {
        var produto = await produtoUseCase.GetProdutoByNome(nome);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(produto);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> CreateProduto(Produto produto, IProdutoUseCase produtoUseCase)
{
    try
    {
        await produtoUseCase.CreateProduto(produto);
        return TypedResults.Created($"/categoria/{produto.Id}", produto);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> UpdateProduto(string id, Produto produtoInput, IProdutoUseCase produtoUseCase)
{
    try
    {
        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        await produtoUseCase.UpdateProduto(id, produtoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> DeleteProduto(string id, IProdutoUseCase produtoUseCase)
{
    try
    {
        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        await produtoUseCase.DeleteProduto(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}
#endregion

#region Carrinho

static async Task<IResult> GetCarrinhoById(string id, ICarrinhoUseCase carrinhoUseCase)
{
    try
    {
        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(carrinho);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}


static async Task<IResult> CreateCarrinho(Carrinho carrinho, ICarrinhoUseCase carrinhoUseCase)
{
    try
    {
        await carrinhoUseCase.CreateCarrinho(carrinho);
        return TypedResults.Created($"/carrinho/{carrinho.Id}", carrinho);
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}


static async Task<IResult> AddProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, IProdutoUseCase produtoUseCase, CarrinhoBody carrinhoBody)
{
    try
    {
        var carrinho = await carrinhoUseCase.AddProdutoCarrinho(carrinhoBody.IdProduto, carrinhoBody.IdCarrinho, carrinhoBody.Quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch(Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
   
}

static async Task<IResult> RemoveProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, CarrinhoBody carrinhoBody)
{
    try
    {
        var carrinho = await carrinhoUseCase.RemoveProdutoCarrinho(carrinhoBody.IdProduto, carrinhoBody.IdCarrinho, carrinhoBody.Quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch (Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }

}

static async Task<IResult> UpdateCarrinho(string id, Carrinho carrinhoInput, ICarrinhoUseCase carrinhoUseCase)
{
    try
    {
        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        await carrinhoUseCase.UpdateCarrinho(id, carrinhoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> DeleteCarrinho(string id, ICarrinhoUseCase carrinhoUseCase)
{
    try
    {
        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        await carrinhoUseCase.DeleteCarrinho(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        //logar dps
        throw new Exception(ex.Message);
    }
}
#endregion

#region Pedido
static async Task<IResult> GetAllPedidosAtivos(IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidosAtivos();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetAllPedidos(IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidos();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> GetPedidoById(string id, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido não encontrado.");

        return TypedResults.Ok(pedido);
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}


static async Task<IResult> CreatePedidoFromCarrinho(string idCarrinho, IPedidoUseCase pedidoUseCase, ICarrinhoUseCase carrinhoUseCase)
{
    try
    {
        if (await carrinhoUseCase.GetCarrinhoById(idCarrinho) is Carrinho carrinho)
        {
            var pedido = await pedidoUseCase.CreatePedidoFromCarrinho(carrinho);
            return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
        }

        return TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> CreatePedido(Pedido pedidoInput, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.CreatePedido(pedidoInput);
        return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}


static async Task<IResult> ConfirmarPedido(string id, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.ConfirmarPedido(id);
        return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> UpdatePedido(string id, Pedido pedidoInput, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido não encontrado.");

        await pedidoUseCase.UpdatePedido(id, pedidoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> UpdateStatusPedido(string id, int status, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido não encontrado.");

        await pedidoUseCase.UpdateStatusPedido(id, status);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

static async Task<IResult> DeletePedido(string id, IPedidoUseCase pedidoUseCase)
{
    try
    {
        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido não encontrado.");

        await pedidoUseCase.DeletePedido(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}
#endregion