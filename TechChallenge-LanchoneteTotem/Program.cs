using Application.UseCases;
using Application.UseCases.Interfaces;
using Domain.Entities;
using Domain.Entities.DTO;
using Domain.Repositories;
using Domain.ValueObjects;
using Infra.Configurations.Database;
using Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TechChallenge_LanchoneteTotem.Model;
using static Domain.Entities.Pedido;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
builder.Services.AddTransient<IInitialDataSeed, InitialDataSeed>();

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
builder.Services.AddLogging();

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

//por enquanto apenas no metodo de teste (ser� implementado futuramente)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("token_admin", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "token"));

//torna obrigatorio o authorize em todas as requisi��es
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

var teste = app.MapGroup("/").WithTags("Requisi��es de teste");
teste.MapGet("/teste", GetTeste).WithName("GetTeste").WithOpenApi();
//teste.MapGet("/teste", GetTeste).WithName("GetTeste").WithOpenApi().RequireAuthorization("token_admin");
teste.MapGet("/seed", SeedInitialData).WithName("SeedInitialData").WithOpenApi();

#region endpoint Usuario
var usuarios = app.MapGroup("/usuario").WithTags("Usuario");

usuarios.MapGet("/", GetAllUsuarios).WithName("GetAllUsuarios").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os usu�rios", description: "Retorna uma lista de usu�rios")).Produces(200).Produces(400).Produces(404).Produces(500);
usuarios.MapGet("/id/{id}", GetUsuarioById).WithName("GetUsuarioById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usu�rio pelo id", description: "Retorna o usu�rio encontrado")).Produces(200).Produces(400).Produces(404).Produces(500);
usuarios.MapGet("/cpf/{cpf}", GetUsuarioByCPF).WithName("GetUsuarioByCPF").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usu�rio pelo cpf", description: "Retorna o usu�rio encontrado")).Produces(200).Produces(400).Produces(404).Produces(500);
usuarios.MapGet("/email/{email}", GetUsuarioByEmail).WithName("GetUsuarioByEmail").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter usu�rio pelo e-mail", description: "Retorna o usu�rio encontrado")).Produces(200).Produces(400).Produces(404).Produces(500);
usuarios.MapPost("/", CreateUsuario).WithName("CreateUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar novo usu�rio", description: "Cria um novo usu�rio e retorna o usu�rio cadastrado")).Produces(201).Produces(400).Produces(404).Produces(500);
usuarios.MapPut("/{id}", UpdateUsuario).WithName("UpdateUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar usu�rio existente", description: "Atualiza os dados do usu�rio")).Produces(204).Produces(400).Produces(404).Produces(500); ;
usuarios.MapDelete("/{id}", DeleteUsuario).WithName("DeleteUsuario").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar usu�rio", description: "Deleta o usu�rio")).Produces(204).Produces(400).Produces(404).Produces(500);
#endregion

#region endpoint Categoria
var categorias = app.MapGroup("/categoria").WithTags("Categoria");

categorias.MapGet("/", GetAllCategorias).WithName("GetAllCategorias").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todas as categorias", description: "Retorna uma lista de categorias")).Produces(200).Produces(400).Produces(404).Produces(500);
categorias.MapGet("/{id}", GetCategoriaById).WithName("GetCategoriaById").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter categoria pelo id", description: "Retorna uma categoria")).Produces(200).Produces(400).Produces(404).Produces(500);
categorias.MapGet("/nome/{nome}", GetCategoriaByNome).WithName("GetCategoriaByNome").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter categoria pelo nome", description: "Retorna uma categoria")).Produces(200).Produces(400).Produces(404).Produces(500);
categorias.MapPost("/", CreateCategoria).WithName("CreateCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar nova categoria", description: "Cria uma nova categoria")).Produces(201).Produces(400).Produces(404).Produces(500);
categorias.MapPut("/{id}", UpdateCategoria).WithName("UpdateCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar categoria existente", description: "Atualizar uma categoria existente")).Produces(204).Produces(400).Produces(404).Produces(500);
categorias.MapDelete("/{id}", DeleteCategoria).WithName("DeleteCategoria").WithOpenApi().WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deleta uma categoria", description: "Deleta categoria existente")).Produces(204).Produces(400).Produces(404).Produces(500);
#endregion

#region endpoint Produto
var produtos = app.MapGroup("/produto").WithTags("Produto");

