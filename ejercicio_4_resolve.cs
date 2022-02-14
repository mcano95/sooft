/*
A partir de las clases CajaRepository y SucursalRepository, crear la clase BaseRepository<T> 
que unifique los métodos GetAllAsync y GetOneAsync
Crear un abstract BaseEntity que defina la property Id y luego modificar las entities Caja y Sucursal para que hereden de BaseEntity 
Aclaración: Se deben respetar la interfaces. 
*/

namespace Domain.Entities
{
    public class Caja : BaseEntity<Guid>
    {
        public int SucursalId { get; }
        public string Descripcion { get; }
        public int TipoCajaId { get; }

        public Caja(Guid id, int sucursalId, string descripcion, int tipoCajaId)
        {
            Id = id;
            SucursalId = sucursalId;   
            Descripcion = descripcion;
            TipoCajaId = tipoCajaId;
        }
    }

    public class Sucursal : BaseEntity<int>
    {
        public string Direccion { get; }
        public string Telefono { get; }

        public Sucursal(int id, string direccion, string telefono)
        {
            Id = id;
            Direccion = direccion;
            Telefono = telefono;
        }
    }

    public abstract class BaseEntity<T>  
    {
        public T Id { get; set; }

    }
}

namespace Infrastructure.Data.Repositories
{
	public interface ICajaRepository 
	{
		Task<IEnumerable<Caja>> GetAllAsync();
		Task<Caja> GetOneAsync(Guid id);
	}
	
	public interface ISucursalRepository
	{
		Task<IEnumerable<Sucursal>> GetAllAsync();
		Task<Sucursal> GetOneAsync(int id);
	}
	
    public class BaseRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetOneAsync(T id);
    }

    public class CajaRepository : BaseRepository<Caja>
    {
        private readonly DataContext _db;

        public CajaRepository(DataContext db) => _db = db;

        public async Task<IEnumerable<Caja>> GetAllAsync() => await _db.Cajas.ToListAsync();

        public async Task<Caja> GetOneAsync(Guid id) => await _db.Cajas.FirstOrDefaultAsync(x => x.Id == id);
    }

    public class SucursalRepository : BaseRepository<Sucursal>
    {
        private readonly DataContext _db;

        public CajaRepository(DataContext db) => _db = db;

        public async Task<IEnumerable<Sucursal>> GetAllAsync() => await _db.Sucursales.ToListAsync();

        public async Task<Sucursal> GetOneAsync(int id) => await _db.Sucursales.FirstOrDefaultAsync(x => x.Id == id);
    }
}