using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Services
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Image, ImageDto>()
                 .ForMember(dest => dest.Uri, opt => opt.MapFrom<ImageUriResolver>());

            CreateMap<Artist, ArtistDto>()
                .ForMember(dest => dest.Image, opt => opt.NullSubstitute(new ArtistImage()));
            CreateMap<ArtistDto, Artist>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Albums, opt => opt.Ignore());

            CreateMap<CreateArtistDto, Artist>();
            CreateMap<CreateTrackDto, Track>();
            CreateMap<CreateAlbumDto, Album>()
                .ForMember(dest => dest.Tracks, opt => opt.Ignore());
            CreateMap<CreatePlaylistDto, Playlist>();

            CreateMap<Album, AlbumDto>()
                .ForMember(dest => dest.Image, opt => opt.NullSubstitute(new AlbumImage()))
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(s => s.Artist == null ? null : s.Artist.Name))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(s => s.Tracks == null ? null : s.Tracks.OrderBy(q => q.DiscNumber).ThenBy(q => q.NumberOf)));
            CreateMap<AlbumDto, Album>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Artist, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Tracks, opt => opt.Ignore());

            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.Uri, opt => opt.MapFrom<TrackFileUriResolver>())
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(s => s.Duration.TotalSeconds))
                .ForMember(dest => dest.ArtistId, opt => opt.MapFrom(s => s.Album.ArtistId))
                .ForMember(dest => dest.AlbumImageUri, opt => opt.MapFrom<ImageUriResolver>())
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(s => (s.Album == null || s.Album.Artist == null) ? null : s.Album.Artist.Name))
                .ForMember(dest => dest.AlbumLabel, opt => opt.MapFrom(s => s.Album == null ? null : s.Album.Label));
            CreateMap<TrackDto, Track>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.Album, opt => opt.Ignore())
                .ForMember(dest => dest.TrackFile, opt => opt.Ignore())
                .ForMember(dest => dest.TrackOfPlaylistElements, opt => opt.Ignore());

            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Image, opt => opt.NullSubstitute(new PlaylistImage()));
            CreateMap<PlaylistDto, Playlist>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.PlaylistElements, opt => opt.Ignore());

            CreateMap<PlaylistElement, PlaylistElementDto>();
            CreateMap<PlaylistElementDto, PlaylistElement>();

            CreateMap<Track, PlaylistTrackDto>()
                .ForMember(dest => dest.ArtistId, opt => opt.MapFrom(s => s.Album.ArtistId))
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(s => s.Album.Artist.Name))
                .ForMember(dest => dest.Uri, opt => opt.MapFrom<TrackFileUriResolver>())
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(s => s.Duration.TotalSeconds))
                .ForMember(dest => dest.AlbumImageUri, opt => opt.MapFrom<ImageUriResolver>());
        }
    }
}
