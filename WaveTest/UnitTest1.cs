using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Wave.Dtos;
using Wave.Models;
using Wave.Services;

namespace WaveTest
{
    public class Tests
    {
        IMapper mapper;

        [SetUp]
        public void BeforeEachTest()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(MapperConfig));
            });
            mapper = new Mapper(configuration);
        }
        
        [Test]
        public void RenumberTest()
        {
            var list = new List<Playlist>() { new Playlist(), new Playlist(), new Playlist() };
            Assert.AreEqual(0, list[0].NumberOf);
            Assert.AreEqual(0, list[1].NumberOf);
            Assert.AreEqual(0, list[2].NumberOf);

            list.Renumber();

            Assert.AreEqual(0, list[0].NumberOf);
            Assert.AreEqual(1, list[1].NumberOf);
            Assert.AreEqual(2, list[2].NumberOf);

            Assert.Pass();
        }

        [Test]
        public void TrackFileUriResolverIsNull()
        {
            var file = new Track();
            var resolver = new TrackFileUriResolver();
            var dest = new TrackDto();

            resolver.Resolve(file, dest, nameof(dest.Uri), null);

            Assert.IsNull(dest.Uri);
        }

        [Test]
        public void CreateArtistMapper()
        {
            var dto = new CreateArtistDto
            {
                Name = "testName",
                Description = "test desc"
            };

            var res = mapper.Map<Artist>(dto);

            Assert.IsNotNull(res);
            Assert.AreEqual(dto.Name, res.Name);
            Assert.AreEqual(dto.Description, res.Description);
        }

        [Test]
        public void CreateplaylistMapper()
        {
            var dto = new CreatePlaylistDto
            {
                 Title = "test title"
            };

            var res = mapper.Map<Playlist>(dto);

            Assert.IsNotNull(res);
            Assert.AreEqual(dto.Title, res.Title);
        }

    }
}