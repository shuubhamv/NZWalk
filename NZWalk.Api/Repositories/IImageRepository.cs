using NZWalk.Api.Models.Domain;

namespace NZWalk.Api.Repositories
{
    public interface IImageRepository
    {
        Task <Image>Upload(Image image);
    }
}