produtos.MapGet("/", GetAllProdutos).WithName("GetAllProdutos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os produtos", description: "Retorna uma lista de produtos")).Produces(200).Produces(400).Produces(404).Produces(500);
produtos.MapGet("/{id}", GetProdutoById).WithName("GetProdutoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter produto por id", description: "Retorna um produto pelo seu id")).Produces(200).Produces(400).Produces(404).Produces(500);
produtos.MapGet("/nome/{nome}", GetProdutoByNome).WithName("GetProdutoByNome").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter produto por nome", description: "Retorna um produto pelo seu nome")).Produces(200).Produces(400).Produces(404).Produces(500);
produtos.MapGet("/categoria/{id}", GetAllProdutosPorCategoria).WithName("GetAllProdutosPorCategoria").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter todos os produtos de uma categoria", description: "Retorna uma lista de produtos com base no id de uma categoria")).Produces(200).Produces(400).Produces(404).Produces(500);
produtos.MapPost("/", CreateProduto).WithName("CreateProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar produto", description: "Cria um novo produto")).Produces(201).Produces(400).Produces(404).Produces(500);
produtos.MapPut("/{id}", UpdateProduto).WithName("UpdateProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar produto", description: "Atualiza um produto existente")).Produces(204).Produces(400).Produces(404).Produces(500);
produtos.MapDelete("/{id}", DeleteProduto).WithName("DeleteProduto").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar produto", description: "Deleta um produto pelo id")).Produces(204).Produces(400).Produces(404).Produces(500);
#endregion

#region endpoint Carrinho
var carrinho = app.MapGroup("/carrinho").WithTags("Carrinho");

carrinho.MapGet("/{id}", GetCarrinhoById).WithName("GetCarrinhoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter carrinho", description: "Obter carrinho pelo id")).Produces(200).Produces(400).Produces(404).Produces(500);
carrinho.MapPost("/addProduto", AddProdutoCarrinho).WithName("AddProdutoCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Adicionar produto ao carrinho", description: "Adiciona um produto ao carrinho com base no id do produto, id do carrinho e quantidade")).Produces(200).Produces(400).Produces(404).Produces(500);
carrinho.MapPost("/RemoveProduto", RemoveProdutoCarrinho).WithName("RemoveProdutoCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Remover produto do carrinho", description: "Remove um produto ao carrinho com base no id do produto, id do carrinho")).Produces(200).Produces(400).Produces(404).Produces(500);
carrinho.MapPost("/", CreateCarrinho).WithName("CreateCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar carrinho", description: "Cria um novo carrinho")).Produces(201).Produces(400).Produces(404).Produces(500);
carrinho.MapPut("/{id}", UpdateCarrinho).WithName("UpdateCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar carrinho", description: "Atualiza as informa��es do carrinho")).Produces(204).Produces(400).Produces(404).Produces(500);
carrinho.MapDelete("/{id}", DeleteCarrinho).WithName("DeleteCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar carrinho", description: "Deleta o carrinho pelo id")).Produces(204).Produces(400).Produces(404).Produces(500);
#endregion

#region endpoint Pedido
var pedido = app.MapGroup("/pedido").WithTags("Pedido");

