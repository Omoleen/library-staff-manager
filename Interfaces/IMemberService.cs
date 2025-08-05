using StaffManagementN.Models;

namespace StaffManagementN.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<MemberModel>> GetAllMembersAsync();
    Task<MemberModel> GetMemberByIdAsync(int id);
    Task<MemberModel> CreateMemberAsync(MemberModel member);
    Task<MemberModel> UpdateMemberAsync(int id, MemberModel member);
    Task DeleteMemberAsync(int id);
} 