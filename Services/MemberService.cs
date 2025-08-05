using Microsoft.EntityFrameworkCore;
using StaffManagementN.Data;
using StaffManagementN.Interfaces;
using StaffManagementN.Models;

namespace StaffManagementN.Services;

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _context;

    public MemberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemberModel>> GetAllMembersAsync()
    {
        return await _context.Members.ToListAsync();
    }

    public async Task<MemberModel> GetMemberByIdAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.BorrowedBooks)
            .ThenInclude(bb => bb.Book)
            .FirstOrDefaultAsync(m => m.MemberId == id);

        if (member == null)
        {
            throw new KeyNotFoundException($"Member with ID {id} not found.");
        }

        return member;
    }

    public async Task<MemberModel> CreateMemberAsync(MemberModel member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<MemberModel> UpdateMemberAsync(int id, MemberModel member)
    {
        if (id != member.MemberId)
        {
            throw new ArgumentException("ID mismatch");
        }

        var existingMember = await _context.Members.FindAsync(id);
        if (existingMember == null)
        {
            throw new KeyNotFoundException($"Member with ID {id} not found.");
        }

        _context.Entry(existingMember).CurrentValues.SetValues(member);
        await _context.SaveChangesAsync();

        return member;
    }

    public async Task DeleteMemberAsync(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member == null)
        {
            throw new KeyNotFoundException($"Member with ID {id} not found.");
        }

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
    }
} 