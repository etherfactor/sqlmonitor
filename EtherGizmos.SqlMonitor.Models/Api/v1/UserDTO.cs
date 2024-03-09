using Asp.Versioning;
using Asp.Versioning.OData;
using AutoMapper;
using EtherGizmos.SqlMonitor.Models.Database;
using EtherGizmos.SqlMonitor.Models.Extensions;
using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Models.Api.v1;

public class UserDTO
{
    [Required]
    public Guid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? EmailAddress { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public bool? IsActive { get; set; } = true;

    [Required]
    public bool? IsAdministrator { get; set; } = false;

    public DateTimeOffset? LastLoginAt { get; set; }

    public Task EnsureValid(IQueryable<User> records)
    {
        records.EnsureUnique((e => e.Username, Username));

        if (EmailAddress != null)
            records.EnsureUnique((e => e.EmailAddress, EmailAddress));

        return Task.CompletedTask;
    }
}

public class UserDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<UserDTO>("users");
        var entity = builder.EntityType<UserDTO>();

        entity.Namespace = "EtherGizmos.PerformancePulse";
        entity.Name = entity.Name.Replace("DTO", "");

        entity.IgnoreAll();

        if (apiVersion >= ApiVersions.V0_1)
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id);
            /* Begin Audit */
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.CreatedByUserId);
            entity.Property(e => e.ModifiedAt);
            entity.Property(e => e.ModifiedByUserId);
            /*  End Audit  */
            entity.Property(e => e.Username);
            entity.Property(e => e.Password);
            entity.Property(e => e.EmailAddress);
            entity.Property(e => e.Name);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IsAdministrator);
            entity.Property(e => e.LastLoginAt);
        }
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
        toDto.MapMember(dest => dest.LastLoginAt, src => src.LastLoginAtUtc);

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
        fromDto.MapMember(dest => dest.LastLoginAtUtc, src => src.LastLoginAt);

        return @this;
    }
}
