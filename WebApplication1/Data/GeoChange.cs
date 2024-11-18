using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

public class GeoChange
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string GeoJson { get; set; }

    [Required]
    public string Description { get; set; }

    [BindNever]
    public string UserId { get; set; }
}