pedido.MapGet("/ativos", GetAllPedidosAtivos).WithName("GetAllPedidosAtivos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos ativos", description: "Retorna uma lista de pedidos ativos")).Produces(200).Produces(400).Produces(404).Produces(500);
pedido.MapGet("/prontos", GetAllPedidosProntosParaRetirada).WithName("GetAllPedidosProntosParaRetirada").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos prontos para retirada", description: "Retorna uma lista de pedidos com status 'pronto' para serem retirados")).Produces(200).Produces(400).Produces(404).Produces(500);
pedido.MapGet("/finalizados", GetAllPedidosFinalizados).WithName("GetAllPedidosFinalizados").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos finalizados", description: "Retorna uma lista de pedidos com status 'Finalizado'")).Produces(200).Produces(400).Produces(404).Produces(500);
pedido.MapGet("/", GetAllPedidos).WithName("GetAllPedidos").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedidos", description: "Retorna uma lista de pedidos")).Produces(200).Produces(400).Produces(404).Produces(500);
pedido.MapGet("/{id}", GetPedidoById).WithName("GetPedidoById").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Obter pedido por id", description: "Retorna um pedido pelo id")).Produces(200).Produces(400).Produces(404).Produces(500);
pedido.MapPost("/", CreatePedido).WithName("CreatePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido", description: "Cria um novo pedido")).Produces(201).Produces(400).Produces(404).Produces(500);
pedido.MapPost("/fromCarrinho", CreatePedidoFromCarrinho).WithName("CreatePedidoFromCarrinho").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Criar pedido a partir do carrinho", description: "Cria um novo pedido utilizando o id de um carrinho")).Produces(201).Produces(400).Produces(404).Produces(500);
pedido.MapPost("/finalizar/{id}", FinalizarPedido).WithName("FinalizarPedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Finalizar pedido", description: "Finaliza o pedido gerando QRcode para pagamento")).Produces(201).Produces(400).Produces(404).Produces(500);
pedido.MapPost("/confirmar/{id}", ConfirmarPedido).WithName("ConfirmarPedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Confirmar pedido", description: "Confirma o pagamento do pedido")).Produces(201).Produces(400).Produces(404).Produces(500);
pedido.MapPut("/{id}", UpdatePedido).WithName("UpdatePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar pedido", description: "Atualizar informa��es do pedido")).Produces(204).Produces(400).Produces(404).Produces(500);
pedido.MapPut("/status/{id}", UpdateStatusPedido).WithName("UpdateStatusPedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Atualizar status pedido", description: "Atualiza o status do pedido")).Produces(204).Produces(400).Produces(404).Produces(500);
pedido.MapDelete("/{id}", DeletePedido).WithName("DeletePedido").WithOpenApi().WithMetadata(new SwaggerOperationAttribute(summary: "Deletar pedido", description: "Deleta o pedido pelo id")).Produces(204).Produces(400).Produces(404).Produces(500);
#endregion

app.Run();

static async Task<IResult> GetTeste(IMongoCollection<Categoria> collection)
{
    return TypedResults.Ok("TESTE OK 1");
}

static async Task<IResult> SeedInitialData(IInitialDataSeed seedRepository)
{
    await seedRepository.CarregarDadosIniciais();
    return TypedResults.Ok();
}

