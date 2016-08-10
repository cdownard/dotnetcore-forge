using System;
using System.ComponentModel.DataAnnotations;

namespace Forge.Model
{
    public sealed class ModelProperty
    {
        [Required] 
        public string Name { get; set; }

        [Required] 
        public Type Type { get; set; }
    }
}