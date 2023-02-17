using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;

namespace MyMovieList.Business.Services;

public class SeedStateService : ISeedStateService
{
    private readonly ApiDbContext _context;

    public SeedStateService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<SeedState?> Get(int seedStateId)
    {
        return await _context.SeedStates.FindAsync(seedStateId);
    }

    public async Task<SeedState?> GetBySource(string source)
    {
        return await _context.SeedStates.FirstOrDefaultAsync(x => x.Source == source);
    }

    public async Task Add(SeedState seedState)
    {
        await _context.SeedStates.AddAsync(seedState);
        await _context.SaveChangesAsync();
    }

    public async Task Update(SeedState seedState)
    {
        _context.SeedStates.Update(seedState);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(SeedState seedState)
    {
        _context.SeedStates.Remove(seedState);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int seedStateId)
    {
        var seedState = await Get(seedStateId);

        if (seedState is not null)
        {
            await Delete(seedState);
        }
    }
}
