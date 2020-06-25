
namespace Desastres.Web.Data.Entities
{
    public class Funcion
    {
        public int EntidadesId { get; set; }
        public int TipoDesastresId { get; set; }
        public Entidad Entidades { get; set; }
        public TipoDesastre TipoDesastres { get; set; }
    }
}
