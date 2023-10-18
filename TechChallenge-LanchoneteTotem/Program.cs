using Microsoft.EntityFrameworkCore;
using TechChallenge_LanchoneteTotem.Data.UsuarioDb;
using TechChallenge_LanchoneteTotem.Model.DTO;
using TechChallenge_LanchoneteTotem.Model.Usuario;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UsuarioDb>(opt => opt.UseInMemoryDatabase("UsuarioList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var usuarios = app.MapGroup("/usuario");

usuarios.MapGet("/", GetAllUsuarios).WithName("GetAllUsuarios").WithOpenApi();
usuarios.MapGet("/id/{id}", GetUsuarioById).WithName("GetUsuarioById").WithOpenApi();
usuarios.MapGet("/cpf/{cpf}", GetUsuarioByCPF).WithName("GetUsuarioByCPF").WithOpenApi();
usuarios.MapGet("/email/{email}", GetUsuarioByEmail).WithName("GetUsuarioByEmail").WithOpenApi();
usuarios.MapPost("/", CreateUsuario).WithName("CreateUsuario").WithOpenApi();
usuarios.MapPut("/{id}", UpdateUsuario).WithName("UpdateUsuario").WithOpenApi();
usuarios.MapDelete("/{id}", DeleteUsuario).WithName("DeleteUsuario").WithOpenApi();


app.Run();

#region Usuario
static async Task<IResult> GetAllUsuarios(UsuarioDb db)
{
    return TypedResults.Ok(await db.Usuarios.Select(x => new UsuarioDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetUsuarioById(int id, UsuarioDb db)
{
    return await db.Usuarios.FindAsync(id)
        is Usuario usuario
        ? TypedResults.Ok(new UsuarioDTO(usuario))
        : TypedResults.NotFound();
}

static async Task<IResult> GetUsuarioByCPF(string cpf, UsuarioDb db)
{
    var usuario = await db.Usuarios.Where(u => u.CPF == cpf).Select(x => new UsuarioDTO(x)).ToListAsync();
     
    if(usuario is null)
        return TypedResults.NotFound();

    return TypedResults.Ok(usuario);
}

static async Task<IResult> GetUsuarioByEmail(string email, UsuarioDb db)
{
    var usuario = await db.Usuarios.Where(u => u.Email == email).Select(x => new UsuarioDTO(x)).ToListAsync();

    if (usuario is null)
        return TypedResults.NotFound();

    return TypedResults.Ok(usuario);
}

static async Task<IResult> CreateUsuario(UsuarioDTO usuarioDTO, UsuarioDb db)
{
    var usuario = new Usuario
    {
        Nome = usuarioDTO.Nome,
        CPF = usuarioDTO.CPF,
        Email = usuarioDTO.Email
    };

    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();

    usuarioDTO = new UsuarioDTO(usuario);

    return TypedResults.Created($"/usuario/{usuarioDTO.Id}", usuarioDTO);
}

static async Task<IResult> UpdateUsuario(int id, UsuarioDTO usuarioDTO, UsuarioDb db)
{
    var usuario = await db.Usuarios.FindAsync(id);

    if (usuario is null) return TypedResults.NotFound();

    usuario.Nome = usuarioDTO.Nome;
    usuario.Email = usuarioDTO.Email;
    usuario.CPF = usuarioDTO.CPF;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteUsuario(int id, UsuarioDb db)
{
    if(await db.Usuarios.FindAsync(id) is Usuario usuario)
    {
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
#endregion