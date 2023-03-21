using System.ComponentModel.DataAnnotations.Schema;


namespace lojinha_backend.Model
{
    [Table("Produtos")]
    public class Produto
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("descricao")]
        public string Descricao { get; set; }
        [Column("preco")]
        public decimal Preco { get; set; }
        [Column("descricaoPreco")]
        public string DescricaoPreco { get; set; }
        [Column("quantidadeEstoque")]
        public int QuantidadeEstoque { get; set; } 
        
    }
}
