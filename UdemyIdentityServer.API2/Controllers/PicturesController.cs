﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UdemyIdentityServer.API2.Models;

namespace UdemyIdentityServer.API2.Controllers
{
    //API2 => https://localhost:5020;


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        [Authorize(Policy = "ReadPicture")]//UpdateOrCreate=Policy ile UpdateOrCreate şartını sağlaması lazım bu şart ise startupdaki 58. satırdan bahsediyor. aynısını karışlıyorsa yapacak.Bu şartı sağlarsa bu metodu ulaşacak.Tokenin gelemesi yeterli değil, aynı zamanda scobunda ilgili authorize ne ise oda olması gerekiyor. 
        [HttpGet]
        public IActionResult GetPictures()
        {
            var pictures = new List<Picture>() {
                new Picture{Id=1, Name="Doğa resmi" , Url="dogaresmi.jpg"},
                new Picture{Id=1, Name="fil resmi"  , Url="dogaresmi.jpg"},
                new Picture{Id=1, Name="aslan resmi", Url="dogaresmi.jpg"},
                new Picture{Id=1, Name="fare resmi" , Url="dogaresmi.jpg"},
                new Picture{Id=1, Name="kedi resmi" , Url="dogaresmi.jpg"},
                new Picture{Id=1, Name="köpek resmi", Url="dogaresmi.jpg"}
           };
            return Ok(pictures);
        }
    }
}