namespace Sempi5.Domain.SpecializationEntity
{
    /**
     * SpecializationDTO.cs created by Ricardo Guimarães on 10/12/2024
     */
    public class SpecializationCreateDTO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
    }

    public class SpecializationUpdateDTO
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
    }

    public class SpecializationDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
    }
}
