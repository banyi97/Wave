using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Wave.Dtos;
using Wave.Models;
using Wave.Services;
using Wave.Validators;

namespace WaveTest
{
    public class Tests
    {
        IMapper mapper;

        [OneTimeSetUp]
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

        [Test]
        public void CreateAlbumMapper()
        {
            var date = DateTime.Now;
            var dto = new CreateAlbumDto
            {
                Label = "test title",
                ReleaseDate = date
            };

            var res = mapper.Map<Album>(dto);
   
            Assert.IsNotNull(res);
            Assert.AreEqual(dto.Label, res.Label);
            Assert.AreEqual(dto.ReleaseDate, res.ReleaseDate);
            Assert.IsNull(res.Image);
            Assert.IsEmpty(res.Tracks);
        }

        [Test]
        public void CreateplaylistFluent()
        {
            var dto = new CreatePlaylistDto
            {
                Title = "test title"
            };
            var validator = new CreatePlaylistValidator();
            
            var res = validator.Validate(dto);

            Assert.IsNotNull(res);
            Assert.IsTrue(res.IsValid);
        }

        [Test]
        public void CreateplaylistFluentNotValidDto()
        {
            var dto = new CreatePlaylistDto
            {
                Title = null
            };
            var validator = new CreatePlaylistValidator();

            var res = validator.Validate(dto);

            Assert.IsNotNull(res);
            Assert.IsFalse(res.IsValid);
        }

    }
}