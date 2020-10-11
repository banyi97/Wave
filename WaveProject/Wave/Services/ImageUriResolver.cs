using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Services
{
    public class ImageUriResolver : IValueResolver<Image, ImageDto, string>, IValueResolver<Track, TrackDto, string>, IValueResolver<Track, PlaylistTrackDto, string>
    {
        private readonly IOptions<AzureBlobConfig> _config;
        public ImageUriResolver(IOptions<AzureBlobConfig> config)
        {
            this._config = config;
        }
        public string Resolve(Image source, ImageDto destination, string destMember, ResolutionContext context)
        {
            if (source.Id is null)
                return $"assets/img/wave.jpg";
            return $"{this._config.Value.BlobUri}/{this._config.Value.ContainerImg}/{source.Id}";
        }

        public string Resolve(Track source, TrackDto destination, string destMember, ResolutionContext context)
        {
            if (source.Album?.Image is null)
                return $"assets/img/wave.jpg";
            return $"{this._config.Value.BlobUri}/{this._config.Value.ContainerImg}/{source.Album.Image.Id}";
        }

        public string Resolve(Track source, PlaylistTrackDto destination, string destMember, ResolutionContext context)
        {
            if (source.Album?.Image is null)
                return $"assets/img/wave.jpg";
            return $"{this._config.Value.BlobUri}/{this._config.Value.ContainerImg}/{source.Album.Image.Id}";
        }
    }
}
