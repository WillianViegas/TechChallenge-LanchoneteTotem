using Application.UseCases;
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
builder.Services.AddTransient<IUsuarioUseCase, UsuarioUseCase>();
builder.Services.Configure<DatabaseConfig> (builder.Configuration.GetSection(nameof(DatabaseConfig)));
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
    var categorias = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(categorias);
}

#region Usuario
static async Task<IResult> GetAllUsuarios(IUsuarioUseCase usuarioUseCase)
{
    var usuarios = usuarioUseCase.GetAllUsuarios();
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
    usuarioUseCase.UpdateUsuario(id, usuarioDTO);
    return TypedResults.NoContent();
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

static async Task<IResult> GetAllCategorias(IMongoCollection<Categoria> collection)
{
    var categorias = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(categorias.Select(x => new CategoriaDTO(x)).ToArray());
}

static async Task<IResult> GetCategoriaById(string id, IMongoCollection<Categoria> collection)
{
    var categoria = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (categoria is null) return TypedResults.NotFound();

    return TypedResults.Ok(new CategoriaDTO(categoria));
}


///TODO Buscar categoria pelo nome///


static async Task<IResult> CreateCategoria(Categoria categoria, IMongoCollection<Categoria> collection)
{
    await collection.InsertOneAsync(categoria);

    return TypedResults.Created($"/categoria/{categoria.Id}", categoria);
}

static async Task<IResult> UpdateCategoria(string id, Categoria categoriaInput, IMongoCollection<Categoria> collection)
{
    var categoria = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (categoria is null) return TypedResults.NotFound();

    categoria.Nome = categoriaInput.Nome;
    categoria.Ativa = categoriaInput.Ativa;

    await collection.ReplaceOneAsync(x => x.Id.ToString() == id, categoria);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCategoria(string id, IMongoCollection<Categoria> collection)
{
    if (await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Categoria categoria)
    {
        await collection.DeleteOneAsync(x => x.Id.ToString() == id);
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
    var produto = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

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

    return TypedResults.Created($"/categoria/{produto.Id}", produto);
}

static async Task<IResult> UpdateProduto(string id, Produto produtoInput, IMongoCollection<Produto> collection)
{
    var produto = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (produto is null) return TypedResults.NotFound();

    produto.Nome = produtoInput.Nome;
    produto.Descricao = produtoInput.Descricao;
    produto.Preco = produtoInput.Preco;
    produto.CategoriaId = produtoInput.CategoriaId is null ? produto.CategoriaId : produtoInput.CategoriaId; //fazer uma validação melhor dps

    await collection.ReplaceOneAsync(x => x.Id.ToString() == id, produto);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduto(string id, IMongoCollection<Produto> collection)
{
    if (await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Produto produto)
    {
        await collection.DeleteOneAsync(x => x.Id.ToString() == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Carrinho

static async Task<IResult> GetCarrinhoById(string id, IMongoCollection<Carrinho> collection)
{
    var carrinho = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (carrinho is null) return TypedResults.NotFound();

    return TypedResults.Ok(carrinho);
}


static async Task<IResult> CreateCarrinho(Carrinho carrinho, IMongoCollection<Carrinho> collectionCarrinho, IMongoCollection<Produto> collectionProduto)
{
    foreach (var p in carrinho.Produtos)
    {
        var produto = await collectionProduto.Find(x => x.Id == p.Id).FirstOrDefaultAsync();

        if (produto is null) return TypedResults.BadRequest($"Produto não encontrado, id:{p.Id}");
    }

    carrinho.Ativo = true;

    await collectionCarrinho.InsertOneAsync(carrinho);

    return TypedResults.Created($"/carrinho/{carrinho.Id}", carrinho);
}


static async Task<IResult> AddProdutoCarrinho(string idProduto, string idCarrinho, IMongoCollection<Carrinho> collectionCarrinho, IMongoCollection<Produto> collectionProduto, int quantidade = 1)
{
    var produto = await collectionProduto.Find(x => x.Id.ToString() == idProduto).FirstOrDefaultAsync();
    if (produto is null) return TypedResults.NotFound($"Produto não encontrado, id:{produto.Id}");


    var carrinho = new Carrinho();

    if (string.IsNullOrEmpty(idCarrinho))
    {
        var produtoLista = new List<Produto>();
        //revisar essa parte dps

        for (int i = 0; i < quantidade; i++)
        {
            produtoLista.Add(produto);
        }

        carrinho = new Carrinho()
        {
            Produtos = produtoLista,
            Total = produtoLista.Sum(x => x.Preco),
            Ativo = true
        };

        await collectionCarrinho.InsertOneAsync(carrinho);

        return TypedResults.Created($"/carrinho/{carrinho.Id}", carrinho);
    }

    carrinho = await collectionCarrinho.Find(x => x.Id.ToString() == idCarrinho).FirstOrDefaultAsync();

    for (int i = 0; i < quantidade; i++)
    {
        carrinho.Produtos.Add(produto);
    }

    carrinho.Total = carrinho.Produtos.Sum(x => x.Preco);

    await collectionCarrinho.ReplaceOneAsync(x => x.Id.ToString() == idCarrinho, carrinho);
    return TypedResults.NoContent();
}

static async Task<IResult> RemoveProdutoCarrinho(IMongoCollection<Carrinho> collectionCarrinho, IMongoCollection<Produto> collectionProduto, string idProduto, string idCarrinho, int quantidade = 1)
{
    var produto = await collectionProduto.Find(x => x.Id.ToString() == idProduto).FirstOrDefaultAsync();
    var carrinho = await collectionCarrinho.Find(x => x.Id.ToString() == idCarrinho).FirstOrDefaultAsync();

    if (produto is null) return TypedResults.NotFound();
    if (carrinho is null) return TypedResults.NotFound(); //validar dps 


    if (!carrinho.Produtos.Any(x => x.Id.ToString() == idProduto)) return TypedResults.NotFound();



    for (int i = 0; i < quantidade; i++)
    {
        var produtoCarrinho = carrinho.Produtos.FirstOrDefault(x => x.Id.ToString() == idProduto);

        if (produtoCarrinho is null) break;

        carrinho.Produtos.Remove(produtoCarrinho);
    }

    carrinho.Total = carrinho.Produtos.Sum(x => x.Preco);

    await collectionCarrinho.ReplaceOneAsync(x => x.Id.ToString() == idCarrinho, carrinho);
    return TypedResults.NoContent();
}

static async Task<IResult> UpdateCarrinho(string id, Carrinho carrinhoInput, IMongoCollection<Carrinho> collection)
{
    var carrinho = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (carrinho is null) return TypedResults.NotFound();

    carrinho.Produtos = carrinhoInput.Produtos; //validar remoção da quantidade;
    carrinho.Total = carrinho.Produtos.Sum(x => x.Preco);

    await collection.ReplaceOneAsync(x => x.Id.ToString() == id, carrinho);
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteCarrinho(string id, IMongoCollection<Carrinho> collection)
{
    if (await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Carrinho carrinho)
    {
        await collection.DeleteOneAsync(x => x.Id.ToString() == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion

#region Pedido
static async Task<IResult> GetAllPedidosAtivos(IMongoCollection<Pedido> collection)
{
    var pedidos = await collection.Find(x => x.Status != PedidoStatus.Finalizado).ToListAsync();
    return TypedResults.Ok(pedidos);
}

static async Task<IResult> GetAllPedidos(IMongoCollection<Pedido> collection)
{
    var pedidos = await collection.Find(_ => true).ToListAsync();
    return TypedResults.Ok(pedidos);
}

static async Task<IResult> GetPedidoById(string id, IMongoCollection<Pedido> collection)
{
    var pedido = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (pedido is null) return TypedResults.NotFound();

    return TypedResults.Ok(pedido);
}


static async Task<IResult> CreatePedido(string idCarrinho, IMongoCollection<Carrinho> collectionCarrinho, IMongoCollection<Pedido> collectionPedido)
{
    var carrinho = await collectionCarrinho.Find(x => x.Id.ToString() == idCarrinho).FirstOrDefaultAsync();

    if (carrinho is null) return TypedResults.NotFound();

    var numeroPedido = await collectionPedido.Find(_ => true).ToListAsync();

    var pedido = new Pedido
    {
        Produtos = carrinho.Produtos,
        Total = carrinho.Total,
        Status = 0,
        DataCriacao = DateTime.Now,
        Numero = numeroPedido.Count + 1,
        Usuario = carrinho.Usuario,
        IdCarrinho = idCarrinho
    };

    await collectionPedido.InsertOneAsync(pedido);

    return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
}


static async Task<IResult> ConfirmarPedido(string id, IMongoCollection<Carrinho> collectionCarrinho, IMongoCollection<Pedido> collectionPedido)
{
    var pedido = await collectionPedido.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (pedido is null) return TypedResults.NotFound();

    //fazer solicitação do QRCode para pagamento(antes ou durante essa chamada)

    //passando status pra pago por enquanto (ver como funciona na api do mercado pago)
    pedido.Status = PedidoStatus.Pago;
    pedido.Pagamento = new Pagamento()
    {
        Tipo = Pagamento.TipoPagamento.QRCode,
        QRCodeUrl = "www.usdfhosdfsdhfosdfhsdofhdsfds.com.br",
    };

    await collectionPedido.ReplaceOneAsync(x => x.Id.ToString() == id, pedido);


    //desativa o carrinho (pensar se futuramente n é melhor excluir)
    var carrinho = await collectionCarrinho.Find(x => x.Id.ToString() == pedido.IdCarrinho).FirstOrDefaultAsync();
    if (carrinho != null)
    {
        carrinho.Ativo = false;
        await collectionCarrinho.ReplaceOneAsync(x => x.Id == carrinho.Id, carrinho);
    }

    return TypedResults.Created($"/pedido/{pedido.Id}", pedido);
}

static async Task<IResult> UpdatePedido(string id, Pedido pedidoInput, IMongoCollection<Pedido> collection)
{
    var pedido = await collection.Find(x => x.Id.ToString().ToString()== id).FirstOrDefaultAsync();

    if (pedido is null) return TypedResults.NotFound();

    pedido.Produtos = pedidoInput.Produtos;
    pedido.Total = pedido.Produtos.Sum(x => x.Preco);
    pedido.Usuario = pedidoInput.Usuario;

    await collection.ReplaceOneAsync(x => x.Id.ToString() == id, pedido);
    return TypedResults.NoContent();
}

static async Task<IResult> UpdateStatusPedido(string id, int status, IMongoCollection<Pedido> collection)
{
    var pedido = await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    if (pedido is null) return TypedResults.NotFound();

    //adicionar umas regras para atualização posteriormente
    pedido.Status = (PedidoStatus)status;

    await collection.ReplaceOneAsync(x => x.Id.ToString() == id, pedido);
    return TypedResults.NoContent();
}

static async Task<IResult> DeletePedido(string id, IMongoCollection<Pedido> collection)
{
    if (await collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync() is Pedido pedido)
    {
        await collection.DeleteOneAsync(x => x.Id.ToString() == id);
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion