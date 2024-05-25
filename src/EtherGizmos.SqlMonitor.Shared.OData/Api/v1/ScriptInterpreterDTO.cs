using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Shared.Models.Api.v1;

public class ScriptInterpreterDTO
{
    public int Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public DateTimeOffset? ModifiedAt { get; set; }

    public Guid? ModifiedByUserId { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public string? Command { get; set; }

    [Required]
    public string? Arguments { get; set; }

    [Required]
    public string? Extension { get; set; }

    public Task EnsureValid(IQueryable<ScriptInterpreter> records)
    {
        return Task.CompletedTask;
    }
}

public class ScriptInterpreterDTOConfiguration : IModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string? routePrefix)
    {
        var entitySet = builder.EntitySet<ScriptInterpreterDTO>("scriptInterpreters");
        var entity = builder.EntityType<ScriptInterpreterDTO>();

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
            entity.Property(e => e.Name);
            entity.Property(e => e.Description);
            entity.Property(e => e.Command);
            entity.Property(e => e.Arguments);
            entity.Property(e => e.Extension);
        }
    }
}

public static class ForScriptInterpreterDTO
{
    public static IProfileExpression AddScriptInterpreter(this IProfileExpression @this)
    {
        var toDto = @this.CreateMap<ScriptInterpreter, ScriptInterpreterDTO>();
        toDto.IgnoreAllMembers();
        toDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        toDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        toDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        toDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        toDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        toDto.MapMember(dest => dest.Name, src => src.Name);
        toDto.MapMember(dest => dest.Description, src => src.Description);
        toDto.MapMember(dest => dest.Command, src => src.Command);
        toDto.MapMember(dest => dest.Arguments, src => src.Arguments);
        toDto.MapMember(dest => dest.Extension, src => src.Extension);

        var fromDto = @this.CreateMap<ScriptInterpreterDTO, ScriptInterpreter>();
        fromDto.IgnoreAllMembers();
        fromDto.MapMember(dest => dest.Id, src => src.Id);
        /* Begin Audit */
        fromDto.MapMember(dest => dest.CreatedAt, src => src.CreatedAt);
        fromDto.MapMember(dest => dest.CreatedByUserId, src => src.CreatedByUserId);
        fromDto.MapMember(dest => dest.ModifiedAt, src => src.ModifiedAt);
        fromDto.MapMember(dest => dest.ModifiedByUserId, src => src.ModifiedByUserId);
        /*  End Audit  */
        fromDto.MapMember(dest => dest.Name, src => src.Name);
        fromDto.MapMember(dest => dest.Description, src => src.Description);
        fromDto.MapMember(dest => dest.Command, src => src.Command);
        fromDto.MapMember(dest => dest.Arguments, src => src.Arguments);
        fromDto.MapMember(dest => dest.Extension, src => src.Extension);

        return @this;
    }
}
