using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public string DisplayName { get; set; } = null!;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }

    //navigation property
    [JsonIgnore]
    public List<Photo> Photos { get; set; } = [];
    
    [JsonIgnore]
    public List<MemberLike> LikedMembers { get; set; } = []; // ειναι τα μέλη που εγώ έχω κάνει like (δηλαδή εγώ είμαι το SourceMember)

    [JsonIgnore]
    public List<MemberLike> LikedByMembers { get; set; } = []; // ειναι τα μέλη που έχουν κάνει like σε εμένα (δηλαδή εγώ είμαι το TargetMember)

    [ForeignKey(nameof(Id))]
    [JsonIgnore]
    public AppUser User { get; set; } = null!;

}
