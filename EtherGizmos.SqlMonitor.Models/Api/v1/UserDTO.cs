using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

[Display(Name = "User", GroupName = "users")]
public class UserDTO
{
    [Required]
    [Display(Name = "id")]
    public Guid Id { get; set; }

    [Display(Name = "created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [Display(Name = "created_by_user_id")]
    public Guid? CreatedByUserId { get; set; }

    [Display(Name = "modified_at")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [Display(Name = "modified_by_user_id")]
    public Guid? ModifiedByUserId { get; set; }

    [Required]
    [Display(Name = "username")]
    public string? Username { get; set; }

    [Required]
    [Display(Name = "password")]
    public string? Password { get; set; }

    [Display(Name = "email_address")]
    public string? EmailAddress { get; set; }

    [Required]
    [Display(Name = "name")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "is_active")]
    public bool? IsActive { get; set; } = true;

    [Required]
    [Display(Name = "is_administrator")]
    public bool? IsAdministrator { get; set; } = false;

    [Display(Name = "last_login_at_utc")]
    public DateTimeOffset? LastLoginAtUtc { get; set; }

    public Task EnsureValid(IQueryable<User> records)
    {
        records.EnsureUnique((e => e.Username, Username));

        if (EmailAddress != null)
            records.EnsureUnique((e => e.EmailAddress, EmailAddress));

        return Task.CompletedTask;
    }
}

public static class ForUserDTO
{
    public static IProfileExpression AddUser(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<User, UserDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Username, src => src.Username);
        toDto.MapMember(dest => dest.Password, src => "***");
        toDto.MapMember(dest => dest.EmailAddress, src => src.EmailAddress);
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        toDto.MapMember(dest => dest.IsAdministrator, src => src.IsAdministrator);
        toDto.MapMember(dest => dest.LastLoginAtUtc, src => src.LastLoginAtUtc);

        var fromDto = @this.CreateMap<UserDTO, User>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.Username, src => src.Username);
        fromDto.MapMember(dest => dest.PasswordHash, src => src.Password,
            opt => opt.PreCondition(ctx => ctx.Password != "***")); //Only map back if not masked
        fromDto.MapMember(dest => dest.EmailAddress, src => src.EmailAddress);
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.IsActive, src => src.IsActive);
        fromDto.MapMember(dest => dest.IsAdministrator, src => src.IsAdministrator);
        fromDto.MapMember(dest => dest.LastLoginAtUtc, src => src.LastLoginAtUtc);

        return @this;
    }

    public static ODataModelBuilder AddUser(this ODataModelBuilder @this)
    {
        var entitySet = @this.EntitySetWithAnnotations<UserDTO>();

        var entity = @this.EntityTypeWithAnnotations<UserDTO>();
        entity.HasKey(e => e.Id);
        /* Begin Audit */
        entity.PropertyWithAnnotations(e => e.CreatedAt);
        entity.PropertyWithAnnotations(e => e.CreatedByUserId);
        entity.PropertyWithAnnotations(e => e.ModifiedAt);
        entity.PropertyWithAnnotations(e => e.ModifiedByUserId);
        /*  End Audit  */
        entity.PropertyWithAnnotations(e => e.Username);
        entity.PropertyWithAnnotations(e => e.Password);
        entity.PropertyWithAnnotations(e => e.EmailAddress);
        entity.PropertyWithAnnotations(e => e.Name);
        entity.PropertyWithAnnotations(e => e.IsActive);
        entity.PropertyWithAnnotations(e => e.IsAdministrator);
        entity.PropertyWithAnnotations(e => e.LastLoginAtUtc);

        return @this;
    }
}
