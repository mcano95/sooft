/*
Teniendo en cuenta la librería ICache, que fue escrita e implementada por otro equipo y utiliza una cache del tipo Key Value,
tomar la clase CajaRepository y modificar los métodos AddAsync, GetAllAsync, GetAllBySucursalAsync y GetOneAsync para que utilicen cache.

Datos:
    * Existen en la empresa 20 sucursales
    * Como mucho hay 100 cajas en la base

Restricción:    
	* Solo es posible utilizar 1 key (IMPORTANTE)
	
Aclaración:
	* No realizar una implementación de ICache, otro equipo la esta brindando
*/
public interface ICache
{
    Task AddAsync<T>(string key, T obj, int? durationInMinutes);
    Task<T> GetOrDefaultAsync<T>(string key);
    Task RemoveAsync(string key);
}

public class CajaRepository
{
    private readonly DataContext _db;
    private readonly ICache _cache;

    public CajaRepository(DataContext db, ICache cache)
    {
        _db = db ?? throw new ArgumentNullException(nameof(DataContext);
        _cache = cache;
    }
       

    public async Task AddAsync(Entities.Caja caja)
    {
        try
        {
            await _db.Cajas.AddAsync(caja);
            await _db.SaveChangesAsync();
            _cache.AddAsync<Entities.Caja>((string)caja.Id, caja, 60);

        } catch (Exception ex)
        {
            throw new Exception("Error al guardar la caja: " + ex);
        }
    }

    public async Task<IEnumerable<Entities.Caja>> GetAllAsync()
    {
        IEnumerable<Entities.Caja> cajas = new IEnumerable<Entities.Caja>();

        cajas = await _db.Cajas
            .ToListAsync();

        if (cajas != null)
        {
            foreach (Entities.Caja c in cajas)
            {
                if (_cache.GetOrDefaultAsync<Entities.Caja>(c.Id.ToString()) == null)
                {
                    _cache.AddAsync<Entities.Caja>(c.Id.ToString(), c, 60);

                }
            }
        }
            return cajas;
    }

    public async Task<IEnumerable<Entities.Caja>> GetAllBySucursalAsync(int sucursalId)
    {
        IEnumerable<Entities.Caja> cajas = new IEnumerable<Entities.Caja>();
       
        cajas = await _db.Cajas.Where(x => x.SucursalId == sucursalId).ToListAsync();

            if (cajas != null)
            {
                foreach (Entities.Caja c in cajas)
                {
                    if (_cache.GetOrDefaultAsync<Entities.Caja>(c.Id.ToString()) == null)
                    {
                        _cache.AddAsync<Entities.Caja>(c.Id.ToString(), c, 60);

                    }
                }
            }
        return cajas;
    }

    public async Task<Entities.Caja> GetOneAsync(Guid id)
    {
        Entities.Caja caja = new Entities.Caja();
        caja = _cache.GetOrDefaultAsync<Entities.Caja>(id.ToString());
        if (caja == null)
        {
           caja = await _db.Cajas.FirstOrDefaultAsync(x => x.Id == id);
           _cache.AddAsync<Entities.Caja>((string)caja.Id, caja, 60);
        }

        return caja;
    }
}
