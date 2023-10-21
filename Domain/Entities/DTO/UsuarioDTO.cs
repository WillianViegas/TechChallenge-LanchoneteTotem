namespace Domain.Entities.DTO
{
    public class UsuarioDTO
    {
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }

        public UsuarioDTO() { }

        public UsuarioDTO(Usuario usuario) =>
            (Id, Nome, Email, CPF) = (usuario.Id.ToString(), usuario.Nome, usuario.Email, usuario.CPF);
    }
}
