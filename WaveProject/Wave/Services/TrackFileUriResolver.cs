using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wave.Dtos;
using Wave.Models;

namespace Wave.Services
{
    public class TrackFileUriResolver : IValueResolver<Track, TrackDto, string>, IValueResolver<Track, PlaylistTrackDto, string>
    {
        public string Resolve(Track source, TrackDto destination, string destMember, ResolutionContext context)
        {
            if (source.TrackFile is null)
                return null;
            return $"https://localhost:44363/api/Player/{source.Id}";
        }

        public string Resolve(Track source, PlaylistTrackDto destination, string destMember, ResolutionContext context)
        {
            if (source.TrackFile is null)
                return null;
            return $"https://localhost:44363/api/Player/{source.Id}";
        }
    }
}
