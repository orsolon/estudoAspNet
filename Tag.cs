public class Tag {
    public int Id { get; set; }
    public string Name { get; set; }
    //para o entity não deixar a propriedade producId da Tabela Tag no banco como null
    public int ProductId { get; set; }
}
