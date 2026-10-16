namespace Application.Mappers.Interfaces
{
    public interface IEntityMapper<TModel, TDto>
    {
        TDto ToDto(TModel model);

        TModel ToModel(TDto dto);

        void ApplyTo(TDto dto, TModel model);
    }
}