#region Usuario
static async Task<IResult> GetAllUsuarios(IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        var usuarios = await usuarioUseCase.GetAllUsuarios();
        if (!usuarios.Any()) return TypedResults.NotFound("Nenhum usu�rio encontrado");

        return TypedResults.Ok(usuarios.Select(x => new UsuarioDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao buscar usu�rios.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetUsuarioById(string id, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if(string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");


        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usu�rio n�o encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao buscar usu�rio pelo id. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetUsuarioByCPF(string cpf, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(cpf))
            return TypedResults.BadRequest("CPF inv�lido");

        var usuario = await usuarioUseCase.GetUsuarioByCPF(cpf);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usu�rio n�o encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao buscar usu�rio pelo cpf. CPF: {cpf}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetUsuarioByEmail(string email, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(email))
            return TypedResults.BadRequest("E-mail inv�lido");

        var usuario = await usuarioUseCase.GetUsuarioByEmail(email);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usu�rio n�o encontrado");

        return TypedResults.Ok(usuario);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao buscar usu�rio pelo e-mail. E-mail: {email}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> CreateUsuario(UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if(usuarioDTO is null)
            return TypedResults.BadRequest("Dados do usu�rio inv�lidos");

        usuarioDTO = await usuarioUseCase.CreateUsuario(usuarioDTO);
        return TypedResults.Created($"/usuario/{usuarioDTO.Id}", usuarioDTO);
    }
    catch (ValidationException ex)
    {
        var error = "Erro ao criar usu�rio";

        if(ex.Message.Contains("CPF"))
            error = "Erro ao criar usu�rio. CPF j� cadastrado";

        log.LogError(error, ex);
        return TypedResults.BadRequest(error);
    }
    catch (Exception ex)
    {
        var erro = "Erro ao criar usu�rio";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> UpdateUsuario(string id, UsuarioDTO usuarioDTO, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usu�rio n�o encontrado");

        await usuarioUseCase.UpdateUsuario(id, usuarioDTO);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar o usu�rio. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> DeleteUsuario(string id, IUsuarioUseCase usuarioUseCase, ILogger<UsuarioUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var usuario = await usuarioUseCase.GetUsuarioById(id);
        if (usuario is null || string.IsNullOrEmpty(usuario.Id)) return TypedResults.NotFound("Usu�rio n�o encontrado");

        await usuarioUseCase.DeleteUsuario(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao deletar o usu�rio. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}
#endregion

#region Categoria

static async Task<IResult> GetAllCategorias(ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        var categorias = await categoriaUseCase.GetAllCategorias();
        return TypedResults.Ok(categorias.Select(x => new CategoriaDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao buscar categorias";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetCategoriaById(string id, ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(categoria);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter a categoria pelo id. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetCategoriaByNome(string nome, ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(nome))
            return TypedResults.BadRequest("Nome inv�lido");

        var categoria = await categoriaUseCase.GetCategoriaByNome(nome);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(categoria);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter a categoria pelo nome. Nome: {nome}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> CreateCategoria(Categoria categoria, ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        await categoriaUseCase.CreateCategoria(categoria);
        return TypedResults.Created($"/categoria/{categoria.Id}", categoria);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao criar categoria";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> UpdateCategoria(string id, Categoria categoriaInput, ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        await categoriaUseCase.UpdateCategoria(id, categoriaInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar categoria. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> DeleteCategoria(string id, ICategoriaUseCase categoriaUseCase, ILogger<CategoriaUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var categoria = await categoriaUseCase.GetCategoriaById(id);
        if (categoria is null || string.IsNullOrEmpty(categoria.Id)) return TypedResults.NotFound();

        await categoriaUseCase.DeleteCategoria(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao deletar a categoria. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}
#endregion

#region Produto

static async Task<IResult> GetAllProdutos(IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        var produtos = await produtoUseCase.GetAllProdutos();
        return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
    }
    catch(Exception ex)
    {
        var erro = $"Erro ao obter produtos";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetProdutoById(string id, IProdutoUseCase produtoUseCase,ILogger<ProdutoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(produto);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter o produto pelo id. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}


static async Task<IResult> GetAllProdutosPorCategoria(string id, IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var produtos = await produtoUseCase.GetAllProdutosPorCategoria(id);
        return TypedResults.Ok(produtos.Select(x => new ProdutoDTO(x)).ToArray());
    }
    catch (Exception ex)
    {
        var erro = $"Obter todos os produtos pelo id da categoria. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetProdutoByNome(string nome, IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(nome))
        {
            log.LogWarning("Nome inv�lido");
            return TypedResults.BadRequest("Nome inv�lido");
        }

        var produto = await produtoUseCase.GetProdutoByNome(nome);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(produto);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter produto pelo nome. Nome: {nome}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> CreateProduto(Produto produto, IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        await produtoUseCase.CreateProduto(produto);
        return TypedResults.Created($"/categoria/{produto.Id}", produto);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao criar produto.";
        log.LogError(erro, ex);
        return TypedResults.Problem($"Erro ao criar produto.");
    }
}

static async Task<IResult> UpdateProduto(string id, Produto produtoInput, IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        await produtoUseCase.UpdateProduto(id, produtoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar o produto. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> DeleteProduto(string id, IProdutoUseCase produtoUseCase, ILogger<ProdutoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var produto = await produtoUseCase.GetProdutoById(id);
        if (produto is null || string.IsNullOrEmpty(produto.Id)) return TypedResults.NotFound();

        await produtoUseCase.DeleteProduto(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao deletar o produto. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}
#endregion

#region Carrinho

static async Task<IResult> GetCarrinhoById(string id, ICarrinhoUseCase carrinhoUseCase, ILogger<CarrinhoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        return TypedResults.Ok(carrinho);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter o carrinho. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}


static async Task<IResult> CreateCarrinho(Carrinho carrinho, ICarrinhoUseCase carrinhoUseCase, ILogger<CarrinhoUseCase> log)
{
    try
    {
        await carrinhoUseCase.CreateCarrinho(carrinho);
        return TypedResults.Created($"/carrinho/{carrinho.Id}", carrinho);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao criar carrinho.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}


static async Task<IResult> AddProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, IProdutoUseCase produtoUseCase, CarrinhoBody carrinhoBody, ILogger<CarrinhoUseCase> log)
{
    try
    {
        if (carrinhoBody is null)
        {
            log.LogWarning("Dados para adi��o do produto inv�lidos");
            return TypedResults.BadRequest("Dados para adi��o do produto inv�lidos");
        }

        var carrinho = await carrinhoUseCase.AddProdutoCarrinho(carrinhoBody.IdProduto, carrinhoBody.IdCarrinho, carrinhoBody.Quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch(Exception ex)
    {
        var erro = $"Erro ao adicionar produto no carrinho.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
   
}

static async Task<IResult> RemoveProdutoCarrinho(ICarrinhoUseCase carrinhoUseCase, CarrinhoBody carrinhoBody, ILogger<CarrinhoUseCase> log)
{
    try
    {
        if (carrinhoBody is null)
        {
            log.LogWarning("Dados para remo��o do produto inv�lidos");
            return TypedResults.BadRequest("Dados para remo��o do produto inv�lidos");
        }

        var carrinho = await carrinhoUseCase.RemoveProdutoCarrinho(carrinhoBody.IdProduto, carrinhoBody.IdCarrinho, carrinhoBody.Quantidade);
        return TypedResults.Ok(carrinho);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao remover produto do carrinho.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }

}

static async Task<IResult> UpdateCarrinho(string id, Carrinho carrinhoInput, ICarrinhoUseCase carrinhoUseCase, ILogger<CarrinhoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        await carrinhoUseCase.UpdateCarrinho(id, carrinhoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar o carrinho. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> DeleteCarrinho(string id, ICarrinhoUseCase carrinhoUseCase, ILogger<CarrinhoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Id inv�lido");
            return TypedResults.BadRequest("Id inv�lido");
        }

        var carrinho = await carrinhoUseCase.GetCarrinhoById(id);
        if (carrinho is null || string.IsNullOrEmpty(carrinho.Id)) return TypedResults.NotFound();

        await carrinhoUseCase.DeleteCarrinho(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao deletar o carrinho. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}
#endregion

#region Pedido
static async Task<IResult> GetAllPedidosAtivos(IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidosAtivos();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter os pedidos ativos.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetAllPedidosProntosParaRetirada(IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidosProntosParaRetirada();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter os pedidos finalizados.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetAllPedidosFinalizados(IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidosFinalizados();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter os pedidos finalizados.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetAllPedidos(IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        var pedidos = await pedidoUseCase.GetAllPedidos();
        return TypedResults.Ok(pedidos);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter os pedidos ativos.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> GetPedidoById(string id, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido n�o encontrado.");

        return TypedResults.Ok(pedido);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao obter o pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}


static async Task<IResult> CreatePedidoFromCarrinho(string idCarrinho, IPedidoUseCase pedidoUseCase, ICarrinhoUseCase carrinhoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(idCarrinho))
            return TypedResults.BadRequest("idCarrinho inv�lido");

        if (await carrinhoUseCase.GetCarrinhoById(idCarrinho) is Carrinho carrinho)
        {
            var pedido = await pedidoUseCase.CreatePedidoFromCarrinho(carrinho);
            return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
        }

        return TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao criar pedido a partir do carrinho. IdCarrinho: {idCarrinho}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> CreatePedido(Pedido pedidoInput, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        var pedido = await pedidoUseCase.CreatePedido(pedidoInput);
        return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao criar o pedido.";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> FinalizarPedido(string id, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.FinalizarPedido(id);
        return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao confirmar criar o pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> ConfirmarPedido(string id, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.ConfirmarPedido(id);
        return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao confirmar criar o pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> UpdatePedido(string id, Pedido pedidoInput, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido n�o encontrado.");

        await pedidoUseCase.UpdatePedido(id, pedidoInput);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar o pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> UpdateStatusPedido(string id, int status, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido n�o encontrado.");

        await pedidoUseCase.UpdateStatusPedido(id, status);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao atualizar o status do pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}

static async Task<IResult> DeletePedido(string id, IPedidoUseCase pedidoUseCase, ILogger<PedidoUseCase> log)
{
    try
    {
        if (string.IsNullOrEmpty(id))
            return TypedResults.BadRequest("Id inv�lido");

        var pedido = await pedidoUseCase.GetPedidoById(id);
        if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido n�o encontrado.");

        await pedidoUseCase.DeletePedido(id);
        return TypedResults.NoContent();
    }
    catch (Exception ex)
    {
        var erro = $"Erro ao deletar o pedido. Id: {id}";
        log.LogError(erro, ex);
        return TypedResults.Problem(erro);
    }
}
#endregion